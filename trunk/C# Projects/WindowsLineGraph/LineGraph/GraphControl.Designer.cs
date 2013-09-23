namespace Graph
{
    partial class LineGraph
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlYAxis = new System.Windows.Forms.Panel();
            this.lblYAxis = new System.Windows.Forms.Label();
            this.pnlXAxis = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlGraph1 = new System.Windows.Forms.Panel();
            this.lblGraph1 = new System.Windows.Forms.Label();
            this.lblGraph1Colour = new System.Windows.Forms.Label();
            this.pnlGraph2 = new System.Windows.Forms.Panel();
            this.lblGraph2 = new System.Windows.Forms.Label();
            this.lblGraph2Color = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblXaxis = new System.Windows.Forms.Label();
            this.panelExtra = new System.Windows.Forms.Panel();
            this.panelYExtra = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlMainPlotArea = new System.Windows.Forms.Panel();
            this.pnlYAxis.SuspendLayout();
            this.pnlXAxis.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlGraph1.SuspendLayout();
            this.pnlGraph2.SuspendLayout();
            this.panelExtra.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlYAxis
            // 
            this.pnlYAxis.BackColor = System.Drawing.Color.Transparent;
            this.pnlYAxis.Controls.Add(this.lblYAxis);
            this.pnlYAxis.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlYAxis.Location = new System.Drawing.Point(0, 0);
            this.pnlYAxis.Name = "pnlYAxis";
            this.pnlYAxis.Size = new System.Drawing.Size(36, 214);
            this.pnlYAxis.TabIndex = 0;
            this.pnlYAxis.Paint += new System.Windows.Forms.PaintEventHandler(this.mpanelYAxisPaint);
            // 
            // lblYAxis
            // 
            this.lblYAxis.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblYAxis.AutoSize = true;
            this.lblYAxis.BackColor = System.Drawing.Color.Transparent;
            this.lblYAxis.ForeColor = System.Drawing.Color.Black;
            this.lblYAxis.Location = new System.Drawing.Point(20, 90);
            this.lblYAxis.Name = "lblYAxis";
            this.lblYAxis.Size = new System.Drawing.Size(13, 65);
            this.lblYAxis.TabIndex = 1;
            this.lblYAxis.Text = "l\r\na\r\nb\r\ne\r\nl";
            // 
            // pnlXAxis
            // 
            this.pnlXAxis.BackColor = System.Drawing.Color.Transparent;
            this.pnlXAxis.Controls.Add(this.panel2);
            this.pnlXAxis.Controls.Add(this.panel3);
            this.pnlXAxis.Controls.Add(this.lblXaxis);
            this.pnlXAxis.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlXAxis.Location = new System.Drawing.Point(0, 214);
            this.pnlXAxis.Name = "pnlXAxis";
            this.pnlXAxis.Size = new System.Drawing.Size(757, 40);
            this.pnlXAxis.TabIndex = 1;
            this.pnlXAxis.Paint += new System.Windows.Forms.PaintEventHandler(this.mpanelXaxisPaint);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnlGraph1);
            this.panel2.Controls.Add(this.pnlGraph2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(515, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(206, 40);
            this.panel2.TabIndex = 9;
            // 
            // pnlGraph1
            // 
            this.pnlGraph1.Controls.Add(this.lblGraph1);
            this.pnlGraph1.Controls.Add(this.lblGraph1Colour);
            this.pnlGraph1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlGraph1.Location = new System.Drawing.Point(2, 0);
            this.pnlGraph1.Name = "pnlGraph1";
            this.pnlGraph1.Size = new System.Drawing.Size(102, 40);
            this.pnlGraph1.TabIndex = 20;
            // 
            // lblGraph1
            // 
            this.lblGraph1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGraph1.AutoSize = true;
            this.lblGraph1.BackColor = System.Drawing.Color.Transparent;
            this.lblGraph1.Location = new System.Drawing.Point(36, 24);
            this.lblGraph1.Name = "lblGraph1";
            this.lblGraph1.Size = new System.Drawing.Size(42, 13);
            this.lblGraph1.TabIndex = 18;
            this.lblGraph1.Text = "Graph1";
            // 
            // lblGraph1Colour
            // 
            this.lblGraph1Colour.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGraph1Colour.AutoSize = true;
            this.lblGraph1Colour.BackColor = System.Drawing.Color.Transparent;
            this.lblGraph1Colour.Location = new System.Drawing.Point(7, 21);
            this.lblGraph1Colour.Name = "lblGraph1Colour";
            this.lblGraph1Colour.Size = new System.Drawing.Size(28, 13);
            this.lblGraph1Colour.TabIndex = 17;
            this.lblGraph1Colour.Text = "       ";
            this.lblGraph1Colour.Paint += new System.Windows.Forms.PaintEventHandler(this.lblGraph1Colour_Paint);
            // 
            // pnlGraph2
            // 
            this.pnlGraph2.Controls.Add(this.lblGraph2);
            this.pnlGraph2.Controls.Add(this.lblGraph2Color);
            this.pnlGraph2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlGraph2.Location = new System.Drawing.Point(104, 0);
            this.pnlGraph2.Name = "pnlGraph2";
            this.pnlGraph2.Size = new System.Drawing.Size(102, 40);
            this.pnlGraph2.TabIndex = 19;
            // 
            // lblGraph2
            // 
            this.lblGraph2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGraph2.AutoSize = true;
            this.lblGraph2.BackColor = System.Drawing.Color.Transparent;
            this.lblGraph2.Location = new System.Drawing.Point(33, 24);
            this.lblGraph2.Name = "lblGraph2";
            this.lblGraph2.Size = new System.Drawing.Size(42, 13);
            this.lblGraph2.TabIndex = 14;
            this.lblGraph2.Text = "Graph2";
            // 
            // lblGraph2Color
            // 
            this.lblGraph2Color.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGraph2Color.AutoSize = true;
            this.lblGraph2Color.BackColor = System.Drawing.Color.Transparent;
            this.lblGraph2Color.Location = new System.Drawing.Point(10, 21);
            this.lblGraph2Color.Name = "lblGraph2Color";
            this.lblGraph2Color.Size = new System.Drawing.Size(28, 13);
            this.lblGraph2Color.TabIndex = 13;
            this.lblGraph2Color.Text = "       ";
            this.lblGraph2Color.Paint += new System.Windows.Forms.PaintEventHandler(this.lblGraph2Color_Paint);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(721, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(36, 40);
            this.panel3.TabIndex = 8;
            // 
            // lblXaxis
            // 
            this.lblXaxis.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblXaxis.AutoSize = true;
            this.lblXaxis.BackColor = System.Drawing.Color.Transparent;
            this.lblXaxis.ForeColor = System.Drawing.Color.Black;
            this.lblXaxis.Location = new System.Drawing.Point(351, 26);
            this.lblXaxis.Name = "lblXaxis";
            this.lblXaxis.Size = new System.Drawing.Size(35, 13);
            this.lblXaxis.TabIndex = 0;
            this.lblXaxis.Text = "label1";
            // 
            // panelExtra
            // 
            this.panelExtra.BackColor = System.Drawing.Color.Transparent;
            this.panelExtra.Controls.Add(this.pnlYAxis);
            this.panelExtra.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelExtra.Location = new System.Drawing.Point(721, 0);
            this.panelExtra.Name = "panelExtra";
            this.panelExtra.Size = new System.Drawing.Size(36, 214);
            this.panelExtra.TabIndex = 3;
            // 
            // panelYExtra
            // 
            this.panelYExtra.BackColor = System.Drawing.Color.Transparent;
            this.panelYExtra.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelYExtra.Location = new System.Drawing.Point(0, 0);
            this.panelYExtra.Name = "panelYExtra";
            this.panelYExtra.Size = new System.Drawing.Size(721, 10);
            this.panelYExtra.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 204);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 10);
            this.panel1.TabIndex = 0;
            // 
            // pnlMainPlotArea
            // 
            this.pnlMainPlotArea.BackColor = System.Drawing.Color.Transparent;
            this.pnlMainPlotArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainPlotArea.Location = new System.Drawing.Point(0, 10);
            this.pnlMainPlotArea.Name = "pnlMainPlotArea";
            this.pnlMainPlotArea.Size = new System.Drawing.Size(721, 194);
            this.pnlMainPlotArea.TabIndex = 9;
            // 
            // LineGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnlMainPlotArea);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelYExtra);
            this.Controls.Add(this.panelExtra);
            this.Controls.Add(this.pnlXAxis);
            this.Name = "LineGraph";
            this.Size = new System.Drawing.Size(757, 254);
            this.Load += new System.EventHandler(this.LineGraph_Load);
            this.pnlYAxis.ResumeLayout(false);
            this.pnlYAxis.PerformLayout();
            this.pnlXAxis.ResumeLayout(false);
            this.pnlXAxis.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pnlGraph1.ResumeLayout(false);
            this.pnlGraph1.PerformLayout();
            this.pnlGraph2.ResumeLayout(false);
            this.pnlGraph2.PerformLayout();
            this.panelExtra.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlYAxis;
        private System.Windows.Forms.Panel pnlXAxis;
        private System.Windows.Forms.Label lblXaxis;
        private System.Windows.Forms.Label lblYAxis;
        private System.Windows.Forms.Panel panelExtra;
        private System.Windows.Forms.Panel panelYExtra;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlMainPlotArea;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblGraph1;
        private System.Windows.Forms.Label lblGraph1Colour;
        private System.Windows.Forms.Label lblGraph2;
        private System.Windows.Forms.Label lblGraph2Color;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlGraph1;
        private System.Windows.Forms.Panel pnlGraph2;
    }
}
