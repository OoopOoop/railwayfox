using System;
using GalaSoft.MvvmLight.Command;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ToyTrainProject.ViewModels
{
    public class ScanWindowViewModel:ViewModelBase
    {
        public ScanWindowViewModel()
        {
            VideoSourcee = null;
        }
        private RelayCommand _testClickCommand;
        public RelayCommand TestClickCommand => _testClickCommand ?? (_testClickCommand = new RelayCommand(testMethod));

        private void testMethod()
        {
            var test = VideoSourcee;
        }

        private ImageSource _videoSourcee;
        public ImageSource VideoSourcee
        {
            get { return _videoSourcee; }
            set { _videoSourcee = value;
                this.OnPropertyChanged(); }
        }

    }
}
