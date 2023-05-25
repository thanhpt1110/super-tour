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

namespace Super_Tour.CustomControls
{
    /// <summary>
    /// Interaction logic for AccountPasswordBox.xaml
    /// </summary>
    public partial class AccountPasswordBox : UserControl
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.Password;
            //password.Password (get Password)
        }
        public AccountPasswordBox()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += OnPasswordChanged;
        }
    }
}
