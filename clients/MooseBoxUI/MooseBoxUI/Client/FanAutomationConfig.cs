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
    /// Defines an abstraction for an automation entry for a singular numbered Fan.
    /// </summary>
    internal sealed class FanConfig
    {
        /// <summary>
        /// Warmest allowable temperature reading, in Celsius.
        /// </summary>
        internal Single PowerOnThresholdCelsius { get; set; }

        /// <summary>
        /// Number of the Fan to automate.
        /// </summary>
        internal Byte FanNumber { get; set; }
    }

    /// <summary>
    /// Defines an abstraction for [0...N] fan automation entries to correlate to a temperature sensor.
    /// </summary>
    internal sealed class FanAutomationConfig
    {
        /// <summary>
        /// Serial number of the temperature sensor to correlate fan automation to.
        /// </summary>
        internal string SerialNumber { get; set; }

        /// <summary>
        /// [0...N] fan automation entries' instructions.
        /// </summary>
        internal List<FanConfig> FansConfig { get; set; }
    }
}
