using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOH.POD.ViewModels
{
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
}
