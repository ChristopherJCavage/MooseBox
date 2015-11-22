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
using System.Threading.Tasks;

namespace MooseBoxUI.Client.REST
{
    /// <summary>
    /// Defines an interface for a MooseBox REST API, regardless of version.
    /// </summary>
    internal interface IMooseBoxRESTAPI
    {
        Task RegisterFanAutomation(Byte fanNumber,
                                   string serialNumber,
                                   Single celsiusThreshold);

        Task UnegisterFanAutomation(Byte fanNumber);

        Task ClearFanCtrlData(Byte fanNumber);

        Task PowerFanCtrl(Byte fanNumber, bool powerOn);

        Task<Tuple<bool, DateTime>> QueryFanCtrlData(Byte fanNumber);

        Task<List<Tuple<bool, DateTime>>> QueryFanCtrlData(Byte fanNumber, DateTime startTimestamp, DateTime stopTimestamp);

        Task<Tuple<DateTime, DateTime>> QueryFanCtrlTimestamps(Byte fanNumber);

        Task<List<FanAutomationConfig>> ListFanAutomationConfig();

        Task<Tuple<UInt64, UInt64>> QuerySystemInformation();

        Task<Tuple<Version, Version, Version>> QuerySystemVersion();

        Task RegisterTemperatureAlarm(string serialNumber,
                                      Single celsiusMin,
                                      Single celsiusMax,
                                      string emailAddress);

        Task UnregisterTemperatureAlarm(string emailAddress);

        Task UnregisterTemperatureAlarm(string emailAddress, string serialNumber);

        Task<List<TemperatureAlarmConfig>> ListTemperatureAlarmConfig();

        Task ClearTemperatureSensorData(string serialNumber);

        Task<Tuple<DateTime, Single>> QueryTemperatureSensorData(string serialNumber);

        Task<List<Tuple<DateTime, Single>>> QueryTemperatureSensorData(string serialNumber, DateTime startTimestamp, DateTime stopTimestamp);

        Task<List<string>> QueryTemperatureSensorSerialNumbers();

        Task<Tuple<DateTime, DateTime>> QueryTemperatureSensorTimestamps(string serialNumber);
    }
}
