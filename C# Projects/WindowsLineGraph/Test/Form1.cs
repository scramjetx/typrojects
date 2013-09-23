using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        float[] p = { 2.3f, 3.5f, .9f, .4f, 5f, 21.6f, .4f, 15f, 3.5f, .9f, .4f, 3.6f, 21.6f, 34.3f, 30.6f, .19f, 21.6f, .4f, 15f, 34.3f, 30.6f, .19f, 3.5f, .9f, .4f, 3.6f, 21.6f, 34.3f, 30.6f, 21.6f, };
        float[] p1 = { 1.5f, 34.3f, 30.6f, .19f, 3.5f, .9f, .4f, 3.6f, 21.6f, 34.3f, 30.6f, 21.6f, .4f, 15f, 3.5f, .9f, .4f, 5f, 3.6f, 21.6f, .19f, 21.6f, .4f, 15f, 34.3f, 30.6f, .19f, 3.5f, .9f, .4f, 3.6f, 21.6f, };
        string[] t = { "1045", "1000", "1012", "1200", "1000", "1100", "2000", "1020", "1010", "1345", "1234", "1000", "1500", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1400", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1300", "1000", "1200", "1100", "1000", "1000", "1000", "1000", "1000", "1032", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000", "1000" };
        private void Form1_Load(object sender, EventArgs e)
        {
            FillColors(cmbBackColors);
            FillColors(cmbPlotLine1Color);
            FillColors(cmbPlotLine2Color);
            FillColors(cmbXGridColor);
            FillColors(cmbYGridColor);
            Default();
            
        }

        private void FillColors(ComboBox cmbTemp)
        {
            ArrayList ColorList = new ArrayList();
            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                cmbTemp.Items.Add(c.Name);
            }

        }
        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i > 29)
                i = 0;
            lineGraph1.Add(p[i],p1[i], t[i]);
            i++;
            
        }

        private void cmbGridSpacingX_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.GridSpacingX = Convert.ToInt32(cmbGridSpacingX.Text);
        }

        private void cmbIsLogarithmic_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.IsLogarithmic = Convert.ToBoolean(cmbIsLogarithmic.Text);
            if (Convert.ToBoolean(cmbIsLogarithmic.Text) == true)
            {
                EnableLogControls(true);
                EnableLinControls(false);
            }
            else
            {
                EnableLogControls(false);
                EnableLinControls(true);
            }
        }

        private void EnableLinControls(bool res)
        {
            cmbYGridInterval.Enabled =res;
        }

        private void EnableLogControls(bool res)
        {
            cmbLogInc.Enabled = res;
            cmbLogMin.Enabled = res;
        }

        private void cmbLogInc_SelectedIndexChanged(object sender, EventArgs e)
        {
            float res;
            float.TryParse(cmbLogInc.Text, out res);
            lineGraph1.LogarithmicIncFactor = res;
        }

        private void cmbLogMin_SelectedIndexChanged(object sender, EventArgs e)
        {
             float res;
            float.TryParse(cmbLogMin.Text, out res);
            lineGraph1.LogarithmicMin = res;
        }

        private void cmbNoOfGraphs_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.NoOfGraphs = Convert.ToInt32(cmbNoOfGraphs.Text);
        }

        private void cmbPlotLin1Width_SelectedIndexChanged(object sender, EventArgs e)
        {
            float res;
            float.TryParse(cmbPlotLin1Width.Text, out res);
            lineGraph1.PlotLine1Width = res;
        }

        private void cmbPlotLine2Width_SelectedIndexChanged(object sender, EventArgs e)
        {
            float res;
            float.TryParse(cmbPlotLine2Width.Text, out res);
            lineGraph1.PlotLine2Width = res;
        }

        private void cmbXValDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.XValueDisplayInterval = Convert.ToInt32(cmbXValDisplay.Text);
        }

        private void txtXAxisText_TextChanged(object sender, EventArgs e)
        {
            lineGraph1.XAxisHeader = txtXAxisText.Text;
        }

        private void txtYAxisText_TextChanged(object sender, EventArgs e)
        {
            lineGraph1.YAxisHeader = txtYAxisText.Text;
        }

        private void cmbNoOfYGrids_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.NoOfYGrids = Convert.ToInt32(cmbNoOfYGrids.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void Default()
        {
            cmbBackColors.SelectedIndex = 8;// "Black";
            lineGraph1.BackgroundColor = Color.Black;
            cmbGridSpacingX.SelectedIndex = 15;
            cmbIsLogarithmic.SelectedIndex = 0;
            cmbLogInc.SelectedIndex = 3;
            cmbLogMin.SelectedIndex = 0;
            cmbNoOfGraphs.SelectedIndex = 0;
            cmbXValDisplay.SelectedIndex = 0;
            cmbNoOfYGrids.SelectedIndex = 0;
            cmbPlotLine1Color.Text = "YellowGreen";
            lineGraph1.PlotLine1Color = Color.YellowGreen;
            cmbPlotLine2Color.Text = "Red";
            lineGraph1.PlotLine2Color = Color.Red;
            cmbPlotLin1Width.SelectedIndex = 1;
            cmbPlotLine2Width.SelectedIndex = 1;
            cmbXGridColor.Text = "SeaGreen";
            lineGraph1.XGridColor = Color.SeaGreen;
            cmbYGridColor.Text = "SeaGreen";
            lineGraph1.YGridColor = Color.SeaGreen;
            txtXAxisText.Text = "Time";
            txtYAxisText.Text = "BandWidth";
            cmbYGridInterval.SelectedIndex = 1;
            txtxGraph1Name.Text = "BandWidth1";
            txtGraph2Name.Text = "BandWidth2";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Default();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            lineGraph1.Clear();
            i = 0;
        }

        private void cmbYGridInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.YGridInterval = Convert.ToInt32(cmbYGridInterval.Text);
        }

        private void cmbBackColors_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = Color.FromName(n);
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
            }

        }

        private void cmbBackColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.BackgroundColor = Color.FromName(cmbBackColors.Text);
        }

        private void cmbPlotLine1Color_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.PlotLine1Color = Color.FromName(cmbPlotLine1Color.Text);
        }

        private void cmbPlotLine2Color_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.PlotLine2Color = Color.FromName(cmbPlotLine2Color.Text);
        }

        private void cmbXGridColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.XGridColor = Color.FromName(cmbXGridColor.Text);
        }

        private void cmbYGridColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineGraph1.YGridColor = Color.FromName(cmbYGridColor.Text);
        }

        private void txtxGraph1Name_TextChanged(object sender, EventArgs e)
        {
            lineGraph1.Graph1Name = txtxGraph1Name.Text;
        }

        private void txtGraph2Name_TextChanged(object sender, EventArgs e)
        {
            lineGraph1.Graph2Name = txtGraph2Name.Text;
        }
    }
}
