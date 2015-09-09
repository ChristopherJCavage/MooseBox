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
 * Defines an abstraction for a MooseBox Redis DataStore.  Abstracts database
 * operations for any trusted client running on MooseBox.
 *
 * @param hostname MooseBox hostname containing Redis DB to connect to (i.e. 127.0.0.1).
 * @param port Redis DB port as it is configured on the MooseBox (i.e. 6379).
 * @remarks Publish-Subscribe oerations are not included.
 */
function MooseBoxDataStore(hostname, port) {
    //Create one shared instance for Redis DB key lookup / building.
    this.m_mbrKeys = new MooseBoxRedisKeys();

    //Create Redis DB client for all non Pub/Sub operations.
    this.m_redisC = new redis.createClient(port, hostname);

    this.m_redisC.on('connect', function() {
        console.log('MooseBox DataStore Redis DB client connected.');
    });

    this.m_redisC.on('error', function(err) {
        console.log('MooseBox DataStore Redis DB error. Error: ' + err);
    });
};

                                /******************/
                                /*** PUBLIC API ***/
                                /******************/

/**
 * Adds a temperature reading to the data store and associates it with an expiration.
 * Optionally, accumulates the temperature reading into the historical data sorted set.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param celsius Temperature value, in degrees Celsius.
 * @param timestamp Timestamp reading was sampled at.
 * @param addHistorical true to add to the historical reading; false otherwise.
 * @param callback Optional callback of type function(err, reply) to invoke for error information.
 * @remarks O(log N)
 */
MooseBoxDataStore.prototype.addTemperatureReading = function(serialNumber, celsius, timestamp, addHistorical, callback) {
    var CURRENT_VALUE_TIMEOUT_S = 25;

    //Parameter Validations.
    if (!serialNumber || 0 == serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    if (!celsius || !timestamp || !addHistorical) //callback is optional.
        throw 'celsius, timestamp, addHistorical, callback cannot be null';

    //Always add the temperature to the current reading O(1) slot; build our key.
    var currentReadingKey = this.m_mbrKeys.getTemperatureCurrentReadingKey(serialNumber);

    //Wrap it in a JSON object.
    var currentReadingObj = {};

    currentReadingObj.Celsius = celsius;
    currentReadingObj.Timestamp = timestamp;

    //Set the value with an expiry timeout.
    this.m_redisC.set(currentReadingKey, JSON.stringify(currentReadingObj), function(err, reply) {
        if (!err && reply === 'OK')
        {
            //The set value was successful, now expire it.
            this.m_redisC.expire(currentReadingKey, CURRENT_VALUE_TIMEOUT_S, function(err, reply) {
                var raiseCallback = true;

                //If we are also adding to the historical readings, do so now.  We give them the option
                //because on the Raspberry PI 2 we don't have oodles of memory so they might rate-limit.
                if (!err && reply === 1 && addHistorical === true)
                {
                    //Build the historical reading key.
                    var histReadingKey = this.m_mbrKeys.getTemperatureHistoricalReadingKey(serialNumber);

                    //Wrap up in a timestamp-unique JSON object for the sorted set.
                    var histReadingObj = {};

                    histReadingObj.Celsius = celsius;
                    histReadingObj.Timestamp = timestamp;

                    //Finally, add it to the sorted set with the timestamp as the score. Just pass it the callback for raising.
                    raiseCallback = false;

                    this.m_redisC.zadd(histReadingKey, timestamp, JSON.stringify(histReadingObj), callback); //Last async call (optional).
                }

                //Report status to user if this is the last async call.
                if (raiseCallback === true && callback)
                    callback(err, reply);
            }.bind(this));
        }
        else if (callback)
            callback(err, reply);
    }.bind(this));
}

/**
 * Adds a fan control reading to the data store; both live and historical data.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param powerOn true if the fan was powered; false otherwise.
 * @param timestamp Timestamp the fan control operation was sourced at.
 * @param callback Optional callback of type function(err, reply) to invoke for error information.
 * @remarks O(log N)
 */
MooseBoxDataStore.prototype.addFanCtrlReading = function(fanNumber, powerOn, timestamp, callback) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'fanNumber cannot be negative';

    if (!timestamp)
        throw 'timestamp cannot be null';

    //Fan readings are sporadic; always add them to the current and historical datasets.
    var currentReadingKey = this.m_mbrKeys.getFanCtrlCurrentReadingKey(fanNumber);
    var histReadingKey = this.m_mbrKeys.getFanCtrlHistoricalReadingKey(fanNumber);

    //Wrap the reading in a JSON object.
    var obj = {};

    obj.PowerOn = powerOn;
    obj.Timestamp = timestamp;

    var jsonText = JSON.stringify(obj);

    //Simply set the value; fan power on/off has no concept of timeout (i.e. it's a controlled state).
    this.m_redisC.set(currentReadingKey, jsonText, function(err, reply) {
        if (!err && reply === 'OK') //The SET operation was successful; now add it to the historical sorted set dataset.
            this.m_redisC.zadd(histReadingKey, timestamp, jsonText, callback);
        else if (callback)
            callback(err, reply);
    }.bind(this));
}

