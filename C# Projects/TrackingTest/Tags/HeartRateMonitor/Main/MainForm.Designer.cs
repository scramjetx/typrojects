namespace TrackingTest
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.labelHeartRate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ledHeartBeat = new TH.WinComponents.Led();
            this.myLineGraph = new Graph.LineGraph();
            this.pictureBoxWorkingImage = new System.Windows.Forms.PictureBox();
            this.labelStats = new System.Windows.Forms.Label();
            this.buttonStopStream = new System.Windows.Forms.Button();
            this.buttonPlayLiveStream = new System.Windows.Forms.Button();
            this.buttonOpenResults = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.pictureBoxProcessedVideo = new System.Windows.Forms.PictureBox();
            this.buttonPlayVideo = new System.Windows.Forms.Button();
            this.timerUpdateUI = new System.Windows.Forms.Timer(this.components);
            this.timerHeartBeatSensor = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorkingImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcessedVideo)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1195, 657);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.labelHeartRate);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.ledHeartBeat);
            this.tabPage2.Controls.Add(this.myLineGraph);
            this.tabPage2.Controls.Add(this.pictureBoxWorkingImage);
            this.tabPage2.Controls.Add(this.labelStats);
            this.tabPage2.Controls.Add(this.buttonStopStream);
            this.tabPage2.Controls.Add(this.buttonPlayLiveStream);
            this.tabPage2.Controls.Add(this.buttonOpenResults);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.videoSourcePlayer);
            this.tabPage2.Controls.Add(this.pictureBoxProcessedVideo);
            this.tabPage2.Controls.Add(this.buttonPlayVideo);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1187, 631);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test App";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe Print", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1046, 413);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 33);
            this.label4.TabIndex = 41;
            this.label4.Text = "Heart Rate";
            // 
            // labelHeartRate
            // 
            this.labelHeartRate.AutoSize = true;
            this.labelHeartRate.Font = new System.Drawing.Font("Segoe Print", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeartRate.Location = new System.Drawing.Point(1067, 426);
            this.labelHeartRate.Name = "labelHeartRate";
            this.labelHeartRate.Size = new System.Drawing.Size(79, 61);
            this.labelHeartRate.TabIndex = 40;
            this.labelHeartRate.Text = "00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(782, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 20);
            this.label2.TabIndex = 39;
            this.label2.Text = "Processed Video";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(983, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 20);
            this.label1.TabIndex = 38;
            this.label1.Text = "Image Mean Squared";
            // 
            // ledHeartBeat
            // 
            this.ledHeartBeat.BackColor = System.Drawing.Color.Transparent;
            this.ledHeartBeat.ColorOff = System.Drawing.Color.Red;
            this.ledHeartBeat.ColorOn = System.Drawing.Color.Green;
            this.ledHeartBeat.Location = new System.Drawing.Point(1066, 321);
            this.ledHeartBeat.Name = "ledHeartBeat";
            this.ledHeartBeat.Size = new System.Drawing.Size(70, 67);
            this.ledHeartBeat.TabIndex = 37;
            this.ledHeartBeat.Text = "led1";
            // 
            // myLineGraph
            // 
            this.myLineGraph.BackgroundColor = System.Drawing.Color.Black;
            this.myLineGraph.BufferSize = 50000;
            this.myLineGraph.Graph1Name = "Graph1";
            this.myLineGraph.Graph2Name = "Graph2";
            this.myLineGraph.GridSpacingX = 10;
            this.myLineGraph.GridSpacingY = 13;
            this.myLineGraph.IsBWVisible = false;
            this.myLineGraph.IsLogarithmic = false;
            this.myLineGraph.IsSDVisible = false;
            this.myLineGraph.IsSMAVisible = false;
            this.myLineGraph.Location = new System.Drawing.Point(252, 302);
            this.myLineGraph.LogarithmicIncFactor = 1.7F;
            this.myLineGraph.LogarithmicMax = 40642.34F;
            this.myLineGraph.LogarithmicMin = 1F;
            this.myLineGraph.Max = 15F;
            this.myLineGraph.Min = 0F;
            this.myLineGraph.Name = "myLineGraph";
            this.myLineGraph.NoOfGraphs = 2;
            this.myLineGraph.NoOfYGrids = 20;
            this.myLineGraph.PlotLine1Color = System.Drawing.Color.YellowGreen;
            this.myLineGraph.PlotLine1Width = 2F;
            this.myLineGraph.PlotLine2Color = System.Drawing.Color.YellowGreen;
            this.myLineGraph.PlotLine2Width = 2F;
            this.myLineGraph.Size = new System.Drawing.Size(788, 326);
            this.myLineGraph.TabIndex = 36;
            this.myLineGraph.XAxisBackColor = System.Drawing.Color.Transparent;
            this.myLineGraph.XAxisHeader = "Time";
            this.myLineGraph.XGridColor = System.Drawing.Color.SeaGreen;
            this.myLineGraph.XValueDisplayInterval = 10;
            this.myLineGraph.YAxisBackColor = System.Drawing.Color.Transparent;
            this.myLineGraph.YAxisHeader = "v\na\nl\nu\ne\ns\n";
            this.myLineGraph.YGridColor = System.Drawing.Color.SeaGreen;
            this.myLineGraph.YGridInterval = 400;
            // 
            // pictureBoxWorkingImage
            // 
            this.pictureBoxWorkingImage.BackColor = System.Drawing.SystemColors.Info;
            this.pictureBoxWorkingImage.Location = new System.Drawing.Point(729, 81);
            this.pictureBoxWorkingImage.Name = "pictureBoxWorkingImage";
            this.pictureBoxWorkingImage.Size = new System.Drawing.Size(229, 189);
            this.pictureBoxWorkingImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxWorkingImage.TabIndex = 35;
            this.pictureBoxWorkingImage.TabStop = false;
            // 
            // labelStats
            // 
            this.labelStats.AutoSize = true;
            this.labelStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStats.Location = new System.Drawing.Point(1031, 133);
            this.labelStats.Name = "labelStats";
            this.labelStats.Size = new System.Drawing.Size(51, 20);
            this.labelStats.TabIndex = 34;
            this.labelStats.Text = "label1";
            // 
            // buttonStopStream
            // 
            this.buttonStopStream.Font = new System.Drawing.Font("Segoe Print", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStopStream.Location = new System.Drawing.Point(128, 348);
            this.buttonStopStream.Name = "buttonStopStream";
            this.buttonStopStream.Size = new System.Drawing.Size(106, 65);
            this.buttonStopStream.TabIndex = 30;
            this.buttonStopStream.Text = "STOP";
            this.buttonStopStream.UseVisualStyleBackColor = true;
            this.buttonStopStream.Click += new System.EventHandler(this.buttonStopStream_Click);
            // 
            // buttonPlayLiveStream
            // 
            this.buttonPlayLiveStream.Font = new System.Drawing.Font("Segoe Print", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlayLiveStream.Location = new System.Drawing.Point(128, 280);
            this.buttonPlayLiveStream.Name = "buttonPlayLiveStream";
            this.buttonPlayLiveStream.Size = new System.Drawing.Size(106, 62);
            this.buttonPlayLiveStream.TabIndex = 29;
            this.buttonPlayLiveStream.Text = "Play Live Stream";
            this.buttonPlayLiveStream.UseVisualStyleBackColor = true;
            this.buttonPlayLiveStream.Click += new System.EventHandler(this.buttonPlayLiveStream_Click);
            // 
            // buttonOpenResults
            // 
            this.buttonOpenResults.Font = new System.Drawing.Font("Segoe Print", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenResults.Location = new System.Drawing.Point(6, 348);
            this.buttonOpenResults.Name = "buttonOpenResults";
            this.buttonOpenResults.Size = new System.Drawing.Size(116, 65);
            this.buttonOpenResults.TabIndex = 22;
            this.buttonOpenResults.Text = "Open Results";
            this.buttonOpenResults.UseVisualStyleBackColor = true;
            this.buttonOpenResults.Click += new System.EventHandler(this.buttonOpenResults_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(459, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "Processed Video";
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.Location = new System.Drawing.Point(6, 28);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(322, 242);
            this.videoSourcePlayer.TabIndex = 5;
            this.videoSourcePlayer.Text = "videoSourcePlayer1";
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer_NewFrame);
            // 
            // pictureBoxProcessedVideo
            // 
            this.pictureBoxProcessedVideo.BackColor = System.Drawing.SystemColors.Info;
            this.pictureBoxProcessedVideo.Location = new System.Drawing.Point(360, 28);
            this.pictureBoxProcessedVideo.Name = "pictureBoxProcessedVideo";
            this.pictureBoxProcessedVideo.Size = new System.Drawing.Size(322, 242);
            this.pictureBoxProcessedVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxProcessedVideo.TabIndex = 4;
            this.pictureBoxProcessedVideo.TabStop = false;
            // 
            // buttonPlayVideo
            // 
            this.buttonPlayVideo.Font = new System.Drawing.Font("Segoe Print", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlayVideo.Location = new System.Drawing.Point(6, 280);
            this.buttonPlayVideo.Name = "buttonPlayVideo";
            this.buttonPlayVideo.Size = new System.Drawing.Size(116, 62);
            this.buttonPlayVideo.TabIndex = 0;
            this.buttonPlayVideo.Text = "Play Video";
            this.buttonPlayVideo.UseVisualStyleBackColor = true;
            this.buttonPlayVideo.Click += new System.EventHandler(this.buttonPlayVideo_Click);
            // 
            // timerUpdateUI
            // 
            this.timerUpdateUI.Enabled = true;
            this.timerUpdateUI.Interval = 30;
            this.timerUpdateUI.Tick += new System.EventHandler(this.timerUpdateUI_Tick);
            // 
            // timerHeartBeatSensor
            // 
            this.timerHeartBeatSensor.Interval = 33;
            this.timerHeartBeatSensor.Tick += new System.EventHandler(this.timerHeartBeatSensor_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 678);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorkingImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcessedVideo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonPlayVideo;
        private System.Windows.Forms.PictureBox pictureBoxProcessedVideo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOpenResults;
        private System.Windows.Forms.Button buttonPlayLiveStream;
        private System.Windows.Forms.Button buttonStopStream;
        private System.Windows.Forms.Timer timerUpdateUI;
        private System.Windows.Forms.Label labelStats;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.PictureBox pictureBoxWorkingImage;
        private Graph.LineGraph myLineGraph;
        private System.Windows.Forms.Timer timerHeartBeatSensor;
        private TH.WinComponents.Led ledHeartBeat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelHeartRate;
        private System.Windows.Forms.Label label4;
    }
}

