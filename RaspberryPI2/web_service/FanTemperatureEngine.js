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
 * Defines an abstraction for the core fan controlling algorithm logic to independently
 * toggle N fans' power states based on readings from M temperature sensors.  Like the
 * TemperatureAlarm module, this module will not actually integrate with the RedisDB 
 * Pub/Sub to execute the messages; instead, it merely manages the calculations.
 *
 * @param fanConfigObj Initial state and pairing information (i.e. JSON from RedisDB).
 */
function FanTemperatureEngine(configObj) {
    //Set members.
    this.m_registeredSensors = new Object();
    this.m_registeredFansSet = new Object();

    //Fans config is maintained in the DataStore; it may or may not exist.
    //
    //Fans are coupled to temperature sensors, since they act on temperature.
    //
    //It is saved as a JSON object that more-or-less looks like this:
    //    {
    //        "RegisteredSensors": [
    //            {
    //                "SerialNumber": 1234,
    //                "FansConfig": [
    //                    { 
    //                        "FanNumber": 1,
    //                        "PowerOnThresholdCelsius": 42
    //                    },
    //    
    //                    { 
    //                        "FanNumber": 2,
    //                        "PowerOnThresholdCelsius": 23
    //                    },   
    //                ]
    //            },
    //
    //            {
    //                "SerialNumber": 4321,
    //                "FansConfig": [
    //                    { 
    //                        "FanNumber": 3,
    //                        "PowerOnThresholdCelsius": -5
    //                    }
    //                ]
    //            }
    //        ]
    //    }
    //
    //Convert the JSON into our (useable) registration object now.
    if (configObj)
        for(i = 0; i < configObj.RegisteredSensors.length; i++)
            for (j = 0; j < configObj.RegisteredSensors[i].length; j++)
                this.register(configObj.RegisteredSensors[i].SerialNumber,
                              configObj.RegisteredSensors[i][j].FanNumber,
                              configObj.RegisteredSensors[i][j].PowerOnThresholdCelsius);
}

/**
 * Registered a fan to be associated with a specific iButtonLink T-Probe Temperature Sensor
 * with a specific threshold.  A fan can only be registered once to any temperature sensor.
 * Because of this strong independence of sensors, thresholds and fans, it allows the MooseBox
 * to effectively implement "Sensor Regions" if we decide to build the box to be 15" long or more.
 *
 * @param serialNumber iButtonLink T-Probe Sensor Serial Number to register to.
 * @param fanNumber USB fan to register; specifically to say, YKUSH USB port to power-toggle.
 * @param powerOnThresholdCelsius Highest temperature reading, in degrees Celsius, that can be 
 *                                tolerated because a cooling fan should be powered. Essentially, if
 *                                temperature reading is > than threshold: Power On; <= Power Off.
 * @remkars Unlike Temperature Alarms, an UPDATE operation is not performed if fan is already registered.
 * @remarks O(1)
 */
FanTemperatureEngine.prototype.register = function(serialNumber, fanNumber, powerOnThresholdCelsius) {
    //Parameter Validations.
    if (null === serialNumber || undefined === serialNumber)
        throw 'serialNumber cannot be null';

    if (null === fanNumber || undefined === fanNumber)
        throw 'fanNumber cannot be null';

    if (fanNumber < 0)
        throw 'fanNumber must be >= 0';

    if (null === powerOnThresholdCelsius || undefined === powerOnThresholdCelsius)
        throw 'powerOnThresholdCelsius cannot be null';

    //First, we do *NOT* allow one fan to be associated with multiple temperature sensors.
    //I had thought about this, and I realized that it's too dangerous because one region
    //may kick it off and start cooling and that could fire termperature alarms for another
    //region because it's too cold.  So, if you have to ask this question: buy more hardware!
    //Anyway, check for the uniqueness condition now using our 'Set' to simplify logic and 
    //avoid O(N) scanning.
    if (fanNumber in this.m_registeredFansSet)
        if (this.m_registeredFansSet.hasOwnProperty(fanNumber))
            throw 'fanNumber is m_registeredFansSet registered. Please unregister it first';

    //There are no more oppurtunities for failure; add it as a registered fan in the "Set" now.
    this.m_registeredFansSet[fanNumber] = serialNumber;

    //Now, add it to the real registration object; we have to account for the situation where
    //this is a first time on the temperature sensor too.
    var fanObj = new Object();

    fanObj.FanNumber = fanNumber;
    fanObj.PowerOnThresholdCelsius = powerOnThresholdCelsius;

    if (!(serialNumber in this.m_registeredSensors))
        this.m_registeredSensors[serialNumber] = []; //First time serial #.

    //Unconditionally append this registration to the serial number.
    this.m_registeredSensors[serialNumber].push(fanObj);
}

/**
 * Unregisters a fan from the Fan-Temperature Engine algorithm and its associated temperature sensor.
 *
 * @param fanNumber USB Fan to unregister (YKUSH USB Port).
 * @remarks O(M); where M = number fans registered to a T-Probe Sensor Serial Number.
 */
