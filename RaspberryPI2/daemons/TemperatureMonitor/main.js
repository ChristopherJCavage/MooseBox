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
var argv = require('minimist')(process.argv.slice(2));
var dateFormat = require('dateformat');
var fs = require('fs');
var redis = require("redis");
var TemperatureMonitor = require('./TemperatureMonitor.js');

//Constants
var MOOSE_TEMPERATURE_MONITOR_VERSION = "1.0.0";

/**
 * Main entry point for MooseBox Temperature Monitor.
 */
function main(argv)
{
    var isVerbose = (argv.v !== undefined);
    
    //Handle command-line arguments; prioritize requests (some are mutally exclusive).
    if (argv.h == true)
        displayHelp();
    else if (argv.z == true)
        displayVersion();
    else
    {
        //Everything from hereon requires configuration file.
        var configFile = "./tempMonitor.config.json";

        if (argv.f !== undefined)
            configFile = argv.f;

        var jsonText = fs.readFileSync(configFile, 'utf8');

        var jsonRoot = JSON.parse(jsonText);

        //We also require hardware; if we weren't overriden with one try default.
        var tty = jsonRoot.DefaultTTY;

        if (argv.t !== undefined)
            tty = argv.t;

        //And more than likely, a connection to redis.
        var redisPubSubC = new redis.createClient(jsonRoot.Redis.Port, jsonRoot.Redis.Hostname);

        //Subscribe to new data.
        temperatureMonitor = new TemperatureMonitor(jsonRoot.DigiTempConfigPath, tty);
        
        temperatureMonitor.on('data', function(reading)
            {
                var print = isVerbose;

                //Ascertain if this is a one-shot operation or continuous mode (default).
                if (argv.y == true)
                {
                    //We assume to be in verbose mode for this operation.
                    print = true;

                    temperatureMonitor.stop();

                    //Foricbly end Redis so the Event Loop aborts and console control is returned.
                    redisPubSubC.end();
                }
                else //Using the Pub/Sub connection, publish to our dedicated temperature channel.
                    redisPubSubC.publish(jsonRoot.Redis.PubSub.Sensor0, reading);

                //Optionally pretty-print the temperature to the console.
                if (print == true)
                    console.log(dateFormat(reading.Timestamp, "yyyy-mm-dd hh:MM:ss") + ":  " + reading.Celsius + " C");
            });

        //If this is a one-shot just do it.
        if (argv.y == true)
        {
            console.log("Temperature monitoring initiated.");

            temperatureMonitor.start();
        }
        else
        {
            //Let RedisDB connectivity handle the logic...
            redisPubSubC.on('connect', function() 
                {
                    //With the database connection established, start temperature monitoring.
                    if (isVerbose == true)
                    {
                        console.log("RedisDB connected.");

                        console.log("Temperature monitoring initiated.");
                    }

                    temperatureMonitor.start();
                });

            redisPubSubC.on('error', function(err)
                {
                    //RedisDB client automatically attempts reconnects so long as we handle this event (so it doesn't except).
                    if (isVerbose == true)
                    {
                        console.log("RedisDB disconnected; reconnecting...");

                        console.log("Temperature monitoring stopped.");
                    }

                    //However, polling the temperature sensors is folly; pause it for now.
                    temperatureMonitor.stop();
                });
        }
    }
}

/**
 * Displays help information to the console.
 */
function displayHelp()
{
    console.log("MooseBox Temperature Monitor");
    console.log("------------------------------------------------------------------------");
    console.log("Monitors 0...N DS18B20 Temperautre Senors using an iButtonLink 1-wire");
    console.log("LinkUSB FTDI/RJ-45 adapter and reports their current temperature to");
    console.log("a RedisDB publish/subscribe channel and/or a verbose console message.");
    console.log();
    console.log("Options:");
    console.log("  -d Run as a daemon.");
    console.log("  -h Display help information.");
    console.log("  -f Path to configuration file (default is './tempMonitor.config.json')");
    console.log("  -t Set TTY (overrides config default)");
    console.log("  -v Verbose mode.");
    console.log("  -y No monitoring; report current temperature and quit.");
    console.log("  -z Report version information.");
}

/**
 * Displays version information to the console.
 */
function displayVersion()
{
    console.log("MooseBox Tempertuare Monitor v" + MOOSE_TEMPERATURE_MONITOR_VERSION);
}

//Invoke Main.
main(argv);
