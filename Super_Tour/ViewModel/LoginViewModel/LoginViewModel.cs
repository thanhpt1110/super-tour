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
        public string username_text;
        public string password_text;
        private string converted_password;
        public RelayCommand CommandLogin;
        public RelayCommand CommandForgotPassword;
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
            CommandLogin = new RelayCommand(Login,CanLogin);
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
            if (string.IsNullOrEmpty(username_text) || string.IsNullOrEmpty(password_text))
            {
                MessageBox.Show("Please enter your username or password", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (checkLogin())
            {
                MainView view = new MainView();
                Application.Current.MainWindow.Hide();
                view.Show();
            }
            else
            {
                MessageBox.Show("Username or password is wrong", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private bool checkLogin()
        {
            ConvertPassToMD5();
            return db.ACCOUNTs.Where(p => p.Username == username_text && p.Password == converted_password).SingleOrDefault() != null;
        }
        private void ConvertPassToMD5()
        {
            converted_password = Constant.convertPassToMD5(password_text);
        }

    }
}