/**
 * Queries the current temperature, which may have expired.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param callback Callback of the type function(err, serialNumber, reading) to invoke when
 *                 temperature is queried; a JSON reading object will be provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.queryCurrentTemperature = function(serialNumber, callback) {
    //Parameter Validations.
    if (!serialNumber || 0 == serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    //Build the key for the query.
    var key = this.m_mbrKeys.getTemperatureCurrentReadingKey(serialNumber);

    this._getValueWorker(key, function(err, obj) {
        //Forward with Serial # piggy-backed.
        callback(err, serialNumber, obj);
    }.bind(this));
}

/**
 * Queries zero or more historical temperature readings within a time-based range from the sorted set.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param startTimestamp Timestamp to begin temperature queries at; inclusive.
 * @param endTimestamp Timestamp to end temperature queries at; inclusive.
 * @param callback Callback of of type function(err, serialNumber, readings) to invoke when
 *                 temperature readings are queried. a JSON list of JSON objects will be provided.
 * @remarks O(log(N) + M)
 * @see http://redis.io/commands/zrangebyscore
 */
MooseBoxDataStore.prototype.queryHistoricalTemperatures = function(serialNumber, startTimestamp, endTimestamp, callback) {
    //Parameter Validations.
    if (!serialNumber || 0 == serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    if (!startTimestamp || !endTimestamp || !callback)
        throw 'startTimestamp, endTimestamp, callback cannot be null';

    if (endTimestamp < startTimestamp)
        throw 'startTimestamp <= X <= endTimestamp. startTimestamp: ' + startTimestamp + ', endTimestamp: ' + endTimestamp;

    //Build the key for the query.
    var key = this.m_mbrKeys.getTemperatureHistoricalReadingKey(serialNumber);

    //Query the sorted set for our timestamp range.
    this.m_redisC.zrangebyscore(key, startTimestamp, endTimestamp, function(err, reply) {
        var objs = [];

        //Reply is an object array; convert it to a JSON list; each element is one of our JSON objects.
        if (!err && reply)
            for(i = 0; i < reply.length; i++)
                objs.push(JSON.parse(reply[i]));

        //Pass up results (error or otherwise).
        callback(err, serialNumber, objs);
    }.bind(this));
}

/**
 * Gets the first and last timestamps, if any exist, in the sorted set for temperature values.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @param callback Callback of type function(err, startTimestamp, endTimestamp) to invoke.
 * @remarks O(log(N) + M)
 */
MooseBoxDataStore.prototype.getFirstLastTemperatureTimestamps = function(serialNumber, callback) {
    //Parameter Validations.
    if (!serialNumber || 0 == serialNumber.length)
        throw 'serialNumber cannot be null / empty';

    //Build the key for the query.
    var key = this.m_mbrKeys.getTemperatureHistoricalReadingKey(serialNumber);

    //Call worker method to perform query.
    this._queryTimestampRangeWorker(key, callback);
}

/**
 * Queries the current fan control power state for a numbered fan.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param callback Callback of type function(err, fanNumber, reading) to invoke when the 
 *                 fan control power state has been queried.  A JSON object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.queryCurrentFanCtrl = function(fanNumber, callback) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'fanNumber cannot be negative';

    //Build the key for the query.
    var key = this.m_mbrKeys.getFanCtrlCurrentReadingKey(fanNumber);

    this._getValueWorker(key, function(err, obj) {
        //Forward with Fan # piggy-backed.
        callback(err, fanNumber, obj);
    }.bind(this));
}

/**
 * Queries zero or more historical fan control power state readings within a time-based range from the sorted set.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param startTimestamp Timestamp to begin fan control queries at; inclusive.
 * @param endTimestamp Timestamp to end fan control queries at; inclusive.
 * @param callback Callback of of type function(err, fanNumber, readings) to invoke when fan control readings are
 *                 queried. a JSON list of JSON objects will be provided.
 * @remarks O(log(N) + M)
 * @see http://redis.io/commands/zrangebyscore
 */
MooseBoxDataStore.prototype.queryHistoricalFanCtrl = function(fanNumber, startTimestamp, endTimestamp, callback) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'fanNumber cannot be negative';

    if (!startTimestamp || !endTimestamp || !callback)
        throw 'startTimestamp, endTimestamp, callback cannot be null';

    if (endTimestamp < startTimestamp)
        throw 'startTimestamp <= X <= endTimestamp. startTimestamp: ' + startTimestamp + ', endTimestamp: ' + endTimestamp;

    //Build the key for the query.
    var key = this.m_mbrKeys.getFanCtrlHistoricalReadingKey(fanNumber);

    //Query the sorted set for our timestamp range.
    this.m_redisC.zrangebyscore(key, startTimestamp, endTimestamp, function(err, reply) {
        var objs = [];

        //Reply is an object array; convert it to a JSON list; each element is one of our JSON objects.
        if (!err && reply)
            for(i = 0; i < reply.length; i++)
                objs.push(JSON.parse(reply[i]));

        //Pass up results (error or otherwise).
        callback(err, fanNumber, objs);
    }.bind(this));
}

