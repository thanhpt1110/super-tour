using Super_Tour.Model;
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

namespace Super_Tour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            SUPER_TOUR db = new SUPER_TOUR();
            List < TYPE_PACKAGE>  PACKAGEs= db.TYPE_PACKAGEs.ToList();
            foreach(TYPE_PACKAGE typePackage in PACKAGEs)
            {
                Console.WriteLine("ID: {0} Name: {1}", typePackage.Id_Type_Package.ToString(), typePackage.Name_Type);
            }    
        }
    }
}
