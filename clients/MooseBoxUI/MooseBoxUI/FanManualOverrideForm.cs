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
using MooseBoxUI.Client;
using MooseBoxUI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// Defines WinForms Fan Control Manual Override Panel.
    /// </summary>
    public partial class FanManualOverrideForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fanAutomationRef">Reference to MooseBox Fan Automation object.</param>
        public FanManualOverrideForm(FanAutomation fanAutomationRef)
        {
            //Parameter Validations.
            if (fanAutomationRef == null)
                throw new ArgumentNullException("FanAutomation fanAutomationRef");

            //WinForms UI Initialization.
            InitializeComponent();

            //Set Members.
            m_fanAutomationRef = fanAutomationRef;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// UI Event Handler for Loading.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void FanManualOverrideForm_Load(object sender, EventArgs e)
        {
            //Reset all Radio Buttons.
            radioButtonFan1Off.Checked = false;
            radioButtonFan1On.Checked = false;
            radioButtonFan2Off.Checked = false;
            radioButtonFan2On.Checked = false;
            radioButtonFan3Off.Checked = false;
            radioButtonFan3On.Checked = false;

            groupBoxFan1.Enabled = false;
            groupBoxFan2.Enabled = false;
            groupBoxFan3.Enabled = false;

            buttonSave.Enabled = false;

            //Update all Radio Buttons.
            ThreadPool.QueueUserWorkItem(obj =>
                {
                    try
                    {
                        //Kick off all of the Fan Queries.
                        Tuple<bool, DateTime> fan1Reading = AsyncHelper.RunSync(() => { return Fan.Fan1.QueryCurrentReading(); });
                        Tuple<bool, DateTime> fan2Reading = AsyncHelper.RunSync(() => { return Fan.Fan2.QueryCurrentReading(); });
                        Tuple<bool, DateTime> fan3Reading = AsyncHelper.RunSync(() => { return Fan.Fan3.QueryCurrentReading(); });

                        //Initialize Radio Buttons.
                        Invoke(new Action(() =>
                            {
                                UpdateRadioButton(Fan.Fan1, fan1Reading.Item1);
                                UpdateRadioButton(Fan.Fan2, fan2Reading.Item1);
                                UpdateRadioButton(Fan.Fan3, fan3Reading.Item1);

                                groupBoxFan1.Enabled = !m_fanAutomationRef.IsRegistered(Fan.Fan1);
                                groupBoxFan2.Enabled = !m_fanAutomationRef.IsRegistered(Fan.Fan2);
                                groupBoxFan3.Enabled = !m_fanAutomationRef.IsRegistered(Fan.Fan3);

                                buttonSave.Enabled = true;
                            }));
                    }
                    catch { } 
                });
        }

        /// <summary>
        /// Event Handler for user click of Save button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            UpdateFan(Fan.Fan1);
            UpdateFan(Fan.Fan2);
            UpdateFan(Fan.Fan3);

            this.Close();
        }

        /// <summary>
        /// Event Handler for user click of Cancel button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Worker Methods
        /// <summary>
        /// Updates a USB Fan for a manual override operation.
        /// </summary>
        /// <param name="fan">USB Fan to update.</param>
        private void UpdateFan(Fan fan)
        {
            Debug.Assert(fan != null);

            switch (fan.Number)
            {
                case 1: AsyncHelper.RunSync(() => { return Fan.Fan1.SetPower(radioButtonFan1On.Checked); }); break;
                case 2: AsyncHelper.RunSync(() => { return Fan.Fan2.SetPower(radioButtonFan2On.Checked); }); break;
                case 3: AsyncHelper.RunSync(() => { return Fan.Fan3.SetPower(radioButtonFan3On.Checked); }); break;

                default:
                    throw new ArgumentOutOfRangeException("Fan fan",
                                                          string.Format("Unknown Fan Number. Found: {0}",
                                                                        fan.Number));
            }
        }

        /// <summary>
        /// Synchronizes UI's radio buttons to reflect Fan power states.
        /// </summary>
        /// <param name="fan">USB Fan to synchronize UI to.</param>
        /// <param name="isPowered">true if powered; false otherwise.</param>
        private void UpdateRadioButton(Fan fan, bool isPowered)
        {
            Debug.Assert(fan != null);

            switch(fan.Number)
            {
                case 1:
                    if (isPowered == true)
                        radioButtonFan1On.Checked = true;
                    else
                        radioButtonFan1Off.Checked = true;

                    break;

                case 2:
                    if (isPowered == true)
                        radioButtonFan2On.Checked = true;
                    else
                        radioButtonFan2Off.Checked = true;

                    break;

                case 3:
                    if (isPowered == true)
                        radioButtonFan3On.Checked = true;
                    else
                        radioButtonFan3Off.Checked = true;

                    break;

                default:
                    throw new ArgumentOutOfRangeException("Fan fan",
                                                          string.Format("Unknown Fan Number. Found: {0}",
                                                                        fan.Number));
            }
        }
        #endregion

        private readonly FanAutomation m_fanAutomationRef;
    }
}
