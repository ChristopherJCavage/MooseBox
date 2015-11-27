namespace MooseBoxUI
{
    partial class TemperaturePlotForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemperaturePlotForm));
            this.chartTemperatureHistory = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStop = new System.Windows.Forms.DateTimePicker();
            this.buttonChart = new System.Windows.Forms.Button();
            this.labelSelectStartTime = new System.Windows.Forms.Label();
            this.labelSelectStopTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTemperatureSensors = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartTemperatureHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTemperatureHistory
            // 
            chartArea1.Name = "ChartAreaMain";
            this.chartTemperatureHistory.ChartAreas.Add(chartArea1);
            this.chartTemperatureHistory.Location = new System.Drawing.Point(12, 12);
            this.chartTemperatureHistory.Name = "chartTemperatureHistory";
            series1.ChartArea = "ChartAreaMain";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsVisibleInLegend = false;
            series1.Name = "SeriesTempTime";
            this.chartTemperatureHistory.Series.Add(series1);
            this.chartTemperatureHistory.Size = new System.Drawing.Size(972, 461);
            this.chartTemperatureHistory.TabIndex = 0;
            this.chartTemperatureHistory.Text = "Temperature vs. Time";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "TitleMain";
            title1.Text = "Temperature vs. Time";
            this.chartTemperatureHistory.Titles.Add(title1);
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "ddddd, MMMM dd, yyyy hh:mm:ss tt";
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(286, 510);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(274, 20);
            this.dateTimePickerStart.TabIndex = 2;
            // 
            // dateTimePickerStop
            // 
            this.dateTimePickerStop.CustomFormat = "ddddd, MMMM dd, yyyy hh:mm:ss tt";
            this.dateTimePickerStop.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStop.Location = new System.Drawing.Point(575, 511);
            this.dateTimePickerStop.Name = "dateTimePickerStop";
            this.dateTimePickerStop.Size = new System.Drawing.Size(274, 20);
            this.dateTimePickerStop.TabIndex = 3;
            // 
            // buttonChart
            // 
            this.buttonChart.Location = new System.Drawing.Point(864, 488);
            this.buttonChart.Name = "buttonChart";
            this.buttonChart.Size = new System.Drawing.Size(120, 43);
            this.buttonChart.TabIndex = 4;
            this.buttonChart.Text = "Plot Data";
            this.buttonChart.UseVisualStyleBackColor = true;
            this.buttonChart.Click += new System.EventHandler(this.buttonChart_Click);
            // 
            // labelSelectStartTime
            // 
            this.labelSelectStartTime.AutoSize = true;
            this.labelSelectStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectStartTime.Location = new System.Drawing.Point(282, 487);
            this.labelSelectStartTime.Name = "labelSelectStartTime";
            this.labelSelectStartTime.Size = new System.Drawing.Size(153, 20);
            this.labelSelectStartTime.TabIndex = 5;
            this.labelSelectStartTime.Text = "Select Start Time:";
            // 
            // labelSelectStopTime
            // 
            this.labelSelectStopTime.AutoSize = true;
            this.labelSelectStopTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectStopTime.Location = new System.Drawing.Point(571, 488);
            this.labelSelectStopTime.Name = "labelSelectStopTime";
            this.labelSelectStopTime.Size = new System.Drawing.Size(151, 20);
            this.labelSelectStopTime.TabIndex = 6;
            this.labelSelectStopTime.Text = "Select Stop Time:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 487);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Select Sensor:";
            // 
            // comboBoxTemperatureSensors
            // 
            this.comboBoxTemperatureSensors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemperatureSensors.FormattingEnabled = true;
            this.comboBoxTemperatureSensors.Items.AddRange(new object[] {
            "59002A218F928",
            "F7002A215B828"});
            this.comboBoxTemperatureSensors.Location = new System.Drawing.Point(12, 510);
            this.comboBoxTemperatureSensors.Name = "comboBoxTemperatureSensors";
            this.comboBoxTemperatureSensors.Size = new System.Drawing.Size(251, 21);
            this.comboBoxTemperatureSensors.TabIndex = 21;
            this.comboBoxTemperatureSensors.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemperatureSensors_SelectedIndexChanged);
            // 
            // TemperaturePlotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 543);
            this.Controls.Add(this.comboBoxTemperatureSensors);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelSelectStopTime);
            this.Controls.Add(this.labelSelectStartTime);
            this.Controls.Add(this.buttonChart);
            this.Controls.Add(this.dateTimePickerStop);
            this.Controls.Add(this.dateTimePickerStart);
            this.Controls.Add(this.chartTemperatureHistory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemperaturePlotForm";
            this.Text = "Plot Historical Temperature Readings";
            this.Load += new System.EventHandler(this.TemperaturePlotForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartTemperatureHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartTemperatureHistory;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerStop;
        private System.Windows.Forms.Button buttonChart;
        private System.Windows.Forms.Label labelSelectStartTime;
        private System.Windows.Forms.Label labelSelectStopTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTemperatureSensors;
    }
}