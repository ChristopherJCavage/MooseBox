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
using MooseBoxUI.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace MooseBoxUI.Client.REST
{
    /// <summary>
    /// Defines an abstraction for the 2015 version #1 MooseBox REST API.
    /// </summary>
    internal sealed class MooseBoxRESTAPI_v1_0 : IMooseBoxRESTAPI
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseUrl">Base URL address to the MooseBox, which should be "MooseBox."</param>
        internal MooseBoxRESTAPI_v1_0(Uri baseUrl)
        {
            //Parameter Validations.
            if (baseUrl == null)
                throw new ArgumentNullException("Uri baseUrl");

            //Set Members.
            m_restClient = new RestClient(baseUrl);
        }
        #endregion

        #region IMooseBoxRESTAPI Implementation

        #region Fans
        /// <summary>
        /// Clears all historical control data (i.e. power on/off) associated with a numbered fan.
        /// </summary>
        /// <param name="fanNumber">Numbered fan to clear historical data from.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task ClearFanCtrlData(Byte fanNumber)
        {
            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/peripherals/fan/data/clear",
                             Method.DELETE,
                             restRequest => { restRequest.AddParameter(ParamFanNumber, fanNumber); });
        }

        /// <summary>
        /// Manually powers on/off a USB fan in the event it does not conflict with Fan Automation.
        /// </summary>
        /// <param name="fanNumber">Numbered fan to power on or off.</param>
        /// <param name="powerOn">true to power fan; false to unpower.</param>
        /// <returns>Asynchronous processing task.</returns>
        public async Task PowerFanCtrl(Byte fanNumber, bool powerOn)
        {
            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/peripherals/fan/data/clear",
                             Method.PUT,
                             restRequest =>
                             {
                                 restRequest.AddParameter(ParamFanNumber, fanNumber);
                                 restRequest.AddParameter(ParamPowerOn, powerOn);
                             });
        }

        /// <summary>
        /// Queries historical fan control data with optional timestamp range restrictions.
        /// </summary>
        /// <param name="fanNumber">Numbered fan associated with the data query.</param>
        /// <returns>Current fan control data reading.</returns>
        public async Task<Tuple<bool, DateTime>> QueryFanCtrlData(Byte fanNumber)
        {
            //Call Worker.
            List<Tuple<bool, DateTime>> readings = await QueryFanCtrlDataWorker(fanNumber,
                                                                                DateTime.MinValue,
                                                                                DateTime.MaxValue,
                                                                                false);

            //Return the latest reading from the list, which should be count 1.
            Debug.Assert(readings.Count > 0);

            return readings.Last();
        }

        /// <summary>
        /// Queries historical fan control data with optional timestamp range restrictions.
        /// </summary>
        /// <param name="fanNumber">Numbered fan associated with the data query.</param>
        /// <param name="startTimestamp">Optional timestamp start range.</param>
        /// <param name="stopTimestamp">Optional timestamp stop range.</param>
        /// <returns>[0...N] Historical fan control data readings.</returns>
        /// <remarks>Current value is returned if no timestamp range is provided.</remarks>
        public async Task<List<Tuple<bool, DateTime>>> QueryFanCtrlData(Byte fanNumber, DateTime startTimestamp, DateTime stopTimestamp)
        {
            //Call Worker.
            return await QueryFanCtrlDataWorker(fanNumber,
                                                startTimestamp,
                                                stopTimestamp,
                                                true);
        }

        /// <summary>
        /// Queries the minimum and current maximum timestamps associated with collected data on a numbered fan.
        /// </summary>
        /// <param name="fanNumber">Numbered fan to query data timestamps on.</param>
        /// <returns>Tuple containing the start and stop timestamp results.</returns>
        public async Task<Tuple<DateTime, DateTime>> QueryFanCtrlTimestamps(Byte fanNumber)
        {
            DateTime startTimestamp = DateTime.MinValue;
            DateTime stopTimestamp = DateTime.MaxValue;

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/peripherals/fan/timestamps/query",
                                                          Method.GET,
                                                          restRequest => { restRequest.AddParameter(ParamFanNumber, fanNumber); });

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            startTimestamp = jsonResult.StartTimestamp;
            stopTimestamp = jsonResult.StopTimestamp;

            return Tuple.Create(startTimestamp, stopTimestamp);
        }
        #endregion

        #region Fan Automation
        /// <summary>
        /// Registers a repeating Fan Automation task.
        /// </summary>
        /// <param name="fanNumber">Numbered fan to register task to.</param>
        /// <param name="serialNumber">Serial number of the temperature sensor associated with the automation.</param>
        /// <param name="celsiusThreshold">Warmest allowable temperature reading before powering fans.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task RegisterFanAutomation(Byte fanNumber,
                                                string serialNumber,
                                                Single celsiusThreshold)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/automation/fan/register",
                             Method.PUT,
                             restRequest => 
                                {
                                    restRequest.AddParameter(ParamFanNumber, fanNumber);
                                    restRequest.AddParameter(ParamSerialNumber, serialNumber);
                                    restRequest.AddParameter(ParamCelsiusThreshold, celsiusThreshold);
                                });
        }

        /// <summary>
        /// Unregisters a numbered fan from the MooseBox Automation Engine.
        /// </summary>
        /// <param name="fanNumber">Numbered fan to unregister.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task UnegisterFanAutomation(Byte fanNumber)
        {
            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/automation/fan/unregister",
                             Method.DELETE,
                             restRequest => { restRequest.AddParameter(ParamFanNumber, fanNumber); });
        }

        /// <summary>
        /// Lists current registration information (global).
        /// </summary>
        /// <returns>Fan automation configurataion object with global registration status.</returns>
        public async Task<List<FanAutomationConfig>> ListFanAutomationConfig()
        {
            List<FanAutomationConfig> registeredSensors = new List<FanAutomationConfig>();

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/automation/fan/list", Method.GET);

            //Parse results; from MooseBox's FanAutomation.js:
            //
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
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            foreach (var registeredSensor in jsonResult.RegisteredSensors)
            {
                FanAutomationConfig fanAutomationConfig = new FanAutomationConfig();

                fanAutomationConfig.SerialNumber = registeredSensor.SerialNumber;
                fanAutomationConfig.FansConfig = new List<FanConfig>();

                foreach (var fc in registeredSensor.FansConfig)
                {
                    FanConfig fanConfig = new FanConfig();

                    fanConfig.FanNumber = fc.FanNumber;
                    fanConfig.PowerOnThresholdCelsius = fc.PowerOnThresholdCelsius;

                    fanAutomationConfig.FansConfig.Add(fanConfig);
                }

                registeredSensors.Add(fanAutomationConfig);
            }

            return registeredSensors;
        }
        #endregion

        #region System Infomation
        /// <summary>
        /// Queries RAM and disk memory information for the MooseBox system.
        /// </summary>
        /// <returns>Tuple containing the disk free and total sizes, in bytes.</returns>
        public async Task<Tuple<UInt64, UInt64>> QuerySystemInformation()
        {
            UInt64 diskFreeBytes = 0;
            UInt64 diskTotalBytes = 0;

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/system/memory/query", Method.GET);

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            diskFreeBytes = Convert.ToUInt64(jsonResult.DiskFreeBytes);
            diskTotalBytes = Convert.ToUInt64(jsonResult.DiskTotalBytes);

            return Tuple.Create(diskFreeBytes, diskTotalBytes);
        }

        /// <summary>
        /// Queries as much version information as possible for the MooseBox system.
        /// </summary>
        /// <returns>Tuple containing Main Web-service version, Fan Ctrl Daemon and Temperature Daemon versions.</returns>
        public async Task<Tuple<Version, Version, Version>> QuerySystemVersion()
        {
            //Provide defaults if system has not resolved a version yet.
            string webServiceVersion = DefaultVersionStr;
            string fanCtrlVersion = DefaultVersionStr;
            string temperatureDaemonVersion = DefaultVersionStr;

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/system/version/query", Method.GET);

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            webServiceVersion = jsonResult.WebService;
            fanCtrlVersion = jsonResult.FanCtrlDaemon;
            temperatureDaemonVersion = jsonResult.TemperatureDaemon;

            return Tuple.Create(new Version(webServiceVersion),
                                new Version(fanCtrlVersion),
                                new Version(temperatureDaemonVersion));
        }
        #endregion

        #region Temperature Alarms
        /// <summary>
        /// Reqisters a temperature alarm with the MooseBox.
        /// </summary>
        /// <param name="serialNumber">Serial number of temperature sensor associated with the alarm.</param>
        /// <param name="celsiusMin">Minimum allowable temperature threshold, in Celsius.</param>
        /// <param name="celsiusMax">Maximum allowable temperature threshold, in Celsius.</param>
        /// <param name="emailAddress">Email address to send an alert to when threshold levels break.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task RegisterTemperatureAlarm(string serialNumber,
                                                   Single celsiusMin,
                                                   Single celsiusMax,
                                                   string emailAddress)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            if (string.IsNullOrEmpty(emailAddress) == true)
                throw new ArgumentNullException("string emailAddress");

            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/alarms/temperature/register",
                              Method.PUT,
                              restRequest =>
                                  {
                                      restRequest.AddParameter(ParamSerialNumber, serialNumber);
                                      restRequest.AddParameter(ParamCelsiusMin, celsiusMin);
                                      restRequest.AddParameter(ParamCelsiusMax, celsiusMax);
                                      restRequest.AddParameter(ParamEmailAddress, emailAddress);
                                  });
        }

        /// <summary>
        /// Unregisters a temperature alarm associated with an email address.
        /// </summary>
        /// <param name="emailAddress">Required email address associated with the alarm.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task UnregisterTemperatureAlarm(string emailAddress)
        {
            //Chain-It!
            await UnregisterTemperatureAlarm(emailAddress, string.Empty);
        }

        /// <summary>
        /// Unregisters a temperature alarm associated with an email address.
        /// </summary>
        /// <param name="emailAddress">Required email address associated with the alarm.</param>
        /// <param name="serialNumber">Optional serial number of temperature sensor to restrict unregistration to.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task UnregisterTemperatureAlarm(string emailAddress, string serialNumber)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(emailAddress) == true)
                throw new ArgumentNullException("string emailAddress");

            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/alarms/temperature/unregister",
                              Method.DELETE,
                              restRequest => 
                                 { 
                                     restRequest.AddParameter(ParamEmailAddress, emailAddress); 

                                     if (string.IsNullOrEmpty(serialNumber) == false)
                                         restRequest.AddParameter(ParamSerialNumber, serialNumber); 
                                 });
        }

        /// <summary>
        /// Lists all current registration for MooseBox temperature alarms.
        /// </summary>
        /// <returns>All registered temperature alarms on MooseBox.</returns>
        public async Task<List<TemperatureAlarmConfig>> ListTemperatureAlarmConfig()
        {
            List<TemperatureAlarmConfig> registeredAlarms = new List<TemperatureAlarmConfig>();

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/automation/fan/list", Method.GET);

            //Parse results; from MooseBox's TemperatureAlarm.js:
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
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            foreach (dynamic registeredAlarm in registeredAlarms)
            {
                TemperatureAlarmConfig temperatureAlarmConfig = new TemperatureAlarmConfig();

                temperatureAlarmConfig.SerialNumber = registeredAlarm.SerialNumber;
                temperatureAlarmConfig.Subscribers = new List<TemperatureAlarmSubscriber>();

                foreach (dynamic subscriber in registeredAlarm.Subscriber)
                {
                    TemperatureAlarmSubscriber temperatureAlarmSubscriber = new TemperatureAlarmSubscriber();

                    temperatureAlarmSubscriber.Email = subscriber.Email;
                    temperatureAlarmSubscriber.CelsiusMin = subscriber.CelsiusMin;
                    temperatureAlarmSubscriber.CelsiusMax = subscriber.CelsiusMax;

                    temperatureAlarmConfig.Subscribers.Add(temperatureAlarmSubscriber);
                }

                registeredAlarms.Add(temperatureAlarmConfig);
            }

            return registeredAlarms;
        }
        #endregion

        #region Temperature Sensors
        /// <summary>
        /// Clears all historical data associated with a temperature sensor.
        /// </summary>
        /// <param name="serialNumber">Serial number assoicated with the temperature sensor data.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        public async Task ClearTemperatureSensorData(string serialNumber)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            //Call Worker.
            await RESTWorker("/MooseBox/API/v1.0/temperature/data/clear",
                             Method.DELETE,
                             restRequest => { restRequest.AddParameter(ParamSerialNumber, serialNumber); });
        }

        /// <summary>
        /// Queries current temperature data by timestamps or the current available reading associated with a serial number.
        /// </summary>
        /// <param name="serialNumber">Serial number of the temperature sensor to query data from.</param>
        /// <returns>Current temperature reading.</returns>
        public async Task<Tuple<DateTime, Single>> QueryTemperatureSensorData(string serialNumber)
        {
            //Call Worker.
            List<Tuple<DateTime, Single>> temperatureData = await QueryTemperatureSensorDataWorker(serialNumber,
                                                                                                   DateTime.MinValue,
                                                                                                   DateTime.MaxValue,
                                                                                                   false);

            //Return the latest reading from the list, which should be count 1.
            Debug.Assert(temperatureData.Count > 0);

            return temperatureData.Last();
        }
         
        /// <summary>
        /// Queries historical temperature data by timestamps or the current available reading associated with a serial number.
        /// </summary>
        /// <param name="serialNumber">Serial number of the temperature sensor to query data from.</param>
        /// <param name="startTimestamp">Start timestamp to query historical data from.</param>
        /// <param name="stopTimestamp">Stop timestamp to query historical data from.</param>
        /// <returns>[0...N] Historical data readings.</returns>
        public async Task<List<Tuple<DateTime, Single>>> QueryTemperatureSensorData(string serialNumber, DateTime startTimestamp, DateTime stopTimestamp)
        {
            //Call Worker.
            return await QueryTemperatureSensorDataWorker(serialNumber,
                                                          startTimestamp,
                                                          stopTimestamp,
                                                          true);
        }

        /// <summary>
        /// Queries all supported temperature serial numbers associated with the MooseBox.
        /// </summary>
        /// <returns>[0...N] iButtonLink T-Probe Temperature Sensor Serial Numbers.</returns>
        public async Task<List<string>> QueryTemperatureSensorSerialNumbers()
        {
            List<string> serialNumbers = new List<string>();

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/peripherals/temperature/serial_numbers/query", Method.GET);

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            foreach (string serialNumber in jsonResult)
                serialNumbers.Add(serialNumber);

            return serialNumbers;
        }

        /// <summary>
        /// Queries the minimum and maximum timestamps within a temperature sensor's data.
        /// </summary>
        /// <param name="serialNumber">Serial number of the temperature sensor associated with the data.</param>
        /// <returns>Tuple containing the minimum and maximum timestamps.</returns>
        public async Task<Tuple<DateTime, DateTime>> QueryTemperatureSensorTimestamps(string serialNumber)
        {
            DateTime startTimestamp = DateTime.MinValue;
            DateTime stopTimestamp = DateTime.MaxValue;

            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            //Call Worker.
            dynamic jsonResult = await RESTWorker("/MooseBox/API/v1.0/peripherals/temperature/timestamps/query",
                                                  Method.GET,
                                                  restRequest => { restRequest.AddParameter(ParamSerialNumber, serialNumber); });

            //Parse resultant data.
            startTimestamp = jsonResult.StartTimestamp;
            stopTimestamp = jsonResult.EndTimestmap;

            return Tuple.Create(startTimestamp, stopTimestamp);
        }
        #endregion

        #endregion

        #region Worker Methods
        /// <summary>
        /// Queries historical fan control data with optional timestamp range restrictions.
        /// </summary>
        /// <param name="fanNumber">Numbered fan associated with the data query.</param>
        /// <param name="startTimestamp">Optional timestamp start range.</param>
        /// <param name="stopTimestamp">Optional timestamp stop range.</param>
        /// <param name="restrictByTime">true to query by timestamp range; false otherwise.</param>
        /// <returns>[0...N] Historical fan control data readings.</returns>
        /// <remarks>Current value is returned if no timestamp range is provided.</remarks>
        private async Task<List<Tuple<bool, DateTime>>> QueryFanCtrlDataWorker(Byte fanNumber,
                                                                               DateTime startTimestamp,
                                                                               DateTime stopTimestamp,
                                                                               bool restrictByTime)
        {
            List<Tuple<bool, DateTime>> fanCtrlData = new List<Tuple<bool, DateTime>>();

            //Parameter Validations.
            if (restrictByTime == true)
                if (stopTimestamp < startTimestamp)
                    throw new ArgumentOutOfRangeException("DateTime stopTimestamp",
                                                          string.Format("Start Timestamp <= X <= Stop Timestamp. Start: {0}, Stop: {1}",
                                                                        startTimestamp.ToString(),
                                                                        stopTimestamp.ToString()));

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/peripherals/fan/data/query",
                                                          Method.GET,
                                                          restRequest =>
                                                              { 
                                                                  restRequest.AddParameter(ParamFanNumber, fanNumber);
                                                                  restRequest.AddParameter(ParamTimestampStart, startTimestamp);
                                                                  restRequest.AddParameter(ParamTimestampStop, stopTimestamp);
                                                              });

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            return fanCtrlData;
        }

        /// <summary>
        /// Queries historical temperature data by timestamps or the current available reading associated with a serial number.
        /// </summary>
        /// <param name="serialNumber">Serial number of the temperature sensor to query data from.</param>
        /// <param name="startTimestamp">Start timestamp to query historical data from.</param>
        /// <param name="stopTimestamp">Stop timestamp to query historical data from.</param>
        /// <param name="restrictByTime">true to query by timestamp range; false otherwise.</param>
        /// <returns>[0...N] Historical data readings.</returns>
        internal async Task<List<Tuple<DateTime, Single>>> QueryTemperatureSensorDataWorker(string serialNumber,
                                                                                            DateTime startTimestamp,
                                                                                            DateTime stopTimestamp, 
                                                                                            bool restrictByTime)
        {
            List<Tuple<DateTime, Single>> temperatureData = new List<Tuple<DateTime, Single>>();

            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            if (restrictByTime == true)
                if (stopTimestamp < startTimestamp)
                    throw new ArgumentOutOfRangeException("DateTime stopTimestamp",
                                                          string.Format("Start Timestamp <= X <= Stop Timestamp. Start: {0}, Stop: {1}",
                                                                        startTimestamp.ToString(),
                                                                        stopTimestamp.ToString()));

            //Call Worker.
            IRestResponse restResponse = await RESTWorker("/MooseBox/API/v1.0/peripherals/temperature/data/query",
                                                          Method.GET,
                                                          restRequest =>
                                                              {
                                                                  restRequest.AddParameter(ParamFanNumber, serialNumber);
                                                                  restRequest.AddParameter(ParamTimestampStart, startTimestamp);
                                                                  restRequest.AddParameter(ParamTimestampStop, stopTimestamp);
                                                              });

            //Parse resultant data.
            dynamic jsonResult = JsonConvert.DeserializeObject(restResponse.Content);

            return temperatureData;
        }

        /// <summary>
        /// Worker method to reduce copy-and-past boiler-plate code associated with simple REST requests.
        /// </summary>
        /// <param name="resource">Resource portion of the URL to execute request on.</param>
        /// <param name="method">HTTP Method describing the REST request.</param>
        /// <returns>Validated REST Response object.</returns>
        private async Task<IRestResponse> RESTWorker(string resource, Method method)
        {
            //Chain-it!
            return await RESTWorker(resource,
                                    method,
                                    restRequest => { Debug.Assert(restRequest != null); },
                                    HttpStatusCode.OK);
        }

        /// <summary>
        /// Worker method to reduce copy-and-past boiler-plate code associated with simple REST requests.
        /// </summary>
        /// <param name="resource">Resource portion of the URL to execute request on.</param>
        /// <param name="method">HTTP Method describing the REST request.</param>
        /// <param name="addParamAction">Callback to invoke for context-specific parameter adding.</param>
        /// <returns>Validated REST Response object.</returns>
        private async Task<IRestResponse> RESTWorker(string resource,
                                                     Method method,
                                                     Action<RestRequest> addParamAction)
        {
            //Chain-it!
            return await RESTWorker(resource,
                                    method,
                                    addParamAction,
                                    HttpStatusCode.OK);
        }

        /// <summary>
        /// Worker method to reduce copy-and-past boiler-plate code associated with simple REST requests.
        /// </summary>
        /// <param name="resource">Resource portion of the URL to execute request on.</param>
        /// <param name="method">HTTP Method describing the REST request.</param>
        /// <param name="addParamAction">Callback to invoke for context-specific parameter adding.</param>
        /// <param name="successCodes">[1...N] HTTP Status Codes that indicate a successful REST operation to validate on.</param>
        /// <returns>Validated REST Response object.</returns>
        private async Task<IRestResponse> RESTWorker(string resource,
                                                     Method method,
                                                     Action<RestRequest> addParamAction,
                                                     params HttpStatusCode[] successCodes)
        {
            //Parameter Validations.
            Debug.Assert(string.IsNullOrEmpty(resource) == false);
            Debug.Assert(addParamAction != null);
            Debug.Assert(successCodes != null);
            Debug.Assert(successCodes.Length >= 1);

            //Buid the REST Request.
            RestRequest restRequest = new RestRequest(resource, method);

            addParamAction(restRequest);

            //Execute and Validate the REST Request.
            IRestResponse restResponse = await m_restClient.ExecuteTaskAsync(restRequest);

            if (successCodes.Contains(restResponse.StatusCode) == false)
                throw new MooseBoxServiceException(restResponse);

            return restResponse;
        }
        #endregion

        private readonly RestClient m_restClient;

        private const string ParamCelsiusMin = "celsius_min";
        private const string ParamCelsiusMax = "celsius_max";
        private const string ParamCelsiusThreshold = "celsius_threshold";
        private const string ParamEmailAddress = "email_address";
        private const string ParamFanNumber = "fan_number";
        private const string ParamPowerOn = "power_on";
        private const string ParamSerialNumber = "serial_number";
        private const string ParamTimestampStart = "timestamp_start";
        private const string ParamTimestampStop = "timestamp_stop";

        private const string DefaultVersionStr = "0.0.0.0";
    }
}
