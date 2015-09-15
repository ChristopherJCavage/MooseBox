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
 * Defines an abstraction for a Temperature Alarm manager.  Supports 0...N
 * subscriptions per 0...M iButtonLink T-Probe sensors.  Each subscription
 * is associated with an email address and their temperature thresholds.
 *
 * @param rateLimitMs Rate-limiting timeout to avoid email spamming since temperature
 *                    readings are polled at 1.1Hz.  0 disables rate-limiting.
 * @param alarmsConfigObj Initial subscriber information (i.e. JSON from RedisDB).
 * @remarks Rate-limiting map is not serialized or deserialized; it in-memory only.
 */
function TemperatureAlarm(rateLimitMs, alarmsConfigObj) {
    //Parameter Validations.
    if (rateLimitMs < 0)
        throw 'rateLimitS cannot be negative';

    //Set members.
    this.m_rateLimitMs = rateLimitMs;

    this.m_alarmRateLimitMap = new Object();
    this.m_registeredAlarms = new Object();

    //Alarms config is retained in the DataStore. It may or may not exist.
    //
    //Basically, since MooseBox supports 0...N T-Probe sensors we have to support
    //separate temperature alarms for each T-Probe sensor.  We might have the concept
    //of chunking the MooseBox (which might be > 20ft long) into regions.
    //
    //So, 0...N Temperature Alarms for each T-Probe sensor.  But what about multiple
    //email registrations?  We assume that each registration may want to set their
    //own alarm thresholds too!
    //
    //Ultimately, we're going to end up with a JSON object that more or less looks like this:
    //
    //  {
    //      "RegisteredAlarms": [
    //          {
    //              "SerialNumber": 1234,
    //              "Subscribers": [
    //                  {
    //                      "EmailAddress": "cjcavage@gmail.com",
    //                      "CelsiusMin": 30,
    //                      "CelsiusMax": 50
    //                  },
    //
    //                  //...et cetera
    //              ]
    //          }
    //
    //          //...et cetera
    //      ]
    //  }
    //
    //Convert it to registered alarms for the base case by iterating.  We'll index it by the serial
    //number for quick look up and then it'll just be an O(n) operation where N = Number of subscribers.
    if (alarmsConfigObj && 0 !== alarmsConfigObj.RegisteredAlarms.length)
        for(i = 0; i < alarmsConfigObj.RegisteredAlarms.length; i++)
            this.m_registeredAlarms[alarmsConfigObj.RegisteredAlarms[i].SerialNumber] = alarmsConfigObj.RegisteredAlarms[i].Subscribers;
}

/**
 * Registers a new, or updates an existing, subscriber for Temperature Alarming.
 *
 * @param serialNumber iButtonLink T-Probe sensor to associate the temperature alarm with.
 * @param celsiusMin Inclusive lower-bound for a valid temperature reading; in degrees Celsius.
 * @param celsiusMax Inclusive upper-bound for a valid temperature reading; in degrees Celsius.
 * @param emailAddress Subscriber's email address for alarm notification to be sent to.
 * @return true if new subscriber (add); false if an existing subscriber (update).
 * @remarks O(k); where k = number of current subscribers for T-Probe sensor serial number.
 */
TemperatureAlarm.prototype.register = function(serialNumber, celsiusMin, celsiusMax, emailAddress) {
    //Parameter Validations.
    if (undefined === serialNumber || null === serialNumber)
        throw 'serialNumber cannot be null';

    if (undefined === celsiusMin || null === celsiusMin ||
        undefined === celsiusMax || null === celsiusMax)
        throw 'celsiusMin/celsiusMax cannot be null';

    if (celsiusMin > celsiusMax)
        throw 'celsiusMin <= celsiusMax. celsiusMin: ' + celsiusMin + ', CelsiusMax: ' + celsiusMax;

    if (!emailAddress || 0 === emailAddress.length)
        throw 'emailAddress cannot null / empty';

    //Build a subscription object for this caller.
    var thisSubscriber = {};

    thisSubscriber.CelsiusMin = celsiusMin;
    thisSubscriber.CelsiusMax = celsiusMax;
    thisSubscriber.EmailAddress = emailAddress;

    //In the event that it's already registered, we don't want to overwrite the data.
    //Instead, we want to APPEND to it.  Attempt to query it first.
    var allSubscribers = [].concat(thisSubscriber);

    if (serialNumber in this.m_registeredAlarms)
    {
        var found = false;

        //In the event that this subscriber is already subscribed...  just update it.
        //The remainder of the logic will fall-through nicely for the update operation.
        for(i = 0; i < this.m_registeredAlarms[serialNumber].length; i++)
            if (this.m_registeredAlarms[serialNumber][i].EmailAddress === emailAddress)
            {
                found = true;

                break;
            }

        //Is this a new subscriber being registered?
        if (false === found)
            allSubscribers = allSubscribers.concat(this.m_registeredAlarms[serialNumber]);
    }

    //Now that we resolved existing emails; unconditionally set the reading object!  Easy-peasy! :0)
    this.m_registeredAlarms[serialNumber] = allSubscribers;
}

