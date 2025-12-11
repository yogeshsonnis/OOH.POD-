using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
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
}
