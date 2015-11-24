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
    /// Defines an abstraction for system information such as memory and version reporting.
    /// </summary>
    internal class SysInfo
    {
        #region Public Methods (static)
        /// <summary>
        /// Provides a convenience method to perform a memory percentage calculation.
        /// </summary>
        /// <param name="availableMemoryBytes">Number of bytes available on MooseBox.</param>
        /// <param name="totalMemoryBytes">Total number of bytes on MooseBox.</param>
        /// <returns>0 - 100 percentage value describing memory used.</returns>
        internal static Byte CalculateMemoryPercentUsed(UInt64 availableMemoryBytes, UInt64 totalMemoryBytes)
        {
            Double percentage = 0;

            //Parameter Validates.
            if (totalMemoryBytes == 0)
                throw new DivideByZeroException("totalMemoryBytes cannot be zero");

            //Perform calculation.
            percentage = (Convert.ToDouble(availableMemoryBytes) / Convert.ToDouble(totalMemoryBytes)) * 100.0;

            Debug.Assert(percentage >= 0.0 && percentage <= 100.0);

            return Convert.ToByte(100 - percentage);
        }
        #endregion

        #region Accessors (static)
        /// <summary>
        /// Returns system information instance for the MooseBox.
        /// </summary>
        internal static SysInfo MooseBox
        {
            get { return s_mooseBox.Value; }
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mooseBoxRESTAPI">Instance of a versioned MooseBox REST API.</param>
        internal SysInfo(IMooseBoxRESTAPI mooseBoxRESTAPI)
        {
            //Parameter Validations.
            if (mooseBoxRESTAPI == null)
                throw new ArgumentNullException("IMooseBoxRESTAPI mooseBoxRESTAPI");

            //Set Members.
            m_mooseBoxRESTAPI = mooseBoxRESTAPI;
        }
        #endregion

        #region Public Methods (instance)
        /// <summary>
        /// Queries system memory information.
        /// </summary>
        /// <returns>Available and total memory in bytes.</returns>
        internal async Task<Tuple<UInt64, UInt64>> QuerySystemInformation()
        {
            Tuple<UInt64, UInt64> memoryResult = await m_mooseBoxRESTAPI.QuerySystemInformation();

            return memoryResult;
        }

        /// <summary>
        /// Queries component versions of the MooseBox.
        /// </summary>
        /// <returns>MooseBox web service, fan control daemon and temperature daemon component versions.</returns>
        internal async Task<Tuple<Version, Version, Version>> QuerySystemVersion()
        {
            Tuple<Version, Version, Version> sysVersion = await m_mooseBoxRESTAPI.QuerySystemVersion();

            return sysVersion;
        }
        #endregion

        private readonly IMooseBoxRESTAPI m_mooseBoxRESTAPI;

        private static readonly Lazy<SysInfo> s_mooseBox = new Lazy<SysInfo>(() => { return new SysInfo(MooseBoxRESTAPIFactory.Instance.Create()); }, true);
    }
}
