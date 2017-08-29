using System;
using GalaSoft.MvvmLight.Command;

namespace ToyTrainProject.ViewModels
{
    public class ScanWindowViewModel:ViewModelBase
    {
        public ScanWindowViewModel()
        {
            
        }
        private RelayCommand _testClickCommand;
        public RelayCommand TestClickCommand => _testClickCommand ?? (_testClickCommand = new RelayCommand(testMethod));

        private void testMethod()
        {
            throw new NotImplementedException();
        }
    }
}
