//TO DO:
//
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Math;
using AForge.Imaging.ComplexFilters;
using AForge.Imaging;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;

namespace AutoFocusTest
{
    public partial class Form1 : Form
    {
       
        Bitmap sampleImage;
        Grayscale GrayFilter;
        Bitmap GrayImage;
        ResizeBilinear resizeFilter;
        ComplexImage ci;
        Bitmap FFTimage;
        Bitmap FFTstatsImage;
        ImageStatistics stat2;

        int frameCount = 0;

        public Form1()
        {
            InitializeComponent();
            tabControl1.SelectTab(1);
            button2.Focus();


        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sampleImage = (Bitmap)pictureBox1.Image;
            sampleImage = (Bitmap)AForge.Imaging.Image.FromFile("C:\\SwitchBlade\\Software\\SCFF Fixture\\AutoFocusTest\\Media\\CityInFocus.jpg");

            pictureBox1.Image = sampleImage;

            GrayFilter = new Grayscale(0.2125, 0.7154, 0.0721);

            //image must be cropped to correct aspect ratio for filter
            GrayImage = GrayFilter.Apply(sampleImage);
            pictureBox1.Image = GrayImage;

            //CannyEdgeDetector CED = new CannyEdgeDetector(0, 30);
            //CED.ApplyInPlace(GrayImage);
            //pictureBox1.Image = GrayImage;

            resizeFilter = new ResizeBilinear(1024, 1024);
            GrayImage = resizeFilter.Apply(GrayImage);

            ci = ComplexImage.FromBitmap(GrayImage);
            ci.ForwardFourierTransform();

            FFTimage = ci.ToBitmap();
            pictureBox2.Image = FFTimage;

            
            FFTstatsImage = (Bitmap)FFTimage;

            ImageStatistics stat = new ImageStatistics(FFTstatsImage);

            labelWithoutBlackPixels.Text = stat.PixelsCountWithoutBlack.ToString();
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // create video source
            FileVideoSource fileSource = new FileVideoSource("C:\\SwitchBlade\\Software\\SCFF Fixture\\AutoFocusTest\\Media\\focusVidCity.avi");

            // open it
            OpenVideoSource(fileSource);
        }

        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();
            //videoSourcePlayer.Visible = false;

            // reset statistics
            //statIndex = statReady = 0;

            // start timer
            //timer.Start();

            this.Cursor = Cursors.Default;

            
        }

        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {   
            //just to get an idea of the numbers without grabbing smaller image yet
            if(frameCount%10 == 0)
            {
                sampleImage = (Bitmap)image.Clone();
                sampleImage = CropBitmap(image, 100, 100, 128, 128);

                GrayFilter = new Grayscale(0.2125, 0.7154, 0.0721);

                //image must be cropped to correct aspect ratio for filter
                GrayImage = GrayFilter.Apply(sampleImage);

                resizeFilter = new ResizeBilinear(1024, 512);
                GrayImage = resizeFilter.Apply(GrayImage);

                ci = ComplexImage.FromBitmap(GrayImage);
                ci.ForwardFourierTransform();

                FFTimage = ci.ToBitmap();
                //pictureBox3.Image = FFTimage;

                FFTstatsImage = (Bitmap)FFTimage;

                stat2 = new ImageStatistics(FFTstatsImage);

                //displayImage(GrayImage);
               
            }

            frameCount++;

        }

        private void displayImage(Bitmap image)
        {
            pictureBox3.Image = image;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stat2 != null)
            {
                sampleImage.Dispose();
                labelVideoPixelNoBlack2.Text = labelVideoPixelNoBlack2.Text + stat2.PixelsCountWithoutBlack.ToString() + "\n\r";
            }
        }

        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }
    }
}