/**
 * Gets the first and last timestamps, if any exist, in the sorted set for fan control power values.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @param callback Callback of type function(err, startTimestamp, endTimestamp) to invoke.
 * @remarks O(log(N) + M)
 */
MooseBoxDataStore.prototype.getFirstLastTemperatureTimestamps = function(fanNumber, callback) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'fanNumber cannot be negative';

    //Build the key for the query.
    var key = this.m_mbrKeys.getFanCtrlHistoricalReadingKey(fanNumber);

    //Call worker method to perform query.
    this._queryTimestampRangeWorker(key, callback);
}

/**
 * Queries configuration data for Temperature Alarms.
 *
 * @param callback Callback of type function(err, configObj) to be invoked when query is complete.
 *                 Context-speicific JSON configuration data object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getAlarmsConfig = function(callback) {
    this._getValueWorker(this.m_mbrKeys.CONFIG_ALARMS_KEY, callback);
}

/**
 * Queries configuration data for the Fan Control Daemon.
 *
 * @param callback Callback of type function(err, configObj) to be invoked when query is complete.
 *                 Context-speicific JSON configuration data object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getFanCtrlConfig = function(callback) {
    this._getValueWorker(this.m_mbrKeys.CONFIG_FAN_CTRL_DAEMON_KEY, callback);
}

/**
 * Queries configuration data for the Temperature Daemon.
 *
 * @param callback Callback of type function(err, configObj) to be invoked when query is complete.
 *                 Context-speicific JSON configuration data object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getTemperatureConfig = function(callback) {
    this._getValueWorker(this.m_mbrKeys.CONFIG_TEMPERATURE_DAEMON_KEY, callback);
}

/**
 * Queries version information for the Fan Control Daemon.
 *
 * @param callback Callback of type function(err, versionStr) to be invoked when query is complete.
 *                 Context-speicific JSON configuration data object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getFanCtrlDaemonVersion = function(callback) {
    this._getValueWorker(this.m_mbrKeys.DAEMON_VERSION_FAN_CTRL_KEY, function(err, obj) {
        var versionStr = null;

        if (!err && obj)
            versionStr = obj.VersionStr;

        callback(err, versionStr);
    }.bind(this));
}

/**
 * Queries version information for the Temperature Daemon.
 *
 * @param callback Callback of type function(err, versionStr) to be invoked when query is complete.
 *                 Context-speicific JSON configuration data object is provided.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getTemperatureDaemonVersion = function(callback) {
    this._getValueWorker(this.m_mbrKeys.DAEMON_VERSION_TEMPERATURE_KEY, function(err, obj) {
        var versionStr = null;

        if (!err && obj)
            versionStr = obj.VersionStr;

        callback(err, versionStr);
    }.bind(this));
}

/**
 * Queries attached iButtonLink T-Probe serial numbers.
 *
 * @param callback Callback of type function(err, serialNumbers) to be invoked when query is complete.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.getTemperatureSensorSerialNumbers = function(callback) {
    this._getValueWorker(this.m_mbrKeys.TEMPERATURE_SENSOR_SERIAL_NUMBERS_KEY, function(err, obj) {
        var serialNumbers = null;

        if (!err && obj)
            serialNumbers = obj.SerialNumbers;

        callback(err, serialNumbers);
    }.bind(this));
}

/**
 * Stores configuration data for Temperature Alarms.
 *
 * @param configObj Context-specfic JSON configuration data object to be stored.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setAlarmsConfig = function(configObj, callback) {
    this._setValueWorker(this.m_mbrKeys.CONFIG_ALARMS_KEY, configObj, callback);
}

/**
 * Stores configuration data for the Fan Control Daemon.
 *
 * @param configObj Context-specfic JSON configuration data object to be stored.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setFanCtrlConfig = function(configObj, callback) {
    this._setValueWorker(this.m_mbrKeys.CONFIG_FAN_CTRL_DAEMON_KEY, configObj, callback);
}

/**
 * Stores configuration data for the Temperature Daemon.
 *
 * @param configObj Context-specfic JSON configuration data object to be stored.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setTemperatureConfig = function(configObj, callback) {
    this._setValueWorker(this.m_mbrKeys.CONFIG_TEMPERATURE_DAEMON_KEY, configObj, callback);
}

/**
 * Stores iButtonLink T-Sensor serial numbers attached to the MooseBox.
 *
 * @param serialNumbers List of iButtonLink T-Sensor serial numbers.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setTemperatureSensorSerialNumbers = function(serialNumbers, callback) {
    //Parameter Validations.
    if (!serialNumbers || 0 === serialNumbers.length)
        throw 'serialNumbers cannot be null / empty';

    //Wrap version string in a JSON object.
    var obj = {};

    obj.SerialNumbers = serialNumbers;

    //Forward to worker routine.
    this._setValueWorker(this.m_mbrKeys.TEMPERATURE_SENSOR_SERIAL_NUMBERS_KEY, obj, callback);
}

/**
 * Stores version information for the Fan Control Daemon.
 *
 * @param versionStr Stringified version number.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setFanCtrlDaemonVersion = function(versionStr, callback) {
    //Parameter Validations.
    if (!versionStr || 0 === versionStr.length)
        throw 'versionStr cannot be null / empty';

    //Wrap version string in a JSON object.
    var obj = {};

    obj.VersionStr = versionStr;

    //Forward to worker routine.
    this._setValueWorker(this.m_mbrKeys.DAEMON_VERSION_FAN_CTRL_KEY, obj, callback);
}

/**
 * Stores version information for the Temperature Daemon.
 *
 * @param versionStr Stringified version number.
 * @param callback Optional callback of type function(err) to invoke for error information.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype.setTemperatureDaemonVersion = function(versionStr, callback) {
    //Parameter Validations.
    if (!versionStr || 0 === versionStr.length)
        throw 'versionStr cannot be null / empty';

    //Wrap version string in a JSON object.
    var obj = {};

    obj.VersionStr = versionStr;

    //Forward to worker routine.
    this._setValueWorker(this.m_mbrKeys.DAEMON_VERSION_TEMPERATURE_KEY, obj, callback);
}

                                /*******************/
                                /*** PRIVATE API ***/
                                /*******************/

