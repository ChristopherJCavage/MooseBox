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

namespace MooseBoxUI.Utility
{
    /// <summary>
    /// Defines an abstraction for convert units of common types.
    /// </summary>
    internal static class ConvertUnits
    {
        /// <summary>
        /// Defines an abstraction for conversion of temperature values.
        /// </summary>
        internal static class Temperature
        {
            /// <summary>
            /// Converts a temperature reading in Celsius to Fahrenheit.
            /// </summary>
            /// <param name="celsius">Value in Celsius.</param>
            /// <returns>Value in Fahrenheit.</returns>
            /// <see href="http://www.manuelsweb.com/temp.htm"/>
            internal static Single CelsiusToFahrenheit(Single celsius)
            {
                //°C  x  9/5 + 32 = °F
                Single fahrenheit = celsius * (9.0f / 5.0f) + 32.0f;

                return fahrenheit;
            }

            /// <summary>
            /// Converts a temperature reading in Fahrenheit to Celsius.
            /// </summary>
            /// <param name="fahrenheit">Value in Fahrenheit.</param>
            /// <returns>Value in Celsius.</returns>
            /// <see href="http://www.manuelsweb.com/temp.htm"/>
            internal static Single FahrenheitToCelsius(Single fahrenheit)
            {
                //(°F  -  32)  x  5/9 = °C
                Single celsius = (5.0f / 9.0f) * (fahrenheit - 32.0f);

                return celsius;
            }
        }

        /// <summary>
        /// Defines an abstraction for conversion of computer memory values.
        /// </summary>
        internal static class Memory
        {
            /// <summary>
            /// Converts a memory reading in bytes to megabytes.
            /// </summary>
            /// <param name="bytes">Value in bytes.</param>
            /// <returns>Value in megabytes.</returns>
            internal static UInt64 BytesToMegabytes(UInt64 bytes)
            {
                UInt64 megaBytes = bytes / 1024 / 1024;

                return megaBytes;
            }
        }
    }
}
