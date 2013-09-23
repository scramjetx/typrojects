using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Graph
{
    [ToolboxItem(true)]
    [ToolboxBitmapAttribute(typeof(LineGraph), "Icon.bmp")]
    public partial class LineGraph : UserControl
    {
        #region Constructor

        public LineGraph()
        {
            InitializeComponent();
            this.pnlMainPlotArea.Controls.Add(this.scrollBar);
            this.scrollBar.Visible = false;
            this.plotArea.BackColor = Color.Black;
            this.plotArea.Width = this.pnlMainPlotArea.Width;
            this.plotArea.Height = this.pnlMainPlotArea.Height;
            this.pnlMainPlotArea.Controls.Add(this.plotArea);
            //Register the plotarea paint and the scroll bar events.
            this.plotArea.Paint += new PaintEventHandler(plotAreaPaint);
            this.scrollBar.Scroll += new ScrollEventHandler(scrollBarScroll);
            this.plotArea.Resize += delegate { this.SetScrollBarProperties(); };
            this.LayoutControls();
            this.Resize += delegate { this.LayoutControls(); };
            this.plotArea.MouseDoubleClick += new MouseEventHandler(plotArea_MouseDoubleClick);
            CalcLogarithmicMax();
        }

        private void CalcLogarithmicMax()
        {
            float temp = LogarithmicMin;
            for (int i = 0; i <NoOfYGrids; i++)
            {
                temp = temp * LogarithmicIncFactor;
            }
            LogarithmicMax = temp;
        }

        void plotArea_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.isGraphUpdate = true;
            this.SetScrollBarProperties();
            this.scrollBar.Value = this.scrollBar.Maximum;
            this.InvalidateAndUpdateScrollBar();         
          
        }

        #endregion

        #region graph variables

        //Creates the buffer size
        Buffer values = new Buffer(50000);

        //Create a plot area
        readonly PlotArea plotArea = new PlotArea();

        //Creates scroll bar
        readonly HScrollBar scrollBar = new HScrollBar();

        //Offset of points to be ploted
        int gridScrollOffset = 0;

        //Offset of the time in the x axis to be ploted.
        int gridXScrollOffset = 0;

        //Delegate to handle cross thread communication in case of layout controls.
        private delegate void LayoutControl();

        //Delegate to handle cross thread communication in case of set sroll bar properties.
        private delegate void ScrollBarDelgate();

        //Contains the total time of the points in case of 60 sec and 10 min
        double totalPointTime = 0;

        //contains the moves of the scroll bar
        int noOfMoves = 0;

        //Contains the last total point time
        double tracker = 0;

        //Points that are used while plotting the points
        PointF p = new PointF();
        PointF p1 = new PointF();
        PointF q = new PointF();
        PointF q1 = new PointF();

        //To check whether the graph should be updated or not
        public bool isGraphUpdate = true;

        #endregion

        #region Properties

        private Color _backColor = Color.Black;
        [Browsable(true), Category("Graph Properties")]
        public Color BackgroundColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                this.plotArea.BackColor = value;
                this.InvalidateAndUpdateScrollBar();
            }
        }

        private Color _xgridColor = Color.SeaGreen;
        [Browsable(true), Category("Graph Properties")]
        public Color XGridColor
        {
            get { return _xgridColor; }
            set
            {
                _xgridColor = value;
                this.InvalidateAndUpdateScrollBar();
            }
        }

        private Color _ygridColor = Color.SeaGreen;
        [Browsable(true), Category("Graph Properties")]
        public Color YGridColor
        {
            get { return _ygridColor; }
            set
            {
                _ygridColor = value;
                this.InvalidateAndUpdateScrollBar();
            }
        }

        private Color _plotLineColor = Color.YellowGreen;
        [Browsable(true), Category("Graph Properties")]
        public Color PlotLine1Color
        {
            get { return _plotLineColor; }
            set
            {
                _plotLineColor = value;
                lblGraph1Colour.Invalidate();
            }
        }

        private Color _plotLine2Color = Color.YellowGreen;
        [Browsable(true), Category("Graph Properties")]
        public Color PlotLine2Color
        {
            get { return _plotLine2Color; }
            set
            {
                _plotLine2Color = value;
                lblGraph2Color.Invalidate();
            }
        }

        private float _plotLineWidth = 2f;
        [Browsable(true), Category("Graph Properties")]
        public float PlotLine1Width
        {
            get { return _plotLineWidth; }
            set
            {
                _plotLineWidth = value;
            }
        }

        private float _plotLine2Width = 2f;
        [Browsable(true), Category("Graph Properties")]
        public float PlotLine2Width
        {
            get { return _plotLine2Width; }
            set
            {
                _plotLine2Width = value;
            }
        }

        private int _bufSize = 50000;
        [Browsable(false), Category("Graph Properties")]
        public int BufferSize
        {
            get { return _bufSize; }
            set
            {
                _bufSize = value;
                values = new Buffer(_bufSize);
            }
        }

        private int _gridSpacingX = 20;
        [Browsable(true), Category("Graph Properties")]
        public int GridSpacingX
        {
            get { return _gridSpacingX; }
            set
            {
                _gridSpacingX = value;
                this.InvalidateAndUpdateScrollBar();
            }
        }

        private Color _xAxisgridColor = Color.Transparent;
        [Browsable(true), Category("Graph Properties")]
        public Color XAxisBackColor
        {
            get { return _xAxisgridColor; }
            set
            {
                _xAxisgridColor = value;
                pnlXAxis.Invalidate();
            }
        }

        private Color _yAxisgridColor = Color.Transparent;
        [Browsable(true), Category("Graph Properties")]
        public Color YAxisBackColor
        {
            get { return _yAxisgridColor; }
            set
            {
                _yAxisgridColor = value;
                pnlYAxis.Invalidate();
            }
        }

        private int _xValueDisplayInterval = 10;
        [Browsable(true), Category("Graph Properties")]
        public int XValueDisplayInterval
        {
            get { return _xValueDisplayInterval; }
            set
            {
                _xValueDisplayInterval = value;
                pnlXAxis.Invalidate();
            }
        }

        private int _gridSpacingY = 20;
        [Browsable(false), Category("Graph Properties")]
        public int GridSpacingY
        {
            get { return _gridSpacingY; }
            set
            {
                _gridSpacingY = value;
                //this.InvalidateAndUpdateScrollBar();
            }
        }

        private int _noOfYGrids = 10;
        [Browsable(true), Category("Graph Properties")]
        public int NoOfYGrids
        {
            get { return _noOfYGrids; }
            set
            {
                _noOfYGrids = value;
                float temp = LogarithmicMin;
                for (int i = 0; i < _noOfYGrids; i++)
                {
                    temp = temp * _logarithmicIncFactor;
                }
                LogarithmicMax = temp;
                this.InvalidateAndUpdateScrollBar();
                pnlYAxis.Invalidate();
            }
        }

        private int _yGridInterval = 10;
        [Browsable(true), Category("Graph Properties")]
        public int YGridInterval
        {
            get { return _yGridInterval; }
            set
            {
                _yGridInterval = value;
                pnlYAxis.Invalidate();
            }
        }

        private bool _isLogarithmic;
        [Browsable(true), Category("Graph Properties")]
        public bool IsLogarithmic
        {
            get { return _isLogarithmic; }
            set
            {
                _isLogarithmic = value;
                pnlYAxis.Invalidate();
                plotArea.Invalidate();
            }
        }

        private string _xAxisLabel;
        [Browsable(true), Category("Graph Properties")]
        public string XAxisHeader
        {
            get { return _xAxisLabel; }
            set
            {
                _xAxisLabel = value;
                lblXaxis.Text = _xAxisLabel;
                lblXaxis.Invalidate();
            }
        }

        private string _yAxisLabel;
        [Browsable(true), Category("Graph Properties")]
        public string YAxisHeader
        {
            get { return _yAxisLabel; }
            set
            {
                if (value.Length > 0)
                {
                    string t = value.Replace("\n", "");
                    value = t;
                }
                char[] temp = value.ToCharArray();
                StringBuilder tempBuilder = new StringBuilder();
                for (int i = 0; i < temp.Length; i++)
                {
                    tempBuilder.Append(temp[i]+"\n");
                }
                lblYAxis.Text = "";
                lblYAxis.Text = tempBuilder.ToString();
                _yAxisLabel = tempBuilder.ToString();
                lblYAxis.AutoSize = true;
                lblYAxis.Location = new Point(22, pnlYAxis.Height / 2 - lblYAxis.Height / 2);
                lblYAxis.Invalidate();
            }
        }

        private float _logarithmicMin;
        [Browsable(true), Category("Graph Properties")]
        public float LogarithmicMin
        {
            get { return _logarithmicMin; }
            set { _logarithmicMin = value;
            float temp = _logarithmicMin;
            for (int i = 0; i < NoOfYGrids; i++)
            {
                temp = temp * LogarithmicIncFactor;
            }
            LogarithmicMax = temp;
            pnlYAxis.Invalidate();
            this.plotArea.Invalidate();
            }
        }

        private float _logarithmicIncFactor=4;
        [Browsable(true), Category("Graph Properties")]
        public float LogarithmicIncFactor
        {
            get { return _logarithmicIncFactor; }
            set
            {
                _logarithmicIncFactor = value;
                float temp = _logarithmicMin;
                for (int i = 0; i < NoOfYGrids; i++)
                {
                    temp = temp * _logarithmicIncFactor;
                }
                LogarithmicMax = temp;
                pnlYAxis.Invalidate();
                this.plotArea.Invalidate();
            }
        }

        private string _graph1Name="Graph1";
        [Browsable(true), Category("Graph Properties")]
        public string Graph1Name
        {
            get { return _graph1Name; }
            set
            {
                _graph1Name = value;
                UpdateGraph1Name();
            }
        }

        private string _graph2Name="Graph2";
        [Browsable(true), Category("Graph Properties")]
        public string Graph2Name
        {
            get { return _graph2Name; }
            set
            {
                _graph2Name = value;
                UpdateGraph2Name();
            }
        }

        private int _noOfGraphs=1;
        [Browsable(true), Category("Graph Properties")]
        public int NoOfGraphs
        {
            get { return _noOfGraphs; }
            set
            {
                _noOfGraphs = value;
                if (_noOfGraphs > 2)
                _noOfGraphs = 2;
                if (_noOfGraphs < 1)
                    _noOfGraphs = 1;
                UpdateGraphNames();
                this.plotArea.Invalidate();
            }
        }
      

        private float _logarithmicMax=0;
        [Browsable(false), Category("Graph Properties")]
        public float LogarithmicMax
        {
            get { return _logarithmicMax; }
            set { _logarithmicMax = value; }
        }


        private float _min = 0;
        [Browsable(false)]
        public float Min
        {
            get { return this._min; }

            set
            {
                this._min = value;
            }
        }

        private float _max = 15;
        [Browsable(false)]
        public float Max
        {
            get { return this._max; }

            set
            {
                this._max = value;

            }
        }
        [Browsable(false)]
        public float DX
        {
            get { return GridSpacingX; }
        }

        private bool _isBWVisible;
        public bool IsBWVisible
        {
            get { return _isBWVisible; }
            set { _isBWVisible = value;
            this.plotArea.Invalidate();
            }
        }

        private bool _isSMAVisible;
        public bool IsSMAVisible
        {
            get { return _isSMAVisible; }
            set { _isSMAVisible = value;
            this.plotArea.Invalidate();
            }
        }

        private bool _isSDVisible;
        public bool IsSDVisible
        {
            get { return _isSDVisible; }
            set { _isSDVisible = value;
            this.plotArea.Invalidate();
            }
        }

        public string RedLabel
        {
            set
            {
                this.lblGraph2.Text = value;
                this.lblGraph2.ForeColor = Color.Black;
            }
        }


        public string YellowGreenLabel
        {
            set
            {
                this.lblGraph1.Text = value;
                this.lblGraph1.ForeColor = Color.Black;
            }
        }

        //new addition by me for minimum graphed value
        private int MinimumY = 0;
        private int MaximumY = 0;

        #endregion

        #region Graph Methods

        //set new Minimum Y value which changes the YgridInterval accordingly
        public void setMinAndMaxY(int min, int max)
        {
            this.MinimumY = min;
            this.MaximumY = max;
            this.YGridInterval = (this.MaximumY-this.MinimumY) / this.NoOfYGrids;
        }

        public int getMinY()
        {
            return this.MinimumY;
        }

        public int getMaxY()
        {
            return this.MaximumY;
        }

        //This function is used to layout all the controls.
        void LayoutControls()
        {
            try
            {
                //Call from cross thread is checked here
                if (this.InvokeRequired)
                {
                    this.Invoke(new LayoutControl(LayoutControls));
                }
                else
                {
                    this.plotArea.Location = new Point(0, 0);
                    this.plotArea.Size = new Size(this.pnlMainPlotArea.Width, this.scrollBar.Visible ? this.pnlMainPlotArea.Height - this.scrollBar.Height : this.pnlMainPlotArea.Height);
                    this.scrollBar.Location = new Point(0, this.plotArea.Height);
                    this.scrollBar.Size = new Size(this.pnlMainPlotArea.Width, this.scrollBar.Height);
                    this.pnlYAxis.Invalidate();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //This function plots grid and the points on the grid
        void plotAreaPaint(object sender, PaintEventArgs e)
        {
            GridSpacingY = this.plotArea.Height / NoOfYGrids;
            this.PaintGraph(e.Graphics);
            this.PaintGrid(e.Graphics);
        }

        //This method paints the grid each time calledl
        private void PaintGrid(Graphics g)
        {
            try
            {
                //For smoothing.
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Color.FromName(XGridColor.Name)))
                {
                    // Draw all visible, vertical gridlines                     
                    for (int i = this.plotArea.Width - gridScrollOffset; i >= 0; i -= GridSpacingX)
                    {
                        g.DrawLine(p, i, 0, i, this.plotArea.Height);

                    }
                }

                //Draw all horizontal lines.
                using (Pen p = new Pen(Color.FromName(YGridColor.Name)))
                {
                    // Draw all visible, horizontal gridlines 
                    for (int i = this.plotArea.Height; i >= 0; i -= GridSpacingY)
                    {
                        g.DrawLine(p, 0, i, this.plotArea.Width, i);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Error
            }
        }

        //This method paints the graph ie points on the graph
        
        void PaintGraph(Graphics g)
        {
            try
            {
                //For smooth drawing.
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                #region general
               
                    float dx = this.DX;
                    float x = this.plotArea.Width;
                    int t = this.values.timeValCount;
                    int i = this.values.PointCount;
                    int pointtracker = 0; 

                    #region Point

                    if (this.values.PointCount == 0)
                        return;

                    if (--t <= 0)
                        return;
                        float distance = this.plotArea.Width;
                        //To determine the distance after the plot area
                        if(!this.scrollBar.Visible)
                            distance -= Convert.ToInt32(totalPointTime) * GridSpacingX;
                        else
                            distance -= Convert.ToInt32(totalPointTime) * GridSpacingX - (((this.scrollBar.Maximum - this.scrollBar.LargeChange) - noOfMoves) * GridSpacingX);

                        //To determine the distance in the plot area.
                        if (!this.scrollBar.Visible)
                            x -= Convert.ToInt32(totalPointTime) * GridSpacingX;
                        else
                            x -= Convert.ToInt32(totalPointTime) * GridSpacingX - (((this.scrollBar.Maximum - this.scrollBar.LargeChange) - noOfMoves) * GridSpacingX);

                        //Assign the x and y points here
                        p.X=x;
                        //Here the ScaleY function is called where the function returns the height in terms of pixels.
                        p.Y=ScaleY(this.values.GetIndex(0, "Point"));
                        //For Graph2
                        if (NoOfGraphs == 2)
                        {
                            p1.X=x;
                            p1.Y = ScaleY(this.values.GetIndex2(0, "Point")) ;//= new PointF(x, ScaleY(this.values.GetIndex2(0, "Point")));
                        }
                     
                    pointtracker =  1;
                    //Continuous looping till all the points are plotted from the buffer.
                    for (; ; )
                    {
                        if (pointtracker == i || i <= 0)
                            break;
                        //Scale X is called here to determine the x point where the point has to be plotted on to the plot area.
                        ScaleX((Convert.ToInt64(this.values.GetTIMEIndex(pointtracker, "PointTime"))), ref x);
                        q.X=x;
                        q.Y=ScaleY(this.values.GetIndex(pointtracker, "Point"));
                        //Actual drawing takes place here
                        using (Pen pen = new Pen(Color.FromName(PlotLine1Color.Name), PlotLine1Width))
                        {
                            g.DrawLine(pen, p, q);
                        }
                        //Here the first point become the previous point
                        p = q;
                        if (NoOfGraphs == 2)
                        {
                            q1.X=x;
                            q1.Y=ScaleY(this.values.GetIndex2(pointtracker, "Point"));
                            using (Pen pen = new Pen(Color.FromName(PlotLine2Color.Name), PlotLine2Width))
                            {
                                g.DrawLine(pen, p1, q1);
                            }
                            p1 = q1;
                        }
                        
                        //Incremented to check whether all the points from the buffer has been plotted.
                        pointtracker++;
                    }
                 
                    #endregion

                #endregion
            }
            catch (Exception ex)
            {
                //Log the error
            }

        }

        //For scaling the x axis point for 60sec and 10 min graph.
        private void ScaleX(float source, ref float x)
        {
            try
            {
                double tempsource = Math.Ceiling(source);
                //Divide the grid to ten equal parts
                float temp = (float)this.GridSpacingX / 10;
                //Differnce is divided by 100 to make it 100 equal parts
                double d = tempsource / 100;
                //The differnce is mutiplid with gridspacing ten equla parts
                double ans = d * temp;
                //Round the abtained result. 
                ans = Math.Round(ans, 2);
                //Add the result to the reference variable for further action.
                x += (float)ans;
            }
            catch (Exception ex)
            {
               //Log the error
            }
           
        }

        //This method scales the point based on the height of the plot area
        float ScaleY(float y)
        {
            try
            {
                //Calculate the height of each grid.
                float eachgrid = this.plotArea.Height / NoOfYGrids;
                //Calculate the extra space if any
                float extraspace = this.plotArea.Height - (eachgrid * NoOfYGrids);
                if (!IsLogarithmic)
                {
                    int h = this.plotArea.Height - 2;
                    //I changed the y value to subtract off the minY so it is scaled from MinY to MaxY range not 0 to MaxY range
                    //the divisor is the total value range of the graph. Then inside the brackets becomes a percentage the y is of that range times the height h
                    float t = h - ((y - this.MinimumY - 0) / ((NoOfYGrids * YGridInterval) - Min)) * h;
                    if (t < 0)//if t exceeds the limit of graph then make it saturated point
                        t = 1;
                    //Return the point in terms of pixels.
                    return t + extraspace;
                }
                else
                {
                    //If the point is 0 then return the height of the plot area which represents a point on the plot area as 0.
                    if (y == 0)
                        return this.plotArea.Height;
                    
                    double ans = y;
                    //Assign the Min and Max value for logarithmic scale.
                    double min = LogarithmicMin;
                    double max = LogarithmicMax;                    
                    if (ans <= min)
                        return this.plotArea.Height - extraspace - 1;
                    //logarthmic formula to calculate Y-point on logarthmic scale
                    float t = (float)((this.plotArea.Height-extraspace) * ((Math.Log10(ans) - Math.Log10(min)) / (Math.Log10(max) - Math.Log10(min))));
                    float ypoint = t; 
                    //if Calculated point is less or equal to 0 then ypoiny is 0
                    if (ypoint < 0)
                        ypoint = 0;

                    //If its more than the plot area than return the height - extra space
                    if (ypoint > (this.plotArea.Height - extraspace))
                        ypoint = (this.plotArea.Height - extraspace);
                    //Return the actual point.
                    return (this.plotArea.Height - ypoint);
                }
            }
            catch (Exception ex)
            {
                //Log the error
            }
            return 0f;
        }

        void scrollBarScroll(object sender, ScrollEventArgs e)
        {
            //to get pos of scroll in different scale
            //int totalp = 0;
            //if(Is60SecGraph)
            //    totalp=Convert.ToInt32(this.totalPointTime - 61);
            //else if(Is10MinGraph)
            //    totalp = Convert.ToInt32(this.totalPointTime - 618);
            //else if(Is30minGraph)
            //    totalp = Convert.ToInt32(this.totalPointTime30min - 1839/3)-5;
            if (e.NewValue < e.OldValue)
            {
                isGraphUpdate = false;
                noOfMoves = e.NewValue;
                this.InvalidateAndUpdateScrollBar();
                return;
            }
            //else if (noOfMoves == totalp)
            //    isGraphUpdate = true;
            this.InvalidateAndUpdateScrollBar();
            //Debug.WriteLine(noOfMoves + ",time:" + totalp);            
        }

        //Called when scroll bar is to be adjusted.
        void SetScrollBarProperties()
        {
            try
            {
                //Call from cross thread is checked here.
                if (this.InvokeRequired)
                {
                    this.Invoke(new ScrollBarDelgate(SetScrollBarProperties));
                }
                else
                {
                    bool scrollBarWasVisible = scrollBar.Visible;
                    //To find no of points the graph is divided.
                    int largeChange = (int)(this.plotArea.Width / this.DX);
                    double time = 0;
                    //Check for 30 min or normal mode
                    time = totalPointTime;
                    this.scrollBar.LargeChange = largeChange;
                    //check to find the no of points exceeding the no of vertical divisions in the graph.
                    //If No
                    if (time < (int)(this.plotArea.Width / this.DX))
                    {
                        this.scrollBar.Visible = false;
                    }
                    //If Yes
                    else
                    {
                        int shift = this.scrollBar.Maximum - (this.scrollBar.Value + this.scrollBar.LargeChange);
                        if (shift < 0) shift = 0;

                        this.scrollBar.Visible = true;
                        this.scrollBar.Minimum = 0;
                        this.scrollBar.Maximum = Convert.ToInt32(totalPointTime);
                        this.scrollBar.LargeChange = largeChange;
                        this.scrollBar.SmallChange = 1;

                        if (!scrollBarWasVisible)
                        {
                            if(this.scrollBar.Maximum - this.scrollBar.LargeChange>=0)
                            this.scrollBar.Value = this.scrollBar.Maximum - this.scrollBar.LargeChange;
                        }
                        else
                        {
                            int value = this.scrollBar.Maximum - shift - this.scrollBar.LargeChange;
                            if (value < 0) value = 0;
                            noOfMoves = value;
                            this.scrollBar.Value = value;
                            this.scrollBar.BringToFront();
                        }
                    }
                    if (scrollBarWasVisible != this.scrollBar.Visible)
                    {
                        this.LayoutControls();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }    

        //This function adds the points to be ploted in to the buffer
        public void Add(float pointVal,float point2Val, string mstrTime)
        {
            //If for the first time since the graph should start from zero the default values as mentioned below are addd.
            if (this.values.PointCount == 0)
            {
                this.values.PointAdd(0);
                this.values.Point2Add(0);
                this.values.TimeValAdd("0");
                this.values.TimeAdd("0");
                //this.values.SmaAdd(0);
                totalPointTime = 0;
                tracker = 0;
            }
            //point value added to point buffer for 60 sec and 10 min graph
            this.values.PointAdd(pointVal);
            this.values.Point2Add(point2Val);
            //this.values.SmaAdd(0);
            //X axis time is added to time value buffer.
            this.values.TimeValAdd(mstrTime);
            //Total time value is collected here.
             totalPointTime+= Convert.ToDouble(mstrTime)/1000;
             if (tracker != Convert.ToInt32(totalPointTime) && tracker < Convert.ToInt32(totalPointTime))
             {
                 if (Convert.ToInt32(totalPointTime) - tracker >= 2)
                 {
                     //To check if the total time may some time jump more than 1 sec if packet duration takes more than a sec
                     double ty = Convert.ToInt32(totalPointTime) - tracker;
                     int count = 1;
                     while (ty > 1)
                     {
                         this.values.TimeAdd((tracker +count).ToString());
                         count++;
                         ty--;
                     }
                 }
                 //To store the previous total time value for reference
                 tracker = Convert.ToInt32(totalPointTime);
                 this.values.TimeAdd(Convert.ToInt32(totalPointTime).ToString());
             }

            gridScrollOffset += (int)DX;
            gridXScrollOffset += (int)DX;
            if (gridScrollOffset > GridSpacingX)
                gridScrollOffset = gridScrollOffset % GridSpacingX;
            if (gridXScrollOffset >= GridSpacingX)
                gridXScrollOffset = gridXScrollOffset % GridSpacingX;
            if(isGraphUpdate)
                this.InvalidateAndUpdateScrollBar();
        }

        //Method is called when the graph as to be redrawn after setting all the points.
        void InvalidateAndUpdateScrollBar()
        {
            this.SetScrollBarProperties();
            this.plotArea.Invalidate();
            this.pnlXAxis.Invalidate();
        }

        //Method is called when need to paint the y axis values
        private void mpanelYAxisPaint(object sender, PaintEventArgs e)
        {
            try
            {
                //Take the graphics object to consideration
                Graphics g = e.Graphics;
                pnlYAxis.BackColor = Color.FromName(YAxisBackColor.Name);
                int l = 0;
                //set the min y value to draw on the graph axis and draw scale from there
                l = this.MinimumY;
                float k = LogarithmicMin;//Assign the minlogarithmic value
                int plen = 0;
                //Calculate the length of each grid
                int temp = plotArea.Height / NoOfYGrids;
                //Font object to paint Y axis
                Font mfYAxisFont = new Font("Arial", 7f, FontStyle.Regular);

                for (int i = 0; i <= NoOfYGrids; i++)
                {
                    //If linear scale
                    if (!IsLogarithmic)
                    {
                        using (Pen p = new Pen(Color.Black))
                        {
                            //For correct alignment of characters to right we are padding it.
                            g.DrawString(l.ToString().PadLeft(l.ToString().Length), mfYAxisFont, Brushes.Black, new PointF(4f, this.plotArea.Height + panelYExtra.Height - plen-7));
                        }
                        plen = plen + temp;
                        //Adding the increment factor
                        l = l + YGridInterval;
                    }
                        //if logarithmic
                    else
                    {
                        using (Pen p = new Pen(Color.Black))
                        {
                            //For correct alignment of characters to right we are padding it.
                            g.DrawString(k.ToString().PadLeft(k.ToString().Length), mfYAxisFont, Brushes.Black, new PointF(0f, this.plotArea.Height + panelYExtra.Height - plen-7));
                        }
                        plen = plen + temp;
                        //mulitply the increment factor
                        k = k * LogarithmicIncFactor;
                    }
                }
                //Dispose the fone object as graphics object are very costly operations.
                mfYAxisFont.Dispose();
            }
            catch (Exception ex)
            {
                //Log the error
            }
        }

        //Method is called to point the x axis values ie time
        private void mpanelXaxisPaint(object sender, PaintEventArgs e)
        {
            try
            {
                int temp = 0;
                //Consider the graphics object of the x axis panel.
                Graphics g = e.Graphics;
                pnlXAxis.BackColor = Color.FromName(XAxisBackColor.Name);
                //The smooting mode is set to Antialias which gives very smoot effect
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int j = 0;
                //If scroll bar visible then calculate the number of moves the scroll bar as scrolled.
                if (!scrollBar.Visible)
                    temp = Convert.ToInt32(totalPointTime) * GridSpacingX;
                else
                    temp = Convert.ToInt32(totalPointTime) * GridSpacingX - (((this.scrollBar.Maximum - this.scrollBar.LargeChange) - noOfMoves) * GridSpacingX);

                if (j < 0) return;
                for (int i = (this.pnlXAxis.Width - temp - this.panelExtra.Width); i <= this.pnlXAxis.Width && j >= 0; i += GridSpacingX, j++)
                {   
                    if (j >= this.values.timeCount)
                        return;
                    //Draw the string by calculating the x and y values.
                    if (Convert.ToInt32(this.values.GetTIMEIndex(j, "Time")) % XValueDisplayInterval == 0)
                    {
                        g.DrawString(this.values.GetTIMEIndex(j, "Time"), new Font("Arial", 8f, FontStyle.Regular), Brushes.Black, new PointF(i-3, this.pnlXAxis.Height / 12));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //Log the error
            }
        }
          
        //Method to clear all the buffers
        public void Clear()
        {
            this.values.ClearBuffer();
            this.plotArea.Invalidate();
            this.pnlXAxis.Invalidate();
            totalPointTime = 0;
            SetScrollBarProperties();
        }

        private void labelRed_Paint(object sender, PaintEventArgs e)
        {
            Point x = new Point(0, this.lblGraph2Color.Height - 2);
            Point y = new Point(this.lblGraph2Color.Width, this.lblGraph2Color.Height - 2);
            using (Pen p = new Pen(Brushes.Red, 2))
            {
                e.Graphics.DrawLine(p, x, y);
            }
        }

        private void LineGraph_Load(object sender, EventArgs e)
        {
            CalcLogarithmicMax();
        }

        private void lblGraph1Colour_Paint(object sender, PaintEventArgs e)
        {
            Point x = new Point(0, this.lblGraph1Colour.Height - 2);
            Point y = new Point(this.lblGraph1Colour.Width, this.lblGraph1Colour.Height - 2);
            using (Pen p = new Pen(PlotLine1Color, 2))
            {
                e.Graphics.DrawLine(p, x, y);
            }
        }

        private void UpdateGraph1Name()
        {
            lblGraph1.Text = Graph1Name;
            lblGraph1.Invalidate();
        }

        private void UpdateGraph2Name()
        {
            lblGraph2.Text = Graph2Name;
            lblGraph2.Invalidate();
        }

        private void UpdateGraphNames()
        {
            if (NoOfGraphs == 1)
            {
                pnlGraph2.Visible = false;
            }
            if (NoOfGraphs == 2)
            {
                pnlGraph2.Visible = true;
            }
        }

        private void lblGraph2Color_Paint(object sender, PaintEventArgs e)
        {
            Point x = new Point(0, this.lblGraph2Color.Height - 2);
            Point y = new Point(this.lblGraph2Color.Width, this.lblGraph2Color.Height - 2);
            using (Pen p = new Pen(PlotLine2Color, 2))
            {
                e.Graphics.DrawLine(p, x, y);
            }
        }


       
        #endregion
 
    }
}
