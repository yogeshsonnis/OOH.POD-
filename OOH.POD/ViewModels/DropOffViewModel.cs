using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public class DropOffViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;

        public DropOffViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            // PickUpCommand = new RelayCommand(_ => _mainVM.NavigateToPickup());
            DeliveryDriverCommand = new RelayCommand(_ => _mainVM.NavigateToDeliveryDriver());
        }


        public ICommand PickUpCommand { get; }
        public ICommand DeliveryDriverCommand { get; }

    }
}
