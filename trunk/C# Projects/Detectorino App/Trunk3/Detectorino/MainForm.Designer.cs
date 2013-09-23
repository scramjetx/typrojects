namespace Detectorino
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
            this.buttonSendEmail = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.buttonOpenPort = new System.Windows.Forms.Button();
            this.buttonClosePort = new System.Windows.Forms.Button();
            this.timerLogData = new System.Windows.Forms.Timer(this.components);
            this.timerCountDown = new System.Windows.Forms.Timer(this.components);
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelCountDownTimer = new System.Windows.Forms.Label();
            this.ledStatus = new TH.WinComponents.Led();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTimeSinceMovement = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelTimePresent = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timerToggleLED = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSendEmail
            // 
            this.buttonSendEmail.Location = new System.Drawing.Point(15, 278);
            this.buttonSendEmail.Name = "buttonSendEmail";
            this.buttonSendEmail.Size = new System.Drawing.Size(145, 49);
            this.buttonSendEmail.TabIndex = 0;
            this.buttonSendEmail.Text = "SendEmail";
            this.buttonSendEmail.UseVisualStyleBackColor = true;
            this.buttonSendEmail.Click += new System.EventHandler(this.buttonSendEmail_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM3";
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // buttonOpenPort
            // 
            this.buttonOpenPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.buttonOpenPort.Location = new System.Drawing.Point(12, 12);
            this.buttonOpenPort.Name = "buttonOpenPort";
            this.buttonOpenPort.Size = new System.Drawing.Size(145, 50);
            this.buttonOpenPort.TabIndex = 1;
            this.buttonOpenPort.Text = "Open Serial";
            this.buttonOpenPort.UseVisualStyleBackColor = true;
            this.buttonOpenPort.Click += new System.EventHandler(this.buttonOpenPort_Click);
            // 
            // buttonClosePort
            // 
            this.buttonClosePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.buttonClosePort.Location = new System.Drawing.Point(12, 70);
            this.buttonClosePort.Name = "buttonClosePort";
            this.buttonClosePort.Size = new System.Drawing.Size(145, 50);
            this.buttonClosePort.TabIndex = 2;
            this.buttonClosePort.Text = "Close Serial";
            this.buttonClosePort.UseVisualStyleBackColor = true;
            this.buttonClosePort.Click += new System.EventHandler(this.buttonClosePort_Click);
            // 
            // timerLogData
            // 
            this.timerLogData.Interval = 60000;
            this.timerLogData.Tick += new System.EventHandler(this.timerLogData_Tick);
            // 
            // timerCountDown
            // 
            this.timerCountDown.Interval = 1000;
            this.timerCountDown.Tick += new System.EventHandler(this.timerCountDown_Tick);
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.buttonStart.Location = new System.Drawing.Point(15, 222);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(145, 50);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelCountDownTimer
            // 
            this.labelCountDownTimer.AutoSize = true;
            this.labelCountDownTimer.Location = new System.Drawing.Point(157, 43);
            this.labelCountDownTimer.Name = "labelCountDownTimer";
            this.labelCountDownTimer.Size = new System.Drawing.Size(68, 25);
            this.labelCountDownTimer.TabIndex = 4;
            this.labelCountDownTimer.Text = "0.00V";
            // 
            // ledStatus
            // 
            this.ledStatus.BackColor = System.Drawing.Color.Transparent;
            this.ledStatus.ColorOff = System.Drawing.SystemColors.Control;
            this.ledStatus.ColorOn = System.Drawing.Color.Red;
            this.ledStatus.FlashIntervals = "300";
            this.ledStatus.Location = new System.Drawing.Point(57, 143);
            this.ledStatus.Name = "ledStatus";
            this.ledStatus.Size = new System.Drawing.Size(63, 65);
            this.ledStatus.TabIndex = 5;
            this.ledStatus.Text = "led1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Count Down:";
            // 
            // labelTimeSinceMovement
            // 
            this.labelTimeSinceMovement.AutoSize = true;
            this.labelTimeSinceMovement.Location = new System.Drawing.Point(240, 67);
            this.labelTimeSinceMovement.Name = "labelTimeSinceMovement";
            this.labelTimeSinceMovement.Size = new System.Drawing.Size(96, 25);
            this.labelTimeSinceMovement.TabIndex = 7;
            this.labelTimeSinceMovement.Text = "00:00:00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Time Since Movement:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Time Present:";
            // 
            // labelTimePresent
            // 
            this.labelTimePresent.AutoSize = true;
            this.labelTimePresent.Location = new System.Drawing.Point(157, 89);
            this.labelTimePresent.Name = "labelTimePresent";
            this.labelTimePresent.Size = new System.Drawing.Size(96, 25);
            this.labelTimePresent.TabIndex = 10;
            this.labelTimePresent.Text = "00:00:00";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelCountDownTimer);
            this.groupBox1.Controls.Add(this.labelTimePresent);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.labelTimeSinceMovement);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(206, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(342, 166);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stats";
            // 
            // timerToggleLED
            // 
            this.timerToggleLED.Interval = 50;
            this.timerToggleLED.Tick += new System.EventHandler(this.timerToggleLED_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 386);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ledStatus);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonClosePort);
            this.Controls.Add(this.buttonOpenPort);
            this.Controls.Add(this.buttonSendEmail);
            this.Name = "MainForm";
            this.Text = "Detectorino";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSendEmail;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button buttonOpenPort;
        private System.Windows.Forms.Button buttonClosePort;
        private System.Windows.Forms.Timer timerLogData;
        private System.Windows.Forms.Timer timerCountDown;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelCountDownTimer;
        private TH.WinComponents.Led ledStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTimeSinceMovement;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelTimePresent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timerToggleLED;
    }
}

