using OOH.POD.Models;
using OOH.POD.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public class GridPageViewModel:ViewModelBase
    {
        private readonly MainViewModel _mainVM;
        private readonly JsonStorageService _jsonService;

        private Compartment _selectedCompartment;
        public Compartment SelectedCompartment
        {
            get => _selectedCompartment;
            set { _selectedCompartment = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Compartment> Compartments { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCompartmentCommand { get; }
        public ICommand AddShipmentCommand { get; }

        public GridPageViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            _jsonService = new JsonStorageService();

            Compartments = new ObservableCollection<Compartment>();

            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            AddCompartmentCommand = new RelayCommand(_ => AddCompartment());
            AddShipmentCommand = new RelayCommand(c => AddShipment(c));
        }

        public async Task LoadAsync()
        {
            var list = await _jsonService.LoadAsync();
            Compartments.Clear();

            foreach (var c in list)
            {
                if (c.Shipments == null)
                    c.Shipments = new ObservableCollection<Shipment>();
                else
                    c.Shipments = new ObservableCollection<Shipment>(c.Shipments);

                Compartments.Add(c);
            }
        }

        public async Task SaveAsync()
        {
            await _jsonService.SaveAsync(Compartments.ToList());
        }

        private void AddCompartment()
        {
            var newCompartment = new Compartment
            {
                CompartmentName = "",
                CompartmentSize = "",
                CompartmentType = "",
                IsClosed = false,
                IsLocked = false,
                IsOutOfService = false,
                Shipments = new ObservableCollection<Shipment>()
            };

            Compartments.Add(newCompartment);
            SelectedCompartment = newCompartment;
        }

        private void AddShipment(object parameter)
        {
            if (parameter is Compartment compartment)
            {
                compartment.Shipments.Add(new Shipment
                {
                    TrackingNumber = "",
                    TimeStored = DateTime.Now,
                    TimeOverdue = DateTime.Now.AddDays(1),
                    TimeStayed = "0h"
                });

                // Optional: if you want the inner DataGrid to update automatically
                if (SelectedCompartment != compartment)
                    SelectedCompartment = compartment;
            }
        }

    }
}
