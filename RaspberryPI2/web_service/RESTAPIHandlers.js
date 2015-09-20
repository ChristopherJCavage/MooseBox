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
var FanAutomation = require('./FanAutomation.js');
var MooseBoxDataStore = require('../common/data_store/MooseBoxDataStore.js');
var TemperatureAlarm = require('./TemperatureAlarm.js');

var bodyParser = require('body-parser');
var express = require('express');

/**
 * Defines all supported REST API versions of MooseBox.
 */
RESTAPIHandlers.prototype.API_VERSION_v1_0 = 'v1.0';

/**
 * Defines an abstraction for a module of handler methods specific to the REST API.
 * This is the main/top-level of the application, and so we have knowledge of all the 
 * other top-level component instances (e.g. alarms, Redis, etc) to glue logic together.
 *
 * @param mooseBoxPubSubRef Reference to the MooseBox Pub/Sub.
 * @param mooseBoxDataStoreRef Reference to the MooseBox DataStore.
 * @param temperatureAlarmRef Reference to the TemperatureAlarm object.
 * @param fanAutomationRef Reference to the FanAutomation object.
 * @remarks All handler methods below will be commented with the REST API interface, as 
 *          opposed to the JavaScript routine interface.
 */
function RESTAPIHandlers(mooseBoxPubSubRef, mooseBoxDataStoreRef, fanAutomationRef, temperatureAlarmRef) {
    var port = process.env.PORT || 8080;

    //Parameter Validations.
    if (!mooseBoxPubSubRef)
        throw 'mooseBoxPubSubRef cannot be null';

    if (!mooseBoxDataStoreRef)
        throw 'mooseBoxDataStoreRef cannot be null';

    if (!fanAutomationRef)
        throw 'fanAutomationRef cannot be null';

    if (!temperatureAlarmRef)
        throw 'temperatureAlarmRef cannot be null';

    //Set Members
    this.m_express = new express();
    this.m_router = express.Router();

    this.m_mooseBoxDataStoreRef = mooseBoxDataStoreRef;
    this.m_mooseBoxPubSubRef = mooseBoxPubSubRef;
    this.m_fanAutomationRef = fanAutomationRef;
    this.m_temperatureAlarmRef = temperatureAlarmRef;

    //Configure Express routing.
    this.m_express.use(bodyParser.urlencoded({ extended: true }));
    this.m_express.use(bodyParser.json());

    //Prefex all routes with /MooseBox/API/v1.0.
    this.m_express.use('/MooseBox/API/' + this.API_VERSION_v1_0, this.m_router);

    //Now, register all routes handled by MooseBox.
    
                                               /* DELETE */
    this.m_router.route('/alarms/temperature/unregister').delete(this.onTemperatureAlarmUnregister.bind(this));

    this.m_router.route('/automation/fan/unregister').delete(this.onFanAutomationUnregister.bind(this));

                                                /* GET */
    this.m_router.route('/alarms/temperature/list').get(this.onTemperatureAlarmList.bind(this));

    this.m_router.route('/automation/fan/list').get(this.onFanAutomationList.bind(this));

    this.m_router.route('/peripherals/fan/data/query').get(this.onFanCtrlDataQuery.bind(this));
    this.m_router.route('/peripherals/fan/timestamps/query').get(this.onFanCtrlTimestampsQuery.bind(this));

    this.m_router.route('/peripherals/temperature/data/query').get(this.onTemperatureSensorDataQuery.bind(this));
    this.m_router.route('/peripherals/temperature/serial_numbers/query').get(this.onTemperatureSensorSerialNumbersQuery.bind(this));
    this.m_router.route('/peripherals/temperature/timestamps/query').get(this.onTemperatureSensorTimestampsQuery.bind(this));

                                                /* PUT */
    this.m_router.route('/alarms/temperature/register').put(this.onTemperatureAlarmRegister.bind(this));

    this.m_router.route('/automation/fan/register').put(this.onFanAutomationRegister.bind(this));

    this.m_router.route('/control/fan/power').put(this.onFanCtrlPower.bind(this));

    //Start the RESTful web service.
    this.m_express.listen(port);
}

                                /**************************/
                                /*** HTTP DELETE METHOD ***/
                                /**************************/

