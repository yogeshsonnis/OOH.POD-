using OOH.POD.Pages;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            HomeVM = new HomeViewModel(this);
            DeliveryDriverVM = new DeliveryDriverViewModel(this);
            RecipientsPageVM = new RecipientsPageViewModel(this);
            FrmScanBarcodePageVM = new FrmScanBarcodePageViewModel(this);
            FrmEnterBarcodePageVM = new FrmEnterBarcodePageViewModel(this);
            SqlliteDataGridVM = new SqlliteDataGridViewModel(this);
            GridPageVM = new GridPageViewModel(this);
            DropOffVM = new DropOffViewModel(this);
            CurrentScreen = HomeVM;
            NavigateBackCommand = new RelayCommand(_ => NavigateBack());
        }

        public ICommand NavigateBackCommand { get; }

        public HomeViewModel HomeVM { get; }

        public GridPageViewModel GridPageVM { get; }
        public DeliveryDriverViewModel DeliveryDriverVM { get; }
        public RecipientsPageViewModel RecipientsPageVM { get; }
        public SqlliteDataGridViewModel SqlliteDataGridVM { get; }
        public FrmScanBarcodePageViewModel FrmScanBarcodePageVM { get; }
        public FrmEnterBarcodePageViewModel FrmEnterBarcodePageVM { get; }
        public DropOffViewModel DropOffVM { get; }

        private ViewModelBase _currentScreen;
        public ViewModelBase CurrentScreen
        {
            get => _currentScreen;
            set
            {
                _currentScreen = value; OnPropertyChanged();
                OnPropertyChanged(nameof(IsOnDropOffScreen));
            }
        }

        public bool IsOnDropOffScreen => CurrentScreen == DropOffVM || CurrentScreen == DeliveryDriverVM || CurrentScreen == RecipientsPageVM || CurrentScreen == FrmScanBarcodePageVM || CurrentScreen == FrmEnterBarcodePageVM || CurrentScreen == GridPageVM || CurrentScreen == SqlliteDataGridVM;
        public void NavigateToDeliveryDriver() => CurrentScreen = DeliveryDriverVM;
        public void NavigateToDropOff() => CurrentScreen = DropOffVM;
        public void NavigateBack()
        {
            if (CurrentScreen == DropOffVM)
            {
                CurrentScreen = HomeVM;
            }
            else if (CurrentScreen == DeliveryDriverVM)
            {
                CurrentScreen = DropOffVM;
            }
            else if (CurrentScreen == RecipientsPageVM)
            {
                CurrentScreen = DeliveryDriverVM;
            }
            else if (CurrentScreen == FrmScanBarcodePageVM)
            {
                CurrentScreen = RecipientsPageVM;
            }
            else if (CurrentScreen == FrmEnterBarcodePageVM)
            {
                CurrentScreen = FrmScanBarcodePageVM;
            }
            else if (CurrentScreen == SqlliteDataGridVM)
            {
                CurrentScreen = FrmEnterBarcodePageVM;
            }
            else if (CurrentScreen == GridPageVM)
            {
                CurrentScreen = SqlliteDataGridVM;
            }
        }

        public void NavigateTorecipients() => CurrentScreen = RecipientsPageVM;
        public void NavigateNextPage() => CurrentScreen = SqlliteDataGridVM;
        public void NavigateToFrmScanBarcode() => CurrentScreen = FrmScanBarcodePageVM;
        public void NavigateToFrmEnterBarcode() => CurrentScreen = FrmEnterBarcodePageVM;

        public void NavigateToGridPage() => CurrentScreen = SqlliteDataGridVM;
    }
}
