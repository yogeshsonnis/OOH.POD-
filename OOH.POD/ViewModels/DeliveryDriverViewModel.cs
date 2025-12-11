using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public class DeliveryDriverViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;
        public ObservableCollection<Carrier> Carriers { get; }

        public DeliveryDriverViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            Carriers = new ObservableCollection<Carrier>
            {
                new Carrier { LogoPath = "/Images/oohpod.png" },
                new Carrier {  LogoPath = "/Images/UPS.png" },
                new Carrier {  LogoPath = "/Images/dPdpng.png" },
                new Carrier { LogoPath = "/Images/amazon.png" },
                new Carrier { LogoPath = "/Images/YOdelpng.png" },
                new Carrier { LogoPath = "/Images/GLSpng.png" },
                new Carrier { LogoPath = "/Images/DHLpng.png" },
                new Carrier { LogoPath = "/Images/anpost.png" },
            };
            NavigateToRecipientCommand = new RelayCommand(_ => _mainVM.NavigateTorecipients());

        }
        public ICommand NavigateToRecipientCommand { get; }


    }
}
