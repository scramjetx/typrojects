using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using AForge; //gives IntPoints etc...
using AForge.Controls;
using AForge.Fuzzy;
using AForge.Genetic;
using AForge.Imaging;
using AForge.Imaging.ComplexFilters;
using AForge.Imaging.Formats;
using AForge.Imaging.Filters;
using AForge.Imaging.IPPrototyper;
using AForge.MachineLearning;
using AForge.Math;
using AForge.Math.Geometry;
using AForge.Neuro;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Vision;

namespace MyAForgeToolbox
{
    public class MyTemplateMatching
    {
        //Variables
        public Bitmap RawFrame;
        public Bitmap WorkingImage;
        public Bitmap TemplateImage;

        //Constructor
        public MyTemplateMatching()
        {

        }

        public void doTemplateMatching(Bitmap rawframe, Bitmap templateimage)
        {
            this.RawFrame = rawframe;
            this.TemplateImage = templateimage;
            GrayscaleBT709 filter = new GrayscaleBT709();
            this.RawFrame = filter.Apply(this.RawFrame);
            this.TemplateImage = filter.Apply(this.TemplateImage);
            

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching();
            tm.SimilarityThreshold = 0.85f;
            
            TemplateMatch[] matchings = tm.ProcessImage(this.RawFrame, this.TemplateImage);

            //lock image
            BitmapData data = this.RawFrame.LockBits(
                new Rectangle(0, 0, this.RawFrame.Width, this.RawFrame.Height),
                ImageLockMode.ReadWrite, this.RawFrame.PixelFormat);

            //highlight found matchings
            foreach (TemplateMatch m in matchings)
            {
                Drawing.Rectangle(data, m.Rectangle, Color.White);
                //do something else....
            }
            //unlock image!
            this.RawFrame.UnlockBits(data);


        }


    }
}
