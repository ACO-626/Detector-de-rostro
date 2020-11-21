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
using AForge;
using Emgu.CV;
using Emgu.CV.Structure;


namespace Detector_de_rostro
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }
        FilterInfoCollection filter;
        VideoCaptureDevice device;

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
                comboBox1.Items.Add(device.Name);
            comboBox1.SelectedIndex = 0;
            device = new VideoCaptureDevice();
                            
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[comboBox1.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");

        private void Device_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach(Rectangle rectangle in rectangles)
            {
                using (Graphics graphics=Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Green, 10))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }
            pictureBox1.Image = bitmap;
         

        }

        private void FormPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(device.IsRunning)
            {
                device.Stop();
            }
        }
    }
}
