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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// Defines WinForm main panel for configuration of Temperature Alarms.
    /// </summary>
    public partial class ConfigureTemperatureAlarmForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfigureTemperatureAlarmForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for main UI loading.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void ConfigureTemperatureAlarmForm_Load(object sender, EventArgs e)
        {
            Reload();
        }

        /// <summary>
        /// Event handler a user changing the selection of the Temperature Sensors ComboBox.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void comboBoxTemperatureSensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Event handler a user changing the selection of the Email Addresses ComboBox.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void comboBoxRegisteredEmails_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Event handler for the user clicking the 'Register' button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonCreateNewAlarm_Click(object sender, EventArgs e)
        {
            using (CreateNewAlarmForm createNewAlarmForm = new CreateNewAlarmForm(m_temperatureAlarm))
                if (createNewAlarmForm.ShowDialog() == DialogResult.OK)
                    Reload();
        }

        /// <summary>
        /// Event handler for the user clicking the 'Unregister' button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonUnregister_Click(object sender, EventArgs e)
        {
            TemperatureSensor selectedTemperatureSensor = GetSelectedTemperatureSensor();

            //We need both an email and a sensor to perform an unregistration.
            if (string.IsNullOrEmpty(comboBoxRegisteredEmails.SelectedItem as string) == false && selectedTemperatureSensor != null)
            {
                //Unregister the alarm.
                string emailAddress = comboBoxRegisteredEmails.SelectedItem as string;

                AsyncHelper.RunSync(() => { return m_temperatureAlarm.Unregister(selectedTemperatureSensor, emailAddress); });

                //Reload the UI components.
                Reload();

                //Inform the end-user.
                MessageBox.Show("Temperature Alarm Successfully Unregistered",
                                "Success",
                                MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Please select both the Temperature Sensor and Email Address to unregister from.",
                                "Warning",
                                MessageBoxButtons.OK);
        }

        /// <summary>
        /// Event handler for user click of the 'Exit' button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Worker Methods
        /// <summary>
        /// Worker method to find the selected temperature sensor from the ComboBox.
        /// </summary>
        /// <returns>Select temperature sensor if found; null if none found.</returns>
        private TemperatureSensor GetSelectedTemperatureSensor()
        {
            TemperatureSensor selectedTemperatureSensor = null;

            //Is a Temperature Sensor selected?
            if (comboBoxTemperatureSensors.SelectedItem as string == TemperatureSensor._59000002A218F928.SerialNumber)
                selectedTemperatureSensor = TemperatureSensor._59000002A218F928;
            else if (comboBoxTemperatureSensors.SelectedItem as string == TemperatureSensor._F7000002A215B828.SerialNumber)
                selectedTemperatureSensor = TemperatureSensor._F7000002A215B828;

            return selectedTemperatureSensor;
        }

        /// <summary>
        /// Worker method to completely reload all Temperature Alarm data and synchronize it to the UI.
        /// </summary>
        private void Reload()
        {
            LoadData();

            Update();
        }

        /// <summary>
        /// Worker method to reload all Temperature Alarmt data.
        /// </summary>
        private void LoadData()
        {
            //Query the current state of all Temperature Alarms.
            m_temperatureAlarm = AsyncHelper.RunSync(() => { return TemperatureAlarm.QueryCurrentTemperatureAlarms(); });

            //Erase ComboBox listings and other components.
            comboBoxRegisteredEmails.Items.Clear();

            textBoxTemperatureMin.Text = string.Empty;
            textBoxTemperatureMax.Text = string.Empty;

            //Configure the ComboxBox now.
            try
            {
                TemperatureSensor[] temperatureSensors = new TemperatureSensor[] { TemperatureSensor._59000002A218F928,
                                                                                   TemperatureSensor._F7000002A215B828 };

                foreach (TemperatureSensor temperatureSensor in temperatureSensors)
                {
                    List<Tuple<string, Single, Single>> registeredAlarms = null;

                    if (m_temperatureAlarm.TryGetRegistrationInfo(temperatureSensor, out registeredAlarms) == true)
                    {
                        Debug.Assert(registeredAlarms != null);

                        var emails = registeredAlarms.Select(obj => obj.Item1).Distinct();

                        foreach (var email in emails)
                            comboBoxRegisteredEmails.Items.Add(email);
                    }
                }
            }
            catch { Debug.Assert(false); }
        }

        /// <summary>
        /// Worker method to synchronize temperature alarm state to the UI.
        /// </summary>
        private new void Update()
        {
            bool eraseTextBoxes = false;
            string selectedEmailAddress = string.Empty;
            TemperatureSensor selectedTemperatureSensor = GetSelectedTemperatureSensor();

            Debug.Assert(m_temperatureAlarm != null);

            //Query the current Temperature Alarms.
            try
            {
                //Is an email address selected?
                selectedEmailAddress = comboBoxRegisteredEmails.SelectedItem as string;

                //If both are selected, fill in the temperature data; otherwise, erase the indicators.
                if (string.IsNullOrEmpty(selectedEmailAddress) == false && selectedTemperatureSensor != null)
                {
                    List<Tuple<string, Single, Single>> registeredAlarms = null;

                    if (m_temperatureAlarm.TryGetRegistrationInfo(selectedTemperatureSensor, out registeredAlarms) == true)
                    {
                        Tuple<string, Single, Single> registeredAlarm = registeredAlarms.Find(obj => obj.Item1 == selectedEmailAddress);

                        textBoxTemperatureMin.Text = ConvertUnits.Temperature.CelsiusToFahrenheit(registeredAlarm.Item2).ToString();
                        textBoxTemperatureMax.Text = ConvertUnits.Temperature.CelsiusToFahrenheit(registeredAlarm.Item3).ToString();

                        eraseTextBoxes = false;
                    }
                }
                else
                    eraseTextBoxes = true;
            }
            catch { eraseTextBoxes = true; }

            //Erase temperatures from text boxes if an error occurred or the user didn't select anything.
            if (eraseTextBoxes == true)
            {
                textBoxTemperatureMin.Text = string.Empty;
                textBoxTemperatureMax.Text = string.Empty;
            }
        }
        #endregion

        private TemperatureAlarm m_temperatureAlarm;
    }
}
