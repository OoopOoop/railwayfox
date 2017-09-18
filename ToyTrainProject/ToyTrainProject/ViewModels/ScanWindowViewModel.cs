using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using ToyTrainProject.Models;

namespace ToyTrainProject.ViewModels
{
    public class ScanWindowViewModel : ViewModelBase
    {
        private DeviceInfo _selectedDevice;

        public DeviceInfo SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        private string _responseText;

        public string ResponseText
        {
            get { return _responseText; }
            set { _responseText = value; OnPropertyChanged(); }
        }

        private RelayCommand _submitSnapShotCommand;
        public RelayCommand SubmitSnapShotCommand => _submitSnapShotCommand ?? (_submitSnapShotCommand = new RelayCommand(SubmitSnapShot));

        private ImageSource _snapshotImageSource;

        public ImageSource SnapshotImageSource
        {
            get { return _snapshotImageSource; }
            set { _snapshotImageSource = value; OnPropertyChanged(); }
        }

        private Bitmap _snapshotBitmap;

        public Bitmap SnapshotBitmap
        {
            get { return _snapshotBitmap; }

            set
            {
                if (SetField(ref _snapshotBitmap, value, "SnapshotBitmap"))
                {
                    SnapshotImageSource = ConvertToImageSource(SnapshotBitmap);
                }
            }
        }

        //private double _imageCaptureWidth;
        //public double ImageCaptureWidth
        //{
        //    get { return _imageCaptureWidth; }
        //    set { _imageCaptureWidth = value; OnPropertyChanged(); }
        //}

        //private double _imageCaptureHeight;
        //public double ImageCaptureHeight
        //{
        //    get { return _imageCaptureHeight; }
        //    set { _imageCaptureHeight = value; OnPropertyChanged(); }
        //}

        public ScanWindowViewModel()
        {
          
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private async void SubmitSnapShot()
        {
            var content = await new ComputerVision_AnalyseImage().callService(SnapshotBitmap);
            dynamic parsedJson = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
            ResponseText = Newtonsoft.Json.JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
        }

        public static ImageSource ConvertToImageSource(Bitmap bitmap)
        {
            var imageSourceConverter = new ImageSourceConverter();
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                var snapshotBytes = memoryStream.ToArray();
                return (ImageSource)imageSourceConverter.ConvertFrom(snapshotBytes); ;
            }
        }
    }
}