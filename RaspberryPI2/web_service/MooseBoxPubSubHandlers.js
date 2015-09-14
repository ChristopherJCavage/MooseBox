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
var MooseBoxDataStore = require('../common/data_store/MooseBoxDataStore.js');
var MooseBoxPubSub = require('../common/data_store/MooseBoxPubSub.js');
var MooseBoxRedisDefaults = require('../common/data_store/MooseBoxRedisDefaults.js');

var dateFormat = require('dateformat');

/**
 * Defines an abstraction for a module of handler methods specific to the RedisDB Pub/Sub.
 * This is the main/top-level of the application, and so we have knowledge of all the 
 * other top-level component instances (e.g. alarms, Redis, etc) to glue logic together.
 *
 * @param mooseBoxPubSubRef Reference to the MooseBox Pub/Sub.
 * @param mooseBoxDataStoreRef Reference to the MooseBox DataStore.
 */
function MooseBoxPubSubHandlers(mooseBoxPubSubRef, mooseBoxDataStoreRef) {
    //Parameter Validations.
    if (!mooseBoxPubSubRef)
        throw 'mooseBoxPubSubRef cannot be null';

    if (!mooseBoxDataStoreRef)
        throw 'mooseBoxDataStoreRef cannot be null';

    //Set Members.
    this.m_mooseBoxPubSubRef = mooseBoxPubSubRef;
    this.m_mooseBoxDataStoreRef = mooseBoxDataStoreRef;

    this.m_subscribedTempSerialNumSet = new Object();

    //Depending on who starts first between this web-service and the temperature daemon
    //we should protect ourselves against a possible race-condition by both subscribing
    //to and querying temperature sensor serial numbers so we can subscribe to their data.
    this.m_mooseBoxDataStoreRef.getTemperatureSensorSerialNumbers(this.onGetTemperatureSensorSerialNumbers.bind(this));

    this.m_mooseBoxPubSubRef.subscribeTemperatureSensorSerialNumbers(this.onTemperatureSensorSerialNumbersNotify.bind(this));
}

                                /******************************/
                                /*** PRIVATE API (Handlers) ***/
                                /******************************/

/**
 * Handler routine for a DataStore query operation for iButtonLink T-Probe sensor serial numbers.
 *
 * @param err Error information if applicable.
 * @param serialNumbers 0...N T-Probe sensor serial numbers.
 */
MooseBoxPubSubHandlers.prototype.onGetTemperatureSensorSerialNumbers = function(err, serialNumbers) {
    //Possibly subscribe to new temperature sensors (if no error).
    if (!err && serialNumbers)
        this._refreshTemperatureSensorSubscriptionsWorker(serialNumbers);
}

/**
 * Handler routine for a Pub/Sub notification of newly published iButtonLink T-Probe sensor serial numbers.
 *
 * @param serialNumbers 0...N T-Probe sensor serial numbers.
 */
MooseBoxPubSubHandlers.prototype.onTemperatureSensorSerialNumbersNotify = function(serialNumbers) {
    //Possibly subscribe to new temperature sensors (if no error).
    if (serialNumbers)
        this._refreshTemperatureSensorSubscriptionsWorker(serialNumbers);
} 

/**
 * Handler routine for a Pub/Sub notification of a new published iButtonLink T-Probe temperature reading.
 *
 * @param readingObj Timestamped reading object contain value in degrees Celsius.
 */
MooseBoxPubSubHandlers.prototype.onTemperatureReadingNotify = function(readingObj) {
    //Pretty-print to console for devel-purposes.
    var timestampStr = dateFormat('yyyy-mm-dd_hh:MM:ss');

    console.log(timestampStr + ' [' + readingObj.SerialNumber + '] - ' + readingObj.Celsius + ' C, ' + convertToFahrenheit(readingObj.Celsius) + ' F');

    //TODO: Temperature Alarms

    //TODO: Fan Ctrl Temperature Algo
}

                                /*****************************/
                                /*** PRIVATE API (Workers) ***/
                                /*****************************/

/**
 * Refreshes subscriptions as new Temperature Sensors Serial Numbers are discovered by the MooseBox
 * Temperature Daemon.  We use a "Set" to enforce uniqueness and ensure each sensor is subscribed to
 * only once; and new sensors are subscribed to as they arrive.
 *
 * @param serialNumbers List of serial numbers for subscription candidancy.
 */
MooseBoxPubSubHandlers.prototype._refreshTemperatureSensorSubscriptionsWorker = function(serialNumbers) {
    //Parameter Validations.
    if (!serialNumbers)
        throw 'serialNumbers cannot be null';

    //Go serial # by serial #; if we don't have it in our "Set" already then we need to additionally subscribe.
    //Also, pretty-print to the console for devel-purposes.
    var timestampStr = dateFormat('yyyy-mm-dd_hh:MM:ss');

    for(i = 0; i < serialNumbers.length; i++)
    {
        //Pretty-print.
        console.log(timestampStr + ' - T-Probe Serial Number: ' + serialNumbers[i]);

        //RedisDB Subscription management.
        if (!(serialNumbers[i] in this.m_subscribedTempSerialNumSet))
        {
            //Subscribe to new temperature readings from this sensor (i.e. serial #).
            this.m_mooseBoxPubSubRef.subscribeTemperatureReadingNotify(serialNumbers[i], this.onTemperatureReadingNotify.bind(this));

            //Add it to our "Set" to ensure uniqueness.
            this.m_subscribedTempSerialNumSet[serialNumbers[i]] = true;
        }
    }
}

/**
 * Converts a temperature value in degrees Celsius to degrees Fahrenheit.
 *
 * @param celsius Temperate value in degrees Celsius to convert.
 * @return Value in degrees Fahrenheit.
 * @remarks Consider writing a JavaScript utility library for things like this...
 */
function convertToFahrenheit(celsius) {
    return celsius * (9.0 / 5.0) + 32.0;
}

//Export this class outside this file.
module.exports = MooseBoxPubSubHandlers;
