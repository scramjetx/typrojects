namespace Test
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.cmbYGridColor = new System.Windows.Forms.ComboBox();
            this.cmbXGridColor = new System.Windows.Forms.ComboBox();
            this.cmbPlotLine2Color = new System.Windows.Forms.ComboBox();
            this.cmbPlotLine1Color = new System.Windows.Forms.ComboBox();
            this.cmbBackColors = new System.Windows.Forms.ComboBox();
            this.cmbYGridInterval = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbNoOfYGrids = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtYAxisText = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtXAxisText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbXValDisplay = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPlotLine2Width = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLogInc = new System.Windows.Forms.ComboBox();
            this.cmbNoOfGraphs = new System.Windows.Forms.ComboBox();
            this.cmbIsLogarithmic = new System.Windows.Forms.ComboBox();
            this.cmbLogMin = new System.Windows.Forms.ComboBox();
            this.cmbPlotLin1Width = new System.Windows.Forms.ComboBox();
            this.cmbGridSpacingX = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblGridSpacingX = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.txtxGraph1Name = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtGraph2Name = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.lineGraph1 = new Graph.LineGraph();
            this.groupBox1.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lineGraph1);
            this.groupBox1.Location = new System.Drawing.Point(8, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(727, 268);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Line Graph";
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.txtGraph2Name);
            this.gbProperties.Controls.Add(this.label17);
            this.gbProperties.Controls.Add(this.txtxGraph1Name);
            this.gbProperties.Controls.Add(this.label16);
            this.gbProperties.Controls.Add(this.cmbYGridColor);
            this.gbProperties.Controls.Add(this.cmbXGridColor);
            this.gbProperties.Controls.Add(this.cmbPlotLine2Color);
            this.gbProperties.Controls.Add(this.cmbPlotLine1Color);
            this.gbProperties.Controls.Add(this.cmbBackColors);
            this.gbProperties.Controls.Add(this.cmbYGridInterval);
            this.gbProperties.Controls.Add(this.label15);
            this.gbProperties.Controls.Add(this.button4);
            this.gbProperties.Controls.Add(this.button3);
            this.gbProperties.Controls.Add(this.button2);
            this.gbProperties.Controls.Add(this.button1);
            this.gbProperties.Controls.Add(this.cmbNoOfYGrids);
            this.gbProperties.Controls.Add(this.label14);
            this.gbProperties.Controls.Add(this.txtYAxisText);
            this.gbProperties.Controls.Add(this.label11);
            this.gbProperties.Controls.Add(this.txtXAxisText);
            this.gbProperties.Controls.Add(this.label4);
            this.gbProperties.Controls.Add(this.cmbXValDisplay);
            this.gbProperties.Controls.Add(this.label3);
            this.gbProperties.Controls.Add(this.cmbPlotLine2Width);
            this.gbProperties.Controls.Add(this.label13);
            this.gbProperties.Controls.Add(this.label12);
            this.gbProperties.Controls.Add(this.label9);
            this.gbProperties.Controls.Add(this.label1);
            this.gbProperties.Controls.Add(this.cmbLogInc);
            this.gbProperties.Controls.Add(this.cmbNoOfGraphs);
            this.gbProperties.Controls.Add(this.cmbIsLogarithmic);
            this.gbProperties.Controls.Add(this.cmbLogMin);
            this.gbProperties.Controls.Add(this.cmbPlotLin1Width);
            this.gbProperties.Controls.Add(this.cmbGridSpacingX);
            this.gbProperties.Controls.Add(this.label10);
            this.gbProperties.Controls.Add(this.lblGridSpacingX);
            this.gbProperties.Controls.Add(this.label8);
            this.gbProperties.Controls.Add(this.label7);
            this.gbProperties.Controls.Add(this.label6);
            this.gbProperties.Controls.Add(this.label5);
            this.gbProperties.Controls.Add(this.label2);
            this.gbProperties.Controls.Add(this.lblBackColor);
            this.gbProperties.Location = new System.Drawing.Point(8, 268);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(727, 349);
            this.gbProperties.TabIndex = 2;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Properties";
            // 
            // cmbYGridColor
            // 
            this.cmbYGridColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYGridColor.FormattingEnabled = true;
            this.cmbYGridColor.Location = new System.Drawing.Point(536, 166);
            this.cmbYGridColor.Name = "cmbYGridColor";
            this.cmbYGridColor.Size = new System.Drawing.Size(121, 21);
            this.cmbYGridColor.TabIndex = 55;
            this.cmbYGridColor.SelectedIndexChanged += new System.EventHandler(this.cmbYGridColor_SelectedIndexChanged);
            // 
            // cmbXGridColor
            // 
            this.cmbXGridColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbXGridColor.FormattingEnabled = true;
            this.cmbXGridColor.Location = new System.Drawing.Point(536, 136);
            this.cmbXGridColor.Name = "cmbXGridColor";
            this.cmbXGridColor.Size = new System.Drawing.Size(121, 21);
            this.cmbXGridColor.TabIndex = 54;
            this.cmbXGridColor.SelectedIndexChanged += new System.EventHandler(this.cmbXGridColor_SelectedIndexChanged);
            // 
            // cmbPlotLine2Color
            // 
            this.cmbPlotLine2Color.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlotLine2Color.FormattingEnabled = true;
            this.cmbPlotLine2Color.Location = new System.Drawing.Point(536, 46);
            this.cmbPlotLine2Color.Name = "cmbPlotLine2Color";
            this.cmbPlotLine2Color.Size = new System.Drawing.Size(121, 21);
            this.cmbPlotLine2Color.TabIndex = 53;
            this.cmbPlotLine2Color.SelectedIndexChanged += new System.EventHandler(this.cmbPlotLine2Color_SelectedIndexChanged);
            // 
            // cmbPlotLine1Color
            // 
            this.cmbPlotLine1Color.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlotLine1Color.FormattingEnabled = true;
            this.cmbPlotLine1Color.Location = new System.Drawing.Point(536, 14);
            this.cmbPlotLine1Color.Name = "cmbPlotLine1Color";
            this.cmbPlotLine1Color.Size = new System.Drawing.Size(121, 21);
            this.cmbPlotLine1Color.TabIndex = 52;
            this.cmbPlotLine1Color.SelectedIndexChanged += new System.EventHandler(this.cmbPlotLine1Color_SelectedIndexChanged);
            // 
            // cmbBackColors
            // 
            this.cmbBackColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBackColors.FormattingEnabled = true;
            this.cmbBackColors.Location = new System.Drawing.Point(200, 14);
            this.cmbBackColors.Name = "cmbBackColors";
            this.cmbBackColors.Size = new System.Drawing.Size(123, 21);
            this.cmbBackColors.TabIndex = 51;
            this.cmbBackColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbBackColors_DrawItem);
            this.cmbBackColors.SelectedIndexChanged += new System.EventHandler(this.cmbBackColors_SelectedIndexChanged);
            // 
            // cmbYGridInterval
            // 
            this.cmbYGridInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYGridInterval.FormattingEnabled = true;
            this.cmbYGridInterval.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "35",
            "40",
            "45",
            "50"});
            this.cmbYGridInterval.Location = new System.Drawing.Point(202, 253);
            this.cmbYGridInterval.Name = "cmbYGridInterval";
            this.cmbYGridInterval.Size = new System.Drawing.Size(123, 21);
            this.cmbYGridInterval.TabIndex = 50;
            this.cmbYGridInterval.SelectedIndexChanged += new System.EventHandler(this.cmbYGridInterval_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(122, 257);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(74, 13);
            this.label15.TabIndex = 49;
            this.label15.Text = "Y Grid Interval";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(14, 320);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(64, 23);
            this.button4.TabIndex = 48;
            this.button4.Text = "Clear";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(571, 323);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(144, 23);
            this.button3.TabIndex = 47;
            this.button3.Text = "Restore Default";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(375, 320);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 23);
            this.button2.TabIndex = 46;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(287, 320);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 23);
            this.button1.TabIndex = 45;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbNoOfYGrids
            // 
            this.cmbNoOfYGrids.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNoOfYGrids.FormattingEnabled = true;
            this.cmbNoOfYGrids.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbNoOfYGrids.Location = new System.Drawing.Point(202, 224);
            this.cmbNoOfYGrids.Name = "cmbNoOfYGrids";
            this.cmbNoOfYGrids.Size = new System.Drawing.Size(123, 21);
            this.cmbNoOfYGrids.TabIndex = 44;
            this.cmbNoOfYGrids.SelectedIndexChanged += new System.EventHandler(this.cmbNoOfYGrids_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(122, 228);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 13);
            this.label14.TabIndex = 43;
            this.label14.Text = "No Of Y Grids";
            // 
            // txtYAxisText
            // 
            this.txtYAxisText.Location = new System.Drawing.Point(536, 225);
            this.txtYAxisText.Name = "txtYAxisText";
            this.txtYAxisText.Size = new System.Drawing.Size(121, 20);
            this.txtYAxisText.TabIndex = 42;
            this.txtYAxisText.TextChanged += new System.EventHandler(this.txtYAxisText_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(465, 229);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Y Axis Text";
            // 
            // txtXAxisText
            // 
            this.txtXAxisText.Location = new System.Drawing.Point(536, 196);
            this.txtXAxisText.Name = "txtXAxisText";
            this.txtXAxisText.Size = new System.Drawing.Size(121, 20);
            this.txtXAxisText.TabIndex = 40;
            this.txtXAxisText.TextChanged += new System.EventHandler(this.txtXAxisText_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(465, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "X Axis Text";
            // 
            // cmbXValDisplay
            // 
            this.cmbXValDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbXValDisplay.FormattingEnabled = true;
            this.cmbXValDisplay.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.cmbXValDisplay.Location = new System.Drawing.Point(202, 195);
            this.cmbXValDisplay.Name = "cmbXValDisplay";
            this.cmbXValDisplay.Size = new System.Drawing.Size(123, 21);
            this.cmbXValDisplay.TabIndex = 38;
            this.cmbXValDisplay.SelectedIndexChanged += new System.EventHandler(this.cmbXValDisplay_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(75, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "X Value Display Interval";
            // 
            // cmbPlotLine2Width
            // 
            this.cmbPlotLine2Width.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlotLine2Width.FormattingEnabled = true;
            this.cmbPlotLine2Width.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbPlotLine2Width.Location = new System.Drawing.Point(536, 107);
            this.cmbPlotLine2Width.Name = "cmbPlotLine2Width";
            this.cmbPlotLine2Width.Size = new System.Drawing.Size(121, 21);
            this.cmbPlotLine2Width.TabIndex = 32;
            this.cmbPlotLine2Width.SelectedIndexChanged += new System.EventHandler(this.cmbPlotLine2Width_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(437, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 13);
            this.label13.TabIndex = 31;
            this.label13.Text = "Plot Line 2 Width";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(437, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 13);
            this.label12.TabIndex = 30;
            this.label12.Text = "Plot Line 1 Width";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(441, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Plot Line 2 Color";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "No Of Graphs";
            // 
            // cmbLogInc
            // 
            this.cmbLogInc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogInc.FormattingEnabled = true;
            this.cmbLogInc.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbLogInc.Location = new System.Drawing.Point(202, 107);
            this.cmbLogInc.Name = "cmbLogInc";
            this.cmbLogInc.Size = new System.Drawing.Size(123, 21);
            this.cmbLogInc.TabIndex = 23;
            this.cmbLogInc.SelectedIndexChanged += new System.EventHandler(this.cmbLogInc_SelectedIndexChanged);
            // 
            // cmbNoOfGraphs
            // 
            this.cmbNoOfGraphs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNoOfGraphs.FormattingEnabled = true;
            this.cmbNoOfGraphs.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cmbNoOfGraphs.Location = new System.Drawing.Point(202, 166);
            this.cmbNoOfGraphs.Name = "cmbNoOfGraphs";
            this.cmbNoOfGraphs.Size = new System.Drawing.Size(123, 21);
            this.cmbNoOfGraphs.TabIndex = 20;
            this.cmbNoOfGraphs.SelectedIndexChanged += new System.EventHandler(this.cmbNoOfGraphs_SelectedIndexChanged);
            // 
            // cmbIsLogarithmic
            // 
            this.cmbIsLogarithmic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsLogarithmic.FormattingEnabled = true;
            this.cmbIsLogarithmic.Items.AddRange(new object[] {
            "true",
            "false"});
            this.cmbIsLogarithmic.Location = new System.Drawing.Point(202, 77);
            this.cmbIsLogarithmic.Name = "cmbIsLogarithmic";
            this.cmbIsLogarithmic.Size = new System.Drawing.Size(123, 21);
            this.cmbIsLogarithmic.TabIndex = 19;
            this.cmbIsLogarithmic.SelectedIndexChanged += new System.EventHandler(this.cmbIsLogarithmic_SelectedIndexChanged);
            // 
            // cmbLogMin
            // 
            this.cmbLogMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogMin.FormattingEnabled = true;
            this.cmbLogMin.Items.AddRange(new object[] {
            "0.1",
            "0.2",
            "0.3",
            "0.4",
            "0.5",
            "0.6",
            "0.7",
            "0.8",
            "0.9",
            "1.0"});
            this.cmbLogMin.Location = new System.Drawing.Point(202, 136);
            this.cmbLogMin.Name = "cmbLogMin";
            this.cmbLogMin.Size = new System.Drawing.Size(123, 21);
            this.cmbLogMin.TabIndex = 18;
            this.cmbLogMin.SelectedIndexChanged += new System.EventHandler(this.cmbLogMin_SelectedIndexChanged);
            // 
            // cmbPlotLin1Width
            // 
            this.cmbPlotLin1Width.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlotLin1Width.FormattingEnabled = true;
            this.cmbPlotLin1Width.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbPlotLin1Width.Location = new System.Drawing.Point(536, 77);
            this.cmbPlotLin1Width.Name = "cmbPlotLin1Width";
            this.cmbPlotLin1Width.Size = new System.Drawing.Size(121, 21);
            this.cmbPlotLin1Width.TabIndex = 17;
            this.cmbPlotLin1Width.SelectedIndexChanged += new System.EventHandler(this.cmbPlotLin1Width_SelectedIndexChanged);
            // 
            // cmbGridSpacingX
            // 
            this.cmbGridSpacingX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGridSpacingX.FormattingEnabled = true;
            this.cmbGridSpacingX.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80"});
            this.cmbGridSpacingX.Location = new System.Drawing.Point(202, 46);
            this.cmbGridSpacingX.Name = "cmbGridSpacingX";
            this.cmbGridSpacingX.Size = new System.Drawing.Size(123, 21);
            this.cmbGridSpacingX.TabIndex = 15;
            this.cmbGridSpacingX.SelectedIndexChanged += new System.EventHandler(this.cmbGridSpacingX_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(462, 142);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "X Grid Color";
            // 
            // lblGridSpacingX
            // 
            this.lblGridSpacingX.AutoSize = true;
            this.lblGridSpacingX.Location = new System.Drawing.Point(53, 51);
            this.lblGridSpacingX.Name = "lblGridSpacingX";
            this.lblGridSpacingX.Size = new System.Drawing.Size(141, 13);
            this.lblGridSpacingX.TabIndex = 9;
            this.lblGridSpacingX.Text = "Grid Space X(GridSpacingX)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(65, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Logarithmic(IsLogarithmic)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Log Factor(LogarithmicIncFactor)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(72, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Log Min(LogarithmicMin)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(441, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Plot Line 1 Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(462, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y Grid Color";
            // 
            // lblBackColor
            // 
            this.lblBackColor.AutoSize = true;
            this.lblBackColor.Location = new System.Drawing.Point(50, 18);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(144, 13);
            this.lblBackColor.TabIndex = 0;
            this.lblBackColor.Text = "BackColor(BackgroundColor)";
            // 
            // txtxGraph1Name
            // 
            this.txtxGraph1Name.Location = new System.Drawing.Point(536, 254);
            this.txtxGraph1Name.MaxLength = 12;
            this.txtxGraph1Name.Name = "txtxGraph1Name";
            this.txtxGraph1Name.Size = new System.Drawing.Size(121, 20);
            this.txtxGraph1Name.TabIndex = 57;
            this.txtxGraph1Name.TextChanged += new System.EventHandler(this.txtxGraph1Name_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(449, 258);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(76, 13);
            this.label16.TabIndex = 56;
            this.label16.Text = "Graph 1 Name";
            // 
            // txtGraph2Name
            // 
            this.txtGraph2Name.Location = new System.Drawing.Point(536, 284);
            this.txtGraph2Name.MaxLength = 12;
            this.txtGraph2Name.Name = "txtGraph2Name";
            this.txtGraph2Name.Size = new System.Drawing.Size(121, 20);
            this.txtGraph2Name.TabIndex = 59;
            this.txtGraph2Name.TextChanged += new System.EventHandler(this.txtGraph2Name_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(449, 287);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(76, 13);
            this.label17.TabIndex = 58;
            this.label17.Text = "Graph 2 Name";
            // 
            // lineGraph1
            // 
            this.lineGraph1.BackgroundColor = System.Drawing.Color.Black;
            this.lineGraph1.BufferSize = 1000;
            this.lineGraph1.Graph1Name = "Bandwidth1";
            this.lineGraph1.Graph2Name = "Bandwidth2";
            this.lineGraph1.GridSpacingX = 20;
            this.lineGraph1.GridSpacingY = 37;
            this.lineGraph1.IsBWVisible = false;
            this.lineGraph1.IsLogarithmic = true;
            this.lineGraph1.IsSDVisible = false;
            this.lineGraph1.IsSMAVisible = false;
            this.lineGraph1.Location = new System.Drawing.Point(12, 12);
            this.lineGraph1.LogarithmicIncFactor = 4F;
            this.lineGraph1.LogarithmicMax = 102.4F;
            this.lineGraph1.LogarithmicMin = 0.1F;
            this.lineGraph1.Max = 15F;
            this.lineGraph1.Min = 0F;
            this.lineGraph1.Name = "lineGraph1";
            this.lineGraph1.NoOfGraphs = 2;
            this.lineGraph1.NoOfYGrids = 5;
            this.lineGraph1.PlotLine1Color = System.Drawing.Color.GreenYellow;
            this.lineGraph1.PlotLine1Width = 2F;
            this.lineGraph1.PlotLine2Color = System.Drawing.Color.OrangeRed;
            this.lineGraph1.PlotLine2Width = 2F;
            this.lineGraph1.Size = new System.Drawing.Size(700, 247);
            this.lineGraph1.TabIndex = 0;
            this.lineGraph1.XAxisBackColor = System.Drawing.Color.Transparent;
            this.lineGraph1.XAxisHeader = "Time";
            this.lineGraph1.XGridColor = System.Drawing.Color.SeaGreen;
            this.lineGraph1.XValueDisplayInterval = 10;
            this.lineGraph1.YAxisBackColor = System.Drawing.Color.Transparent;
            this.lineGraph1.YAxisHeader = "B\na\nn\nd\nW\ni\nd\nt\nh\n";
            this.lineGraph1.YGridColor = System.Drawing.Color.SeaGreen;
            this.lineGraph1.YGridInterval = 30;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 622);
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Line Graph";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Graph.LineGraph lineGraph1;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.ComboBox cmbNoOfGraphs;
        private System.Windows.Forms.ComboBox cmbIsLogarithmic;
        private System.Windows.Forms.ComboBox cmbLogMin;
        private System.Windows.Forms.ComboBox cmbPlotLin1Width;
        private System.Windows.Forms.ComboBox cmbGridSpacingX;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblGridSpacingX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBackColor;
        private System.Windows.Forms.ComboBox cmbLogInc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbPlotLine2Width;
        private System.Windows.Forms.ComboBox cmbXValDisplay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtXAxisText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtYAxisText;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbNoOfYGrids;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox cmbYGridInterval;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbBackColors;
        private System.Windows.Forms.ComboBox cmbYGridColor;
        private System.Windows.Forms.ComboBox cmbXGridColor;
        private System.Windows.Forms.ComboBox cmbPlotLine2Color;
        private System.Windows.Forms.ComboBox cmbPlotLine1Color;
        private System.Windows.Forms.TextBox txtxGraph1Name;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtGraph2Name;
        private System.Windows.Forms.Label label17;
    }
}

