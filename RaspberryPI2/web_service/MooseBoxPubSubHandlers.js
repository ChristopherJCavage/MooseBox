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
var email = require('./node_modules/emailjs/email');

/**
 * Defines an abstraction for a module of handler methods specific to the RedisDB Pub/Sub.
 * This is the main/top-level of the application, and so we have knowledge of all the 
 * other top-level component instances (e.g. alarms, Redis, etc) to glue logic together.
 *
 * @param mooseBoxPubSubRef Reference to the MooseBox Pub/Sub.
 * @param mooseBoxDataStoreRef Reference to the MooseBox DataStore.
 * @param temperatureAlarmRef Reference to the TemperatureAlarm object.
 * @param fanAutomationRef Reference to the FanAutomation object.
 */
function MooseBoxPubSubHandlers(mooseBoxPubSubRef, mooseBoxDataStoreRef, temperatureAlarmRef, fanAutomationRef) {
    //Parameter Validations.
    if (!mooseBoxPubSubRef)
        throw 'mooseBoxPubSubRef cannot be null';

    if (!mooseBoxDataStoreRef)
        throw 'mooseBoxDataStoreRef cannot be null';

    if (!temperatureAlarmRef)
        throw 'temperatureAlarmRef cannot be null';

    if (!fanAutomationRef)
        throw 'fanAutomationRef cannot be null';

    //Set Members.
    this.m_mooseBoxPubSubRef = mooseBoxPubSubRef;
    this.m_mooseBoxDataStoreRef = mooseBoxDataStoreRef;
    this.m_temperatureAlarmRef = temperatureAlarmRef;
    this.m_fanAutomationRef = fanAutomationRef;

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
    console.log2('Quried Sensor SNs: ' + JSON.stringify(serialNumbers));

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
    console.log('Sensor SNs Published: ' + JSON.stringify(serialNumbers));

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
    console.log2('SN: ' + readingObj.SerialNumber + ', ' + readingObj.Celsius + ' C, ' + convertToFahrenheit(readingObj.Celsius) + ' F');

    //For every reading that comes in, from every sensor, check it to see if we should send an email.
    var emailAddressList = this.m_temperatureAlarmRef.checkReading(readingObj.SerialNumber, readingObj.Celsius);

    console.log2('Temperature Alarms Checked (Tripped: ' + emailAddressList.length + ')');

    //Send emails to all subscribers in which this condition triggered an alarm.
    for (i = 0; i < emailAddressList.length; i++)
        this._sendAlarmNotificationEmail(emailAddressList[i], readingObj);

    //Again, for every reading from every sensor, ascertain how the 1...N fans need to be powered.
    var fanPowerCtrlList = this.m_fanAutomationRef.getPowerStateInstructions(readingObj.SerialNumber, readingObj.Celsius);

    console.log2('Fan Automation Instructions (Count: ' + fanPowerCtrlList.length + ')');

    //Now, add this to our historical data and instruct the Fan Ctrl Daemon on power states.
    for (i = 0; i < fanPowerCtrlList.length; i++)
    {
        console.log2(' > FanCtrl. SN: ' + readingObj.SerialNumber + ' Fan: ' + fanPowerCtrlList[i].FanNumber + ', Power: ' + fanPowerCtrlList[i].PowerOn);

        this.m_mooseBoxPubSubRef.publishFanCtrlReq(fanPowerCtrlList[i].FanNumber,
                                                   fanPowerCtrlList[i].PowerOn,
                                                   readingObj.Timestamp);

        this.m_mooseBoxDataStoreRef.addFanCtrlReading(fanPowerCtrlList[i].FanNumber,
                                                      fanPowerCtrlList[i].PowerOn,
                                                      readingObj.Timestamp,
                                                      function(err) {
                                                          if (err)
                                                              console.log2('Fan Ctrl Data Add Error. Err: ' + err);
                                                      });
    }
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
    for(i = 0; i < serialNumbers.length; i++)
    {
        console.log2('T-Probe SN: ' + serialNumbers[i]);

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
 * Sends an email via the MooseBox GMail account with Temperature Alarm details.
 *
 * @param emailAddress Recipient email address (i.e. subscriber).
 * @param readingObj Temperature reading that triggered the alarm.
 */
MooseBoxPubSubHandlers.prototype._sendAlarmNotificationEmail = function(emailAddress, readingObj) {
    //Parameter Validations.
    if (!emailAddress)
        throw 'emailAddress cannot be null';

    if (!readingObj)
        throw 'readingObj cannot be null';

    //Make a meaningful email message; note that, we use \n instead of <br>.  No markups.
    var contentsObj = { text:    '', 
                        from:    'MooseBox - DoNotReply <' + process.env.MOOSEBOX_EMAIL_USER + '@gmail.com>', 
                        to:      emailAddress,
                        subject: 'MooseBox Temperature Alarm Triggered!' };

    contentsObj.text += 'THIS IS AN AUTO-GENERATED MESSAGE. PLEASE DO NOT REPLY.\n';
    contentsObj.text += '\n';
    contentsObj.text +=  '------ MooseBox Temperature Alarm Details ------\n';
    contentsObj.text += '\n';
    contentsObj.text += '\tTimestamp (UTC):  ' + dateFormat(readingObj.Timestamp, 'yyyy-mm-dd hh:MM:ss') + '\n';
    contentsObj.text += '\tSerial Number: ' + readingObj.SerialNumber + '\n';
    contentsObj.text += '\tTemperature: ' + readingObj.Celsius + ' C, ' + convertToFahrenheit(readingObj.Celsius) + ' F\n';

    //MooseBox has a unique GMail account setup for alarming.  We do not hardcode this information however.
    var configObj = { user:     process.env.MOOSEBOX_EMAIL_USER,
                      password: process.env.MOOSEBOX_EMAIL_PASS,
                      host:     'smtp.gmail.com', 
                      ssl:      true };

    var mailserver  = email.server.connect(configObj);

    mailserver.send(contentsObj, function(err, message) {
        //Report error to console; we don't really have any other avenue for this.
        if (err)
            console.log2('Email Error: ' + err);
    });
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
