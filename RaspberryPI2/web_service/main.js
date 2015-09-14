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
var MooseBoxPubSubHandlers = require('./MooseBoxPubSubHandlers.js');
var MooseBoxRedisDefaults = require('../common/data_store/MooseBoxRedisDefaults.js');

var argv = require('minimist')(process.argv.slice(2));

//Constants
var MOOSE_BOX_WEB_SERVICE_VERSION = '0.0.1';

/**
 * Main entry point for MooseBox Web-Service.
 */
function main(argv) {
    var mooseBoxRedisDefaults = new MooseBoxRedisDefaults();
    var result = true;

    //Handle command-line arguments; prioritize requests (some are mutally exclusive).
    try {
        if (argv.h)
            displayHelp();
        else if (argv.z)
            displayVersion();
        else if (argv.f)
        {
            //Everything requires RedisDB; override connection defaults?
            var hostname = mooseBoxRedisDefaults.DEFAULT_HOSTNAME;
            var port = mooseBoxRedisDefaults.DEFAULT_PORT;

            if (argv.i)
                hostname = argv.i;

            if (argv.p)
                port = parseInt(argv.p);

            //Now, instantiate the master Pub/Sub and DataStore; we will dependency-inject
            //them into the modules of interest as opposed to having a Singleton (or something).
            mooseBoxDataStore = new MooseBoxDataStore(hostname, port);
            mooseBoxPubSub = new MooseBoxPubSub(hostname, port);

            //Setup the Pub/Sub and REST API handlers.
            mooseBoxPubSubHandlers = new MooseBoxPubSubHandlers(mooseBoxPubSub, mooseBoxDataStore);


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

//Invoke Main.
main(argv);