/**
 * Queries the first and last elements of a sorted set to asertain their timestamps so that
 * a client can present the bounds of a time range to an end-user for query refinement.
 *
 * @param key Redis DB key of the sorted set collection to query.
 * @param callback Callback of type function(err, startTimestamp, endTimestamp) to invoke.
 * @remarks O(log(N) + M)
 * @see http://redis.io/commands/zrange
 */
MooseBoxDataStore.prototype._queryTimestampRangeWorker = function(key, callback) {
    //Default values (no data).
    var startTimestamp = 0;
    var endTimestamp = 0;

    //Parameter Validations.
    if (!key)
        throw 'key cannot be null /empty';

    if (!callback)
        throw 'callback cannot be null /empty';

    //Querying only one element, inclusive, at a time.  0 for first; -1 for last (rewinding).
    //
    //NOTE:  Because we are in control of the datasets, and we *KNOW* the data is embedded with
    //       the timestamp, we can simply just query the timestamp from the value within the
    //       JSON object.  In the event we did not have that, we would have to chain the reply
    //       data into a 'ZSCORE' command for the timestamp (i.e. our version is simplified).
    this.m_redisC.zrange(key, 0, 0, function(err, reply) {
        //We expect an array of 1 (since we asked for first element).
        if (!err && reply && 1 === reply.length)
        {
            //It should be a JSON object.
            var firstObj = JSON.parse(reply[0]);

            startTimestamp = firstObj.Timestamp;

            //Now, query out the second (last) element, inclusive.
            this.m_redisC.zrange(key, -1, -1, function(err, reply) {
                //Again, we expect an array of 1 (requested last element).
                if (!err && reply && 1 === reply.length)
                {
                    //Parse the JSON object.
                    var lastObj = JSON.parse(reply[0]);

                    endTimestamp = lastObj.Timestamp;
                }
                else
                    startTimestamp = 0;

                //Raise the callback with final timestamps (or error).
                callback(err, startTimestamp, endTimestamp);
            }.bind(this));
        }
        else
            callback(err, startTimestamp, endTimestamp);
    }.bind(this));
}

