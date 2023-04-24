using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View.LoginView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;

namespace Super_Tour.ViewModel.LoginViewModel
{
    internal class LoginViewModel: ObservableObject
    {
        private bool _isViewVisible = true;
        public string Username { get; set; }
        public string Password { get; set; }
        private string converted_password;
        public RelayCommand LoginCommand { get;private set; }
        public RelayCommand CommandForgotPassword { get;private set; }
        private SUPER_TOUR db = new SUPER_TOUR();

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            CommandForgotPassword = new RelayCommand(MoveToForgotPass);
        }

        private bool CanLogin(object obj)
        {
            if()
        }

        public void MoveToForgotPass(object a)
        {
            ForgotPass_EmailView view = new ForgotPass_EmailView();
            view.Show();
            Application.Current.MainWindow.Hide();
        }
        public void Login(Object a)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter your username or password", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (checkLogin())
            {
                MessageBox.Show("Login successful");
            }
            else
            {
                MessageBox.Show("Username or password is wrong", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private bool checkLogin()
        {
/*            try
            {*/
                ConvertPassToMD5();
                ACCOUNT a = db.ACCOUNTs.Where(p => p.Username == Username && p.Password == converted_password).SingleOrDefault();
                if (a != null)
                    return true;
/*            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }*/
            return false;
        }
        private void ConvertPassToMD5()
        {
            converted_password = Constant.convertPassToMD5(Password);
        }

    }
}
