using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ToyTrainProject.ViewModels
{
   public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<ScanWindowViewModel>();      
        }

        public ScanWindowViewModel ScanWindowViewModel => SimpleIoc.Default.GetInstance<ScanWindowViewModel>();
    }
}
