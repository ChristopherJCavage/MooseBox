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
var YepKitYKUSH = require('./YepKitYKUSH.js');
var MooseBoxDataStore = require('../../common/data_store/MooseBoxDataStore.js');
var MooseBoxPubSub = require('../../common/data_store/MooseBoxPubSub.js');
var MooseBoxRedisDefaults = require('../../common/data_store/MooseBoxRedisDefaults.js');

var argv = require('minimist')(process.argv.slice(2));
var dateFormat = require('dateformat');

//Constants
var MOOSE_BOX_FAN_CTRL_DAEMON_VERSION = '0.0.3';

/**
 * Main entry point for MooseBox Fan Control Daemon.
 */
function main(argv) {
    var redisDefaults = new MooseBoxRedisDefaults();
    var hostname = redisDefaults.DEFAULT_HOSTNAME;
    var port = redisDefaults.DEFAULT_PORT;
    var result = true;

    //Handle command-line arguments; prioritize requests (some are mutally exclusive):
    //  1. Help Contents
    //  2. Version Information
    //  3. "One-shot" USB Power/Unpower
    //  4. RedisDB Daemon mode w/PubSub
    try {
        if (argv.h == true)
            displayHelp();
        else if (argv.z == true)
            displayVersion();
        else if (argv.x && argv.y)
            manualPowerUSBPort(argv.y, argv.x);
        else if (argv.d)
        {
            //Override defaults for RedisDB Hostname/Port.
            if (argv.i)
                hostname = argv.i;
 
            if (argv.p)
                port = parseInt(argv.p);

            //Now connect to the MooseBox RedisDB Pub/Sub for monitoring.
            automaticPowerUSBPort(hostname, port);
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
 * Allows end-user to manually power/unpower one of the three YKUSH USB ports via the command-line.
 *
 * @param port YKUSH port number.
 * @param power true to power USB port; false otherwise.
 */
function manualPowerUSBPort(port, power) {
    yepKitYKUSH = new YepKitYKUSH();

    //Convert cmd-line string arguments (deferred parameter validations).
    var ykushPort = parseInt(port);
    var yKushPower = (power.toLowerCase() === 'true');

    //Cmd-line validations.
    if (ykushPort < yepKitYKUSH.MIN_PORT_NUMBER || ykushPort > yepKitYKUSH.MAX_PORT_NUMBER)
        console.log('YKUSH Port Number must be ' + yepKitYKUSH.MIN_PORT_NUMBER + ' <= P <= ' + yepKitYKUSH.MAX_PORT_NUMBER);
    else //Power/Unpower the USB port.
        yepKitYKUSH.setPower(ykushPort, yKushPower, function(err, reply) {
            if (err)
                console.log('Failed to power/unpower USB Port ' + ykushPort + '. Err: ' + err);
            else if (true === yKushPower)
                console.log('Successfully powered USB Port ' + ykushPort);
            else
                console.log('Successfully unpowered USB Port ' + ykushPort);
        });
}

/**
 * Monitors published data from the MooseBox RedisDB Pub/Sub and powers/unpowers USB fans appropiately.
 *
 * @param hostname MooseBox RedisDB IP address / hostname.
 * @param port MooseBox RedisDB port number.
 * @remarks Routine runs infinitely.
 */
function automaticPowerUSBPort(hostname, port) {
    //Create our fan controller in an unpowered state and init RedisDB.
    mooseBoxDataStore = new MooseBoxDataStore(hostname, port);
    mooseBoxPubSub = new MooseBoxPubSub(hostname, port);
    yepKitYKUSH = new YepKitYKUSH();

    //Unconditionally store our version number; it will be displayed on the iPhone app.
    mooseBoxDataStore.setFanCtrlDaemonVersion(MOOSE_BOX_FAN_CTRL_DAEMON_VERSION);

    //Before we subscribe, ask RedisDB what all of our last states were.
    //We must do this prior to subscriptions to avoid a race-condition.
    //In the event the daemon restarted, we want to retain last state.
    //
    //Once the RedisDB replies back - SYNCHRONOUSLY - set the fan power
    //then start monitoring the Pub/Sub for requests for fan control switches.
    for(i = yepKitYKUSH.MIN_PORT_NUMBER; i <= yepKitYKUSH.MAX_PORT_NUMBER; i++)
        mooseBoxDataStore.queryCurrentFanCtrl(i, function(err, fanNumber, reply) {

            //It's possible this is the first time through (system); we could get null reply & success.
            if (!err && reply)
                yepKitYKUSH.setPowerSync(fanNumber, reply.PowerOn); 

            //With the fan state guaranteed in the starting state, now we can subscribe to new 
            //requests for this USB Fan channel number (i.e. no race-conditions).  From hereon
            //it will be asynchronous requests - as it should be! 
            mooseBoxPubSub.subscribeFanCtrlReq(fanNumber, function(obj) {
                //Pretty-print published data request; which is to say, the new fan power ("this can't fail").
                var timestampStr = dateFormat('yyyy-mm-dd_hh:MM:ss');

                console.log(timestampStr + ' - Request. Fan: ' + obj.FanNumber + ', Power: ' + obj.PowerOn);

                //Set new value, even if it's the same as the current value.
                yepKitYKUSH.setPower(obj.FanNumber, obj.PowerOn, function(err, reply) {
                    //if the operation was successful, update the DataStore; for a local process
                    //on a local machine, we're not going to bother with Pub/Sub responses.
                    if (!err && reply)
                        mooseBoxDataStore.addFanCtrlReading(obj.Port, obj.PowerOn, obj.Timestamp);
                });
            });
        });

    //That's it, from here the node Redis client keeps us running.
}

/**
 * Displays help information to the console.
 */
function displayHelp() {
    console.log('MooseBox Fan Control Daemon');
    console.log('------------------------------------------------------------------------------');
    console.log('Controller application for powering/unpowering USB fans with the YepKit YKUSH');
    console.log('USB switch. Application can be run standalone with commands entered from the');
    console.log('command-line or as a daemon in unison with \'Forever\' and RedisDB Pub/Sub.');
    console.log();
    console.log('Options:');
    console.log('  -d Run as a daemon. Intended to be ran with \'Forever\' and RedisDB Pub/Sub.');
    console.log('  -h Display help information.');
    console.log('  -i Optional IP address / hostname of the MooseBox RedisDB instance.');
    console.log('  -p Optional port number of the MooseBox RedisDB instance.')
    console.log('  -x Powers/Unpowers YepKit YKUSH USB port. Use with -y');
    console.log('  -y YepKit YKUSH USB port number to control. Use with -x');
    console.log('  -z Report version information.');
}

/**
 * Displays version information of this daemon to the console.
 */
function displayVersion()
{
    console.log("MooseBox Fan Control Daemon v" + MOOSE_BOX_FAN_CTRL_DAEMON_VERSION);
}

//Invoke Main.
main(argv);