/**
 * Unregisters all email addresses associated with a sensor and removes that sensor from registration entirely.
 *
 * @param serialNumber iButtonLink T-Probe serial number to unregister.
 * @return List of all email addresses that were consequently unregistered.
 */
TemperatureAlarm.prototype.unregisterSensor = function(serialNumber) {
    var unregisteredEmailAddresses = [];

    //Parameter Validations.
    if (null === serialNumber || undefined === serialNumber)
        throw 'serialNumber cannot be null';

    //Easier, just find the serial number and create a shallow copy and remove them.
    if (serialNumber in this.m_registeredAlarms && this.m_registeredAlarms.hasOwnProperty(serialNumber))
    {
        //Create a new copy (shallow) of the addresses so we can return them.
        unregisteredEmailAddresses = this.m_registeredAlarms[serialNumber].slice();
    
        //Now, delete this entire entry from our registration object.  Note that, these
        //email addresses may be registered to other sensors (so no chain-calling).
        delete this.m_registeredAlarms[serialNumber];
    }

    return unregisteredEmailAddresses;
}

/**
 * Unregisters an email address associated with a single, or all, iButtonLink T-Probe Sensor(s)
 *
 * @param emailAddress Subscriber's email address to unregister.
 * @param serialNumber Optional iButtonLink T-Probe serial number to unregister from.
 * @remarks O(N + M); where N = number of sensors and M = number of email addresses.
 */
TemperatureAlarm.prototype.unregisterEmailAddress = function(emailAddress, serialNumber) {
    //Parameter Validations.
    if (!emailAddress || 0 === emailAddress.length)
        throw 'emailAddress cannot be null/empty';

    //Email addresses can be registered to multiple sensors; scan each in turn and remove it.
    //Long compound conditional is to only unregister an email address with a specific sensor
    //if a serial number was provided; otherwise, just do a blind unregister with all of them.
    //Also note that because the keys were stringified, we shall stringify our serial number.
    for (var property in this.m_registeredAlarms)
        if (this.m_registeredAlarms.hasOwnProperty(property) && 
            (null === serialNumber || undefined === serialNumber || property === String(serialNumber)))
        {
            //Linear scan the list for the email address.
            for(i = 0; i < this.m_registeredAlarms[property].length; i++)
                if (emailAddress === this.m_registeredAlarms[property][i].EmailAddress)
                {
                    //Each sensor has a list of unique email address; so just remove it and plow on.
                    this.m_registeredAlarms[property].splice(i, 1);

                    break;
                }

            //Now, in the event that was the last element, and it was removed, wipe out the entry too.
            if (0 === this.m_registeredAlarms[property].length)
                delete this.m_registeredAlarms[property];
        }
}

/**
 * Checks a temperature reading from an iButtonLink T-Probe and validates it against all
 * appropiate registered alarms for Min/Max thresholds.
 *
 * @param serialNumber iButtonLink T-Probe serial number from which the reading was sourced.
 * @param celsius Temperature reading in degrees Celsius.
 * @return Array of email address whose alarms are set (i.e. send them emails).
 * @remarks O(k ); where K = number of subscribers registered for a T-Probe sensor.
 */