FanTemperatureEngine.prototype.unregisterFan = function(fanNumber) {
    //Parameter Validations.
    if (null === fanNumber || undefined === fanNumber)
        throw 'fanNumber cannot be null';

    if (fanNumber < 0)
        throw 'fanNumber must be >= 0';

    //Search for the fan to unregister; if it's not there then just ignore (noexcept).
    if (fanNumber in this.m_registeredFansSet)
    {
        //Search for the fan index in the list by first querying the serial number...
        var index = -1;
        var serialNumber = this.m_registeredFansSet[fanNumber];

        if (serialNumber in this.m_registeredSensors)
            for(i = 0; i < this.m_registeredSensors[serialNumber].length; i++)
                if (this.m_registeredSensors[serialNumber][i].FanNumber === fanNumber)
                {
                    index = i;

                    break;
                }

        //We presume this works; if it doesnt', again, just ignore (noexcept).
        if (-1 !== index)
        {
            //Remove it from the list first.
            this.m_registeredSensors[serialNumber].splice(index, 1);

            //Now remove it from the registered fans parallel lookup.
            delete this.m_registeredFansSet[fanNumber];

            //Finally, if that was the last element in the registration list; remove entire entry.
            if (0 === this.m_registeredSensors[serialNumber].length)
                delete this.m_registeredSensors[serialNumber];
        }
    }
}

/**
 * Unregisters a temperature sensor and all of its fans from the Fan-Temperature Engine algorithm.
 *
 * @param serialNumber iButtonLink T-Probe Sensor Serial Number to unregister.
 * @return List of fans that were consequently unregistered.
 * @remarks O(M); where M = number of unregistered fans.
 */ 
FanTemperatureEngine.prototype.unregisterSensor = function(serialNumber) {
    var unregisteredFans = [];

    //Parameter Validations.
    if (null === serialNumber || undefined === serialNumber)
        throw 'serialNumber cannot be null';

    //Unlike a fan unregistration, unregistering by serial number takes down all the fans.
    if (serialNumber in this.m_registeredSensors)
    {
        //Create a new copy (shallow) of the fans so we can return them.
        unregisteredFans = this.m_registeredSensors[serialNumber].slice();

        //Then take our unregistered fans and remove them fan-by-fan, which destroys it at the end!
        for(i = 0; i < unregisteredFans.length; i++)
            this.unregisterFan(unregisteredFans[i].FanNumber);
    }

    return unregisteredFans;
}

/**
 * Scans all registered USB fans for an associated temperature sensor and determines whether
 * they should be powered or unpowered.
 *
 * @param serialNumber iButtonLink T-Probe Sensor Serial Number.
 * @param celsius Current sensor's temperature reading in degrees Celsius.
 * @return 0...N length list of numbered USB fan power state instructions.
 * @remarks O(M); where M = number of registered fans to a temperature sensor.
 */
FanTemperatureEngine.prototype.getPowerStateInstructions = function(serialNumber, celsius) {
    var fanPowerCtrlList = [];

    //Parameter Validations.
    if (null === serialNumber || undefined === serialNumber)
        throw 'serialNumber cannot be null';

    if (null === celsius || undefined === celsius)
        throw 'celsius cannot be null';

    //Let's see if this sensor was ever registered; if it wasn't then just blank list result!
    //If it was registered; then enumerate every element in turn and check thresholds. Recall
    //again that fans cool and temperature rises; if the reading is greater than the threshold
    //then we need to power the fan(s); else, we power them off.
    if (serialNumber in this.m_registeredSensors && this.m_registeredSensors.hasOwnProperty(serialNumber))
        for(i = 0; i < this.m_registeredSensors[serialNumber].length; i++)
        {
            //Create a new instruction object assuming unpowered.
            var instruction = new Object();

            instruction.FanNumber = this.m_registeredSensors[serialNumber][i].FanNumber;
            instruction.PowerOn = false;

            //Now check threshold.
            if (celsius > this.m_registeredSensors[serialNumber][i].PowerOnThresholdCelsius)
                instruction.PowerOn = true;

            //Append it to the resultant list unconditionally.
            fanPowerCtrlList.push(instruction);
        }

    return fanPowerCtrlList;
}

/**
 * Queries all numbered USB fans that are currently registered with the instance.
 *
 * @param serialNumber iButtonLink T-Probe Sensor Serial Number.
 * @return List of registered fans.
 * @remarks O(N)
 */
FanTemperatureEngine.prototype.getRegisteredFans = function() {
    //Return all the keys as a list.
    return Object.keys(this.m_registeredFansSet);
}

/**
 * Queries all T-Probe sensor serial numbers that are currently registered with the instance.
 *
 * @return List of registered T-Probe sensor serial numbers.
 * @remarks O(N)
 */
FanTemperatureEngine.prototype.getRegisteredTemperatureSensors = function() {
    //Return all the keys as a list.
    return Object.keys(this.m_registeredSensors);
}

/**
 * Generates a configuration object ready for JSON stringification for RedisDB.
 *
 * @return JSON ready config object.
 */
FanTemperatureEngine.prototype.getConfigObj = function() {
    var configObj = new Object();

    //Convert the useable registered sensors-fans into the more descriptive JSON version.
    configObj.RegisteredSensors = [];

    for (var property in this.m_registeredSensors)
        if (this.m_registeredSensors.hasOwnProperty(property))
        {
            var obj = new Object();

            obj.SerialNumber = property;
            obj.FansConfig = this.m_registeredSensors[property];

            configObj.RegisteredSensors.push(obj);
        }

    return configObj;
}

//Export this class outside this file.
module.exports = FanTemperatureEngine;
