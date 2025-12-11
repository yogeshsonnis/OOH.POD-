using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;

        public HomeViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            PickUpCommand = new RelayCommand(_ => _mainVM.NavigateToDeliveryDriver());
            DropOffCommand = new RelayCommand(_ => _mainVM.NavigateToDropOff());
        }

        public ICommand PickUpCommand { get; }
        public ICommand DropOffCommand { get; }

    }
}
