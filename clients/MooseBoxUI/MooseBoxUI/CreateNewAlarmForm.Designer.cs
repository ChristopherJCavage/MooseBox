namespace MooseBoxUI
{
    partial class CreateNewAlarmForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewAlarmForm));
            this.comboBoxTemperatureSensors = new System.Windows.Forms.ComboBox();
            this.labelTemperatureSensor = new System.Windows.Forms.Label();
            this.pictureBoxAlarm = new System.Windows.Forms.PictureBox();
            this.labelEmailAddress = new System.Windows.Forms.Label();
            this.labelMinTemperature = new System.Windows.Forms.Label();
            this.labelMaxTemperature = new System.Windows.Forms.Label();
            this.textBoxEmailAddress = new System.Windows.Forms.TextBox();
            this.labelFahrenheit2 = new System.Windows.Forms.Label();
            this.labelFahrenheit1 = new System.Windows.Forms.Label();
            this.textBoxMinTemperature = new System.Windows.Forms.TextBox();
            this.textBoxMaxTemperature = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxTemperatureSensors
            // 
            this.comboBoxTemperatureSensors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemperatureSensors.FormattingEnabled = true;
            this.comboBoxTemperatureSensors.Items.AddRange(new object[] {
            "59002A218F928",
            "F7002A215B828"});
            this.comboBoxTemperatureSensors.Location = new System.Drawing.Point(307, 11);
            this.comboBoxTemperatureSensors.Name = "comboBoxTemperatureSensors";
            this.comboBoxTemperatureSensors.Size = new System.Drawing.Size(222, 21);
            this.comboBoxTemperatureSensors.TabIndex = 22;
            // 
            // labelTemperatureSensor
            // 
            this.labelTemperatureSensor.AutoSize = true;
            this.labelTemperatureSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureSensor.Location = new System.Drawing.Point(113, 12);
            this.labelTemperatureSensor.Name = "labelTemperatureSensor";
            this.labelTemperatureSensor.Size = new System.Drawing.Size(188, 20);
            this.labelTemperatureSensor.TabIndex = 21;
            this.labelTemperatureSensor.Text = "Temperature Sensor?:";
            // 
            // pictureBoxAlarm
            // 
            this.pictureBoxAlarm.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.ErrorImage")));
            this.pictureBoxAlarm.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.Image")));
            this.pictureBoxAlarm.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.InitialImage")));
            this.pictureBoxAlarm.Location = new System.Drawing.Point(12, 15);
            this.pictureBoxAlarm.Name = "pictureBoxAlarm";
            this.pictureBoxAlarm.Size = new System.Drawing.Size(95, 95);
            this.pictureBoxAlarm.TabIndex = 23;
            this.pictureBoxAlarm.TabStop = false;
            // 
            // labelEmailAddress
            // 
            this.labelEmailAddress.AutoSize = true;
            this.labelEmailAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEmailAddress.Location = new System.Drawing.Point(162, 38);
            this.labelEmailAddress.Name = "labelEmailAddress";
            this.labelEmailAddress.Size = new System.Drawing.Size(139, 20);
            this.labelEmailAddress.TabIndex = 24;
            this.labelEmailAddress.Text = "Email Address?:";
            // 
            // labelMinTemperature
            // 
            this.labelMinTemperature.AutoSize = true;
            this.labelMinTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinTemperature.Location = new System.Drawing.Point(142, 64);
            this.labelMinTemperature.Name = "labelMinTemperature";
            this.labelMinTemperature.Size = new System.Drawing.Size(159, 20);
            this.labelMinTemperature.TabIndex = 25;
            this.labelMinTemperature.Text = "Min Temperature?:";
            // 
            // labelMaxTemperature
            // 
            this.labelMaxTemperature.AutoSize = true;
            this.labelMaxTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaxTemperature.Location = new System.Drawing.Point(138, 90);
            this.labelMaxTemperature.Name = "labelMaxTemperature";
            this.labelMaxTemperature.Size = new System.Drawing.Size(163, 20);
            this.labelMaxTemperature.TabIndex = 26;
            this.labelMaxTemperature.Text = "Max Temperature?:";
            // 
            // textBoxEmailAddress
            // 
            this.textBoxEmailAddress.Location = new System.Drawing.Point(307, 38);
            this.textBoxEmailAddress.Name = "textBoxEmailAddress";
            this.textBoxEmailAddress.Size = new System.Drawing.Size(222, 20);
            this.textBoxEmailAddress.TabIndex = 27;
            // 
            // labelFahrenheit2
            // 
            this.labelFahrenheit2.AutoSize = true;
            this.labelFahrenheit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit2.Location = new System.Drawing.Point(413, 90);
            this.labelFahrenheit2.Name = "labelFahrenheit2";
            this.labelFahrenheit2.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit2.TabIndex = 29;
            this.labelFahrenheit2.Text = "°F";
            // 
            // labelFahrenheit1
            // 
            this.labelFahrenheit1.AutoSize = true;
            this.labelFahrenheit1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit1.Location = new System.Drawing.Point(413, 64);
            this.labelFahrenheit1.Name = "labelFahrenheit1";
            this.labelFahrenheit1.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit1.TabIndex = 28;
            this.labelFahrenheit1.Text = "°F";
            // 
            // textBoxMinTemperature
            // 
            this.textBoxMinTemperature.Location = new System.Drawing.Point(307, 64);
            this.textBoxMinTemperature.Name = "textBoxMinTemperature";
            this.textBoxMinTemperature.Size = new System.Drawing.Size(100, 20);
            this.textBoxMinTemperature.TabIndex = 30;
            // 
            // textBoxMaxTemperature
            // 
            this.textBoxMaxTemperature.Location = new System.Drawing.Point(307, 90);
            this.textBoxMaxTemperature.Name = "textBoxMaxTemperature";
            this.textBoxMaxTemperature.Size = new System.Drawing.Size(100, 20);
            this.textBoxMaxTemperature.TabIndex = 31;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(373, 116);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 32;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(454, 116);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 33;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // CreateNewAlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 148);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxMaxTemperature);
            this.Controls.Add(this.textBoxMinTemperature);
            this.Controls.Add(this.labelFahrenheit2);
            this.Controls.Add(this.labelFahrenheit1);
            this.Controls.Add(this.textBoxEmailAddress);
            this.Controls.Add(this.labelMaxTemperature);
            this.Controls.Add(this.labelMinTemperature);
            this.Controls.Add(this.labelEmailAddress);
            this.Controls.Add(this.pictureBoxAlarm);
            this.Controls.Add(this.comboBoxTemperatureSensors);
            this.Controls.Add(this.labelTemperatureSensor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateNewAlarmForm";
            this.Text = "Register New Temperature Alarm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlarm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxTemperatureSensors;
        private System.Windows.Forms.Label labelTemperatureSensor;
        private System.Windows.Forms.PictureBox pictureBoxAlarm;
        private System.Windows.Forms.Label labelEmailAddress;
        private System.Windows.Forms.Label labelMinTemperature;
        private System.Windows.Forms.Label labelMaxTemperature;
        private System.Windows.Forms.TextBox textBoxEmailAddress;
        private System.Windows.Forms.Label labelFahrenheit2;
        private System.Windows.Forms.Label labelFahrenheit1;
        private System.Windows.Forms.TextBox textBoxMinTemperature;
        private System.Windows.Forms.TextBox textBoxMaxTemperature;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}