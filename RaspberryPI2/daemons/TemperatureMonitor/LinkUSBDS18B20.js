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
var exec = require('child_process').exec;

//Constants
LinkUSBDS18B20.prototype.MIN_TEMPERATURE_CELSIUS = -55.0;
LinkUSBDS18B20.prototype.MAX_TEMPERATURE_CELSIUS = +125.0;

LinkUSBDS18B20.prototype.MIN_POLLING_INTERVAL_MS = 900;

/**
 * Defines an abstraction for a daisy-chained DS18B20 temperature sensor to an
 * iButtonLink LinkUSB adapter (FTDI / RJ-45) for the 1-wire interface.
 *
 * As opposed to implementing the 1-wire interface logic it was discovered that
 * there exists a very excellent command-line tool that implements all of the 
 * necessary logic for various digital temperature sensors called 'digitemp'.
 * We shall simply just invoke that.
 *
 * In the event the online documentation is discontinued the following should
 * be done on the Rasberry PI 2.
 *
 * Raspbian Installation (Debian):  'sudo apt-get install digitemp'
 *
 * List Devices:  'sudo digitemp_DS9097U -w -s /dev/ttyUSB0'
 *   *note:  DS90907U is compatible with the DS18B20.
 *
 * Configure:  'sudo digitemp_DS9097U -i -c /etc/digitemp.conf -s /dev/ttyUSB0'
 * 
 * Query (all):  digitemp_DS9097U -q -c /etc/digitemp.conf -a -o %C
 *
 * @see http://veino.com/blog/?p=518
 * @see https://www.digitemp.com/software.shtml
 * @see https://cdn.shopify.com/s/files/1/0164/3524/files/LinkUSB_Users_Guide_V1.3.pdf?1957
 */
function LinkUSBDS18B20(configPath, tty)
{
    //Parameter Validation.
    if (configPath == null)
        throw "No config path";

    if (tty == null)
        throw "TTY cannot be null";

    //Set members.
    this.m_configPath = configPath;
    this.m_tty = tty;
}

/**
 * Asynchronously reads a temperature sensor for the current temperature and converts the raw reading to Celsius.
 *
 * @param num Sensor number to query, as ordered in the digitemp configuraiton file
 * @param callback Function of the type [err, temperatureCelsius].
 * @remarks Recommend having sensors sorted in the configuration file.
 */
LinkUSBDS18B20.prototype.readAsync = function(num, callback)
{
    //Tell digitemp to query the temperature sensor and display the result, without copyrights, in Celsius only.
    var cmd = "digitemp_DS9097U -q -c " + this.m_configPath + " -o %C -t " + num + " s " + this.m_tty;

    //Asynchronously invoke on the command-line.
    exec(cmd, function(error, stdout, stderr)
        {
            //If we didn't complete successfully, then it's an error.  Build a message.
            if (error || stderr)
                callback("error: " + error + ", stderr: " + stderr, 0);
            else if (stdout)
                callback(null, parseFloat(stdout)); //Cmd string enforces numeric string output and only that.
        }.bind(this));
}

//Export this class outside this file.
module.exports = LinkUSBDS18B20;
