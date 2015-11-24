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
    /// Defines an abstraction for a numbered USB Fan plugged into the YepKit YKSUH connected to the MooseBox.
    /// </summary>
    internal sealed class Fan
    {
        #region Accessors (static)
        /// <summary>
        /// Gets USB Fan #1.
        /// </summary>
        internal static Fan Fan1 { get { return s_fan1.Value; } }

        /// <summary>
        /// Gets USB Fan #2.
        /// </summary>
        internal static Fan Fan2 { get { return s_fan2.Value; } }

        /// <summary>
        /// Gets USB Fan #3.
        /// </summary>
        internal static Fan Fan3 { get { return s_fan3.Value; } }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fanNumber">USB Fan number to associate with this instance.</param>
        /// <param name="mooseBoxRESTAPI">Instance to a versioned MooseBox REST API.</param>
        internal Fan(Byte fanNumber, IMooseBoxRESTAPI mooseBoxRESTAPI)
        {
            //Parameter Validations.
            if (fanNumber < MinFanNumber || fanNumber > MaxFanNumber)
                throw new ArgumentOutOfRangeException("Byte fanNumber",
                                                      string.Format("{0} <= Fan <= {1}. Found: {2}",
                                                                    MinFanNumber,
                                                                    MaxFanNumber,
                                                                    fanNumber));

            if (mooseBoxRESTAPI == null)
                throw new ArgumentNullException("IMooseBoxRESTAPI mooseBoxRESTAPI");

            //Set Members.
            m_fanNumber = fanNumber;

            m_mooseBoxRESTAPI = mooseBoxRESTAPI;
        }
        #endregion

        #region Public Methods (instance)
        /// <summary>
        /// Clears all historical power data associated with this fan.
        /// </summary>
        /// <returns>Asynchronous Processing Task.</returns>
        internal async Task Clear()
        {
            //Call MooseBox REST API.
            await m_mooseBoxRESTAPI.ClearFanCtrlData(m_fanNumber);
        }

        /// <summary>
        /// Attempts a manual power on/off set of a USB Fan.
        /// </summary>
        /// <param name="powerOn">true to power fan; false to disable.</param>
        /// <returns>Asynchronous Processing Task.</returns>
        /// <remarks>Numbered fan must not currently be engaged by Fan Automation.</remarks>
        internal async Task SetPower(bool powerOn)
        {
            //Call MooseBox REST API.
            await m_mooseBoxRESTAPI.PowerFanCtrl(m_fanNumber, powerOn);
        }

        /// <summary>
        /// Queries the latest power on/off state reading of this USB Fan.
        /// </summary>
        /// <returns>Tuple with a timestamp and a power on/off state value.</returns>
        internal async Task<Tuple<bool, DateTime>> QueryCurrentReading()
        {
            //Call MooseBox REST API.
            Tuple<bool, DateTime> reading = await m_mooseBoxRESTAPI.QueryFanCtrlData(m_fanNumber);

            return reading;
        }

        /// <summary>
        /// Queries [0...N] historical power on/off readings within a range of timestamps.
        /// </summary>
        /// <param name="startTimestamp">Range start timestamp.</param>
        /// <param name="stopTimestamp">Range end timestamp.</param>
        /// <returns>[0...N] historical power on/off readings.</returns>
        internal async Task<List<Tuple<bool, DateTime>>> QueryHistoricalReadings(DateTime startTimestamp, DateTime stopTimestamp)
        {
            List<Tuple<bool, DateTime>> allHistoricalReadings = new List<Tuple<bool, DateTime>>();

            //Parameter Validations.
            if (stopTimestamp < startTimestamp)
                throw new ArgumentOutOfRangeException("DateTime stopTimestamp",
                                                      string.Format("Start <= X <= Stop.  Start: {0}, Stop: {1}",
                                                                    startTimestamp.ToString(),
                                                                    stopTimestamp.ToString()));

            //Call MooseBox REST API; like temperature, chunk it out to ease embedded device.
            for (DateTime rangeStart = startTimestamp; rangeStart < stopTimestamp; rangeStart += MaxQueryableTimeSpan)
            {
                List<Tuple<bool, DateTime>> rangeReadings = await m_mooseBoxRESTAPI.QueryFanCtrlData(m_fanNumber,
                                                                                                     rangeStart,
                                                                                                     rangeStart + MaxQueryableTimeSpan);

                allHistoricalReadings.AddRange(rangeReadings);
            }

            return allHistoricalReadings;
        }

        /// <summary>
        /// Queries the start and stop timestamps associated with this fan's historical data.
        /// </summary>
        /// <returns>Tuple with start and stop timestamps.</returns>
        internal async Task<Tuple<DateTime, DateTime>> QueryTimestmapRange()
        {
            //Call the REST API for the start and stop timestamps.
            Tuple<DateTime, DateTime> timestamps = await m_mooseBoxRESTAPI.QueryFanCtrlTimestamps(m_fanNumber);

            return timestamps;
        }
        #endregion

        #region Accessors (instance)
        /// <summary>
        /// Gets the number associated with this USB Fan.
        /// </summary>
        internal Byte Number
        {
            get { return m_fanNumber; }
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Compares this instance of a Fan against another for value equality.
        /// </summary>
        /// <param name="obj">Instance to compare against.</param>
        /// <returns>true if equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool result = true;
            Fan other = obj as Fan;

            if (other != null)
                result &= (this.m_fanNumber == other.m_fanNumber);
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
            int hashCode = 'F' + 'A' + 'N';

            hashCode ^= m_fanNumber.GetHashCode();

            return hashCode;
        }
        #endregion

        private readonly Byte m_fanNumber;

        private const Byte MinFanNumber = 1;
        private const Byte MaxFanNumber = 3;

        private readonly IMooseBoxRESTAPI m_mooseBoxRESTAPI;

        private static readonly TimeSpan MaxQueryableTimeSpan = new TimeSpan(2, 0, 0);

        private static readonly Lazy<Fan> s_fan1 = new Lazy<Fan>(() => { return new Fan(1, MooseBoxRESTAPIFactory.Instance.Create()); }, true);
        private static readonly Lazy<Fan> s_fan2 = new Lazy<Fan>(() => { return new Fan(2, MooseBoxRESTAPIFactory.Instance.Create()); }, true);
        private static readonly Lazy<Fan> s_fan3 = new Lazy<Fan>(() => { return new Fan(3, MooseBoxRESTAPIFactory.Instance.Create()); }, true);
    }
}
