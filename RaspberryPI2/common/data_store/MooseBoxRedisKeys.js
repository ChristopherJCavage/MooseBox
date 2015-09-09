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

/**
 * Defines an abstraction for all Redis DB key constants used on MooseBox.
 * Most constants are static, but some must be dynamically generated.
 *
 * @remarks All constants are public / internal.
 */
function MooseBoxRedisKeys() {
    //Data; Configuration.
    this.CONFIG_ALARMS_KEY = 'MooseBox:Data:Config:Alarms';
    this.CONFIG_FAN_CTRL_DAEMON_KEY = 'MooseBox:Data:Config:FanCtrlDaemon';
    this.CONFIG_TEMPERATURE_DAEMON_KEY = 'MooseBox:Data:Config:TemperatureDaemon';

    //Data; System Info.
    this.DAEMON_VERSION_FAN_CTRL_KEY = 'MooseBox:Data:System:Versions:FanCtrlDaemon';
    this.DAEMON_VERSION_TEMPERATURE_KEY = 'MooseBox:Data:System:Versions:TemperatureDaemon';

    this.TEMPERATURE_SENSOR_SERIAL_NUMBERS_KEY = 'MooseBox:Data:System:SerialNumbers:TemperatureSensors';

    //Publish-Subscribe; Configuration.
    this.PUBSUB_NOTIFY_CONFIG_FAN_CTRL_DAEMON_KEY = 'MooseBox:PubSub:Notify:Config:FanCtrlDaemon';
    this.PUBSUB_NOTIFY_CONFIG_TEMPERATURE_DAEMON_KEY = 'MooseBox:PubSub:Notify:Config:TemperatureDaemon';

    //Publish-Subscribe; System Info.
    this.PUBSUB_NOTIFY_SENSOR_SERIAL_NUMBERS_KEY = 'MooseBox:PubSub:Notify:SerialNumbers:TemperatureSensors';
}

                                /********************/
                                /*** INTERNAL API ***/
                                /********************/

/**
 * Dynamically generates a key for a GET / SET operation for a Fan Ctrl current reading.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getFanCtrlCurrentReadingKey = function(fanNumber) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'Fan number cannot be negative';

    //Build Redis Key.
    return 'MooseBox:Data:FanCtrl:' + fanNumber + ':Current';
}

/**
 * Dynamically generates a key for a ZADD / ZRANGEBYSCORE operation for a Fan Ctrl historical reading.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getFanCtrlHistoricalReadingKey = function(fanNumber) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'Fan number cannot be negative';

    //Build Redis Key.
    return 'MooseBox:Data:FanCtrl:' + fanNumber + ':Historical';
}

/**
 * Dynamically generates a key for a PUBLISH / PSUBSCRIBE operation for live Fan Ctrl readings.
 *
 * @param fanNumber Zero-based integer describing the MooseBox fan number.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getFanCtrlPubSubReqKey = function(fanNumber) {
    //Parameter Validations.
    if (fanNumber < 0)
        throw 'Fan number cannot be negative';

    //Build Redis Key.
    return 'MooseBox:PubSub:Req:FanCtrl:' + fanNumber;
}

/**
 * Dynamically generates a key for a GET / SET operation for a Temperature current reading.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getTemperatureCurrentReadingKey = function(serialNumber) {
    //Parameter Validations.
    if (!serialNumber || serialNumber.length === 0)
        throw 'Invalid serial number';

    //Build Redis Key.
    return 'MooseBox:Data:Temperature:' + serialNumber + ':Current';
}

/**
 * Dynamically generates a key for ZADD / ZRANGEBYSCORE operation for a Temperature historical reading.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getTemperatureHistoricalReadingKey = function(serialNumber) {
    //Parameter Validations.
    if (!serialNumber || serialNumber.length === 0)
        throw 'Invalid serial number';

    //Build Redis Key.
    return 'MooseBox:Data:Temperature:' + serialNumber + ':Historical';
}

/**
 * Dynamically generates a key for a PUBLISH / PSUBSCRIBE operation for live Temperature readings.
 *
 * @param serialNumber Serial number of the iButtonLink T-Probe sensor.
 * @return Redis DB Key.
 */
MooseBoxRedisKeys.prototype.getTemperatureReadingPubSubNotifyKey = function(serialNumber) {
    //Parameter Validations.
    if (!serialNumber || serialNumber.length === 0)
        throw 'Invalid serial number';

    //Build Redis Key.
    return 'MooseBox:PubSub:Notify:Temperature:' + serialNumber;
}

//Export this class outside this file.
module.exports = MooseBoxRedisKeys;
