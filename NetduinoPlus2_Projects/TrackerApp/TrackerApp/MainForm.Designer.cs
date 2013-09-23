namespace TrackerApp
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
            this.components = new System.ComponentModel.Container();
            this.textBoxPortName = new System.Windows.Forms.TextBox();
            this.textBoxBaudRate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonSendCMD = new System.Windows.Forms.Button();
            this.textBoxCMD = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.trackBarPan = new System.Windows.Forms.TrackBar();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxPanTilt = new System.Windows.Forms.PictureBox();
            this.labelMouseXYPos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPanTilt)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxPortName
            // 
            this.textBoxPortName.Location = new System.Drawing.Point(706, 446);
            this.textBoxPortName.Name = "textBoxPortName";
            this.textBoxPortName.Size = new System.Drawing.Size(119, 20);
            this.textBoxPortName.TabIndex = 0;
            this.textBoxPortName.Text = "COM5";
            // 
            // textBoxBaudRate
            // 
            this.textBoxBaudRate.Location = new System.Drawing.Point(706, 472);
            this.textBoxBaudRate.Name = "textBoxBaudRate";
            this.textBoxBaudRate.Size = new System.Drawing.Size(119, 20);
            this.textBoxBaudRate.TabIndex = 1;
            this.textBoxBaudRate.Text = "115200";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(659, 449);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "PORT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(659, 475);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "BAUD";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.ForeColor = System.Drawing.Color.Green;
            this.buttonConnect.Location = new System.Drawing.Point(662, 498);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(163, 43);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonSendCMD
            // 
            this.buttonSendCMD.Location = new System.Drawing.Point(12, 38);
            this.buttonSendCMD.Name = "buttonSendCMD";
            this.buttonSendCMD.Size = new System.Drawing.Size(75, 23);
            this.buttonSendCMD.TabIndex = 5;
            this.buttonSendCMD.Text = "Send";
            this.buttonSendCMD.UseVisualStyleBackColor = true;
            this.buttonSendCMD.Click += new System.EventHandler(this.buttonSendCMD_Click);
            // 
            // textBoxCMD
            // 
            this.textBoxCMD.Location = new System.Drawing.Point(12, 12);
            this.textBoxCMD.Name = "textBoxCMD";
            this.textBoxCMD.Size = new System.Drawing.Size(75, 20);
            this.textBoxCMD.TabIndex = 6;
            this.textBoxCMD.Text = "45";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // trackBarPan
            // 
            this.trackBarPan.LargeChange = 1;
            this.trackBarPan.Location = new System.Drawing.Point(12, 498);
            this.trackBarPan.Maximum = 90;
            this.trackBarPan.Name = "trackBarPan";
            this.trackBarPan.Size = new System.Drawing.Size(411, 45);
            this.trackBarPan.TabIndex = 7;
            this.trackBarPan.Value = 45;
            this.trackBarPan.Scroll += new System.EventHandler(this.trackBarPan_Scroll);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // pictureBoxPanTilt
            // 
            this.pictureBoxPanTilt.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBoxPanTilt.Location = new System.Drawing.Point(138, 308);
            this.pictureBoxPanTilt.Name = "pictureBoxPanTilt";
            this.pictureBoxPanTilt.Size = new System.Drawing.Size(158, 158);
            this.pictureBoxPanTilt.TabIndex = 8;
            this.pictureBoxPanTilt.TabStop = false;
            this.pictureBoxPanTilt.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPanTilt_MouseMove);
            // 
            // labelMouseXYPos
            // 
            this.labelMouseXYPos.AutoSize = true;
            this.labelMouseXYPos.Location = new System.Drawing.Point(189, 472);
            this.labelMouseXYPos.Name = "labelMouseXYPos";
            this.labelMouseXYPos.Size = new System.Drawing.Size(57, 13);
            this.labelMouseXYPos.TabIndex = 9;
            this.labelMouseXYPos.Text = "X: --    Y: --";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 553);
            this.Controls.Add(this.labelMouseXYPos);
            this.Controls.Add(this.pictureBoxPanTilt);
            this.Controls.Add(this.trackBarPan);
            this.Controls.Add(this.textBoxCMD);
            this.Controls.Add(this.buttonSendCMD);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBaudRate);
            this.Controls.Add(this.textBoxPortName);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPanTilt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPortName;
        private System.Windows.Forms.TextBox textBoxBaudRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonSendCMD;
        private System.Windows.Forms.TextBox textBoxCMD;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar trackBarPan;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.PictureBox pictureBoxPanTilt;
        private System.Windows.Forms.Label labelMouseXYPos;
    }
}

