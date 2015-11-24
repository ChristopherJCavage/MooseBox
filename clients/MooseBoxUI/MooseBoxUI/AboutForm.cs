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
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// WinForms About versioning dialog.
    /// </summary>
    public partial class AboutForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for About dialog's load.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void AboutForm_Load(object sender, EventArgs e)
        {
            labelClientVersion.Text = AppVersion.MooseBox.ToString();
        }
        #endregion
    }
}
