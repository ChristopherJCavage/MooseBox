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
var ChainedTempSensorMonitor = require('./ChainedTempSensorMonitor.js');
var LinkUSBDS18B20 = require('./LinkUSBDS18B20.js');
var MooseBoxDataStore = require('../../common/data_store/MooseBoxDataStore.js');
var MooseBoxPubSub = require('../../common/data_store/MooseBoxPubSub.js');
var MooseBoxRedisDefaults = require('../../common/data_store/MooseBoxRedisDefaults.js');

var argv = require('minimist')(process.argv.slice(2));
var dateFormat = require('dateformat');
var fs = require('fs');

//Constants
var MOOSE_BOX_TEMPERATURE_DAEMON_VERSION = '0.0.1';

/**
 * Main entry point for MooseBox Fan Control Daemon.
 */
function main(argv) {
    var result = true;

    //Handle command-line arguments; prioritize requests (some are mutally exclusive).
    try {
        if (argv.h)
            displayHelp();
        else if (argv.z)
            displayVersion();
        else if (argv.f)
        {
            //Everything from here-on requires a configuration file.  Parse it now.
            var jsonText = fs.readFileSync(argv.f, 'utf8');

            var jsonRoot = JSON.parse(jsonText);

            //This is a one-shot query(ies) from the temperature sensor?
            //
            //If we are given one -t, it's a string; if multiple -t's it's a list.
            //That's annoying, always transform it into a list-type.
            if (!argv.d && argv.t)
                reportCurrentTemperatureStatus(jsonRoot, [].concat(argv.t));
            else if (argv.d) //Run as a daemon with RedisDB Pub/Sub.
            {
                //Setup RedisDB defaults.
                mooseBoxRedisDefaults = new MooseBoxRedisDefaults();

                var hostname = mooseBoxRedisDefaults.DEFAULT_HOSTNAME;
                var port = mooseBoxRedisDefaults.DEFAULT_PORT;

                //Override RedisDB defaults?
                if (argv.i)
                    hostname = argv.i;

                if (argv.p)
                    port = argv.p;

                //Begin long-polling temperature sensors and report with RedisDB Pub/Sub.
                monitorTemperatureSensors(jsonRoot, hostname, port);
            }
            else
                result = false;
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
 * Queries the current temperature status of 0...N daisy-chained temperature sensors
 * and pretty-prints it to the console.  Intended for manual interaction by developer.
 *
 * @param jsonConfigRoot Temperature daemon JSON config data.
 * @param ttys 0...N TTYs to query (i.e. LinkUSB devices).
 */
function reportCurrentTemperatureStatus(jsonConfigRoot, ttys) {
    //Parameter Validations.
    if (!jsonConfigRoot)
        throw 'jsonConfigRoot cannot be null';

    if (!ttys)
        throw 'ttys cannot be null';

    //We may have 0...N TTYs for the LinkUSB to RJ-45 adapters.
    for(i = 0; i < ttys.length; i++)
    {
        //Alias the TTY so we can pretty-print it in a closure.
        var tty = ttys[i];

        //Construct a new LinkUSB device which has 0...N sensors daisy-chained on it and query.
        linkUSBDS18B20 = new LinkUSBDS18B20(jsonConfigRoot.DigiTempConfigPath, tty);

        linkUSBDS18B20.readAsync(linkUSBDS18B20.SENSOR0, function(err, celsius) {
            //Pretty-print output.
            if (err)
                console.log('Temperature acquisition error (' + tty + '). Err: ' + err);
            else
                console.log('Sensor (' + tty + '): ' + celsius + ' C, ' + convertToFahrenheit(celsius) + ' F');
        });
    }
}

/**
 * Performs infinite long-polling on the 0...N T-Probe sensors daisy-chained on 0...N LinkUSB devices.
 * Data is both pretty-printed to the console and published to the MooseBox RedisDB Pub/Sub.
 * This method is intended to be the "daemon-mode" where this is run in unison with the "forever"
 * command which takes care of the actual daemonifying of this process.
 *
 * @param jsonConfigRoot Temperature Daemon JSON configuration data.
 * @param hostname MooseBox RedisDB hostname / IP address.
 * @param port MooseBox RedisDB port.
 */
function monitorTemperatureSensors(jsonConfigRoot, hostname, port) {
    //Parameter Validations.
    if (!jsonConfigRoot)
        throw 'jsonConfigRoot cannot be null';

    if (!hostname)
        throw 'hostname cannot be null';

    if (!port)
        throw 'port cannot be 0 (reserved) or null';

    //Initialize Datastore and Pub/Sub (RedisDB).
    mooseBoxDataStore = new MooseBoxDataStore(hostname, port);
    mooseBoxPubSub = new MooseBoxPubSub(hostname, port);

    //Report our version number first; unconditionally.
    mooseBoxDataStore.setFanCtrlDaemonVersion(MOOSE_BOX_TEMPERATURE_DAEMON_VERSION);

    //All of our configuration information is maintained in the Datastore. Query that first.
    var allSerialNumbers = [];
    var ttySerialNumbersMap = new Object();

    //Our JSON config should contain all the TTYs to monitor and the location of the
    //DigiTemp's configuration file.  While we're here, let's build a map of all the
    //sensors we're supporting that are daisy-chained to each LinkUSB device.
    for(i = 0; i < jsonConfigRoot.LinkUSBTTYs.length; i++)
    {
        //Alias the TTY now to make life easier.
        var tty = jsonConfigRoot.LinkUSBTTYs[i];

        //Instantiate a new LinkUSB abstraction to perform serial number parsing.
        linkUSBDS18B20 = new LinkUSBDS18B20(jsonConfigRoot.DigiTempConfigPath, tty);

        linkUSBDS18B20.getSensorSerialNumbers(function(err, serialNumbers) {
            if (!err)
            {
                //Add the list of serial numbers to the map for this TTY.
                ttySerialNumbersMap[tty] = serialNumbers

                allSerialNumbers.push(serialNumbers);

                //To avoid more callback-nesting; just give RedisDB what we have; overwriting
                //on a SET operation doesn't hurt anything at all.  More importantly, we are
                //also incrementally publishing on the Pub/Sub.  Do so now.
                mooseBoxDataStore.setTemperatureSensorSerialNumbers(allSerialNumbers);

                mooseBoxPubSub.publishTemperatureSensorSerialNumbers(serialNumbers);

                //Start monitoring this TTY and it's temperature sensors.
                chainedTempSensorMonitor = new ChainedTempSensorMonitor(jsonConfigRoot.DigiTempConfigPath, tty);

                chainedTempSensorMonitor.on('data', function(readingObj) {
                    //Pretty-print to console for devel-purposes.
                    var timestampStr = dateFormat('yyyy-mm-dd_hh:MM:ss');

                    console.log(timestampStr + ' [' + serialNumbers[0] + '] - ' + readingObj.Celsius + ' C, ' + convertToFahrenheit(readingObj.Celsius) + ' F');

                    //Publish new data to the RedisDB Pub/Sub and accumulate it in the historical readings.
                    //I wrote the RedisDB Time-series accumulator (i.e. Sorted Set) to have an optional argument
                    //to not add to the historical data.  We are polling at 1.1Hz, and sopressata takes about 
                    //three months to cure (wine is less).  So, given our environment and *FOUR* months (worst):
                    //
                    // A 1Hz approximation is close enough -
                    //   ~40bytes/reading * 60sec/min * 60min/hr * 24hr/day * 31day/mnth * 4mnths * 2temperature sensors = 428544000 bytes
                    //
                    //   428544000 bytes = 428.5 MB
                    //
                    //   Now add another 100MB for "slop" = 528.5MB
                    //
                    //The MooseBox Raspberry PI 2 has 1GB of RAM and we use a 4GB SD Card.  Given that we aren't
                    //really running any other processes and we don't have the same issue with the fan control's
                    //historical data I'm going to wager that we can sustain 100% data accumulation for the entire
                    //curing process for a winter's batch of sopressata.  This is ideal for temperature graphing!
                    //
                    //The iPhone application will have some kind of purge mechanism in the event my estimation is wrong.
                    //
                    // *NOTE: Recall that as of 9/12/2015 my father decided to not daisy-chain temperature sensors
                    //        from the LinkUSB device.  Instead, we're having one T-Probe per LinkUSB device. 
                    //        For now, that means we know we just have length:1 arrays.  TBD in the future though.
                    mooseBoxDataStore.addTemperatureReading(serialNumbers[0], readingObj.Celsius, readingObj.Timestamp, function(err, reply) {
                        if (!err)
                            console.log('Datastore Error. Err: ' + err);
                    });

                    mooseBoxPubSub.publishTemperatureReadingNotify(serialNumbers[0], readingObj.Celsius, readingObj.Timestamp);
                });

                //Begin infinite long-polling!
                chainedTempSensorMonitor.start();
            }
            else
                console.log('Sensor Serial Number Query Error. Err: ' + err);
        });
    }
}

/**
 * Displays help information to the console.
 */
function displayHelp() {
    console.log('MooseBox Temperature Daemon');
    console.log('------------------------------------------------------------------------------');
    console.log('Responsible for monitoring 0...N iButtonLink T-Probe temperature sensors');
    console.log('connected to 0...N iButtonLink LinkUSB devices.  Temperature data is both');
    console.log('pretty-printed to the console and reported to the MooseBox RedisDB Pub/Sub.');
    console.log('Single-shot temperature queries may also be performed by the SDE(T).')
    console.log();
    console.log('Options:');
    console.log('  -d Run as a daemon. Intended to be ran with \'Forever\' and RedisDB Pub/Sub.');
    console.log('  -f Path to the Temperature Daemon JSON configuration file.');
    console.log('  -h Display help information.');
    console.log('  -i Optional IP address / hostname of the MooseBox RedisDB instance.');
    console.log('  -p Optional port number of the MooseBox RedisDB instance.')
    console.log('  -t TTY of LinkUSB device; use one -t for each TTY connected to MooseBox.');
    console.log('  -z Report version information.');
}

/**
 * Displays version information to the console.
 */
function displayVersion() {
    console.log("MooseBox Temperature Daemon v" + MOOSE_BOX_TEMPERATURE_DAEMON_VERSION);
}

/**
 * Converts a temperature value in degrees Celsius to degrees Fahrenheit.
 *
 * @param celsius Temperate value in degrees Celsius to convert.
 * @return Value in degrees Fahrenheit.
 */
function convertToFahrenheit(celsius) {
    return celsius * (9.0 / 5.0) + 32.0;
}

//Invoke Main.
main(argv);
