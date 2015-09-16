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
var MooseBoxPubSub = require('../common/data_store/MooseBoxPubSub.js');
var MooseBoxPubSubHandlers = require('./MooseBoxPubSubHandlers.js');
var MooseBoxRedisDefaults = require('../common/data_store/MooseBoxRedisDefaults.js');
var TemperatureAlarm = require('./TemperatureAlarm.js');

var argv = require('minimist')(process.argv.slice(2));
var dateFormat = require('dateformat');

                                /**********************/
                                /*** Main / Startup ***/
                                /**********************/

//Public Constants
var MOOSE_BOX_WEB_SERVICE_VERSION = '0.0.1';

var TEMPERATURE_ALARM_RATE_LIMIT_MS = 1000 * 60 * 15; //15 Minutes

/**
 * Container object to hold top-level critical instances to avoid littering them
 * in the JavaScript global namespace and to allow us to bind to the container.
 */
function TheApp() {
    //MooseBox REST API & Pub/Sub Daemon Handlers.
    this.TheHandlersPubSub = null;
    this.TheHandlersREST = null;

    //MooseBox RedisDB Interface.
    this.TheMooseBoxDataStore = null;
    this.TheMooseBoxPubSub = null;

    //MooseBox Business Logic.
    this.TheFanAutomationObj = null;
    this.TheTemperatureAlarmObj = null;
}

/**
 * Main entry point for MooseBox Web-Service.
 *
 * @remarks Think of the Pub/Sub and REST API Handler.js modules as a 'partial' class.
 */
function main(argv) {
    var mooseBoxRedisDefaults = new MooseBoxRedisDefaults();
    var result = true;
    var theApp = new TheApp();

    //Handle command-line arguments; prioritize requests (some are mutally exclusive).
    try {
        if (argv.h)
            displayHelp();
        else if (argv.z)
            displayVersion();
        else if (argv.f)
        {
            //For simple logging purposes, we just want to prefix a timestamp to messages; so we'll just extend console.
            console.log2 = function(msg) {
                if (msg)
                {
                    var timestampStr = dateFormat('yyyy-mm-dd_hh:MM:ss');

                    console.log(timestampStr + ' - ' + msg);
                }
            }

            //Everything requires RedisDB; override connection defaults?
            var hostname = mooseBoxRedisDefaults.DEFAULT_HOSTNAME;
            var port = mooseBoxRedisDefaults.DEFAULT_PORT;

            if (argv.i)
                hostname = argv.i;

            if (argv.p)
                port = parseInt(argv.p);

            //Now, instantiate the master Pub/Sub and DataStore first since they are critical to everything.
            theApp.TheMooseBoxDataStore = new MooseBoxDataStore(hostname, port);
            theApp.TheMooseBoxPubSub = new MooseBoxPubSub(hostname, port);

            console.log2('RedisDB Connection Request @ ' + hostname + ':' + port);

            //MooseBox has Raspbian's initialization sequence to kick off the daemons first, then the web-service:
            //  0. [Implied] RedisDB
            //
            //  1. Temperature Daemon
            //  2. Fan Ctrl Daemon
            //  3. MooseBox WebService
            //
            //Now, then the following must be done before we can kick-off continuous running:
            //  1. Query version numbers.
            //  2. Query (if exists) the Fan Automation registration configuration object.
            //  3. Query (if exists) the Temperature Alarm registration configuration object.
            //  4. With the two instances (or nulls), setup the MooseBox Pub/Sub Handlers (RedisDB).
            //  5. Prime the Fan Automation with the current temperature reading.
            //  6. Enable the REST API.
            //    fin.
            theApp.TheMooseBoxDataStore.getFanCtrlDaemonVersion(onGetFanCtrlDaemonVersion);
            theApp.TheMooseBoxDataStore.getTemperatureDaemonVersion(onGetTemperatureDaemonVersion);
            theApp.TheMooseBoxDataStore.getFanCtrlConfig(onGetFanCtrlConfig.bind(null, theApp));
            theApp.TheMooseBoxDataStore.getTemperatureConfig(onGetTemperatureConfig.bind(null, theApp));
        }
        else
            result = false;
    }
    catch(err) {
        result = false;
    }

    //Display top-level no-op/invalid usage help string.
    if (false === result)
        console.log('Invalid usage; see -h for more information.');
}

/**
 * Displays help information to the console.
 */
