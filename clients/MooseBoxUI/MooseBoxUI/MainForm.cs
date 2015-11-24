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
using MooseBoxUI.Client.REST;
using MooseBoxUI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MooseBoxUI
{
    /// <summary>
    /// WinForms Main control panel form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            m_statusPollTimer = new System.Windows.Forms.Timer();

            m_statusPollTimer.Tick += m_statusPollTimer_Tick;
            m_statusPollTimer.Interval = PollIntervalMs;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// UI Handler for load event.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Configure the actual MooseBox endpoint so we can connect via REST APIs.
            MooseBoxRESTAPIFactory.Instance.Register("http://10.0.1.4:8080");

            //Start the Polling Timer.
            m_statusPollTimer.Start();

            //Build the Fan Automation write-through cache.
            m_fanAutomation = AsyncHelper.RunSync(() => { return FanAutomation.QueryCurrentFanAutomation(); });
        }

        /// <summary>
        /// UI Handler for closing event.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Stop the Polling Timer.
            m_statusPollTimer.Stop();
        }

        /// <summary>
        /// UI Handler for Fans -> Manual Overrides.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void manualOverrideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FanManualOverrideForm fanManualOverrideForm = new FanManualOverrideForm(m_fanAutomation))
                fanManualOverrideForm.ShowDialog();
        }

        /// <summary>
        /// UI Handler for Fans -> Automation Config.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void automationConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FanAutomationForm fanAutomationForm = new FanAutomationForm(m_fanAutomation))
                fanAutomationForm.ShowDialog();
        }

        /// <summary>
        /// UI Handler for Temperature -> Alarms.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void configureAlarmsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ConfigureTemperatureAlarmForm configureTemperatureAlarmForm = new ConfigureTemperatureAlarmForm())
                configureTemperatureAlarmForm.ShowDialog();
        }

        /// <summary>
        /// UI Handler for Help -> About.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
                aboutForm.ShowDialog();
        }

        /// <summary>
        /// UI Handler for Windows.Forms timer elapsed.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void m_statusPollTimer_Tick(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(obj =>
                {
                    try
                    {
                        //Kick off all of the Fan Queries.
                        Tuple<bool, DateTime> fan1Reading = AsyncHelper.RunSync(() => { return Fan.Fan1.QueryCurrentReading(); });
                        Tuple<bool, DateTime> fan2Reading = AsyncHelper.RunSync(() => { return Fan.Fan2.QueryCurrentReading(); });
                        Tuple<bool, DateTime> fan3Reading = AsyncHelper.RunSync(() => { return Fan.Fan3.QueryCurrentReading(); });

                        //Kick off all of the Temperature Queries.
                        Tuple<DateTime, Single> _F7000002A215B828Reading = AsyncHelper.RunSync(() => { return TemperatureSensor._F7000002A215B828.QueryCurrentReading(); });
                        /*Tuple<DateTime, Single> _59000002A218F928Reading = AsyncHelper.RunSync(() => { return TemperatureSensor._59000002A218F928.QueryCurrentReading(); });
                        */
                        //Kick off all of the SysInfo Queries.
                        Tuple<UInt64, UInt64> memoryInfo = AsyncHelper.RunSync(() => { return SysInfo.MooseBox.QuerySystemInformation(); });
                        
                        //Update everything on the UI Thread; everything was successful.
                        Invoke(new Action(() =>
                            {
                                UpdateFanLED(Fan.Fan1, fan1Reading.Item1);
                                UpdateFanLED(Fan.Fan2, fan2Reading.Item1);
                                UpdateFanLED(Fan.Fan3, fan3Reading.Item1);

                        //       UpdateTemperatureIndicator(TemperatureSensor._59000002A218F928, _59000002A218F928Reading.Item2);
                                UpdateTemperatureIndicator(TemperatureSensor._F7000002A215B828, _F7000002A215B828Reading.Item2);
                                
                                UpdateMemoryIndicator(textBoxTotalMemory, memoryInfo.Item2);
                                UpdateMemoryIndicator(textBoxAvailableMemory, memoryInfo.Item1);
                                UpdateMemoryIndicator(SysInfo.CalculateMemoryPercentUsed(memoryInfo.Item1, memoryInfo.Item2));

                                UpdateStatusLED(LEDColors.Green);
                            }));
                    }
                    catch
                    {
                        //Indicate Failure.
                        UpdateStatusLED(LEDColors.Red);
                    }
                });
        }
        #endregion

        #region Worker Methods
        /// <summary>
        /// Changes the color of a numbered fan's power status led on main panel.
        /// </summary>
        /// <param name="fan">Fan to change LED of.</param>
        /// <param name="isPowered">true if a USB Fan is powered; false otherwise.</param>
        private void UpdateFanLED(Fan fan, bool isPowered)
        {
            if (isPowered == true)
                UpdateFanLED(fan, LEDColors.Green);
            else
                UpdateFanLED(fan, LEDColors.Red);
        }

        /// <summary>
        /// Changes the color of a numbered fan's power status led on main panel.
        /// </summary>
        /// <param name="fan">Fan to change LED of.</param>
        /// <param name="ledColor">Color to change LED to.</param>
        private void UpdateFanLED(Fan fan, LEDColors ledColor)
        {
            PictureBox ledPictureBox = null;

            Debug.Assert(fan != null);

            //Lookup the LED WinForm picture box to redraw.
            switch(fan.Number)
            {
                case 1: ledPictureBox = pictureBoxFan1LED; break;
                case 2: ledPictureBox = pictureBoxFan2LED; break;
                case 3: ledPictureBox = pictureBoxFan3LED; break;

                default:
                    throw new ArgumentOutOfRangeException("Fan fan",
                                                          string.Format("Invalid Fan Number. Found: {0}",
                                                                        fan.Number));
            }

            //Redraw the the LED image for the fan.
            Debug.Assert(ledPictureBox != null);

            LEDHelper.ChangeColor(ledPictureBox, ledColor);
        }

        /// <summary>
        /// Updates the memory indicator describing percentage of memory used.
        /// </summary>
        /// <param name="percentUsed">Percentage of memory used, 0 <= X <= 100.</param>
        private void UpdateMemoryIndicator(Byte percentUsed)
        {
            Debug.Assert(percentUsed <= 100);

            textBoxPercentUsed.Text = percentUsed.ToString();
        }

        /// <summary>
        /// Updates on of the memory indicators describing a value in megabytes.
        /// </summary>
        /// <param name="textBox">Memory indicator to update.</param>
        /// <param name="memoryBytes">Memory value in bytes (to be converted).</param>
        private void UpdateMemoryIndicator(TextBox textBox, UInt64 memoryBytes)
        {
            UInt64 memoryMB = ConvertUnits.Memory.BytesToMegabytes(memoryBytes);

            Debug.Assert(textBox != null);

            textBox.Text = memoryMB.ToString();
        }

        /// <summary>
        /// Changes the color of the primary status LED in the lower-right corner.
        /// </summary>
        /// <param name="ledColor">Color to change LED to.</param>
        private void UpdateStatusLED(LEDColors ledColor)
        {
            if (this.InvokeRequired == true)
                Invoke(new Action(() => { LEDHelper.ChangeColor(pictureBoxStatusLED, ledColor); }));
            else
                LEDHelper.ChangeColor(pictureBoxStatusLED, ledColor);
        }

        /// <summary>
        /// Sets the text of the temperature indicator, in Fahrenheit.
        /// </summary>
        /// <param name="temperatureSensor">Temperature sensor associated with the indicator.</param>
        /// <param name="celsius">Temperature reading to display.</param>
        private void UpdateTemperatureIndicator(TemperatureSensor temperatureSensor, Single celsius)
        {
            Debug.Assert(temperatureSensor != null);

            if (temperatureSensor == TemperatureSensor._59000002A218F928)
                textBox59000002A218F928.Text = Math.Round(ConvertUnits.Temperature.CelsiusToFahrenheit(celsius), 2).ToString();
            else if (temperatureSensor == TemperatureSensor._F7000002A215B828)
                textBoxF7000002A215B828.Text = Math.Round(ConvertUnits.Temperature.CelsiusToFahrenheit(celsius), 2).ToString();
            else
                throw new ArgumentOutOfRangeException("TemperatureSensor temperatureSensor",
                                                      string.Format("Invalid Temperature Sensor. Serial #: {0}",
                                                                    temperatureSensor.SerialNumber));
        }
        #endregion

        private FanAutomation m_fanAutomation;

        private readonly System.Windows.Forms.Timer m_statusPollTimer;

        private const int PollIntervalMs = 1000;
    }
}
