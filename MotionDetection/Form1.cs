using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Vision.Motion;
using AForge.Video;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        String cameraUrl;
       
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection fic;
        VideoCaptureDevice device;
        MotionDetector motionDetector;
        MJPEGStream videoStream;
        double amountOfMotion = 0;
        double lowerThresholdValue = 0.008;
        double upperThresholdValue = 0.3;

        private void Form1_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionAreaHighlighting());
            lowerThresholdBox.Text = lowerThresholdValue.ToString();
            upperThresholdBox.Text = upperThresholdValue.ToString();
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cameraUrlTextBox.Text.Length == 0)
            {
                cameraUrl = "http://166.165.35.37:80/mjpg/video.mjpg";
                MessageBox.Show("You didn't specify your camera IP address,\nyou will now be watching a random camera");
            }
            else
            {
                cameraUrl = cameraUrlTextBox.Text;
            }
            try
            {
                videoStream = new MJPEGStream(cameraUrl);
                videoSourcePlayer1.VideoSource = new AsyncVideoSource(videoStream);
                videoSourcePlayer1.Start();
                btnStart.Enabled = false; btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //amountOfMotion = 0;
            videoSourcePlayer1.Stop();
            btnStart.Enabled = true; btnStop.Enabled = false;
            valueLabel.Text = "Amount of motion: 0";
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            amountOfMotion = motionDetector.ProcessFrame(image);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lowerThresholdValue <= amountOfMotion && amountOfMotion <= upperThresholdValue)
            {
                //do whatever you want!!
                valueLabel.Text = "Amount of motion: " + amountOfMotion.ToString();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (lowerThresholdBox.Text.Length == 0 || upperThresholdBox.Text.Length == 0)
            {
                MessageBox.Show("You must specify the threshold range");
            }
            else
            {
                try
                {
                    double lowerThreshParsed = double.Parse(lowerThresholdBox.Text);
                    double upperThreshParsed = double.Parse(upperThresholdBox.Text);

                    if (lowerThreshParsed > upperThreshParsed)
                    {
                        MessageBox.Show("Lower threshold can't be bigger than the upper threshold");
                    }
                    else
                    {
                        lowerThresholdValue = lowerThreshParsed;
                        upperThresholdValue = upperThreshParsed;
                    }
                }catch (Exception)
                {
                    MessageBox.Show("You should only enter digits in the threshold range");   
                }
            }
        }
    }
}
