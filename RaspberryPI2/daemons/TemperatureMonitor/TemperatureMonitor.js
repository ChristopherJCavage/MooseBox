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
var events = require('events');
var LinkUSBDS18B20 = require('./LinkUSBDS18B20.js');
var redis = require('redis');

//Constants
TemperatureMonitor.prototype.TEMPERATURE_SENSOR_NUM0 = 0;

/**
 * Defines an abstraction for a 0...N Temperature Sensor monitor; though, as 
 * of Q3 2015 there is only support for one temperature sensor (#0) because
 * we there exists only one sensor per box.  Subject to change.
 *
 * @param configPath Path to 'digitemp' configuration file.
 * @param tty Default or overriden TTY to connect to LinkUSB device.
 */
function TemperatureMonitor(configPath, tty)
{
    //If this grows, start monitoring N sensors per LinkUSB TTY.
    //For now, logic for only one sensor is fine...
    this.m_linkUSBDS18B20 = new LinkUSBDS18B20(configPath, tty);

    this.m_intervalObj = null;

    //Execute EventEmmitter's constructor...
    events.EventEmitter.call(this);
};

//...and copy all of it's properties to our object instance.
TemperatureMonitor.prototype.__proto__ = events.EventEmitter.prototype;

/**
 * Starts polling the temperature sensors and emitting event data.
 */
TemperatureMonitor.prototype.start = function()
{
    if (this.m_intervalObj != null)
        return;

    this.m_intervalObj = setInterval(this.onIntervalElapsed.bind(this), 
                                     this.m_linkUSBDS18B20.MIN_POLLING_INTERVAL_MS);                   
}

/**
 * Stops polling the temperature sensors.
 */
TemperatureMonitor.prototype.stop = function()
{
    if (this.m_intervalObj == null)
        return;

    clearInterval(this.m_intervalObj);

    this.m_intervalObj = null;
}

/**
 * Timer Elapse Callback.  Polls temperature sensor(s) for the current reading(s) and emits event.
 */
TemperatureMonitor.prototype.onIntervalElapsed = function()
{
    //Poll the attached temperature sensor; as of Q3 2015 we only have, and plan to support one
    //temperature sensor.  However, it is possible we may add more for larger boxes (i.e. N sensors).
    this.m_linkUSBDS18B20.readAsync(this.TEMPERATURE_SENSOR_NUM0, function(err, celsius)
        {
            //If this fails, there's not much we can really do.  Reset the device.
            if (!err)
            {
                //Do some coercion on out-of-range temperatures to be pedantic.
                if (celsius < this.m_linkUSBDS18B20.MIN_TEMPERATURE_CELSIUS)
                    celsius = this.m_linkUSBDS18B20.MIN_TEMPERATURE_CELSIUS;
                else if (celsius > this.m_linkUSBDS18B20.MAX_TEMPERATURE_CELSIUS)
                    celsius = this.this.m_linkUSBDS18B20.MAX_TEMPERATURE_CELSIUS;

                //Create reading object.
                var reading = {};

                reading.Celsius = celsius;
                reading.Timestamp = Date.now();

                //Publish.
                this.emit('data', reading);
            }
        }.bind(this));
}

//Export this class outside this file.
module.exports = TemperatureMonitor;