/**
 * DELETE /MooseBox/API/v1.0/automation/fan/unregister
 *
 * Summary:
 *   Unegisters a USB Fan from the automation instance.
 *
 * Parameters:
 *   fan_number - Required - USB Fan number to unregister.
 *
 * Responses:
 *   200 - OK
 *   400 - Bad Request
 *   500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onFanAutomationUnregister = function(req, res) {
    try {
        console.log2('REST onFanAutomationUnregister FN: ' + req.query.fan_number);

        //Validate required parameters.
        if (!req.query.fan_number)
            res.status(400).send('Missing Parameter(s)');
        else 
        {
            //Attempt to unregister the fan, if it's registered.
            var isChanged = this.m_fanAutomationRef.unregisterFan(parseInt(req.query.fan_number));

            //Next, synchronize to the DataStore (if applicable).
            if (false === isChanged)
                res.status(200).end();
            else
                this._saveFanAutomation(function(err) {
                    if (!err)
                        res.status(200).end();
                    else
                        res.status(500).send(err);
                }.bind(this));
        }
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * DELETE /MooseBox/API/v1.0/alarms/temperature/unregister
 *
 * Summary:
 *   Unegisters an email address from either *ONE* specific temperature sensor or *ALL* temperature sensors.
 *
 * Parameters:
 *   email_address - Required - Email address to send alarm notifications to.
 *   serial_number - Optional - Sensor serial number to restrict unregistration to.
 *
 * Responses:
 *   200 - OK
 *   400 - Bad Request
 *   500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onTemperatureAlarmUnregister = function(req, res) {
    try {
        console.log2('REST onTemperatureAlarmUnregister SN: ' + String(req.query.serial_number) + ', Email: ' + String(req.query.email_address));

        //All query parameters are required.
        if (!req.query.serial_number || !req.query.email_address)
            res.status(400).send('Missing Parameter(s)');
        else
        {
            //First, unregister with the TemperatureAlarm; note that, the second parameter is optional.
            var isChanged = this.m_temperatureAlarmRef.unregisterEmailAddress(req.query.email_address, req.query.serial_number);

            //Next, synchronize to the DataStore (if applicable).
            if (false === isChanged)
                res.status(200).end();
            else
                this._saveTemperatureAlarm(function(err) {
                    if (!err)
                        res.status(200).end();
                    else
                        res.status(500).send(err);
                }.bind(this));
        }
    }
    catch(err) {
        res.status(500).send(err);
    }
}

                                /***********************/
                                /*** HTTP GET METHOD ***/
                                /***********************/

/**
 * GET /MooseBox/API/v1.0/peripherals/fan/data/query
 *
 *   Summary:
 *     Queries fan control data by timestamps or the current available reading.
 *
 *   Parameters:
 *     fan_number      - Required - USB fan number to query control data from.
 *     timestamp_start - Optional - Start timestamp to query historical data from.  Must be paired with 'timestamp_end'.
 *     timestamp_end   - Optional - End timestamp to query historical data from.  Must be paired with 'timestamp_start'.
 * 
 *   Responses:
 *     200 - OK
 *       JSON Payload Example:
 *
 *        [
 *            {
 *                "Timestamp": 123456788,
 *                "PowerOn": true
 *            },
 *            {
 *                "Timestamp": 123456789,
 *                "PowerOn": false
 *            },
 *
 *            //et cetera...
 *        ] 
 *
 *     204 - No Content 
 *     400 - Bad Request
 *     500 - Internal Server Error
 *     507 - Insufficient Storage
 */
