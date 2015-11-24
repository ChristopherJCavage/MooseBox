namespace MooseBoxUI
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelMooseBoxClient = new System.Windows.Forms.Label();
            this.pictureBoxRaspberryPI2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxMoose = new System.Windows.Forms.PictureBox();
            this.labelClientVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaspberryPI2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMoose)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(85, 22);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(204, 42);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "MooseBox";
            // 
            // labelMooseBoxClient
            // 
            this.labelMooseBoxClient.AutoSize = true;
            this.labelMooseBoxClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMooseBoxClient.Location = new System.Drawing.Point(14, 101);
            this.labelMooseBoxClient.Name = "labelMooseBoxClient";
            this.labelMooseBoxClient.Size = new System.Drawing.Size(65, 20);
            this.labelMooseBoxClient.TabIndex = 3;
            this.labelMooseBoxClient.Text = "Client: ";
            // 
            // pictureBoxRaspberryPI2
            // 
            this.pictureBoxRaspberryPI2.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxRaspberryPI2.ErrorImage")));
            this.pictureBoxRaspberryPI2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxRaspberryPI2.Image")));
            this.pictureBoxRaspberryPI2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxRaspberryPI2.InitialImage")));
            this.pictureBoxRaspberryPI2.Location = new System.Drawing.Point(295, 13);
            this.pictureBoxRaspberryPI2.Name = "pictureBoxRaspberryPI2";
            this.pictureBoxRaspberryPI2.Size = new System.Drawing.Size(56, 61);
            this.pictureBoxRaspberryPI2.TabIndex = 2;
            this.pictureBoxRaspberryPI2.TabStop = false;
            // 
            // pictureBoxMoose
            // 
            this.pictureBoxMoose.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxMoose.ErrorImage")));
            this.pictureBoxMoose.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxMoose.Image")));
            this.pictureBoxMoose.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxMoose.InitialImage")));
            this.pictureBoxMoose.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxMoose.Name = "pictureBoxMoose";
            this.pictureBoxMoose.Size = new System.Drawing.Size(67, 64);
            this.pictureBoxMoose.TabIndex = 1;
            this.pictureBoxMoose.TabStop = false;
            // 
            // labelClientVersion
            // 
            this.labelClientVersion.AutoSize = true;
            this.labelClientVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClientVersion.Location = new System.Drawing.Point(85, 105);
            this.labelClientVersion.Name = "labelClientVersion";
            this.labelClientVersion.Size = new System.Drawing.Size(55, 16);
            this.labelClientVersion.TabIndex = 4;
            this.labelClientVersion.Text = "W.X.Y.Z";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 142);
            this.Controls.Add(this.labelClientVersion);
            this.Controls.Add(this.labelMooseBoxClient);
            this.Controls.Add(this.pictureBoxRaspberryPI2);
            this.Controls.Add(this.pictureBoxMoose);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About MooseBox";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaspberryPI2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMoose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxMoose;
        private System.Windows.Forms.PictureBox pictureBoxRaspberryPI2;
        private System.Windows.Forms.Label labelMooseBoxClient;
        private System.Windows.Forms.Label labelClientVersion;
    }
}