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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection fic;
        VideoCaptureDevice device;
        MotionDetector motionDetector;
        double amountOfMotion = 0;
        double lowerThresholdValue = 0.08;
        double upperThresholdValue = 0.3;

        private void Form1_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionAreaHighlighting());
            fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo item in fic)
            {
                comboBoxDevice.Items.Add(item.ToString());
            }
            comboBoxDevice.SelectedIndex = 0;
            lowerThresholdBox.Text = lowerThresholdValue.ToString();
            upperThresholdBox.Text = upperThresholdValue.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(fic[comboBoxDevice.SelectedIndex].MonikerString);
            videoSourcePlayer1.VideoSource = device;
            videoSourcePlayer1.Start();
            btnStart.Enabled = false; btnStop.Enabled=true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            videoSourcePlayer1.Stop();
            amountOfMotion = 0;
            btnStart.Enabled = true; btnStop.Enabled = false;
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            amountOfMotion = motionDetector.ProcessFrame(image);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lowerThresholdValue <= amountOfMotion && amountOfMotion <= upperThresholdValue)
                valueLabel.Text = "Amount of motion: " + amountOfMotion.ToString();
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
                    MessageBox.Show("You must only enter digits in the threshold range");   
                }
            }
        }
    }
}
