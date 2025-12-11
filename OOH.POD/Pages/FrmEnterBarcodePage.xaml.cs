using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OOH.POD.Pages
{
    /// <summary>
    /// Interaction logic for FrmEnterBarcode.xaml
    /// </summary>
    public partial class FrmEnterBarcode : UserControl
    {
        public FrmEnterBarcode()
        {
            InitializeComponent();
        }
        private void Key_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            BarCodeTextBox.Text += btn.Content.ToString();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (BarCodeTextBox.Text.Length > 0)
                BarCodeTextBox.Text = BarCodeTextBox.Text.Substring(0, BarCodeTextBox.Text.Length - 1);
        }
    }
}
