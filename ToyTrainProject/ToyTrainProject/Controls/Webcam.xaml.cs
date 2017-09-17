using AForge.Video.DirectShow;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ToyTrainProject.Models;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace ToyTrainProject.Controls
{
    public partial class Webcam : UserControl
    {
        private static List<DeviceInfo> availableCameraSources;
        private static Webcam host;
        private static VideoCaptureDevice videoDevice;

        public static readonly DependencyProperty SnapshotBitmapProperty =
            DependencyProperty.Register(
                "SnapshotBitmap",
                typeof(Bitmap),
                typeof(Webcam),
                new PropertyMetadata(SnapshotBitmapPropertyChangedCallback));

        public readonly DependencyProperty TakePictureCommandProperty =
            DependencyProperty.Register(
                "TakePictureCommand",
                typeof(ICommand),
                typeof(Webcam),
                new PropertyMetadata(null));

        public readonly DependencyProperty StartMultipleSnapshotProperty =
            DependencyProperty.Register(
                "StartMultipleSnapshot",
                typeof(ICommand),
                typeof(Webcam),
                new PropertyMetadata(null));

        public static readonly DependencyProperty AvailableCameraSourcesProperty =
            DependencyProperty.Register(
                "AvailableCameraSources",
                typeof(IList<DeviceInfo>),
                typeof(Webcam),
                new PropertyMetadata(GetCameraSources()));

        public static readonly DependencyProperty CameraIdProperty =
            DependencyProperty.Register(
                "CameraId",
                typeof(string),
                typeof(Webcam),
                new PropertyMetadata(string.Empty, OnCameraValueChanged, OnCameraCoherceValueChanged));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                "SelectedTime",
                typeof(string),
                typeof(Webcam),
                new PropertyMetadata(string.Empty));

        public string SelectedTime
        {
            get
            {
                return (string)this.GetValue(SelectedTimeProperty);
            }

            set { SetValue(SelectedTimeProperty, value); }
        }

        public ICommand StartMultipleSnapshot
        {
            get { return (ICommand)this.GetValue(StartMultipleSnapshotProperty); }
            set { this.SetValue(StartMultipleSnapshotProperty, value); }
        }

        public ICommand TakePictureCommand
        {
            get
            {
                return (ICommand)this.GetValue(TakePictureCommandProperty);
            }

            set { this.SetValue(TakePictureCommandProperty, value); }
        }

        public Bitmap SnapshotBitmap
        {
            get
            {
                return (Bitmap)this.GetValue(SnapshotBitmapProperty);
            }

            set { SetValue(SnapshotBitmapProperty, value); }
        }

        public IList<FilterInfo> AvailableCameraSources
        {
            get
            {
                return (IList<FilterInfo>)this.GetValue(AvailableCameraSourcesProperty);
            }

            set { SetValue(AvailableCameraSourcesProperty, value); }
        }

        public string CameraId
        {
            get
            {
                return (string)this.GetValue(CameraIdProperty);
            }

            set { SetValue(CameraIdProperty, value); }
        }

        public Webcam()
        {
            this.InitializeComponent();
            TakePictureCommand = new RelayCommand(TakePicture);
            StartMultipleSnapshot = new RelayCommand(TakeMultiplePicture);
            ImagesCollection = new List<Bitmap>();
        }

        private static object OnCameraCoherceValueChanged(DependencyObject dependencyObject, object basevalue)
        {
            host = (Webcam)dependencyObject;
            if (string.IsNullOrEmpty(basevalue.ToString()))
            {
                return null;
            }

            if (availableCameraSources.FirstOrDefault(c => c.UsbString.Equals(basevalue.ToString())) == null)
            {
                if (availableCameraSources != null)
                {
                    var firstOrDefault = availableCameraSources.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        return firstOrDefault.UsbString;
                    }
                }
            }

            return basevalue;
        }

        private static void OnCameraValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            host = (Webcam)dependencyObject;
            if (e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                StopVideoDevice();
                return;
            }

            if (e.NewValue != e.OldValue)
            {
                if (videoDevice != null)
                {
                    StopVideoDevice();
                    InitVideoDevice(e.NewValue.ToString(), (Webcam)dependencyObject);
                }
                else
                {
                    InitVideoDevice(e.NewValue.ToString(), (Webcam)dependencyObject);
                }
            }
        }

        //private void TakePicture()
        //{
        //    try
        //    {

        //        System.Drawing.Point pnlPoint =
        //            host.VideoPlayer.PointToScreen(
        //                new System.Drawing.Point(host.VideoPlayer.ClientRectangle.X, host.VideoPlayer.ClientRectangle.Y)); // get the position of the VideoPlayer
        //        using (var bitmap = new Bitmap(PanelWidth, PanelHeight))
        //        {
        //            using (var g = Graphics.FromImage(bitmap))
        //            {
        //                // generate the image
        //                g.CopyFromScreen(
        //                    pnlPoint, System.Drawing.Point.Empty, new System.Drawing.Size(PanelWidth, PanelHeight));
        //            }

        //            SnapshotBitmap = new Bitmap(bitmap);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //}

        private void TakePicture()
        {
            try
            {
                var playerPoint = new System.Drawing.Point();

                System.Windows.Point point = this.VideoSourceWindowsFormsHost.PointToScreen(new System.Windows.Point(0, 0));

                playerPoint = new System.Drawing.Point((int)point.X, (int)point.Y);


                using (var bitmap = new Bitmap(VideoPlayer.Width, VideoPlayer.Height))
                {
                    using (var graphicsFromImage = Graphics.FromImage(bitmap))
                    {
                        graphicsFromImage.CopyFromScreen(playerPoint.X, playerPoint.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                    }

                    this.SnapshotBitmap = new Bitmap(bitmap);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private List<Bitmap> _imagesCollection;

        public List<Bitmap> ImagesCollection
        {
            get { return _imagesCollection; }
            set { _imagesCollection = value; }
        }

        private void TakeMultiplePicture()
        {
            int miliseconds = Convert.ToInt16(SelectedTime) * 1000;
            SetTimer(miliseconds);
        }

        private void SetTimer(int interval)
        {
            var _timer = new Timer();

            _timer.Interval = interval;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Enabled = true;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            TakePicture();
            ImagesCollection.Add(SnapshotBitmap);
        }

        private static void InitVideoDevice(string cameraString, Webcam host)
        {
            if (string.IsNullOrEmpty(cameraString))
            {
                StopVideoDevice();
                return;
            }

            videoDevice = new VideoCaptureDevice(cameraString);
            host.VideoPlayer.VideoSource = videoDevice;
            host.VideoPlayer.Start();
        }

        private static void StopVideoDevice()
        {
            if (videoDevice == null)
            {
                return;
            }

            videoDevice.SignalToStop();
            videoDevice.WaitForStop();
            videoDevice.Stop();
            videoDevice = null;
        }

        private static IList<DeviceInfo> GetCameraSources()
        {
            availableCameraSources = new List<DeviceInfo>();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in videoDevices)
            {
                availableCameraSources.Add(
                    new DeviceInfo { DisplayName = filterInfo.Name, UsbString = filterInfo.MonikerString });
            }
            return availableCameraSources;
        }

        private static void SnapshotBitmapPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
        }
    }
}