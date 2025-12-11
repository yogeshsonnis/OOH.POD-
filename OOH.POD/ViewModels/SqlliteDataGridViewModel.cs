using OOH.POD.DAL;
using OOH.POD.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
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
}
