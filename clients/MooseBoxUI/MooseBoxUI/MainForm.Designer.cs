namespace MooseBoxUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBoxFan = new System.Windows.Forms.PictureBox();
            this.labelFan1Status = new System.Windows.Forms.Label();
            this.labelFan3Status = new System.Windows.Forms.Label();
            this.labelFan2Status = new System.Windows.Forms.Label();
            this.pictureBoxFan1LED = new System.Windows.Forms.PictureBox();
            this.pictureBoxFan2LED = new System.Windows.Forms.PictureBox();
            this.pictureBoxFan3LED = new System.Windows.Forms.PictureBox();
            this.labelFanStatus = new System.Windows.Forms.Label();
            this.labelF7000002A215B828 = new System.Windows.Forms.Label();
            this.label59000002A218F928 = new System.Windows.Forms.Label();
            this.pictureBoxThermometer = new System.Windows.Forms.PictureBox();
            this.textBoxF7000002A215B828 = new System.Windows.Forms.TextBox();
            this.textBox59000002A218F928 = new System.Windows.Forms.TextBox();
            this.labelFahrenheit1 = new System.Windows.Forms.Label();
            this.labelFahrenheit2 = new System.Windows.Forms.Label();
            this.labelTemperatureStatus = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.labelAvailableMemory = new System.Windows.Forms.Label();
            this.labelMemoryPercentUsed = new System.Windows.Forms.Label();
            this.labelTotalMemory = new System.Windows.Forms.Label();
            this.textBoxAvailableMemory = new System.Windows.Forms.TextBox();
            this.textBoxPercentUsed = new System.Windows.Forms.TextBox();
            this.textBoxTotalMemory = new System.Windows.Forms.TextBox();
            this.labelMB1 = new System.Windows.Forms.Label();
            this.labelMB2 = new System.Windows.Forms.Label();
            this.labelMB3 = new System.Windows.Forms.Label();
            this.labelSystemStatus = new System.Windows.Forms.Label();
            this.pictureBoxHardDrive = new System.Windows.Forms.PictureBox();
            this.pictureBoxStatusLED = new System.Windows.Forms.PictureBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualOverrideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automationConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temperatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureAlarmsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan1LED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan2LED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan3LED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThermometer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHardDrive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatusLED)).BeginInit();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxFan
            // 
            this.pictureBoxFan.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan.Image")));
            this.pictureBoxFan.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan.InitialImage")));
            this.pictureBoxFan.Location = new System.Drawing.Point(12, 53);
            this.pictureBoxFan.Name = "pictureBoxFan";
            this.pictureBoxFan.Size = new System.Drawing.Size(95, 95);
            this.pictureBoxFan.TabIndex = 0;
            this.pictureBoxFan.TabStop = false;
            // 
            // labelFan1Status
            // 
            this.labelFan1Status.AutoSize = true;
            this.labelFan1Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFan1Status.Location = new System.Drawing.Point(113, 55);
            this.labelFan1Status.Name = "labelFan1Status";
            this.labelFan1Status.Size = new System.Drawing.Size(75, 20);
            this.labelFan1Status.TabIndex = 1;
            this.labelFan1Status.Text = "Fan #1: ";
            // 
            // labelFan3Status
            // 
            this.labelFan3Status.AutoSize = true;
            this.labelFan3Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFan3Status.Location = new System.Drawing.Point(113, 128);
            this.labelFan3Status.Name = "labelFan3Status";
            this.labelFan3Status.Size = new System.Drawing.Size(70, 20);
            this.labelFan3Status.TabIndex = 2;
            this.labelFan3Status.Text = "Fan #3:";
            // 
            // labelFan2Status
            // 
            this.labelFan2Status.AutoSize = true;
            this.labelFan2Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFan2Status.Location = new System.Drawing.Point(113, 92);
            this.labelFan2Status.Name = "labelFan2Status";
            this.labelFan2Status.Size = new System.Drawing.Size(75, 20);
            this.labelFan2Status.TabIndex = 3;
            this.labelFan2Status.Text = "Fan #2: ";
            // 
            // pictureBoxFan1LED
            // 
            this.pictureBoxFan1LED.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan1LED.Image")));
            this.pictureBoxFan1LED.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan1LED.InitialImage")));
            this.pictureBoxFan1LED.Location = new System.Drawing.Point(185, 53);
            this.pictureBoxFan1LED.Name = "pictureBoxFan1LED";
            this.pictureBoxFan1LED.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFan1LED.TabIndex = 4;
            this.pictureBoxFan1LED.TabStop = false;
            // 
            // pictureBoxFan2LED
            // 
            this.pictureBoxFan2LED.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan2LED.Image")));
            this.pictureBoxFan2LED.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan2LED.InitialImage")));
            this.pictureBoxFan2LED.Location = new System.Drawing.Point(185, 90);
            this.pictureBoxFan2LED.Name = "pictureBoxFan2LED";
            this.pictureBoxFan2LED.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFan2LED.TabIndex = 5;
            this.pictureBoxFan2LED.TabStop = false;
            // 
            // pictureBoxFan3LED
            // 
            this.pictureBoxFan3LED.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan3LED.Image")));
            this.pictureBoxFan3LED.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFan3LED.InitialImage")));
            this.pictureBoxFan3LED.Location = new System.Drawing.Point(185, 126);
            this.pictureBoxFan3LED.Name = "pictureBoxFan3LED";
            this.pictureBoxFan3LED.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFan3LED.TabIndex = 6;
            this.pictureBoxFan3LED.TabStop = false;
            // 
            // labelFanStatus
            // 
            this.labelFanStatus.AutoSize = true;
            this.labelFanStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFanStatus.Location = new System.Drawing.Point(47, 25);
            this.labelFanStatus.Name = "labelFanStatus";
            this.labelFanStatus.Size = new System.Drawing.Size(126, 25);
            this.labelFanStatus.TabIndex = 7;
            this.labelFanStatus.Text = "Fan Status";
            // 
            // labelF7000002A215B828
            // 
            this.labelF7000002A215B828.AutoSize = true;
            this.labelF7000002A215B828.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelF7000002A215B828.Location = new System.Drawing.Point(301, 56);
            this.labelF7000002A215B828.Name = "labelF7000002A215B828";
            this.labelF7000002A215B828.Size = new System.Drawing.Size(179, 20);
            this.labelF7000002A215B828.TabIndex = 8;
            this.labelF7000002A215B828.Text = "F7000002A215B828:";
            // 
            // label59000002A218F928
            // 
            this.label59000002A218F928.AutoSize = true;
            this.label59000002A218F928.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59000002A218F928.Location = new System.Drawing.Point(301, 93);
            this.label59000002A218F928.Name = "label59000002A218F928";
            this.label59000002A218F928.Size = new System.Drawing.Size(177, 20);
            this.label59000002A218F928.TabIndex = 9;
            this.label59000002A218F928.Text = "59000002A218F928:";
            // 
            // pictureBoxThermometer
            // 
            this.pictureBoxThermometer.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxThermometer.ErrorImage")));
            this.pictureBoxThermometer.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxThermometer.Image")));
            this.pictureBoxThermometer.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxThermometer.InitialImage")));
            this.pictureBoxThermometer.Location = new System.Drawing.Point(265, 54);
            this.pictureBoxThermometer.Name = "pictureBoxThermometer";
            this.pictureBoxThermometer.Size = new System.Drawing.Size(39, 95);
            this.pictureBoxThermometer.TabIndex = 10;
            this.pictureBoxThermometer.TabStop = false;
            // 
            // textBoxF7000002A215B828
            // 
            this.textBoxF7000002A215B828.Location = new System.Drawing.Point(486, 57);
            this.textBoxF7000002A215B828.Name = "textBoxF7000002A215B828";
            this.textBoxF7000002A215B828.ReadOnly = true;
            this.textBoxF7000002A215B828.Size = new System.Drawing.Size(79, 20);
            this.textBoxF7000002A215B828.TabIndex = 11;
            this.textBoxF7000002A215B828.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox59000002A218F928
            // 
            this.textBox59000002A218F928.Location = new System.Drawing.Point(486, 94);
            this.textBox59000002A218F928.Name = "textBox59000002A218F928";
            this.textBox59000002A218F928.ReadOnly = true;
            this.textBox59000002A218F928.Size = new System.Drawing.Size(79, 20);
            this.textBox59000002A218F928.TabIndex = 12;
            this.textBox59000002A218F928.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelFahrenheit1
            // 
            this.labelFahrenheit1.AutoSize = true;
            this.labelFahrenheit1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit1.Location = new System.Drawing.Point(571, 56);
            this.labelFahrenheit1.Name = "labelFahrenheit1";
            this.labelFahrenheit1.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit1.TabIndex = 13;
            this.labelFahrenheit1.Text = "°F";
            // 
            // labelFahrenheit2
            // 
            this.labelFahrenheit2.AutoSize = true;
            this.labelFahrenheit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFahrenheit2.Location = new System.Drawing.Point(571, 94);
            this.labelFahrenheit2.Name = "labelFahrenheit2";
            this.labelFahrenheit2.Size = new System.Drawing.Size(26, 20);
            this.labelFahrenheit2.TabIndex = 14;
            this.labelFahrenheit2.Text = "°F";
            // 
            // labelTemperatureStatus
            // 
            this.labelTemperatureStatus.AutoSize = true;
            this.labelTemperatureStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureStatus.Location = new System.Drawing.Point(317, 26);
            this.labelTemperatureStatus.Name = "labelTemperatureStatus";
            this.labelTemperatureStatus.Size = new System.Drawing.Size(219, 25);
            this.labelTemperatureStatus.TabIndex = 15;
            this.labelTemperatureStatus.Text = "Temperature Status";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 157);
            this.splitter1.TabIndex = 16;
            this.splitter1.TabStop = false;
            // 
            // labelAvailableMemory
            // 
            this.labelAvailableMemory.AutoSize = true;
            this.labelAvailableMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAvailableMemory.Location = new System.Drawing.Point(738, 92);
            this.labelAvailableMemory.Name = "labelAvailableMemory";
            this.labelAvailableMemory.Size = new System.Drawing.Size(158, 20);
            this.labelAvailableMemory.TabIndex = 19;
            this.labelAvailableMemory.Text = "Available Memory: ";
            // 
            // labelMemoryPercentUsed
            // 
            this.labelMemoryPercentUsed.AutoSize = true;
            this.labelMemoryPercentUsed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMemoryPercentUsed.Location = new System.Drawing.Point(738, 128);
            this.labelMemoryPercentUsed.Name = "labelMemoryPercentUsed";
            this.labelMemoryPercentUsed.Size = new System.Drawing.Size(123, 20);
            this.labelMemoryPercentUsed.TabIndex = 18;
            this.labelMemoryPercentUsed.Text = "Percent Used:";
            // 
            // labelTotalMemory
            // 
            this.labelTotalMemory.AutoSize = true;
            this.labelTotalMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotalMemory.Location = new System.Drawing.Point(738, 55);
            this.labelTotalMemory.Name = "labelTotalMemory";
            this.labelTotalMemory.Size = new System.Drawing.Size(126, 20);
            this.labelTotalMemory.TabIndex = 17;
            this.labelTotalMemory.Text = "Total Memory: ";
            // 
            // textBoxAvailableMemory
            // 
            this.textBoxAvailableMemory.Location = new System.Drawing.Point(902, 94);
            this.textBoxAvailableMemory.Name = "textBoxAvailableMemory";
            this.textBoxAvailableMemory.ReadOnly = true;
            this.textBoxAvailableMemory.Size = new System.Drawing.Size(79, 20);
            this.textBoxAvailableMemory.TabIndex = 20;
            this.textBoxAvailableMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPercentUsed
            // 
            this.textBoxPercentUsed.Location = new System.Drawing.Point(902, 130);
            this.textBoxPercentUsed.Name = "textBoxPercentUsed";
            this.textBoxPercentUsed.ReadOnly = true;
            this.textBoxPercentUsed.Size = new System.Drawing.Size(79, 20);
            this.textBoxPercentUsed.TabIndex = 21;
            this.textBoxPercentUsed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxTotalMemory
            // 
            this.textBoxTotalMemory.Location = new System.Drawing.Point(902, 58);
            this.textBoxTotalMemory.Name = "textBoxTotalMemory";
            this.textBoxTotalMemory.ReadOnly = true;
            this.textBoxTotalMemory.Size = new System.Drawing.Size(79, 20);
            this.textBoxTotalMemory.TabIndex = 22;
            this.textBoxTotalMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelMB1
            // 
            this.labelMB1.AutoSize = true;
            this.labelMB1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMB1.Location = new System.Drawing.Point(987, 57);
            this.labelMB1.Name = "labelMB1";
            this.labelMB1.Size = new System.Drawing.Size(35, 20);
            this.labelMB1.TabIndex = 23;
            this.labelMB1.Text = "MB";
            // 
            // labelMB2
            // 
            this.labelMB2.AutoSize = true;
            this.labelMB2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMB2.Location = new System.Drawing.Point(987, 95);
            this.labelMB2.Name = "labelMB2";
            this.labelMB2.Size = new System.Drawing.Size(35, 20);
            this.labelMB2.TabIndex = 24;
            this.labelMB2.Text = "MB";
            // 
            // labelMB3
            // 
            this.labelMB3.AutoSize = true;
            this.labelMB3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMB3.Location = new System.Drawing.Point(987, 129);
            this.labelMB3.Name = "labelMB3";
            this.labelMB3.Size = new System.Drawing.Size(24, 20);
            this.labelMB3.TabIndex = 25;
            this.labelMB3.Text = "%";
            // 
            // labelSystemStatus
            // 
            this.labelSystemStatus.AutoSize = true;
            this.labelSystemStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSystemStatus.Location = new System.Drawing.Point(800, 26);
            this.labelSystemStatus.Name = "labelSystemStatus";
            this.labelSystemStatus.Size = new System.Drawing.Size(163, 25);
            this.labelSystemStatus.TabIndex = 26;
            this.labelSystemStatus.Text = "System Status";
            // 
            // pictureBoxHardDrive
            // 
            this.pictureBoxHardDrive.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxHardDrive.Image")));
            this.pictureBoxHardDrive.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxHardDrive.InitialImage")));
            this.pictureBoxHardDrive.Location = new System.Drawing.Point(629, 58);
            this.pictureBoxHardDrive.Name = "pictureBoxHardDrive";
            this.pictureBoxHardDrive.Size = new System.Drawing.Size(103, 95);
            this.pictureBoxHardDrive.TabIndex = 27;
            this.pictureBoxHardDrive.TabStop = false;
            // 
            // pictureBoxStatusLED
            // 
            this.pictureBoxStatusLED.Image = global::MooseBoxUI.Properties.Resources.RedLED_25x25;
            this.pictureBoxStatusLED.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxStatusLED.InitialImage")));
            this.pictureBoxStatusLED.Location = new System.Drawing.Point(1009, 156);
            this.pictureBoxStatusLED.Name = "pictureBoxStatusLED";
            this.pictureBoxStatusLED.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxStatusLED.TabIndex = 28;
            this.pictureBoxStatusLED.TabStop = false;
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fansToolStripMenuItem,
            this.temperatureToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1034, 24);
            this.menuStripMain.TabIndex = 29;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fansToolStripMenuItem
            // 
            this.fansToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualOverrideToolStripMenuItem,
            this.automationConfigToolStripMenuItem});
            this.fansToolStripMenuItem.Name = "fansToolStripMenuItem";
            this.fansToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.fansToolStripMenuItem.Text = "Fans";
            // 
            // manualOverrideToolStripMenuItem
            // 
            this.manualOverrideToolStripMenuItem.Name = "manualOverrideToolStripMenuItem";
            this.manualOverrideToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.manualOverrideToolStripMenuItem.Text = "Manual Override";
            this.manualOverrideToolStripMenuItem.Click += new System.EventHandler(this.manualOverrideToolStripMenuItem_Click);
            // 
            // automationConfigToolStripMenuItem
            // 
            this.automationConfigToolStripMenuItem.Name = "automationConfigToolStripMenuItem";
            this.automationConfigToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.automationConfigToolStripMenuItem.Text = "Automation Config";
            this.automationConfigToolStripMenuItem.Click += new System.EventHandler(this.automationConfigToolStripMenuItem_Click);
            // 
            // temperatureToolStripMenuItem
            // 
            this.temperatureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureAlarmsToolStripMenuItem});
            this.temperatureToolStripMenuItem.Name = "temperatureToolStripMenuItem";
            this.temperatureToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.temperatureToolStripMenuItem.Text = "Temperature";
            // 
            // configureAlarmsToolStripMenuItem
            // 
            this.configureAlarmsToolStripMenuItem.Name = "configureAlarmsToolStripMenuItem";
            this.configureAlarmsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.configureAlarmsToolStripMenuItem.Text = "Configure Alarms";
            this.configureAlarmsToolStripMenuItem.Click += new System.EventHandler(this.configureAlarmsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 181);
            this.Controls.Add(this.pictureBoxStatusLED);
            this.Controls.Add(this.pictureBoxHardDrive);
            this.Controls.Add(this.labelSystemStatus);
            this.Controls.Add(this.labelMB3);
            this.Controls.Add(this.labelMB2);
            this.Controls.Add(this.labelMB1);
            this.Controls.Add(this.textBoxTotalMemory);
            this.Controls.Add(this.textBoxPercentUsed);
            this.Controls.Add(this.textBoxAvailableMemory);
            this.Controls.Add(this.labelAvailableMemory);
            this.Controls.Add(this.labelMemoryPercentUsed);
            this.Controls.Add(this.labelTotalMemory);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.labelTemperatureStatus);
            this.Controls.Add(this.labelFahrenheit2);
            this.Controls.Add(this.labelFahrenheit1);
            this.Controls.Add(this.textBox59000002A218F928);
            this.Controls.Add(this.textBoxF7000002A215B828);
            this.Controls.Add(this.pictureBoxThermometer);
            this.Controls.Add(this.label59000002A218F928);
            this.Controls.Add(this.labelF7000002A215B828);
            this.Controls.Add(this.labelFanStatus);
            this.Controls.Add(this.pictureBoxFan3LED);
            this.Controls.Add(this.pictureBoxFan2LED);
            this.Controls.Add(this.pictureBoxFan1LED);
            this.Controls.Add(this.labelFan2Status);
            this.Controls.Add(this.labelFan3Status);
            this.Controls.Add(this.labelFan1Status);
            this.Controls.Add(this.pictureBoxFan);
            this.Controls.Add(this.menuStripMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MooseBox Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan1LED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan2LED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFan3LED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThermometer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHardDrive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatusLED)).EndInit();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxFan;
        private System.Windows.Forms.Label labelFan1Status;
        private System.Windows.Forms.Label labelFan3Status;
        private System.Windows.Forms.Label labelFan2Status;
        private System.Windows.Forms.PictureBox pictureBoxFan1LED;
        private System.Windows.Forms.PictureBox pictureBoxFan2LED;
        private System.Windows.Forms.PictureBox pictureBoxFan3LED;
        private System.Windows.Forms.Label labelFanStatus;
        private System.Windows.Forms.Label labelF7000002A215B828;
        private System.Windows.Forms.Label label59000002A218F928;
        private System.Windows.Forms.PictureBox pictureBoxThermometer;
        private System.Windows.Forms.TextBox textBoxF7000002A215B828;
        private System.Windows.Forms.TextBox textBox59000002A218F928;
        private System.Windows.Forms.Label labelFahrenheit1;
        private System.Windows.Forms.Label labelFahrenheit2;
        private System.Windows.Forms.Label labelTemperatureStatus;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label labelAvailableMemory;
        private System.Windows.Forms.Label labelMemoryPercentUsed;
        private System.Windows.Forms.Label labelTotalMemory;
        private System.Windows.Forms.TextBox textBoxAvailableMemory;
        private System.Windows.Forms.TextBox textBoxPercentUsed;
        private System.Windows.Forms.TextBox textBoxTotalMemory;
        private System.Windows.Forms.Label labelMB1;
        private System.Windows.Forms.Label labelMB2;
        private System.Windows.Forms.Label labelMB3;
        private System.Windows.Forms.Label labelSystemStatus;
        private System.Windows.Forms.PictureBox pictureBoxHardDrive;
        private System.Windows.Forms.PictureBox pictureBoxStatusLED;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temperatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualOverrideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automationConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureAlarmsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
    }
}

