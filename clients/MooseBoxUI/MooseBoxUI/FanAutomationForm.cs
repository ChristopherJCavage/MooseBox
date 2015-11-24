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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// Defines WinForms Fan Automation Panel.
    /// </summary>
    public partial class FanAutomationForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fanAutomationRef">Reference to the Fan Automation object.</param>
        public FanAutomationForm(FanAutomation fanAutomationRef) :
            base()
        {
            //Parameter Validations.
            if (fanAutomationRef == null)
                throw new ArgumentNullException("FanAutomation fanAutomationRef");

            //WinForms Initialization.
            InitializeComponent();

            //Set Members.
            m_fanAutomationRef = fanAutomationRef;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for form load event.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void FanAutomationForm_Load(object sender, EventArgs e)
        {
            //Initialize the ComboBoxes.
            PopulateComboBox(comboBoxFan1);
            PopulateComboBox(comboBoxFan2);
            PopulateComboBox(comboBoxFan3);

            //Synchronize the UI to the current Fan Automation state.
            Update();
        }

        /// <summary>
        /// Event handler for user click of Fan 1 Enable/Disable button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonFan1_Click(object sender, EventArgs e)
        {
            ProcessButtonClick(Fan.Fan1,
                               comboBoxFan1,
                               textBoxFan1,
                               buttonFan1);
        }

        /// <summary>
        /// Event handler for user click of Fan 2 Enable/Disable button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonFan2_Click(object sender, EventArgs e)
        {
            ProcessButtonClick(Fan.Fan2,
                               comboBoxFan2,
                               textBoxFan2,
                               buttonFan2);
        }

        /// <summary>
        /// Event handler for user click of Fan 3 Enable/Disable button.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonFan3_Click(object sender, EventArgs e)
        {
            ProcessButtonClick(Fan.Fan3,
                               comboBoxFan3,
                               textBoxFan3,
                               buttonFan3);
        }
        #endregion

        #region Worker Methods
        /// <summary>
        /// Master worker method to process a Fan automation request.
        /// </summary>
        /// <param name="fan">USB Fan to automate.</param>
        /// <param name="comboBox">ComboBox with sensor serial number.</param>
        /// <param name="textBox">TextBox with user-entered celsius threshold.</param>
        /// <param name="button">Enable/Disable button.</param>
        private void ProcessButtonClick(Fan fan,
                                        ComboBox comboBox,
                                        TextBox textBox,
                                        Button button)
        {
            Debug.Assert(fan != null);
            Debug.Assert(comboBox != null);
            Debug.Assert(textBox != null);
            Debug.Assert(button != null);

            //If it's already enabled, then disable it - allow the user to make edits.
            if (button.Text == DisableText)
            {
                //Unregister the fan from automation.
                AsyncHelper.RunSync(() => { return m_fanAutomationRef.Unregister(fan); });

                //Automatically un-power it.
                AsyncHelper.RunSync(() => { return fan.SetPower(false); });

                //Now prompt the user that we unpowered the fan.
                MessageBox.Show(string.Format("USB Fan #{0} has been unpowered. To change power without automation go to Fan Manual Override dialog.",
                                              fan.Number),
                                "Fan Unpowered",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                Debug.Assert(button.Text == EnableText);

                //Ensure they selected a temperature sensor.
                if (comboBox.SelectedItem as string != TemperatureSensor._59000002A218F928.SerialNumber &&
                    comboBox.SelectedItem as string != TemperatureSensor._F7000002A215B828.SerialNumber)
                {
                    MessageBox.Show("Please select a Temperature Sensor to register the fan against.",
                                    "Warning",
                                    MessageBoxButtons.OK);

                    return;
                }

                //Validate the Celsius Range.
                bool isValid = false;
                Single celsiusThreshold = Single.NaN;

                try
                {
                    celsiusThreshold = ConvertUnits.Temperature.FahrenheitToCelsius(Convert.ToSingle(textBox.Text));

                    isValid = (celsiusThreshold >= Limits.MinTemperatureCelsius) && (celsiusThreshold <= Limits.MaxTemperatureCelsius);
                }
                catch { isValid = false; }

                if (isValid == false)
                    MessageBox.Show(string.Format("Temperature Threshold must be between {0}F and {1}F. Please re-enter and try again.",
                                                  ConvertUnits.Temperature.CelsiusToFahrenheit(Limits.MinTemperatureCelsius),
                                                  ConvertUnits.Temperature.CelsiusToFahrenheit(Limits.MaxTemperatureCelsius)),
                                    "Bad Temperature Threshold",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                //Register the fan for automation.
                TemperatureSensor temperatureSensor = null;

                if (comboBox.SelectedItem as string == TemperatureSensor._59000002A218F928.SerialNumber)
                    temperatureSensor = TemperatureSensor._59000002A218F928;
                else
                    temperatureSensor = TemperatureSensor._F7000002A215B828;

                AsyncHelper.RunSync(() => { return m_fanAutomationRef.Register(fan, temperatureSensor, celsiusThreshold); });
            }

            //Call update
            Update();
        }

        /// <summary>
        /// Worker method to populate a ComboBox with all known MooseBox sensor serial numbers.
        /// </summary>
        /// <param name="comboBox">Instance to ComboBox to populate.</param>
        private void PopulateComboBox(ComboBox comboBox)
        {
            Debug.Assert(comboBox != null);

            comboBox.Items.Add(TemperatureSensor._59000002A218F928.SerialNumber);
            comboBox.Items.Add(TemperatureSensor._F7000002A215B828.SerialNumber);
        }

        /// <summary>
        /// Worker method to update all Fans.
        /// </summary>
        private new void Update()
        {
            base.Update();

            Update(Fan.Fan1, comboBoxFan1, textBoxFan1, buttonFan1);
            Update(Fan.Fan2, comboBoxFan2, textBoxFan2, buttonFan2);
            Update(Fan.Fan3, comboBoxFan3, textBoxFan3, buttonFan3);
        }

        /// <summary>
        /// Master worker method to synchronize UI with Fan Automation state.
        /// </summary>
        /// <param name="fan">USB Fan to synchronize to.</param>
        /// <param name="comboBox">ComboBox with sensor serial numbers.</param>
        /// <param name="textBox">TextBox with user-entered celsius thresholds.</param>
        /// <param name="button">Enable/Disable button.</param>
        private void Update(Fan fan, ComboBox comboBox, TextBox textBox, Button button)
        {
            bool found = false;
            Single celsiusThreshold = Single.NaN;
            TemperatureSensor temperatureSensor = null;

            Debug.Assert(fan != null);
            Debug.Assert(comboBox != null);
            Debug.Assert(textBox != null);
            Debug.Assert(button != null);

            found = m_fanAutomationRef.TryGetRegistrationInfo(fan,
                                                              out temperatureSensor,
                                                              out celsiusThreshold);

            //Part I: Update the ComboBoxes.
            if (found == true)
            {
                comboBox.SelectedText = temperatureSensor.SerialNumber;
                comboBox.Enabled = false;
            }
            else
            {
                comboBox.SelectedItem = -1;
                comboBox.Enabled = true;
            }

            //Part II: Update the TextBoxes.
            if (found == true)
            {
                textBoxFan1.Text = ConvertUnits.Temperature.CelsiusToFahrenheit(celsiusThreshold).ToString();
                textBox.ReadOnly = true;
                textBox.Enabled = false;
            }
            else
            {
                textBox.ReadOnly = false;
                textBox.Enabled = true;
            }

            //Part III: Update Buttons.
            if (found == true)
                button.Text = DisableText;
            else
                button.Text = EnableText;
        }
        #endregion

        private readonly FanAutomation m_fanAutomationRef;

        private const string DisableText = "Disabble";
        private const string EnableText = "Enable";
    }
}
