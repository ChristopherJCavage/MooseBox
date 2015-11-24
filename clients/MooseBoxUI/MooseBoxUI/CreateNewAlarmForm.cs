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
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// Defines WinForm for actual registration of a new Temperature Alarm.
    /// </summary>
    public partial class CreateNewAlarmForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="temperatureAlarmRef">Reference to instance of MooseBox Temperature Alarm object.</param>
        public CreateNewAlarmForm(TemperatureAlarm temperatureAlarmRef)
        {
            //Parameter Validations.
            if (temperatureAlarmRef == null)
                throw new ArgumentNullException("TemperatureAlarm temperatureAlarmRef");

            //WinForms UI Initializations.
            InitializeComponent();
            
            //Set Members.
            m_temperatureAlarmRef = temperatureAlarmRef;
        }
        #endregion

        #region Event Handlerso
        /// <summary>
        /// Event handler for user click of the 'Save' button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Is a Temperature Sensor selected?
            TemperatureSensor temperatureSensor = GetSelectedTemperatureSensor();

            if (temperatureSensor == null)
            {
                MessageBox.Show("Please select a Temperature Sensor to register alarm to.",
                                "Warning",
                                MessageBoxButtons.OK);

                return;
            }

            //Is an email entered; is it valid?
            if (IsValidEmailAddress(textBoxEmailAddress.Text) == false)
            {
                MessageBox.Show(string.Format("An invalid email address was entered.{0}Please re-enter and try again.",
                                              Environment.NewLine),
                                "Warning",
                                MessageBoxButtons.OK);

                return;
            }

            //Is this email already registered?  Note that, MooseBox supports a Many relationship but 
            //to make the client UI logic easier (and faster devel time) we're restricting one email
            //per temperature sensor, which is what will happen in practice anyway.
            List<Tuple<string, Single, Single>> registeredAlarms = null;

            if (m_temperatureAlarmRef.TryGetRegistrationInfo(temperatureSensor, out registeredAlarms) == true)
            {
                Debug.Assert(registeredAlarms != null);

                var emails = registeredAlarms.Select(obj => obj.Item1);

                if (emails.Any(email => email == textBoxEmailAddress.Text) == true)
                {
                    MessageBox.Show(string.Format("Email address is already registered to this temperature sensor.{0}Please unregister it first to change temperature ranges.",
                                                  Environment.NewLine),
                                    "Warning",
                                    MessageBoxButtons.OK);

                    return;
                }
            }

            //Are the temperature values valid numerics?  Convert.
            bool validSingles = true;
            Single minTemperatureCelsius = Single.NaN;
            Single maxTemperatureCelsius = Single.NaN;

            try
            {
                minTemperatureCelsius = Convert.ToSingle(textBoxMinTemperature.Text);
                maxTemperatureCelsius = Convert.ToSingle(textBoxMaxTemperature.Text);

                minTemperatureCelsius = ConvertUnits.Temperature.FahrenheitToCelsius(minTemperatureCelsius);
                maxTemperatureCelsius = ConvertUnits.Temperature.FahrenheitToCelsius(maxTemperatureCelsius);
            }
            catch { validSingles = false; }

            if (validSingles == false)
            {
                MessageBox.Show(string.Format("One or both temperature values are invalid.{0}Please re-enter and try again.",
                                              Environment.NewLine),
                                "Warning",
                                MessageBoxButtons.OK);

                return;
            }

            //Are the temperature values within the appropiate ranges?
            if (maxTemperatureCelsius < minTemperatureCelsius)
            {
                MessageBox.Show(string.Format("Minimum Temperature Threshold cannot be greater than the maximum.{0}Please re-enter and try again.",
                                              Environment.NewLine),
                                "Warning",
                                MessageBoxButtons.OK);

                return;
            }
            
            if (minTemperatureCelsius < Limits.MinTemperatureCelsius ||
                minTemperatureCelsius > Limits.MaxTemperatureCelsius ||
                maxTemperatureCelsius < Limits.MinTemperatureCelsius ||
                maxTemperatureCelsius > Limits.MaxTemperatureCelsius)
            {
                MessageBox.Show(string.Format("Min/Max Temperature Thresholds are out of range of the supported values of the sensor.{0}{1}Must be >= {2} F and <= {3} F",
                                              Environment.NewLine,
                                              Environment.NewLine,
                                              ConvertUnits.Temperature.CelsiusToFahrenheit(Limits.MinTemperatureCelsius),
                                              ConvertUnits.Temperature.CelsiusToFahrenheit(Limits.MaxTemperatureCelsius)),
                                "Out of Range Temperature",
                                MessageBoxButtons.OK);

                return;
            }

            //We've done our due diligence to validate inputs; just go for it!
            AsyncHelper.RunSync(() => { return m_temperatureAlarmRef.Register(temperatureSensor,
                                                                              textBoxEmailAddress.Text,
                                                                              minTemperatureCelsius,
                                                                              maxTemperatureCelsius); });

            MessageBox.Show("Alarm Successfully Registered!",
                            "Success",
                            MessageBoxButtons.OK);

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Event handler for user click of the 'Cancel' button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

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
        /// Tests if a string can represent a valid email address.
        /// </summary>
        /// <param name="emailAddress">String to test.</param>
        /// <returns>true if considered a valid email; false otherwise.</returns>
        /// <see href="http://stackoverflow.com/questions/16167983/best-regular-expression-for-email-validation-in-c-sharp"/>
        private bool IsValidEmailAddress(string emailAddress)
        {
            bool isValid = false;
            
            if (string.IsNullOrEmpty(emailAddress) == false)
                isValid = Regex.IsMatch(emailAddress,
                                        @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                                        RegexOptions.IgnoreCase);

            return isValid;
        }
        #endregion

        private readonly TemperatureAlarm m_temperatureAlarmRef;
    }
}
