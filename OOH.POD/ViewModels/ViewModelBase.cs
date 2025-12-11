using OOH.POD.DAL;
using OOH.POD.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
    public class SqlliteDataGridViewModel : ViewModelBase
    {
        public ObservableCollection<Compartment> Compartments { get; set; } = new();
        private readonly CompartmentRepository _repository = new();

        private Compartment? _selectedCompartment;
        public Compartment? SelectedCompartment
        {
            get => _selectedCompartment;
            set { _selectedCompartment = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddShipmentCommand { get; }
        public ICommand DeleteCompartmentCommand { get; } // <-- Add this
        private readonly MainViewModel _mainVM;
        public SqlliteDataGridViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            LoadCommand = new RelayCommand(_ => LoadCompartments());
            SaveCommand = new RelayCommand(_ => SaveCompartments());
            AddShipmentCommand = new RelayCommand(_ => AddShipment(), _ => SelectedCompartment != null);
            DeleteCompartmentCommand = new RelayCommand(_ => DeleteCompartment()); // <-- Add this
        }
        public ICommand NavigateToFrmScanBarcodeCommand { get; }

        private void DeleteCompartment()
        {
            if (SelectedCompartment == null) return;
            var result = MessageBox.Show($"Delete compartment '{SelectedCompartment.CompartmentName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _repository.DeleteCompartment(SelectedCompartment.Id);
                Compartments.Remove(SelectedCompartment);
                SelectedCompartment = null;
            }
        }

        private void LoadCompartments()
        {
            Compartments.Clear();
            var list = _repository.GetCompartmentsWithShipments();
            foreach (var c in list)
                Compartments.Add(c);
        }

        private void SaveCompartments()
        {
            foreach (var compartment in Compartments)
            {
                _repository.UpdateCompartment(compartment); // Save compartment properties
                _repository.UpdateShipments(compartment.Id, compartment.Shipments.ToList()); // Save shipments
            }
            MessageBox.Show("Compartments and shipments saved to SQLite.");
        }

        private void AddShipment()
        {
            if (SelectedCompartment == null) return;
            var newShipment = new Shipment
            {
                TrackingNumber = "",
                TimeStored = System.DateTime.Now,
                TimeOverdue = System.DateTime.Now,
                TimeStayed = ""
            };
            SelectedCompartment.Shipments.Add(newShipment);
            OnPropertyChanged(nameof(Compartments));
        }

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
