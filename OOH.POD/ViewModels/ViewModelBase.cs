using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class Carrier
    {
        public string Name { get; set; }
        public string LogoPath { get; set; } // Optional: for carrier logo
    }
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
    public class RecipientsPageViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;
        public RecipientsPageViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            NavigateToFrmScanBarcodeCommand = new RelayCommand(_ => _mainVM.NavigateToFrmScanBarcode());
        }
        public ICommand NavigateToFrmScanBarcodeCommand { get; }
    }

    public class FrmScanBarcodePageViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;
        public FrmScanBarcodePageViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            NavigateToFrmEnterBarcodeCommand = new RelayCommand(_ => _mainVM.NavigateToFrmEnterBarcode());
        }
        public ICommand NavigateToFrmEnterBarcodeCommand { get; }
    }

    public class FrmEnterBarcodePageViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainVM;
        public FrmEnterBarcodePageViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            NavigateToGridPageCommand = new RelayCommand(_ => _mainVM.NavigateToGridPage());
        }

        public ICommand NavigateToGridPageCommand { get; }
    }

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
