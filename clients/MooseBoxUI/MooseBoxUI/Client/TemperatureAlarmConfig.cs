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
using System;
using System.Collections.Generic;

namespace MooseBoxUI.Client
{
    /// <summary>
    /// Defines an abstraction for a subscriber's data for a Temperature Alarm configurations.
    /// </summary>
    internal sealed class TemperatureAlarmSubscriber
    {
        /// <summary>
        /// Email address of the subscriber.
        /// </summary>
        internal string Email { get; set; }

        /// <summary>
        /// Coldest allowable temperature, in Celsius.
        /// </summary>
        internal Single CelsiusMin { get; set; }

        /// <summary>
        /// Warmest allowable temperature, in Celsius.
        /// </summary>
        internal Single CelsiusMax { get; set; }
    }

    /// <summary>
    /// Defines an abstraction for a configuration object for all Temperature Alarms.
    /// </summary>
    internal sealed class TemperatureAlarmConfig
    {
        /// <summary>
        /// Serial number of the temperature sensor correlated to the Temperature Alarm.
        /// </summary>
        internal string SerialNumber { get; set; }

        /// <summary>
        /// [0...N] subscribers associated with this alarm and their temperature thresholds.
        /// </summary>
        internal List<TemperatureAlarmSubscriber> Subscribers { get; set; }
    }
}
