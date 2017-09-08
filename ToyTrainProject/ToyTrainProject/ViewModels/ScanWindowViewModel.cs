using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using ToyTrainProject.Models;

namespace ToyTrainProject.ViewModels
{
    public class ScanWindowViewModel:ViewModelBase
    {
        private DeviceInfo _selectedDevice;
        public DeviceInfo SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        private string _cameraUsbId;
        public string CameraUsbId
        {
            get { return _cameraUsbId; }
            set { _cameraUsbId = value; OnPropertyChanged(); }
        }


        public ScanWindowViewModel()
        {
           
        }

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
            set { _snapshotBitmap = value; OnPropertyChanged();}
        }
        

        private RelayCommand _takeSnapShotCommand;
        public RelayCommand TakeSnapShotCommand => _takeSnapShotCommand ?? (_takeSnapShotCommand = new RelayCommand(TakeSnapShot));


        private void TakeSnapShot()
        {
            SnapshotImageSource = ConvertToImageSource(SnapshotBitmap);
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
