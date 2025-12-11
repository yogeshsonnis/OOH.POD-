using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
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
}
