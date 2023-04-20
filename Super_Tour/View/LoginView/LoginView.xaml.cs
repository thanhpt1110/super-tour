using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Super_Tour.ViewModel.LoginViewModel;
namespace Super_Tour.View.LoginView
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            //DataContext = new LoginViewModel();
            LoginViewModel loginViewModel = new LoginViewModel();
            Console.WriteLine("Nhap ten dang nhap: ");
            loginViewModel.username_text = "admin";
            //Console.WriteLine("Nhap password: ");
            loginViewModel.password_text = "1234";
            loginViewModel.Login(null);
        }

    }
}