TemperatureAlarm.prototype.checkReading = function(serialNumber, celsius) {
    var result = [];

    //Parameter Validations.
    if (undefined === serialNumber || null === serialNumber)
        throw 'serialNumber cannot be null';

    if (undefined === celsius || null === celsius)
        throw 'celsius cannot be null';

    //First, lookup by serial number - O(1); then scan each of the subscribers
    //in turn to ascertain if this temperature reading passes their thresholds.
    //Finally, to avoid spamming, only alarm if we pass our rate-limiting threshold.
    if (serialNumber in this.m_registeredAlarms)
        for (i = 0; i < this.m_registeredAlarms[serialNumber].length; i++)
            if (celsius < this.m_registeredAlarms[serialNumber][i].CelsiusMin ||
                celsius > this.m_registeredAlarms[serialNumber][i].CelsiusMax)
            {
                var isAlarming = false;

                //Are we already being rate-limited?
                var key = serialNumber + ':' + this.m_registeredAlarms[serialNumber][i].EmailAddress;

                if (key in this.m_alarmRateLimitMap && 0 !== this.m_rateLimitMs)
                {
                    //We are; however, how long ago was it? (JavaScript timestamp subtraction results in milliseconds)
                    var rateLimitTimestamp = this.m_alarmRateLimitMap[key];

                    if (Date.now() - rateLimitTimestamp > this.m_rateLimitMs)
                        isAlarming = true;
                }
                else
                    isAlarming = true; //New!

                //If we alarmed (or are re-alarming); update our rate-limiting map now.
                if (true === isAlarming)
                {
                    this.m_alarmRateLimitMap[key] = Date.now();

                    result.push(this.m_registeredAlarms[serialNumber][i].EmailAddress);
                }
            }

    //Return the list of emails who's thresholds have resulted in an alarm.
    return result;
}

/**
 * Queries all email addresses that are currently registered with the instance.
 *
 * @param serialNumber Optional iButtonLink T-Probe serial number to restrict query to.
 * @return List of registered email addresses.
 * @remarks O(N)
 */
TemperatureAlarm.prototype.getRegisteredEmailAddresses = function(serialNumber) {
    //Return all the keys as a list; however, since the same email address
    //can be registered to multiple sensors we're going to put all the emails
    //into the typical JavaScript object first to ensure uniqueness; then get keys.
    //Also note that, the keys are saved as keys so do that to ours too.
    var registeredEmailAddresses = new Object();

    for (var property in this.m_registeredAlarms)
        if (this.m_registeredAlarms.hasOwnProperty(property) && 
            (null === serialNumber || undefined === serialNumber || property === String(serialNumber)))
            for (i = 0; i < this.m_registeredAlarms[property].length; i++)
                registeredEmailAddresses[this.m_registeredAlarms[property][i].EmailAddress] = true;

    return Object.keys(registeredEmailAddresses);
}

/**
 * Queries all T-Probe sensor serial numbers that are currently registered with the instance.
 *
 * @return List of registered T-Probe sensor serial numbers.
 * @remarks O(N)
 */
TemperatureAlarm.prototype.getRegisteredTemperatureSensors = function() {
    //Return all the keys as a list.
    return Object.keys(this.m_registeredAlarms);
}

/**
 * Creates an object ready for JSON stringification containing a snapshot of all the
 * subscribers currently subscribed to all the iButtonLink T-Probe sensors.
 *
 * @return Object containing all subscriber info; can be passed in this class's constructor.
 * @remarks O(n + m); where N = the total number T-Probe sensors and M = total number of subscribers (ALL).
 */
TemperatureAlarm.prototype.getAlarmsConfig = function() {
    //See comments in constructor for example formatting.
    var alarmsConfig = new Object();

    alarmsConfig.RegisteredAlarms = [];

    //Iterate over all of our objects; which themselves contain lists.
    for (var serialNumber in this.m_registeredAlarms)
        if (this.m_registeredAlarms.hasOwnProperty(serialNumber))
        {
            var registeredAlarm = {};

            registeredAlarm.SerialNumber = serialNumber;

            registeredAlarm.Subscribers = this.m_registeredAlarms[serialNumber];

            alarmsConfig.RegisteredAlarms.push(registeredAlarm);
        }

    return alarmsConfig;
}

//Export this class outside this file.
module.exports = TemperatureAlarm;
