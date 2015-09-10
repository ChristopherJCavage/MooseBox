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

/**
 * Defines an abstraction for a YepKit YKUSH programmatically powerable USB switch.  Device connects
 * to the Raspberry PI 2 via USB/MicroUSB with a 3rd party Linux command as the CONTROL interface.
 *
 * @param initPower true to initialze all ports to a powered/unpowered state; false otherwise.
 * @param powerOn true to power the USB port; false to unpower.
 * @see https://www.yepkit.com/products/ykush
 * @see https://www.yepkit.com/uploads/documents/09c6d_YKUSH_ProductManual_v1.1.1.pdf
 */
function YepKitYKUSH(initPower, powerOn) {
    //Public Constants.
    this.MIN_PORT_NUMBER = 1;
    this.MAX_PORT_NUMBER = 3;

    //Set Members.
    this.m_powerStateLookup = new Object();

    //Set everything to the unpowered state.
    if (true === initPower)
        for(i = this.MIN_PORT_NUMBER; i <= this.MAX_PORT_NUMBER; i++)
            this.setPower(i, powerOn, function(err, reply) {
                //Abort for any initialization error; we cannot construct in zombie state.
                if (err)
                    throw 'Cannot initialize YepKit YKUSH. Err: ' + err;
            });
}

/**
 * Queries the current power state (i.e. on/off) of a YKUSH USB port.
 *
 * @param port YepKit YKUSH USB Port Number; must be 1 <= Port <= 3.
 * @return true if USB port is powered; false otherwise.
 */ 
YepKitYKUSH.prototype.getPower = function(port) {
    //Parameter Validations.
    if (port < this.MIN_PORT_NUMBER || port > this.MAX_PORT_NUMBER)
        throw 'Invalid port.  Must be ' + this.MIN_PORT_NUMBER + ' <= P <= ' + this.MAX_PORT_NUMBER;

    //Lookup cached value in map.
    return this.m_lookupHandlers[port];
};

/**
 * Sets the current power state (i.e. on/off) of a YKUSH USB port.
 *
 * @param port YepKit YKUSH USB Port Number; must be 1 <= Port <= 3.
 * @param powerOn true to power the USB port; false to unpower.
 * @param callback Optional callback of type function(err, stateObj) to 
 *                 be invoked for addendum error and state information.
 * @remarks Currently, YKUSH requires 'sudo' to run.  MooseBox has exempted
 *          password entry for this command.  On terminal:  'sudo visudo'
 *
 *          Edit: <username> ALL=(ALL) NOPASSWD: /usr/local/bin/ykush
 * @see http://askubuntu.com/questions/72267/how-to-allow-execution-without-using-the-sudo
 */ 
YepKitYKUSH.prototype.setPower = function(port, powerOn, callback) {
    //Parameter Validations.
    if (port < this.MIN_PORT_NUMBER || port > this.MAX_PORT_NUMBER)
        throw 'Invalid port.  Must be ' + this.MIN_PORT_NUMBER + ' <= P <= ' + this.MAX_PORT_NUMBER;

    //Build the YKUSH command; device expect command-line interaction.
    var cmd = 'sudo ykush';

    cmd += (powerOn === true) ? ' -u' : ' -d';
    cmd += ' ' + port;

    //Asynchronously invoke on the command-line.
    exec(cmd, function(error, stdout, stderr) {
    {
        var err = null;
        var obj = {};

        if (error || stderr) //Combine errors.
            err = "error: " + error + ", stderr: " + stderr;
        else
        {
            //Update the cached value upon success.
            this.m_powerStateLookup[port] = powerOn;

            //Generate a timestamped state object.
            obj.Port = port;
            obj.PowerOn = powerOn;
            obj.Timestamp = Date.now();
        }

        //Invoke completion callback (Optional).
        if (callback)
            callback(null, obj);
    }.bind(this));
}

//Export this class outside this file.
module.exports = YepKitYKUSH;
