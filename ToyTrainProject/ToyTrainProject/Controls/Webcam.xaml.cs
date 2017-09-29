using AForge.Video.DirectShow;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using ToyTrainProject.Models;
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
  
        public static readonly DependencyProperty PointToScreenProperty =
            DependencyProperty.Register(
                "PointToScreen",
                typeof(System.Drawing.Point),
                typeof(Webcam),
                new PropertyMetadata(null));


        public System.Drawing.Point PointToScreen
        {
            get => (System.Drawing.Point)this.GetValue(PointToScreenProperty);

             set { SetValue(PointToScreenProperty, value); }
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

        private List<Bitmap> _imagesCollection;
        public List<Bitmap> ImagesCollection
        {
            get { return _imagesCollection; }
            set { _imagesCollection = value; }
        }
        
        private static void InitVideoDevice(string cameraString, Webcam host)
        {
            if (string.IsNullOrEmpty(cameraString))
            {
                StopVideoDevice();
                return;
            }

            videoDevice = new VideoCaptureDevice(cameraString) { DesiredFrameSize = new System.Drawing.Size(640, 480) };
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

        private void Webcam_OnLoaded(object sender, RoutedEventArgs e)
        {
            PointToScreen = host.VideoPlayer.PointToScreen(
                new System.Drawing.Point(host.VideoPlayer.ClientRectangle.X, host.VideoPlayer.ClientRectangle.Y));
        }
    }
}