/**
 * Worker routine for Redis DB GET operations.
 *
 * @param key Key to query.
 * @param callback Callback of type function(err, obj) to be invoked upon a query.
 * @remarks O(1)
 */
MooseBoxDataStore.prototype._getValueWorker = function(key, callback) {
    //Parameter Validations.
    if (!key)
        throw 'key cannot be null /empty';

    if (!callback)
        throw 'callback cannot be null /empty';

    //Send the Redis GET command for the key.
    this.m_redisC.get(key, function(err, reply) {
        var obj = {};

        //If we have a non-null-reply, build the JSON object.
        if (!err && reply)
            obj = JSON.parse(reply);

        //Raise the callback with results.
        callback(err, obj);
    }.bind(this));
}

/**
 * Worker routine for Redis DB SET operations.
 *
 * @param key Key to set.
 * @param jsonObj JSON object to store.
 * @param callback Optional callback of type function(err) to be invoked for error infomation.
 */
MooseBoxDataStore.prototype._setValueWorker = function(key, jsonObj, callback) {
    //Parameter Validations.
    if (!key)
        throw 'key cannot be null /empty';

    if (!jsonObj)
        throw 'jsonObj cannot be null / empty';

    //Create JSON text from the object.
    var jsonText = JSON.stringify(jsonObj);

    //Send the Redis SET command; there is no expiry to config objects.
    this.m_redisC.set(key, jsonText, function(err, reply) {
        //Trim off the reply portion and just raise callback with error.
        if (callback) //Callback is optional.
            callback(err);
    }.bind(this));
}

//Export this class outside this file.
module.exports = MooseBoxDataStore;
