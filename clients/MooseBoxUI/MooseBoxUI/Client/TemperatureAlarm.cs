//////////////////////////////////////////////////////////////////////////
// Copyright (C) 2015  Christopher James Cavage (cjcavage@gmail.com)    //
//                                                                      //
// This program is free software; you can redistribute it and/or        //
// modify it under the terms of the GNU General Public license          //
// as published by the Free Software Foundation; either version 2       //
// of the License, or (at your option) any later version.               //
//                                                                      //
// This program is distributed in the hope that it will be useful,      //
// but WITHOUT ANY WARRANTY; without even the implied warranty of       //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        //
// GNU General Public License for more details.                         //
//                                                                      //
// You should have received a copy of the GNU General Public License    //
// along with this program; if not, see <http://www.gnu.org/licenses/>. //
//////////////////////////////////////////////////////////////////////////
using MooseBoxUI.Client.REST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MooseBoxUI.Client
{
    /// <summary>
    /// Defines an abstraction for [0...N] Temperature Alarms which are synchronized to MooseBox.
    /// </summary>
    public sealed class TemperatureAlarm
    {
        #region Public Methods (static)
        /// <summary>
        /// Queries the current state of all temperature alarms on MooseBox.
        /// </summary>
        /// <returns>Current temperature alarm data.</returns>
        internal static async Task<TemperatureAlarm> QueryCurrentTemperatureAlarms()
        {
            IMooseBoxRESTAPI mooseBoxRESTAPI = MooseBoxRESTAPIFactory.Instance.Create();

            List<TemperatureAlarmConfig> registeredAlarms = await mooseBoxRESTAPI.ListTemperatureAlarmConfig();

            TemperatureAlarm temperatureAlarm = new TemperatureAlarm(mooseBoxRESTAPI, registeredAlarms);

            return temperatureAlarm;
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mooseBoxRESTAPI">Instance of a versioned MooseBox REST API.</param>
        internal TemperatureAlarm(IMooseBoxRESTAPI mooseBoxRESTAPI) :
            this(mooseBoxRESTAPI, new List<TemperatureAlarmConfig>()) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mooseBoxRESTAPI">Instance of a versioned MooseBox REST API.</param>
        /// <param name="registeredAlarms">Current MooseBox temperature alarm configuration to synchronize instance to.</param>
        internal TemperatureAlarm(IMooseBoxRESTAPI mooseBoxRESTAPI, List<TemperatureAlarmConfig> registeredAlarms)
        {
            //Parameter Validations.
            if (mooseBoxRESTAPI == null)
                throw new ArgumentNullException("IMooseBoxRESTAPI mooseBoxRESTAPI");

            if (registeredAlarms == null)
                throw new ArgumentNullException("List<TemperatureAlarmConfig> registeredAlarms");

            //Set Members.
            m_mooseBoxRESTAPI = mooseBoxRESTAPI;

            m_registeredAlarmsDict = new Dictionary<Tuple<TemperatureSensor, string>, Tuple<Single, Single>>();

            //Configure N Temperature Alarms if we have something to configure from.
            foreach (var registeredAlarm in registeredAlarms)
            {
                TemperatureSensor temperatureSensor = new TemperatureSensor(registeredAlarm.SerialNumber, mooseBoxRESTAPI);

                foreach (var subscriber in registeredAlarm.Subscribers)
                    m_registeredAlarmsDict.Add(new Tuple<TemperatureSensor, string>(temperatureSensor, subscriber.Email),
                                               new Tuple<Single, Single>(subscriber.CelsiusMin, subscriber.CelsiusMax));
            }
        }
        #endregion

        #region Public Methods (instance)
        /// <summary>
        /// Registers a temperature alarm with MooseBox.  It a temperature reading goes lower or higher than 
        /// the acceptable thresholds, MooseBox will attempt to email receipiant indicating a problem.
        /// </summary>
        /// <param name="temperatureSensor">Temperature sensor to register alarm with.</param>
        /// <param name="emailAddress">Email address to broadcast alerts to.</param>
        /// <param name="celsiusMin">Coldest acceptable temperature threshold.</param>
        /// <param name="celsiusMax">Warmest acceptable temperature threshold.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Register(TemperatureSensor temperatureSensor,
                                     string emailAddress,
                                     Single celsiusMin,
                                     Single celsiusMax)
        {
            bool unregisterPrev = false;
            Tuple<Single, Single> value = null;

            //Parameter Validations.
            if (temperatureSensor == null)
                throw new ArgumentNullException("TemperatureSensor temperatureSensor");

            if (string.IsNullOrEmpty(emailAddress) == true)
                throw new ArgumentNullException("string emailAddress");

            //Is this already registered?
            if (m_registeredAlarmsDict.TryGetValue(Tuple.Create(temperatureSensor, emailAddress), out value) == true)
            {
                Debug.Assert(value != null);

                if (celsiusMin == value.Item1 && celsiusMax == value.Item2)
                    return; //Nothing to do!

                unregisterPrev = true;
            }
        
            //If it's already registered, unregister it first.
            if (unregisterPrev == true)
                await m_mooseBoxRESTAPI.UnregisterTemperatureAlarm(emailAddress, temperatureSensor.SerialNumber);
        
            //Register the temperature alarm.
            await m_mooseBoxRESTAPI.RegisterTemperatureAlarm(temperatureSensor.SerialNumber,
                                                             celsiusMin,
                                                             celsiusMax,
                                                             emailAddress);

            //Update local cache.
            m_registeredAlarmsDict[Tuple.Create(temperatureSensor, emailAddress)] = Tuple.Create(celsiusMin, celsiusMax);
        }

        /// <summary>
        /// Unregisteres a temperature alarm from MooseBox.
        /// </summary>
        /// <param name="temperatureSensor">Temperature sensor with the registered alarm.</param>
        /// <param name="emailAddress">Email address to unregister from sensor.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Unregister(TemperatureSensor temperatureSensor, string emailAddress)
        {
            Tuple<TemperatureSensor, string> key = Tuple.Create(temperatureSensor, emailAddress);

            //Parameter Validations.
            if (temperatureSensor == null)
                throw new ArgumentNullException("TemperatureSensor temperatureSensor");

            if (string.IsNullOrEmpty(emailAddress) == true)
                throw new ArgumentNullException("string emailAddress");

            //Are we registered?
            if (m_registeredAlarmsDict.ContainsKey(key) == true)
            {
                //Unregister it from MooseBox.
                await m_mooseBoxRESTAPI.UnregisterTemperatureAlarm(emailAddress, temperatureSensor.SerialNumber);

                //Remove it from local cache.
                m_registeredAlarmsDict.Remove(key);
            }
        }

        /// <summary>
        /// Queries all registered entries associated with a singular temperature sensor.
        /// </summary>
        /// <param name="temperatureSensor">Temperature sensor to lookup all registered alarms from.</param>
        /// <param name="registeredAlarms">[0...N] temperature alarms registration information.</param>
        /// <returns>true if registration information was queried; false otherwise.</returns>
        /// <remarks>O(N)</remarks>
        internal bool TryGetRegistrationInfo(TemperatureSensor temperatureSensor, 
                                             out List<Tuple<string, Single, Single>> registeredAlarms)
        {
            registeredAlarms = new List<Tuple<string, Single, Single>>();

            //Parameter Validations.
            if (temperatureSensor == null)
                throw new ArgumentNullException("TemperatureSensor temperatureSensor");

            //Enumerate each entry in the dictionary associated with this sensor.
            foreach (var kvp in m_registeredAlarmsDict)
                if (kvp.Key.Item1.Equals(temperatureSensor) == true)
                    registeredAlarms.Add(Tuple.Create(kvp.Key.Item2, kvp.Value.Item1, kvp.Value.Item2));

            return (registeredAlarms.Count > 0);
        }
        #endregion

        private readonly Dictionary<Tuple<TemperatureSensor, string>, Tuple<Single, Single>> m_registeredAlarmsDict;

        private readonly IMooseBoxRESTAPI m_mooseBoxRESTAPI;
    }
}
