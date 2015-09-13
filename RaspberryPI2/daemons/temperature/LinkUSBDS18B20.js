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
var fs = require('fs');

//Public Constants
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
function LinkUSBDS18B20(configPath, tty) {
    //Parameter Validation.
    if (!configPath)
        throw "No config path";

    if (!tty)
        throw "TTY cannot be null";

    //Public Constants.
    this.SENSOR0 = 0;

    //Set members.
    this.m_configPath = configPath;
    this.m_tty = tty;
    this.m_isBusy = false;
}

/**
 * Asynchronously reads a temperature sensor for the current temperature and converts the raw reading to Celsius.
 *
 * @param num Sensor number to query, as ordered in the digitemp configuraiton file
 * @param callback Function of the type [err, temperatureCelsius].
 * @remarks Recommend having sensors sorted in the configuration file.
 */
LinkUSBDS18B20.prototype.readAsync = function(num, callback) {
    //Parameter Validations.
    if (num < 0)
        throw 'num cannot be negative';

    if (!callback)
        throw 'callback cannot be null';

    //Validate no operation is pending.
    if (true === this.m_isBusy)
        throw 'I/O busy';

    this.m_isBusy = true;

    //Tell digitemp to query the temperature sensor and display the result, without copyrights, in Celsius only.
    try {
        var cmd = "digitemp_DS9097U -q -c " + this.m_configPath + " -o %C -t " + num + " s " + this.m_tty;

        //Asynchronously invoke on the command-line.
        exec(cmd, function(error, stdout, stderr) {
            //Pass or fail; we are no longer busy.
            this.m_isBusy = false;

            //If we didn't complete successfully, then it's an error.  Build a message.
            if (error || stderr)
                callback("error: " + error + ", stderr: " + stderr, 0);
            else if (stdout)
                callback(null, parseFloat(stdout)); //Cmd string enforces numeric string output and only that.
        }.bind(this));
    }
    catch(err) {
        this.m_isBusy = false;
    }
}

/**
 * Queries the state of a pending I/O request to the TTY to query daisy-chained temperature sensors.
 *
 * @return true if the LinkUSB device is busy; false otherwise.
 */
LinkUSBDS18B20.prototype.isBusy = function() {
    return this.m_isBusy;
}

/**
 * Parses out all daisy-chained temperature sensors on all supported LinkUSB TTYs that
 * are supported by the 'DigiTemp' command.  This data is all hard-fixed in DigiTemp's
 * configuration file; so, if it's not in the config and a user plugs it into MooseBox
 * the 'DigiTemp' command wont recognize it anyway.
 *
 * @param callback Callback to invoke of type function(err, serialNumbers) when the configuration
 *                 file is successfully parsed.  There may be zero or more serial numbers.
 * @remarks Only sensors associated with this instance's TTY will be reported.
 */
LinkUSBDS18B20.prototype.getSensorSerialNumbers = function(callback) {
    //Whatever is or is not connected; the only supported sensors are those in the configuration file.
    //Parse out this information from the DigiTemp's config file now.  Here is an example formatting:
    //     TTY /dev/ttyUSB0
    //     READ_TIME 1000
    //     LOG_TYPE 1
    //     LOG_FORMAT "%b %d %H:%M:%S Sensor %s C: %.2C F: %.2F"
    //     CNT_FORMAT "%b %d %H:%M:%S Sensor %s #%n %C"
    //     HUM_FORMAT "%b %d %H:%M:%S Sensor %s C: %.2C F: %.2F H: %h%%"
    //     SENSORS 1
    //     ROM 0 0x28 0xB8 0x15 0xA2 0x02 0x00 0x00 0xF7
    fs.readFile(this.m_configPath, function(err, data) {
        //If there is an error; bubble it up.
        if (err)
            callback(err);
        else
        {
            var sensorSerialNumbers = [];

            //Stringify the data so we can parse it; this *IS* text data (dev error otherwise).
            var dataStr = String(data);

            //Start parsing out in sections; of course, we expect exact formatting (see above).
            var ttys = dataStr.split('TTY ');

            //Scan each TTY section until we find the one that matches ours.
            for(i = 0; i < ttys.length; i++)
                if (-1 !== this.m_tty.search(ttys[i]))
                {
                    //We found the section for this LinkUSB / TTY; parse out the lines.
                    var lines = dataStr.split('\n');

                    //Sensor Serial #s are all fixed; so, simply just scan for 'ROM' lines!
                    for(j = 0; j < lines.length; j++)
                        if (-1 !== lines[j].search('ROM '))
                        {
                            //Split into tokens and take the last 8 hex digits (but the first 2 tokens are DC's).
                            var tokens = lines[j].split(' ');

                            if (tokens.length < 10)
                                callback('Line parse error. Length (tokens): ' + tokens.length + ' Line: ' + lines[j], null);
                            else
                            {
                                //Build a hex number (serialized backwards; endianness); do a level of indirection to get rid of '0x' naturally.
                                var index = 10 - 1;
                                var sensorSerialNumber = '';

                                for (k = tokens.length - 1; k >= 0; k--)
                                    if (-1 !== tokens[k].search('0x'))
                                        sensorSerialNumber += parseInt(tokens[k], 16).toString(16).toUpperCase(); //Base16 for Hex Digits

                                //Finally, add this entry to the list.
                                sensorSerialNumbers.push(sensorSerialNumber);
                            }
                        }
                }

            //Invoke user's callback and give what we can.
            callback(null, sensorSerialNumbers);
        }
    }.bind(this));
}

//Export this class outside this file.
module.exports = LinkUSBDS18B20;