function displayHelp() {
    console.log('MooseBox Temperature Daemon');
    console.log('------------------------------------------------------------------------------');
    console.log('<TODO>')
    console.log();
    console.log('Options:');
    console.log('  -d Run as a daemon. Intended to be ran with \'Forever\' and RedisDB Pub/Sub.');
    console.log('  -f Path to the Web-Service JSON configuration file.');
    console.log('  -h Display help information.');
    console.log('  -i Optional IP address / hostname of the MooseBox RedisDB instance.');
    console.log('  -p Optional port number of the MooseBox RedisDB instance.');
    console.log('  -z Report version information.');
}

/**
 * Displays version information to the console.
 */
function displayVersion() {
    console.log("MooseBox Web-Service v" + MOOSE_BOX_WEB_SERVICE_VERSION);
}

                                /**************************/
                                /*** PRIVATE (Handlers) ***/
                                /**************************/

/**
 * Handler rountine for the Fan Ctrl Daemon version number.
 *
 * @param err Error information if applicable.
 * @param versionStr Daemon version.
 */
function onGetFanCtrlDaemonVersion(err, versionStr) {
    if (!err && versionStr && 0 != versionStr.length)
        console.log2('Fan Ctrl Daemon: v' + versionStr);
}

/**
 * Handler rountine for the Temperature Daemon version number.
 *
 * @param err Error information if applicable.
 * @param versionStr Daemon version.
 */
function onGetTemperatureDaemonVersion(err, versionStr) {
    if (!err && versionStr && 0 != versionStr.length)
        console.log2('Temperature Daemon: v' + versionStr);
}

/**
 * Handler routine for the Fan Automation configuration object.
 *
 * @param theApp [Bound] Instance to the top-level instance lookup object.
 * @param err Error information if applicable.
 * @param configObj Queried configuration data, which may be null.
 */
function onGetFanCtrlConfig(theApp, err, configObj) {
    console.log2('Fan Automation ConfigObj Response. ');

    //Invoke worker logic; only create a new TemperatureAlarm if it doesn't exist.
    _configResWorker(theApp, err, configObj, function(actualConfig) {
        if (!theApp.TheFanAutomationObj)
            theApp.TheFanAutomationObj = new FanAutomation(actualConfig);
    });
}

/**
 * Handler routine for the Temperature Alarm configuration object.
 *
 * @param theApp [Bound] Instance to the top-level instance lookup object.
 * @param err Error information if applicable.
 * @param configObj Queried configuration data, which may be null.
 */
function onGetTemperatureConfig(theApp, err, configObj) {
    console.log2('TemperatureAlarm ConfigObj Response.');

    //Invoke worker logic; only create a new TemperatureAlarm if it doesn't exist.
    _configResWorker(theApp, err, configObj, function(actualConfig) {
        if (!theApp.TheTemperatureAlarmObj)
            theApp.TheTemperatureAlarmObj = new TemperatureAlarm(TEMPERATURE_ALARM_RATE_LIMIT_MS, actualConfig);
    });
}

                                /*************************/
                                /*** PRIVATE (Workers) ***/
                                /*************************/

/**
 * Worker routine to avoid redundant code for configuration data queries.
 *
 * @param theApp [Bound] Instance to the top-level instance lookup object.
 * @param err Error information if applicable.
 * @param configObj Queried configuration data, which may be null.
 * @param callback Function to invoke to perform actual object instantiation.
 */
function _configResWorker(theApp, err, configObj, callback) {
    var actualConfig = null;

    //Parameter Validations.
    if (!callback)
        throw 'callback cannot be null';

    //Overwrite the empty config if we queried a real configuration.
    if (!err && undefined !== configObj && null !== configObj)
        if (0 !== Object.keys(configObj).length)
            actualConfig = configObj;

    //Unconditionally have the caller instantiate with one of the two configs.
    callback(actualConfig);

    //Now, if both are primed then we can now setup the Pub/Sub handler methods 
    //and dependency-inject them all of their required references. When the 
    //Pub/Sub is ready, then we can (finally) get the REST API enabled!
    if (theApp.TheMooseBoxPubSub &&
        theApp.TheMooseBoxDataStore &&
        theApp.TheTemperatureAlarmObj &&
        theApp.TheFanAutomationObj)
    {
        console.log2('Starting Pub/Sub');

        theApp.TheHandlersPubSub = new MooseBoxPubSubHandlers(theApp.TheMooseBoxPubSub,
                                                              theApp.TheMooseBoxDataStore,
                                                              theApp.TheTemperatureAlarmObj,
                                                              theApp.TheFanAutomationObj);
    }
}

//Invoke Main.
main(argv);
