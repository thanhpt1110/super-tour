using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class MainLoginViewModel: ObservableObject
    {
        private bool _isViewVisible = true;
        private string _username;
        private string _password;
        public string Username 
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password 
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
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

        public MainLoginViewModel()
        {
            
            LoginCommand = new RelayCommand(Login);
            CommandForgotPassword = new RelayCommand(MoveToForgotPass);
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
                IsViewVisible = false;
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
