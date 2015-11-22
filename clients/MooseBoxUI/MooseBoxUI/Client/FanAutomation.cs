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
    /// Defines an abstraction for automated fan control based on a [1...N] temperature sensors.
    /// </summary>
    internal sealed class FanAutomation
    {
        #region Public Methods (static)
        /// <summary>
        /// Queries the currnet configuration from MooseBox and creates a synchronized instance of FanAutomation.
        /// </summary>
        /// <returns>FanAutomation instance representing current state of MooseBox.</returns>
        internal static async Task<FanAutomation> QueryCurrentFanAutomation()
        {
            IMooseBoxRESTAPI mooseBoxRESTAPI = MooseBoxRESTAPIFactory.Instance.Create();

            List<FanAutomationConfig> registeredFans = await mooseBoxRESTAPI.ListFanAutomationConfig();

            FanAutomation fanAutomation = new FanAutomation(mooseBoxRESTAPI, registeredFans);

            return fanAutomation;
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mooseBoxRESTAPI">Instance to a versioned MooseBox REST API.</param>
        internal FanAutomation(IMooseBoxRESTAPI mooseBoxRESTAPI) :
            this(mooseBoxRESTAPI, new List<FanAutomationConfig>()) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mooseBoxRESTAPI">Instance to a versioned MooseBox REST API.</param>
        /// <param name="registeredFans">Current MooseBox Fan Automation config to build instance from.</param>
        internal FanAutomation(IMooseBoxRESTAPI mooseBoxRESTAPI, List<FanAutomationConfig> registeredFans)
        {
            //Parameter Validations.
            if (mooseBoxRESTAPI == null)
                throw new ArgumentNullException("IMooseBoxRESTAPI mooseBoxRESTAPI");

            if (registeredFans == null)
                throw new ArgumentNullException("List<FanAutomationConfig> registeredFans");

            //Set Members.
            m_mooseBoxRESTAPI = mooseBoxRESTAPI;

            m_registeredFansDict = new Dictionary<Fan, Tuple<TemperatureSensor, Single>>();

            //Populate cache if we're building from a config object.
            foreach (var registeredFan in registeredFans)
            {
                TemperatureSensor temperatureSensor = new TemperatureSensor(registeredFan.SerialNumber, mooseBoxRESTAPI);

                foreach (var fanConfig in registeredFan.FansConfig)
                {
                    Fan fan = new Fan(fanConfig.FanNumber, mooseBoxRESTAPI);

                    m_registeredFansDict[fan] = new Tuple<TemperatureSensor, Single>(temperatureSensor, fanConfig.PowerOnThresholdCelsius);
                }
            }
        }
        #endregion

        #region Public Methods (instance)
        /// <summary>
        /// Registers a fan for automation with an associated temperature sensor.
        /// </summary>
        /// <param name="fan">USB Fan to register.</param>
        /// <param name="temperatureSensor">Temperature sensor to register against.</param>
        /// <param name="celsiusThreshold">Warmest allowable reading before automating fan cooling.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Register(Fan fan, TemperatureSensor temperatureSensor, Single celsiusThreshold)
        {
            bool unregisterPrev = false;
            Tuple<TemperatureSensor, Single> value = null;

            //Parameter Validations.
            if (fan == null)
                throw new ArgumentNullException("Fan fan");

            if (temperatureSensor == null)
                throw new ArgumentNullException("TemperatureSensor temperatureSensor");

            //Is it already registered?  Are we changing?
            if (m_registeredFansDict.TryGetValue(fan, out value) == true)
            {
                Debug.Assert(value != null);
                Debug.Assert(value.Item1 != null);

                if (value.Item1 == temperatureSensor && value.Item2 == celsiusThreshold)
                    return; //Nothing to do!

                unregisterPrev = true;
            }

            //User is requesting an overwrite, which is an unregistration first.
            if (unregisterPrev == true)
                await m_mooseBoxRESTAPI.UnegisterFanAutomation(fan.Number);

            //Register the new fan information.
            await m_mooseBoxRESTAPI.RegisterFanAutomation(fan.Number,
                                                          temperatureSensor.SerialNumber,
                                                          celsiusThreshold);

            //Update the local cache.
            m_registeredFansDict[fan] = new Tuple<TemperatureSensor, Single>(temperatureSensor, celsiusThreshold);
        }

        /// <summary>
        /// Unregisters a fan from temperature automation.
        /// </summary>
        /// <param name="fan">USB Fan to unregister.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Unregister(Fan fan)
        {
            //Parameter Validations.
            if (fan == null)
                throw new ArgumentNullException("Fan fan");

            //Is it registered?
            if (m_registeredFansDict.ContainsKey(fan) == false)
                return; //Nothing to do!

            //It is, attempt an unregistration.
            await m_mooseBoxRESTAPI.UnegisterFanAutomation(fan.Number);

            //Now remove it from local cache.
            m_registeredFansDict.Remove(fan);
        }

        /// <summary>
        /// Attempts to get current registration information for a USB Fan, if it was registered.
        /// </summary>
        /// <param name="fan">USB Fan to query information for.</param>
        /// <param name="temperatureSensor">Temperature sensor fan is tied to.</param>
        /// <param name="celsiusThreshold">Warmest allowd reading before fan cooling is automated.</param>
        /// <returns>true if registration information was queried; false otherwise.</returns>
        internal bool TryGetRegistrationInfo(Fan fan,
                                             out TemperatureSensor temperatureSensor,
                                             out Single celsiusThreshold)
        {
            bool result = false;
            Tuple<TemperatureSensor, Single> value = null;

            temperatureSensor = null;
            celsiusThreshold = Single.NaN;

            //Parameter Validations.
            if (fan == null)
                throw new ArgumentNullException("Fan fan");

            //Check our cache, which is synchronized.
            if (m_registeredFansDict.TryGetValue(fan, out value) == true)
            {
                //Set outputs.
                temperatureSensor = value.Item1;
                celsiusThreshold = value.Item2;

                result = true;
            }

            return result;
        }
        #endregion

        private readonly IMooseBoxRESTAPI m_mooseBoxRESTAPI;

        private readonly Dictionary<Fan, Tuple<TemperatureSensor, Single>> m_registeredFansDict;
    }
}
