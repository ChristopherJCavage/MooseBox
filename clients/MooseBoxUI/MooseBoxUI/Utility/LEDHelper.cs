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
using System.Diagnostics;
using System.Windows.Forms;

namespace MooseBoxUI.Utility
{
    /// <summary>
    /// Defines a utility class to assist with LED icons on the UI.
    /// </summary>
    internal static class LEDHelper
    {
        /// <summary>
        /// Changes the color of an LED PictureBox.
        /// </summary>
        /// <param name="ledPictureBox">WinFom LED PictureBox to change.</param>
        /// <param name="ledColor">Color to set LED to.</param>
        internal static void ChangeColor(PictureBox ledPictureBox, LEDColors ledColor)
        {
            string resourceUrl = string.Empty;

            //Parameter Validations.
            if (ledPictureBox == null)
                throw new ArgumentNullException("PictureBox ledPictureBox");

            //Switch on the color and update the image.
            switch(ledColor)
            {
                case LEDColors.Green: resourceUrl = @"..\..\Resources\GreenLED_25x25.png"; break;
                case LEDColors.Yellow: resourceUrl = @"..\..\Resources\YellowLED_25x25.png"; break;
                case LEDColors.Red: resourceUrl = @"..\..\Resources\RedLED_25x25.png"; break;
            
                default:
                    throw new ArgumentOutOfRangeException("LEDColors ledColor",
                                                          string.Format("Unsupported Color. Color: {0}",
                                                                        ledColor.ToString()));
            }

            //Attempt to load it.
            Debug.Assert(string.IsNullOrEmpty(resourceUrl) == false);
            
            ledPictureBox.Load(resourceUrl);
        }
    }
}
