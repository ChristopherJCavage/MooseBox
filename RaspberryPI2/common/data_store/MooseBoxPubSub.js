/************************************************************************
 * Copyright (C) 2015  Christopher James Cavage (cjcavage@gmail.com)    *
 *                                                                      *
 * This program is free software; you can redistribute it and/or        *
 * modify it under the terms of the GNU General Public license          *
 * as published by the Free Software Foundation; either version 2       *
 * of the License, or (at your option) any later version.               *
 *                                                                      *
 * This program is distributed in the hope that it will be useful,      *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        * 
 * GNU General Public License for more details.                         *
 *                                                                      *
 * You should have received a copy of the GNU General Public License    *
 * along with this program; if not, see <http://www.gnu.org/licenses/>. *
 ************************************************************************/
var MooseBoxRedisKeys = require('./MooseBoxRedisKeys.js');

var events = require('events');
var redis = require('redis');

/**
 * Defines an abstraction for MooseBox Publish-Subscribe Messaging.  Abstracts Redis
 * DB database operations for any trusted client running on MooseBox.
 *
 * @param hostname MooseBox hostname containing Redis DB to connect to (i.e. 127.0.0.1).
 * @param port Redis DB port as it is configured on the MooseBox (i.e. 6379).
 * @remarks Currently only allows a single-subscriber; may be revisited by extending Event Emmitter.
 */
function MooseBoxPubSub(hostname, port) {
    //Parameter Validations.
    if (!hostname || 0 === hostname.length)
        throw 'Hostname cannot be empty / null';

    if (port < 0)
        throw 'Port cannot be negative';

    //Create our message/callback map.
    this.m_lookupHandlers = new Object();

    //Redis requires a dedicated client connection for Publish-Subscribe; subscribe to RedisDB events.
    this.m_redisPubSubP = new redis.createClient(port, hostname);
    this.m_redisPubSubS = new redis.createClient(port, hostname);

    this.m_redisPubSubP.on('connect', this.onRedisConnected.bind(this));
    this.m_redisPubSubP.on('error', this.onRedisError.bind(this));
    this.m_redisPubSubP.on('message', this.onRedisMessagePublished.bind(this));

    this.m_redisPubSubS.on('connect', this.onRedisConnected.bind(this));
    this.m_redisPubSubS.on('error', this.onRedisError.bind(this));
    this.m_redisPubSubS.on('message', this.onRedisMessagePublished.bind(this));
};

                                /******************/
                                /*** PUBLIC API ***/
                                /******************/

/**
 * Publishes live fan control power state data to subscribing clients on MooseBox.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param powerOn true if the numbered fan was powered; false otherwise.
 * @param timestamp Timestamp the fan control operation was sourced at.
 */
MooseBoxPubSub.prototype.publishFanCtrlReq = function(fanNumber, powerOn, timestamp)
{
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (fanNumber < 0)
        throw 'Fan number cannot be negative';

    //Get the dynamic key to publish on.
    var key = mbrKeys.getFanCtrlPubSubReqKey(fanNumber);

    //Build the JSON object.
    var obj = {};

    obj.PowerOn = powerOn;
    obj.Timestamp = timestamp;

    //Publish it.
    this.m_redisPubSubP.publish(key, JSON.stringify(obj));
}

/**
 * Publishes live temperature data to subscribing clients on MooseBox.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param celsius Temperature value, in degrees Celsius.
 * @param timestamp Timestamp the fan control operation was sourced at.
 */
MooseBoxPubSub.prototype.publishTemperatureReadingNotify = function(serialNumber, celsius, timestamp) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!serialNumber || 0 === serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    if (!celsius)
        throw 'celsius cannot be null';

    if (!timestamp)
        throw 'timestamp cannot be null';

    //Get the dynamic key to publish on.
    var key = mbrKeys.getTemperatureReadingPubSubNotifyKey(serialNumber);

    //Build the JSON object.
    var obj = {};

    obj.Celsius = celsius;
    obj.Timestamp = timestamp;

    //Publish it.
    this.m_redisPubSubP.publish(key, JSON.stringify(obj));
}

/**
 * Publishes Fan Control configuration data to subscribing clients on MooseBox.
 *
 * @param configObj JSON configuration data object.
 */
MooseBoxPubSub.prototype.publishFanCtrlConfig = function(configObj) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!configObj)
        throw 'configObj cannot be null';

    //Publish it.
    this.m_redisPubSubP.publish(mbrKeys.PUBSUB_NOTIFY_CONFIG_FAN_CTRL_DAEMON_KEY, JSON.stringify(configObj));
}

/**
 * Publishes Temperature configuration data to subscribing clients on MooseBox.
 *
 * @param configObj JSON configuration data object.
 */
MooseBoxPubSub.prototype.publishTemperatureConfig = function(configObj) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!configObj)
        throw 'configObj cannot be null';

    //Publish it.
    this.m_redisPubSubP.publish(mbrKeys.PUBSUB_NOTIFY_CONFIG_TEMPERATURE_DAEMON_KEY, JSON.stringify(configObj));
}

/**
 * Publishes zero or more iButtonLink T-Sensor serial numbers to subscribing clients on MooseBox.
 *
 * @param list List of iButtonLink T-Sensor serial numbers.
 */
