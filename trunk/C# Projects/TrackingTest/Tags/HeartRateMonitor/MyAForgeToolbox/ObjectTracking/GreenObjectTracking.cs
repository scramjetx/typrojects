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
    public class GreenObjectTracking
    {
        public Bitmap RawFrame;
        public Bitmap WorkingImage;

        //Constructor
        public GreenObjectTracking()
        {

        }


        public void doGreenObjectTracking(Bitmap rawframe)
        {
            RawFrame = rawframe;
            YCbCrExtractChannel filter = new YCbCrExtractChannel(YCbCr.CrIndex);
            Bitmap crChannelImage = filter.Apply(RawFrame);

            Threshold t = new Threshold(97);

            WorkingImage = t.Apply(crChannelImage);
            Invert invertFilter = new Invert();

            //image must have blobs white on black background to filter them
            invertFilter.ApplyInPlace(WorkingImage);


            BlobCounterBase bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinWidth = 15;
            bc.MinHeight = 15;
            //bc.MaxHeight = 30;
            //bc.MaxWidth = 30;
            
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(WorkingImage);
            Blob[] blobs = bc.GetObjectsInformation();

            if (blobs.Length > 0)
            {
                // create convex hull searching algorithm
                GrahamConvexHull hullFinder = new GrahamConvexHull();

                //draw box around tracked object
                //lock whole image to draw on it. Must be raw frame or color frame
                BitmapData data = RawFrame.LockBits(
                    new Rectangle(0, 0, RawFrame.Width, RawFrame.Height),
                    ImageLockMode.ReadWrite, RawFrame.PixelFormat);

                //process each blob
                foreach (Blob blob in blobs)
                {
                    List<IntPoint> leftPoints, rightPoints, edgePoints;
                    edgePoints = new List<IntPoint>();
                    //get blobs edge points
                    bc.GetBlobsLeftAndRightEdges(blob, out leftPoints, out rightPoints);

                    edgePoints.AddRange(leftPoints);
                    edgePoints.AddRange(rightPoints);

                    // blob's convex hull
                    List<IntPoint> hull = hullFinder.FindHull(edgePoints);

                    Rectangle r = new Rectangle(blob.Rectangle.X, blob.Rectangle.Y, blob.Rectangle.Width, blob.Rectangle.Height);

                    //draw an outline on the object
                    Drawing.Polygon(data, hull, Color.Red);

                    //draw a rectangle around the object
                    Drawing.Rectangle(data, r, Color.Crimson);

                    //draw a second one a little bigger
                    r.Inflate(10, 10);
                    Drawing.Rectangle(data, r, Color.Cornsilk);
                }

                //must remember to unlock image!!
                RawFrame.UnlockBits(data);

                //for (int i = 0; i < blobs.Length; i++)
                //{

                //    //Debug.WriteLine(blobs[i].Rectangle.Width.ToString() + "   " + blobs[i].Rectangle.Height.ToString());
                //    Debug.WriteLine(blobs[i].Rectangle.Location.X.ToString());// + "" + blobs[i].Rectangle.Location.Y.ToString());
                //    //Debug.WriteLine("");
                //}
            }

        }
        
            

    }
}
