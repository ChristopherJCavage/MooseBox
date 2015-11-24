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
using System.Threading.Tasks;

namespace MooseBoxUI.Client
{
    /// <summary>
    /// Defines an abstraction for a iButtonLink T-Probe Temperature Sensor associated with the MooseBox.
    /// </summary>
    internal sealed class TemperatureSensor
    {
        #region Public Methods (static)
        /// <summary>
        /// Queries all temperature sensors associated with MooseBox and creates instances (i.e. static construction).
        /// </summary>
        /// <returns>[1...N] MooseBox Temperature Sensors.</returns>
        internal async static Task<List<TemperatureSensor>> GetTemperatureSensors()
        {
            List<TemperatureSensor> temperatureSensors = new List<TemperatureSensor>();
            IMooseBoxRESTAPI mooseBoxRESTAPI = MooseBoxRESTAPIFactory.Instance.Create();

            //Query all available serial numbers.
            List<string> serialNumbers = await mooseBoxRESTAPI.QueryTemperatureSensorSerialNumbers();

            //Convert to TemperatureSensor instances.
            foreach (string serialNumber in serialNumbers)
                temperatureSensors.Add(new TemperatureSensor(serialNumber, mooseBoxRESTAPI));

            return temperatureSensors;
        }
        #endregion

        #region Accessors (static)
        /// <summary>
        /// Gets the Temperature Sensor with serial #F7000002A215B828.
        /// </summary>
        internal static TemperatureSensor _F7000002A215B828
        {
            get { return s_F7000002A215B828.Value; }
        }

        /// <summary>
        /// Gets the Temperature Sensor with serial #59000002A218F928.
        /// </summary>
        internal static TemperatureSensor _59000002A218F928
        {
            get { return s_59000002A218F928.Value; }
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serialNumber">Serial number of the temperature sensor.</param>
        /// <param name="mooseBoxRESTAPI">Versioned MooseBox REST API instance.</param>
        internal TemperatureSensor(string serialNumber, IMooseBoxRESTAPI mooseBoxRESTAPI)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(serialNumber) == true)
                throw new ArgumentNullException("string serialNumber");

            if (mooseBoxRESTAPI == null)
                throw new ArgumentNullException("IMooseBoxRESTAPI mooseBoxRESTAPI");

            //Set Members.
            m_serialNumber = serialNumber;

            m_mooseBoxRESTAPI = mooseBoxRESTAPI;
        }
        #endregion

        #region Public Methods (instance)
        /// <summary>
        /// Clears all historical data associated with a temperature sensor on the MooseBox.
        /// </summary>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Clear()
        {
            //Call the REST API to clear all of this sensor's data.
            await m_mooseBoxRESTAPI.ClearTemperatureSensorData(m_serialNumber);
        }

        /// <summary>
        /// Queries the latest temperature reading associated with a temperature sensor on the MooseBox.
        /// </summary>
        /// <returns>Latest temperature reading.</returns>
        internal async Task<Tuple<DateTime, Single>> QueryCurrentReading()
        {
            //Call the REST API for the current reading.
            Tuple<DateTime, Single> currentReading = await m_mooseBoxRESTAPI.QueryTemperatureSensorData(m_serialNumber);

            return currentReading;
        }

        /// <summary>
        /// Queries [0...N] historical readings within a range of timestamps.
        /// </summary>
        /// <param name="startTimestamp">Timestamp to start range query at.</param>
        /// <param name="stopTimestamp">Timestamp to end range query at.</param>
        /// <returns>[0...N] historical readings.</returns>
        internal async Task<List<Tuple<DateTime, Single>>> QueryHistoricalReadings(DateTime startTimestamp, DateTime stopTimestamp)
        {
            List<Tuple<DateTime, Single>> allHistoricalReadings = new List<Tuple<DateTime, Single>>();

            //Parameter Validations.
            if (stopTimestamp < startTimestamp)
                throw new ArgumentOutOfRangeException("DateTime stopTimestamp",
                                                      string.Format("Start <= X <= Stop.  Start: {0}, Stop: {1}",
                                                                    startTimestamp.ToString(),
                                                                    stopTimestamp.ToString()));

            //Raspberry PI 2 is still an embedded device; ease on memory - do it in chunks.
            for (DateTime rangeStart = startTimestamp; rangeStart < stopTimestamp; rangeStart += MaxQueryableTimeSpan)
            {
                List<Tuple<DateTime, Single>> rangeReadings = await m_mooseBoxRESTAPI.QueryTemperatureSensorData(m_serialNumber,
                                                                                                                 rangeStart,
                                                                                                                 rangeStart + MaxQueryableTimeSpan);

                allHistoricalReadings.AddRange(rangeReadings);
            }

            return allHistoricalReadings;
        }

        /// <summary>
        /// Queries the start and stop timestamps associated with this temperature sensor's historical data.
        /// </summary>
        /// <returns>Tuple with start and stop timestamps.</returns>
        internal async Task<Tuple<DateTime, DateTime>> QueryTimestmapRange()
        {
            //Call the REST API for the start and stop timestamps.
            Tuple<DateTime, DateTime> timestamps = await m_mooseBoxRESTAPI.QueryTemperatureSensorTimestamps(m_serialNumber);

            return timestamps;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets the serial number associated with this temperature sensor.
        /// </summary>
        internal string SerialNumber
        {
            get { return m_serialNumber; }
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Compares this instance of TemperatureSensor against another for value equality.
        /// </summary>
        /// <param name="obj">Instance to compare against.</param>
        /// <returns>true if equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool result = true;
            TemperatureSensor other = obj as TemperatureSensor;

            if (other != null)
                result &= (this.m_serialNumber == other.m_serialNumber);
            else
                result = false;

            return result;
        }

        /// <summary>
        /// Generates a hash code based on instance data.
        /// </summary>
        /// <returns>Generated hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = 'T' + 'E' + 'M' + 'P' + 'E' + 'R' + 'A' + 'T' + 'U' + 'R' + 'E' + 'S' + 'E' + 'N' + 'S' + 'O' + 'R';

            hashCode ^= m_serialNumber.GetHashCode();

            return hashCode;
        }
        #endregion

        private readonly string m_serialNumber;

        private readonly IMooseBoxRESTAPI m_mooseBoxRESTAPI;

        private static readonly TimeSpan MaxQueryableTimeSpan = new TimeSpan(1, 30, 0);

        private static readonly Lazy<TemperatureSensor> s_F7000002A215B828 = new Lazy<TemperatureSensor>(() => { return new TemperatureSensor("F7002A215B828", MooseBoxRESTAPIFactory.Instance.Create()); }, true);
        private static readonly Lazy<TemperatureSensor> s_59000002A218F928 = new Lazy<TemperatureSensor>(() => { return new TemperatureSensor("59002A218F928", MooseBoxRESTAPIFactory.Instance.Create()); }, true);
    }
}
