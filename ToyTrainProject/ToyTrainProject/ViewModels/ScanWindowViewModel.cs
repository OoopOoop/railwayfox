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

        private RelayCommand _testClickCommand;
        public RelayCommand TestClickCommand => _testClickCommand ?? (_testClickCommand = new RelayCommand(testMethod));

        private void testMethod()
        {
           
        }
        
    }
}
