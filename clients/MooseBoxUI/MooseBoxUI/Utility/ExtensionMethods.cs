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
    /// Defines a series of disjoint extension utility methods for the entire MooseBoxUI project.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Converts a C# DateTime to a JavaScript Date object.
        /// </summary>
        /// <param name="dateTime">DateTime to convert from.</param>
        /// <returns>JavaScipt date ticks.</returns>
        /// <see href="http://stackoverflow.com/questions/1016847/converting-net-datetime-to-json"/>
        internal static double ToUnixTicks(this DateTime dateTime)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dateTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);

            return ts.TotalMilliseconds;
        }

        /// <summary>
        /// Converts a JavaScript Date object to a C# DateTime.
        /// </summary>
        /// <param name="unixTicks">JavaScript date ticks.</param>
        /// <returns>Converted C# DateTime.</returns>
        /// <see href="http://renditionprotocol.blogspot.com/2013/08/convert-javascript-epoch-milliseconds.html"/>
        internal static DateTime FromUnixTicks(Double unixTicks)
        {
            DateTime _01jan1970 = new DateTime(1970, 1, 1);

            DateTime convertedDT = _01jan1970.AddTicks(Convert.ToInt64(unixTicks) * 10000);

            return convertedDT;
        }
    }
}
