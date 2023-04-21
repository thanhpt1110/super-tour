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
using System.Windows.Shapes;
using Super_Tour.ViewModel.TourViewModel;
namespace Super_Tour.View.Tour
{
    /// <summary>
    /// Interaction logic for UpdateTourView.xaml
    /// </summary>
    public partial class UpdateTourView : Window
    {
        public UpdateTourView()
        {
            InitializeComponent();
            DataContext = new UpdateTourView();
        }
    }
}
