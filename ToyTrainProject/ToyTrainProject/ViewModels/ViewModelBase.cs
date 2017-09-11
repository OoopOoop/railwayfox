using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToyTrainProject.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public INavigationService _navigationService;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        
        private RelayCommand<string> _navigateToCommand;
        public RelayCommand<string> NavigateToCommand => _navigateToCommand ?? (_navigateToCommand = new RelayCommand<string>(
            (string commandParameter) =>
            {
                _navigationService.NavigateTo(commandParameter);
            }));
    }
}
