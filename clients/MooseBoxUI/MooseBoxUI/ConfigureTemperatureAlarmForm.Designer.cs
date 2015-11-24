namespace MooseBoxUI
{
    partial class ConfigureTemperatureAlarmForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureTemperatureAlarmForm));
            this.labelTemperatureAlarm = new System.Windows.Forms.Label();
            this.labelEmailAddress = new System.Windows.Forms.Label();
            this.labelTemperatureSensor = new System.Windows.Forms.Label();
            this.comboBoxTemperatureSensors = new System.Windows.Forms.ComboBox();
            this.comboBoxRegisteredEmails = new System.Windows.Forms.ComboBox();
            this.textBoxTemperatureMin = new System.Windows.Forms.TextBox();
            this.textBoxTemperatureMax = new System.Windows.Forms.TextBox();
            this.labelTemperatureMin = new System.Windows.Forms.Label();
            this.labelTemperatureMax = new System.Windows.Forms.Label();
            this.labelFahrenheit2 = new System.Windows.Forms.Label();
            this.labelFahrenheit1 = new System.Windows.Forms.Label();
            this.buttonUnregister = new System.Windows.Forms.Button();
            this.buttonCreateNewAlarm = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.pictureBoxAlarm = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTemperatureAlarm
            // 
            this.labelTemperatureAlarm.AutoSize = true;
            this.labelTemperatureAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureAlarm.Location = new System.Drawing.Point(236, 9);
            this.labelTemperatureAlarm.Name = "labelTemperatureAlarm";
            this.labelTemperatureAlarm.Size = new System.Drawing.Size(205, 25);
            this.labelTemperatureAlarm.TabIndex = 19;
            this.labelTemperatureAlarm.Text = "Registered Alarms";
            // 
            // labelEmailAddress
            // 
            this.labelEmailAddress.AutoSize = true;
            this.labelEmailAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEmailAddress.Location = new System.Drawing.Point(140, 79);
            this.labelEmailAddress.Name = "labelEmailAddress";
            this.labelEmailAddress.Size = new System.Drawing.Size(161, 20);
            this.labelEmailAddress.TabIndex = 17;
            this.labelEmailAddress.Text = "Registered Email?:";
            // 
            // labelTemperatureSensor
            // 
            this.labelTemperatureSensor.AutoSize = true;
            this.labelTemperatureSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureSensor.Location = new System.Drawing.Point(113, 38);
            this.labelTemperatureSensor.Name = "labelTemperatureSensor";
            this.labelTemperatureSensor.Size = new System.Drawing.Size(188, 20);
            this.labelTemperatureSensor.TabIndex = 16;
            this.labelTemperatureSensor.Text = "Temperature Sensor?:";
            // 
            // comboBoxTemperatureSensors
            // 
            this.comboBoxTemperatureSensors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemperatureSensors.FormattingEnabled = true;
            this.comboBoxTemperatureSensors.Items.AddRange(new object[] {
            "59002A218F928",
            "F7002A215B828"});
            this.comboBoxTemperatureSensors.Location = new System.Drawing.Point(307, 37);
            this.comboBoxTemperatureSensors.Name = "comboBoxTemperatureSensors";
            this.comboBoxTemperatureSensors.Size = new System.Drawing.Size(222, 21);
            this.comboBoxTemperatureSensors.TabIndex = 20;
            this.comboBoxTemperatureSensors.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemperatureSensors_SelectedIndexChanged);
            // 
            // comboBoxRegisteredEmails
            // 
            this.comboBoxRegisteredEmails.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegisteredEmails.FormattingEnabled = true;
            this.comboBoxRegisteredEmails.Items.AddRange(new object[] {
            "59000002A218F928",
            "F7000002A215B828"});
            this.comboBoxRegisteredEmails.Location = new System.Drawing.Point(307, 78);
            this.comboBoxRegisteredEmails.Name = "comboBoxRegisteredEmails";
            this.comboBoxRegisteredEmails.Size = new System.Drawing.Size(222, 21);
            this.comboBoxRegisteredEmails.TabIndex = 21;
            this.comboBoxRegisteredEmails.SelectedIndexChanged += new System.EventHandler(this.comboBoxRegisteredEmails_SelectedIndexChanged);
            // 
            // textBoxTemperatureMin
            // 
            this.textBoxTemperatureMin.Location = new System.Drawing.Point(201, 121);
            this.textBoxTemperatureMin.Name = "textBoxTemperatureMin";
            this.textBoxTemperatureMin.ReadOnly = true;
            this.textBoxTemperatureMin.Size = new System.Drawing.Size(100, 20);
            this.textBoxTemperatureMin.TabIndex = 22;
            // 
            // textBoxTemperatureMax
            // 
            this.textBoxTemperatureMax.Location = new System.Drawing.Point(405, 121);
            this.textBoxTemperatureMax.Name = "textBoxTemperatureMax";
            this.textBoxTemperatureMax.ReadOnly = true;
            this.textBoxTemperatureMax.Size = new System.Drawing.Size(100, 20);
            this.textBoxTemperatureMax.TabIndex = 23;
            // 
            // labelTemperatureMin
            // 
            this.labelTemperatureMin.AutoSize = true;
            this.labelTemperatureMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureMin.Location = new System.Drawing.Point(148, 121);
            this.labelTemperatureMin.Name = "labelTemperatureMin";
            this.labelTemperatureMin.Size = new System.Drawing.Size(47, 20);
            this.labelTemperatureMin.TabIndex = 24;
            this.labelTemperatureMin.Text = "Min: ";
            // 
            // labelTemperatureMax
            // 
            this.labelTemperatureMax.AutoSize = true;
            this.labelTemperatureMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureMax.Location = new System.Drawing.Point(348, 121);
            this.labelTemperatureMax.Name = "labelTemperatureMax";
            this.labelTemperatureMax.Size = new System.Drawing.Size(51, 20);
            this.labelTemperatureMax.TabIndex = 25;
            this.labelTemperatureMax.Text = "Max: ";
            // 
            // labelFahrenheit2
            // 
            this.labelFahrenheit2.AutoSize = true;
            this.labelFahrenheit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit2.Location = new System.Drawing.Point(511, 121);
            this.labelFahrenheit2.Name = "labelFahrenheit2";
            this.labelFahrenheit2.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit2.TabIndex = 27;
            this.labelFahrenheit2.Text = "°F";
            // 
            // labelFahrenheit1
            // 
            this.labelFahrenheit1.AutoSize = true;
            this.labelFahrenheit1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit1.Location = new System.Drawing.Point(307, 121);
            this.labelFahrenheit1.Name = "labelFahrenheit1";
            this.labelFahrenheit1.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit1.TabIndex = 26;
            this.labelFahrenheit1.Text = "°F";
            // 
            // buttonUnregister
            // 
            this.buttonUnregister.Location = new System.Drawing.Point(553, 78);
            this.buttonUnregister.Name = "buttonUnregister";
            this.buttonUnregister.Size = new System.Drawing.Size(123, 23);
            this.buttonUnregister.TabIndex = 28;
            this.buttonUnregister.Text = "Unregister Alarm";
            this.buttonUnregister.UseVisualStyleBackColor = true;
            this.buttonUnregister.Click += new System.EventHandler(this.buttonUnregister_Click);
            // 
            // buttonCreateNewAlarm
            // 
            this.buttonCreateNewAlarm.Location = new System.Drawing.Point(553, 35);
            this.buttonCreateNewAlarm.Name = "buttonCreateNewAlarm";
            this.buttonCreateNewAlarm.Size = new System.Drawing.Size(123, 23);
            this.buttonCreateNewAlarm.TabIndex = 29;
            this.buttonCreateNewAlarm.Text = "Register New Alarm";
            this.buttonCreateNewAlarm.UseVisualStyleBackColor = true;
            this.buttonCreateNewAlarm.Click += new System.EventHandler(this.buttonCreateNewAlarm_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(553, 121);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(123, 23);
            this.buttonExit.TabIndex = 30;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // pictureBoxAlarm
            // 
            this.pictureBoxAlarm.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.ErrorImage")));
            this.pictureBoxAlarm.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.Image")));
            this.pictureBoxAlarm.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxAlarm.InitialImage")));
            this.pictureBoxAlarm.Location = new System.Drawing.Point(12, 21);
            this.pictureBoxAlarm.Name = "pictureBoxAlarm";
            this.pictureBoxAlarm.Size = new System.Drawing.Size(95, 95);
            this.pictureBoxAlarm.TabIndex = 18;
            this.pictureBoxAlarm.TabStop = false;
            // 
            // ConfigureTemperatureAlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 159);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonCreateNewAlarm);
            this.Controls.Add(this.buttonUnregister);
            this.Controls.Add(this.labelFahrenheit2);
            this.Controls.Add(this.labelFahrenheit1);
            this.Controls.Add(this.labelTemperatureMax);
            this.Controls.Add(this.labelTemperatureMin);
            this.Controls.Add(this.textBoxTemperatureMax);
            this.Controls.Add(this.textBoxTemperatureMin);
            this.Controls.Add(this.comboBoxRegisteredEmails);
            this.Controls.Add(this.comboBoxTemperatureSensors);
            this.Controls.Add(this.labelTemperatureAlarm);
            this.Controls.Add(this.pictureBoxAlarm);
            this.Controls.Add(this.labelEmailAddress);
            this.Controls.Add(this.labelTemperatureSensor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureTemperatureAlarmForm";
            this.Text = "Configure Temperature Alarms";
            this.Load += new System.EventHandler(this.ConfigureTemperatureAlarmForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlarm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTemperatureAlarm;
        private System.Windows.Forms.PictureBox pictureBoxAlarm;
        private System.Windows.Forms.Label labelEmailAddress;
        private System.Windows.Forms.Label labelTemperatureSensor;
        private System.Windows.Forms.ComboBox comboBoxTemperatureSensors;
        private System.Windows.Forms.ComboBox comboBoxRegisteredEmails;
        private System.Windows.Forms.TextBox textBoxTemperatureMin;
        private System.Windows.Forms.TextBox textBoxTemperatureMax;
        private System.Windows.Forms.Label labelTemperatureMin;
        private System.Windows.Forms.Label labelTemperatureMax;
        private System.Windows.Forms.Label labelFahrenheit2;
        private System.Windows.Forms.Label labelFahrenheit1;
        private System.Windows.Forms.Button buttonUnregister;
        private System.Windows.Forms.Button buttonCreateNewAlarm;
        private System.Windows.Forms.Button buttonExit;
    }
}