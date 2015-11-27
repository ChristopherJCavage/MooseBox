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
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MooseBoxUI
{
    /// <summary>
    /// Defines WinForms implementation for Historical Temperature Data Plots.
    /// </summary>
    public partial class TemperaturePlotForm : Form
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        public TemperaturePlotForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for first loading of the UI form.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void TemperaturePlotForm_Load(object sender, EventArgs e)
        {
            //Some more customizations to the chart to pretty it up...
            Debug.Assert(chartTemperatureHistory.ChartAreas.Count > 0);

            chartTemperatureHistory.ChartAreas[0].AxisX.Title = "Time";
            chartTemperatureHistory.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd HH:mm:ss";
            chartTemperatureHistory.ChartAreas[0].AxisY.Title = "Temperature (°F)";

            chartTemperatureHistory.Series[0].XValueType = ChartValueType.DateTime;

           // chartTemperatureHistory.Series[0].AxisLabel = "dd.MM.yyyy HH:mm:ss";

            chartTemperatureHistory.ChartAreas[0].AxisY.LabelStyle.Format = "0.0";

            //Initialize selectable UI components.
            Update();
        }

        /// <summary>
        /// Event handler for user request to plot data based on sensor # and time constraints.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void buttonChart_Click(object sender, EventArgs e)
        {
            TemperatureSensor temperatureSensor = GetSelectedTemperatureSensor();

            //UI Selection Validations.
            if (dateTimePickerStop.Value < dateTimePickerStart.Value)
            {
                MessageBox.Show("Time selection for stop cannot be less than start time.",
                                "Invalid Time Selections",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                return;
            }

            //Attempt to query data.
            if (temperatureSensor != null)
            {
                //Convert timestamps to UTC for MooseBox query.
                DateTime startTimeUTC = dateTimePickerStart.Value.ToUniversalTime();
                DateTime stopTimeUTC = dateTimePickerStop.Value.ToUniversalTime();

                //Query temperature readings within selected range.
                List<Tuple<DateTime, Single>> readings = AsyncHelper.RunSync(() => { return temperatureSensor.QueryHistoricalReadings(startTimeUTC, stopTimeUTC); });

                //Clear the graph so we don't get stale date on redraws.
                Debug.Assert(chartTemperatureHistory.Series.Count > 0);

                chartTemperatureHistory.Series[0].Points.Clear();

                //Re-populate the chart with this batch of historical data.
                Single minFahrenheit = Single.MaxValue;
                Single maxFahrenheit = Single.MinValue;

                foreach (var reading in readings)
                {
                    Single fahrenheit = ConvertUnits.Temperature.CelsiusToFahrenheit(reading.Item2);
                    DateTime localTimestamp = reading.Item1.ToLocalTime();

                    //Add plot point.
                    chartTemperatureHistory.Series[0].Points.AddXY(localTimestamp, fahrenheit);

                    //Update minimum value.
                    if (fahrenheit < minFahrenheit)
                        minFahrenheit = fahrenheit;

                    //Update maximum value.
                    if (fahrenheit > maxFahrenheit)
                        maxFahrenheit = fahrenheit;
                }

                //Update the bounds/scaling so it looks better.
                chartTemperatureHistory.ChartAreas[0].AxisY.Minimum = minFahrenheit - TempPlotPadding;
                chartTemperatureHistory.ChartAreas[0].AxisY.Maximum = maxFahrenheit + TempPlotPadding;

                //Redraw the graph.
                chartTemperatureHistory.Update();
            }
        }

        /// <summary>
        /// Event handler for indication that the user changed selection within the ComboBox's DropDownList.
        /// </summary>
        /// <param name="sender">Instance of object raising event.</param>
        /// <param name="e">Generic event args.</param>
        private void comboBoxTemperatureSensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            TemperatureSensor temperatureSensor = GetSelectedTemperatureSensor();

            //If an item was selected, query it's min/max range of timestamps.
            if (temperatureSensor != null)
            {
                //Query the Min/Max timestamps we can allow the DateTimePickers to be at; convert timestamps to Local for humans.
                Tuple<DateTime, DateTime> utcTimestamps = AsyncHelper.RunSync(() => { return temperatureSensor.QueryTimestmapRange(); });
                Tuple<DateTime, DateTime> localTimestamps = Tuple.Create(utcTimestamps.Item1.ToLocalTime(), utcTimestamps.Item2.ToLocalTime());

                dateTimePickerStart.MinDate = localTimestamps.Item1;
                dateTimePickerStart.MaxDate = localTimestamps.Item2;
                dateTimePickerStart.Value = localTimestamps.Item1;

                dateTimePickerStop.MinDate = localTimestamps.Item1;
                dateTimePickerStop.MaxDate = localTimestamps.Item2;
                dateTimePickerStop.Value = localTimestamps.Item2;

                //Potentially unlock UI components.
                Update();
            }
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
        /// Updates UI components selectability based on user selection state.
        /// </summary>
        private void Update()
        {
            bool enable = (GetSelectedTemperatureSensor() != null);

            chartTemperatureHistory.Enabled = enable;

            dateTimePickerStart.Enabled = enable;
            dateTimePickerStop.Enabled = enable;

            buttonChart.Enabled = enable;
        }
        #endregion

        private const Single TempPlotPadding = 1.5f;
    }
}
