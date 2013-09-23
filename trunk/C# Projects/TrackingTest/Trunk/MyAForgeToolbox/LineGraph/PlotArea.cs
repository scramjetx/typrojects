using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Graph
{
    class PlotArea : Control
    {
        #region Constructor

        public PlotArea()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        #endregion
    }
}
