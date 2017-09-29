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
        
        private Point _pointToScreen;
        public Point PointToScreen
        {
            get { return _pointToScreen; }
            set { _pointToScreen = value; OnPropertyChanged();}
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

        public ScanWindowViewModel()
        {
        }


        private RelayCommand _takePictureCommand;
        public RelayCommand TakePictureCommand => _takePictureCommand ?? (_takePictureCommand = new RelayCommand(TakePicture));

        private void TakePicture()
        {
            try
            {
                const int PanelWidth = 750;
                const int PanelHeight = 600;

                using (var bitmap = new Bitmap(PanelWidth, PanelHeight))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        // generate the image
                        g.CopyFromScreen(
                            PointToScreen, System.Drawing.Point.Empty, new System.Drawing.Size(PanelWidth, PanelHeight));
                    }

                    SnapshotBitmap = new Bitmap(bitmap);
                }
            }
            catch (Exception exception)
            {
                throw new NotImplementedException();
            }
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