RESTAPIHandlers.prototype.onFanCtrlDataQuery = function(req, res) {
    try {
        console.log2('REST onFanCtrlDataQuery Fan #: ' + String(req.query.fan_number) + ', Start: ' + String(req.query.timestamp_start) + ', End: ' + String(req.query.timestamp_end));

        //All queries require a USB fan number.
        if (!req.query.fan_number)
            res.status(400).send('Query requires a USB fan Number.');
        else if (!req.query.timestamp_start && req.query.timestamp_end)
            res.status(400).send('Historical data query requires a start timestamp.');
        else if (req.query.timestamp_start) //Historical Data Query?
        {
            //Default the last timestamp to the current reading.
            var endTimestamp = Date.now();

            if (req.query.timestamp_end)
                endTimestamp = req.query.timestamp_end;

            //Query the data store for our results.
            this.m_mooseBoxDataStoreRef.queryHistoricalFanCtrl(req.query.fan_number,
                                                               req.query.timestamp_start,
                                                               endTimestamp,
                                                               function(err, fanNumber, objs) {
                //If we successfully queried data, response it; otherwise, distinguish between no data and errors.
                if (!err && objs && 0 !== objs.length)
                    res.status(200).json(objs);
                else if (!err)
                    res.status(204).send('No Data Available. Start: ' + req.query.timestamp_start + ', End: ' + endTimestamp);
                else
                    res.status(500).send(err);
            }.bind(this));
        }
        else //We are performing a current data query.
            this.m_mooseBoxDataStoreRef.queryCurrentFanCtrl(req.query.fan_number, function(err, fanNumber, obj) {
                if (!err && obj && false === _isEmpty(obj))
                {
                    //Build a response list of count = 1 and send.
                    var currentReading = [];

                    currentReading.push(obj);

                    res.status(200).json(currentReading);
                }
                else if ((!err && !obj) || (obj && true == _isEmpty(obj)))
                    res.status(204).send('No Data Available');
                else
                    res.status(500).send(err);
            });
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * GET /MooseBox/API/v1.0/peripherals/fan/timestamps/query
 *
 *   Summary:
 *     Queries the start and end timestamps of all fan control data (i.e. duty cycle). Timestamps are local to MooseBox.
 *
 *   Parameters:
 *     fan_number - Required - USB fan number to query control timestamps from.
 *
 *   Responses:
 *     200 - OK
 *       JSON Payload Example:
 *
 *         {
 *             "StartTimestamp": 12345678,
 *             "EndTimestamp": 87654321
 *         }
 *
 *     204 - No Content
 *     400 - Bad Request
 *     500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onFanCtrlTimestampsQuery = function(req, res) {
    try {
        console.log2('REST onFanCtrlDataQuery Fan #: ' + String(req.query.fan_number));

        //All queries require a USB Port / Fan Number.
        if (!req.query.fan_number)
            res.status(400).send('Query requires a Fan Number.');
        else
            this.m_mooseBoxDataStoreRef.getFirstLastFanCtrlTimestamps(req.query.fan_number, function(err, startTimestamp, endTimestamp) {
                //If successful, forward data in a JSON object; otherwise, distinguish between no data and error.
                if (!err && startTimestamp && endTimestamp)
                {
                    var obj = new Object();

                    obj.StartTimestamp = startTimestamp;
                    obj.EndTimestamp = endTimestamp;

                    res.status(200).json(obj);
                }
                else if (!err && !startTimestamp && !endTimestamp)
                    res.status(204).send('No Data Available');
                else
                    res.status(500).send(err);
            });
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * GET /MooseBox/API/v1.0/automation/fan/list
 *
 *   Summary:
 *     Lists current registration information (global).
 * 
 *   Responses:
 *     200 - OK
 *       JSON Payload Example: See 'FanAutomation.js' constructor.
 *
 *     204 - No Content
 *     500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onFanAutomationList = function(req, res) {
    try {
        console.log2('REST onFanAutomationList');

        //Like Temperature Alarms, just report the config object; it has everything anyway.
        var configObj = this.m_fanAutomationRef.getConfigObj();

        //Response it.
        res.status(200).json(configObj);
    }
    catch(err) {
        res.status(500).send(err);
    }

}

/**
 * GET /MooseBox/API/v1.0/alarms/temperature/list
 *
 *   Summary:
 *     Lists current registration information (global).
 * 
 *   Responses:
 *     200 - OK
 *       JSON Payload Example: See 'TemperatureAlarm.js' constructor.
 *
 *     204 - No Content
 *     500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onTemperatureAlarmList = function(req, res) {
    try {
        console.log2('REST onTemperatureAlarmList');

        //Currently, we don't need to use the independent email/serial accessors; the config object is everything need!
        var configObj = this.m_temperatureAlarmRef.getAlarmsConfig();

        //Response it.
        res.status(200).json(configObj);
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * GET /MooseBox/API/v1.0/peripherals/temperature/data/query
 *
 *   Summary:
 *     Queries temperature data by timestamps or the current available reading.
 *
 *   Parameters:
 *     serial_number   - Required - Sensor serial number to query temperature data from.
 *     timestamp_start - Optional - Start timestamp to query historical data from.  Must be paired with 'timestamp_end'.
 *     timestamp_end   - Optional - End timestamp to query historical data from.  Must be paired with 'timestamp_start'.
 * 
 *   Responses:
 *     200 - OK
 *       JSON Payload Example:
 *
 *        [
 *            {
 *                "Timestamp": 123456788,
 *                "Celsius": 42
 *            },
 *            {
 *                "Timestamp": 123456789,
 *                "Celsius": 43
 *            },
 *
 *            //et cetera...
 *        ] 
 *
 *     204 - No Content 
 *     400 - Bad Request
 *     500 - Internal Server Error
 *     507 - Insufficient Storage
 */
RESTAPIHandlers.prototype.onTemperatureSensorDataQuery = function(req, res) {
    try {
        console.log2('REST onTemperatureSensorDataQuery SN: ' + String(req.query.serial_number) + ', Start: ' + String(req.query.timestamp_start) + ', End: ' + String(req.query.timestamp_end));

        //All queries require a sensor serial number.
        if (!req.query.serial_number)
            res.status(400).send('Query requires a iButtonLink T-Probe Serial Number.');
        else if (!req.query.timestamp_start && req.query.timestamp_end)
            res.status(400).send('Historical data query requires a start timestamp.');
        else if (req.query.timestamp_start) //Historical Data Query?
        {
            //Default the last timestamp to the current reading.
            var endTimestamp = Date.now();

            if (req.query.timestamp_end)
                endTimestamp = req.query.timestamp_end;

            //Query the data store for our results.
            this.m_mooseBoxDataStoreRef.queryHistoricalTemperatures(req.query.serial_number,
                                                                    req.query.timestamp_start,
                                                                    endTimestamp,
                                                                    function(err, serialNumber, objs) {
                //If we successfully queried data, response it; otherwise, distinguish between no data and errors.
                if (!err && objs && 0 !== objs.length)
                    res.status(200).json(objs);
                else if (!err)
                    res.status(204).send('No Data Available. Start: ' + req.query.timestamp_start + ', End: ' + endTimestamp);
                else
                    res.status(500).send(err);
            }.bind(this));
        }
        else //We are performing a current data query.
            this.m_mooseBoxDataStoreRef.queryCurrentTemperature(req.query.serial_number, function(err, serialNumber, obj) {
                if (!err && obj && false === _isEmpty(obj))
                {
                    //Build a response list of count = 1 and send.
                    var currentReading = [];

                    currentReading.push(obj);

                    res.status(200).json(currentReading);
                }
                else if ((!err && !obj) || (obj && true == _isEmpty(obj)))
                    res.status(204).send('No Data Available');
                else
                    res.status(500).send(err);
            });
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * GET /MooseBox/API/v1.0/peripherals/temperature/serial_numbers/query
 *
 *   Summary:
 *     Queries a list of all supported iButtonLink T-Probe Temperature Sensors attached/supported by MooseBox.
 * 
 *   Parameters:
 *     None
 *
 *   Responses:
 *     200 - OK
 *       JSON Payload Example:
 *
 *         [
 *             "FFF0123400FF",
 *             "FFF0123500FF",
 *             //et cetera....
 *         ]
 *
 *     204 - No Content Available
 *     500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onTemperatureSensorSerialNumbersQuery = function(req, res) {
    try {
        console.log2('REST onTemperatureSensorSerialNumbersQuery');

        this.m_mooseBoxDataStoreRef.getTemperatureSensorSerialNumbers(function(err, obj) {
            //If the query was successful, forward it; otherwise, distinguish between no data and error.
            if (!err && obj && 0 !== obj.length)
                res.status(200).json(obj);
            else if (obj)
                res.status(204).send('No Data Available');
            else
                res.status(500).send(err);
        });
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * GET /MooseBox/API/v1.0/peripherals/temperature/timestamps/query
 *
 *   Summary:
 *     Queries the start and end timestamps of all historical temperature data. Timestamps are local to MooseBox.
 *
 *   Parameters:
 *     serial_number - Required - Sensor serial number to query temperature timestamps from.
 *
 *   Responses:
 *     200 - OK
 *       JSON Payload Example:
 *
 *         {
 *             "StartTimestamp": 12345678,
 *             "EndTimestamp": 87654321
 *         }
 *
 *     204 - No Content
 *     400 - Bad Request
 *     500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onTemperatureSensorTimestampsQuery = function(req, res) {
    try {
        console.log2('REST onTemperatureSensorTimestampsQuery SN: ' + String(req.query.serial_number));

        //All queries require a sensor serial number.
        if (!req.query.serial_number)
            res.status(400).send('Query requires a iButtonLink T-Probe Serial Number.');
        else
            this.m_mooseBoxDataStoreRef.getFirstLastTemperatureTimestamps(req.query.serial_number, function(err, startTimestamp, endTimestamp) {
                //If successful, forward data in a JSON object; otherwise, distinguish between no data and error.
                if (!err && startTimestamp && endTimestamp)
                {
                    var obj = new Object();

                    obj.StartTimestamp = startTimestamp;
                    obj.EndTimestamp = endTimestamp;

                    res.status(200).json(obj);
                }
                else if (!err && !startTimestamp && !endTimestamp)
                    res.status(204).send('No Data Available');
                else
                    res.status(500).send(err);
            });
    }
    catch(err) {
        res.status(500).send(err);
    }
}

                                /***********************/
                                /*** HTTP PUT METHOD ***/
                                /***********************/

/**
 * PUT /MooseBox/API/v1.0/alarms/temperature/register
 *
 * Summary:
 *   Registers an email address with temperature thresholds for alarming.
 *
 * Parameters:
 *   serial_number - Required - Sensor serial number to register against (i.e. listen to temperatures on).
 *   celsius_min   - Required - Inclusive minimum temperature allowed.
 *   celsius_max   - Required - Inclusive maximum temperature allowed.
 *   email_address - Required - Email address to send alarm notifications to.
 *
 * Responses:
 *   200 - OK
 *   400 - Bad Request
 *   500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onTemperatureAlarmRegister = function(req, res) {
    try {
        console.log2('REST onTemperatureAlarmRegister SN: ' + String(req.query.serial_number) + 
                     ', Min: ' + String(req.query.celsius_min) +
                     ', Max: ' + String(req.query.celsius_max) + 
                     ', Email: ' + String(req.query.email_address));

        //All query parameters are required.
        if (!req.query.serial_number ||
            !req.query.celsius_min ||
            !req.query.celsius_max ||
            !req.query.email_address)
            res.status(400).send('Missing Parameter(s)');
        else
        {
            //First, register with the TemperatureAlarm; if this entry exists it just overwrites (i.e. Update).
            this.m_temperatureAlarmRef.register(req.query.serial_number,
                                                parseFloat(req.query.celsius_min),
                                                parseFloat(req.query.celsius_max),
                                                req.query.email_address);

            //Now, because it mutated (i.e. NEW, UPDATE), store it in the DataStore.
            this._saveTemperatureAlarm(function(err) {
                if (!err)
                    res.status(200).end();
                else
                    res.status(500).send(err);
            }.bind(this));
        }
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * PUT /MooseBox/API/v1.0/automation/fan/register
 *
 * Summary:
 *   Registers a USB Fan to a temperature sensor with a threshold for automation power on/off.
 *
 * Parameters:
 *   serial_number     - Required - Sensor serial number to register against (i.e. listen to temperatures on).
 *   fan_number        - Required - USB Fan number to register.
 *   celsius_threshold - Required - Highest tolerable temperature reading, in celsius, before powering the fan.
 *
 * Responses:
 *   200 - OK
 *   400 - Bad Request
 *   409 - Conflict
 *   500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onFanAutomationRegister = function(req, res) {
    try {
        console.log2('REST onFanAutomationRegister SN: ' + req.query.serial_number + ', FN: ' + req.query.fan_number + ', Thres: ' + req.query.celsius_threshold);

        //All parameters are required; check now.
        if (!req.query.serial_number || 
            !req.query.fan_number || 
            !req.query.celsius_threshold)
            res.status(400).send('Missing Parameter(s)');
        else if (true === this.m_fanAutomationRef.isRegistered(parseInt(req.query.fan_number)))
            res.status(409).send('USB Fan is already registered');
        else
        {
            //We are cleared to registered; do so now.
            this.m_fanAutomationRef.register(req.query.serial_number,
                                             parseInt(req.query.fan_number),
                                             parseFloat(req.query.celsius_threshold));

            //Synchronize registration with RedisDB.
            this._saveFanAutomation(function(err) {
                if (!err)
                    res.status(200).end();
                else
                    res.status(500).send(err);
            }.bind(this));
        }
    }
    catch(err) {
        res.status(500).send(err);
    }
}

/**
 * PUT /MooseBox/API/v1.0/control/fan/power
 *
 * Summary:
 *   Manually powers on/off a USB fan in the event it does not conflict with Fan Automation.
 *
 * Parameters:
 *   fan_number - Required - USB Fan number to register.
 *   power_on   - Required - true to power the USB fan on; false to power off.
 *
 * Responses:
 *   200 - OK
 *   400 - Bad Request
 *   409 - Conflict
 *   500 - Internal Server Error
 */
RESTAPIHandlers.prototype.onFanCtrlPower = function(req, res) {
    try {
        console.log2('onFanCtrlPower FN: ' + req.query.fan_number + ', PowerOn: ' + req.query.power_on);

        //All parameters are required; check now.
        if (!req.query.fan_number || !req.query.power_on)
            res.status(400).send('Missing Parameter(s)');
        else if (true === this.m_fanAutomationRef.isRegistered(parseInt(req.query.fan_number)))
            res.status(409).send('Manual power on/off conflicts with automation.');
        else
        {
            var fanNumber = parseInt(req.query.fan_number);
            var powerOn = JSON.parse(req.query.power_on);
            var timestamp = Date.now();

            //Publish the change to the daemon.
            this.m_mooseBoxPubSubRef.publishFanCtrlReq(fanNumber, powerOn, timestamp);

            //Update the DataStore.
            this.m_mooseBoxDataStoreRef.addFanCtrlReading(fanNumber,
                                                          powerOn,
                                                          timestamp,
                                                          function(err) {
                                                              if (!err)
                                                                  res.status(200).end();
                                                              else
                                                                  res.status(500).send(err);
                                                          });
        }
    }
    catch(err) {
        res.status(500).send(err);
    }
}

                                /*******************/
                                /*** PRIVATE API ***/
                                /*******************/

/**
 * Worker method to query current state of FanAutomation and stores it in RedisDB.
 *
 * @param callback Function of type function(err) to be invoked upon completion.
 */
RESTAPIHandlers.prototype._saveFanAutomation = function(callback) {
    //Parameter Validations.
    if (!callback)
        throw 'callback cannot be null';

    //Query the configuration data.
    var configObj = this.m_fanAutomationRef.getConfigObj();

    //Write it to RedisDB.
    this.m_mooseBoxDataStoreRef.setFanCtrlConfig(configObj, callback);
}

/**
 * Worker method to query current state of the TemperatureAlarm and stores it in RedisDB.
 *
 * @param callback Function of type function(err) to be invoked upon completion.
 */
RESTAPIHandlers.prototype._saveTemperatureAlarm = function(callback) {
    //Parameter Validations.
    if (!callback)
        throw 'callback cannot be null';

    //Query the configuration data.
    var configObj = this.m_temperatureAlarmRef.getAlarmsConfig();

    //Write it to RedisDB.
    this.m_mooseBoxDataStoreRef.setTemperatureConfig(configObj, callback);
}

/**
 * Worker method to test if an object is empty.
 *
 * @param obj Object to test.
 * @return true if empty; false otherwise.
 * @remarks MooseBox does not currently use JQuery, so we include this method.
 * @see http://stackoverflow.com/questions/679915/how-do-i-test-for-an-empty-javascript-object
 */
RESTAPIHandlers.prototype._isEmpty = function(obj) {
    var result = true;

    for(var prop in obj)
        if(obj.hasOwnProperty(prop))
        {
            result = false;

            break;
        }
    
    return result;
}

//Export this class outside this file.
module.exports = RESTAPIHandlers;





