MooseBoxPubSub.prototype.publishTemperatureSensorSerialNumbers = function(serialNumbers) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!serialNumbers)
        throw 'serialNumbers cannot be null / empty';

    //Publish it, if there is something to publish.
    if (0 !== serialNumbers.length)
        this.m_redisPubSubP.publish(mbrKeys.PUBSUB_NOTIFY_SENSOR_SERIAL_NUMBERS_KEY, JSON.stringify(serialNumbers));
}

/**
 * Subscribes to the Fan Control power data Publish-Subscribe channel.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param callback Callback of type function(jsonObj) to be invoked when new data is published.
 */
MooseBoxPubSub.prototype.subscribeFanCtrlReq = function(fanNumber, callback) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!callback)
        throw 'callback cannot be null';

    //Subscribe to the channel with dynamic key, if we haven't already done so.
    var key = mbrKeys.getFanCtrlPubSubReqKey(fanNumber);

    this._subscribeWorker(key, callback);
}

/**
 * Subscribes to the Temperature data Publish-Subscribe channel.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param callback Callback of type function(jsonObj) to be invoked when new data is published.
 */
MooseBoxPubSub.prototype.subscribeTemperatureReadingNotify = function(serialNumber, callback) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Parameter Validations.
    if (!serialNumber || 0 === serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    //Subscribe to the channel with dynamic key, if we haven't already done so.
    var key = mbrKeys.getTemperatureReadingPubSubNotifyKey(serialNumber);

    this._subscribeWorker(key, callback);
}

/**
 * Subscribes to the Fan Control configuration data Publish-Subscribe channel.
 *
 * @param callback Callback of type function(jsonObj) to be invoked when new data is published.
 */
MooseBoxPubSub.prototype.subscribeFanCtrlConfig = function(callback) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Subscribe to the Redis Pub/Sub channel if we haven't already done so.
    this._subscribeWorker(mbrKeys.PUBSUB_NOTIFY_CONFIG_FAN_CTRL_DAEMON_KEY, callback);
}

/**
 * Subscribes to the Temperature configuration data Publish-Subscribe channel.
 *
 * @param callback Callback of type function(jsonObj) to be invoked when new data is published.
 */
MooseBoxPubSub.prototype.subscribeTemperatureConfig = function(callback) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Subscribe to the Redis Pub/Sub channel if we haven't already done so.
    this._subscribeWorker(mbrKeys.PUBSUB_NOTIFY_CONFIG_TEMPERATURE_DAEMON_KEY, callback);
}

/**
 * Subscribes to the iButtonLink T-Sensor serial numbers data Publish-Subscribe channel.
 *
 * @param callback Callback of type function(jsonObj) to be invoked when new data is published.
 */
MooseBoxPubSub.prototype.subscribeTemperatureSensorSerialNumbers = function(callback) {
    var mbrKeys = new MooseBoxRedisKeys();

    //Subscribe to the Redis Pub/Sub channel if we haven't already done so.
    this._subscribeWorker(mbrKeys.PUBSUB_NOTIFY_SENSOR_SERIAL_NUMBERS_KEY, callback);
}

                                /******************************/
                                /*** PRIVATE API (Handlers) ***/
                                /******************************/

/**
 * Redis DB handler routine for connection event.
 */
MooseBoxPubSub.prototype.onRedisConnected = function() {
    console.log('MooseBox Redis PubSub Connected.');
}

/**
 * Redis DB handler routine for error event.
 *
 * @param err Error object.
 */
MooseBoxPubSub.prototype.onRedisError = function(err) {
    console.log('MooseBox Redis PubSub Error. Err: ' + err);
}

/**
 * Redis DB handler routine for Publish-Subscribe channel data.
 *
 * @param channel Publish-Subscribe channel message occurred on.
 * @param message Context-specific message contents.
 */
MooseBoxPubSub.prototype.onRedisMessagePublished = function(channel, message) {
    //Every message in our system is in JSON; parse it unilaterally.
    if (null === message || 0 == message.length)
        return;

    var jsonRoot = JSON.parse(message);

    //Now, attempt to lookup the channel in our handlers callback map; if applicable...
    if (null === jsonRoot)
        return;

    var handler = this.m_lookupHandlers[channel];

    if (null !== handler)
        handler(jsonRoot); //Invoke Handler.
}

                                /*****************************/
                                /*** PRIVATE API (Workers) ***/
                                /*****************************/

/**
 * Worker routine to subscribe to a Publish-Subscribe channel on Redis.
 *
 * @param key Redis DB channel key to subscribe to.
 * @param callback Event handler to invoke for message data.
 * @remarks Single-subscriber as Q3 2015.
 */
MooseBoxPubSub.prototype._subscribeWorker = function(key, callback) {
    //Parameter Validations.
    if (!key || 0 === key.length)
        throw 'key cannot be null / empty';

    if (!callback)
        throw 'callback cannot be null';

    //Subscribe to the Redis Pub/Sub channel IFF we haven't already subscribed.
    if (null !== this.m_lookupHandlers[key])
    {
        this.m_redisPubSubS.subscribe(key);

        //Add the handler to our map.
        this.m_lookupHandlers[key] = callback;
    }
}

//Export this class outside this file.
module.exports = MooseBoxPubSub;
