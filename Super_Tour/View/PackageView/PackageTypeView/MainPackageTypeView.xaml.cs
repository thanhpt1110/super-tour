using Super_Tour.ViewModel;
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

namespace Super_Tour.View
{
    /// <summary>
    /// Interaction logic for MainPackageTypeView.xaml
    /// </summary>
    public partial class MainPackageTypeView : UserControl
    {
        public MainPackageTypeView()
        {
            InitializeComponent();
            this.DataContext = new MainPackageTypeViewModel();
        }
    }
}
