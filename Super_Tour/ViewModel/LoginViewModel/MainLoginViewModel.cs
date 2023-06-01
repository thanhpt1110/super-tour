using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class MainLoginViewModel: ObservableObject
    {
        #region Declare variable
        private bool _isInternetConnectionError = false;
        private bool _isViewVisible = true;
        private string _username = null;
        private string _password = null;
        private bool executeButton = true;
        private string converted_password;
        private SUPER_TOUR db = null;
        private MainView _mainView = null;
        private MainViewModel _mainViewModel = null;
        #endregion

        #region Declare binding
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

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        #endregion

        #region Command
        public RelayCommand LoginCommand { get;private set; }
        public RelayCommand CommandForgotPassword { get;private set; }
        #endregion

        #region Constructor 
        public MainLoginViewModel()
        {
            db = SUPER_TOUR.db;
            LoginCommand = new RelayCommand(Login, canExecute);
            CommandForgotPassword = new RelayCommand(MoveToForgotPass);
        }
        #endregion

        #region Forgot password
        public void MoveToForgotPass(object a)
        {
            ForgotPass_EmailView view = new ForgotPass_EmailView();
            view.Show();
            Application.Current.MainWindow.Hide();
        }
        #endregion

        #region Login event
        public bool canExecute(object a)
        {
            return executeButton;
        }

        public async void Login(object a)
        {
            if(string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                MyMessageBox.ShowDialog("Please enter username and password.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            executeButton = false;
            try
            {
                // Thực hiện truy vấn cơ sở dữ liệu để kiểm tra thông tin người dùng
                ConvertPassToMD5();
                ACCOUNT user = null;
                await Task.Run(()=> 
                {
                   try
                   {
                       user = db.ACCOUNTs.FirstOrDefault(u => u.Username == Username && u.Password == converted_password);
                   }
                   catch (Exception ex)
                   {
                        if (ex.Message.Equals("The provider did not return a ProviderManifestToken string."))
                        {
                            MyMessageBox.ShowDialog("Please check your internet connection again!", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                            _isInternetConnectionError = true;
                        }
                   }
                });

                if (_isInternetConnectionError)
                    return;

                // Nếu thông tin đăng nhập hợp lệ
                if (user != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MyApp.CurrentUser = user;
                        
                        // Check if exist MainViewModel in current launching
                        if (MyApp.MainViewModel == null)
                        {
                            _mainViewModel = new MainViewModel();
                            MyApp.MainViewModel = _mainViewModel;
                        }
                        else
                            _mainViewModel = MyApp.MainViewModel;

                        // Set the current user for MainView and its Data Context
                        _mainViewModel.CurrentUser = user;
                        _mainView = new MainView();
                        _mainView.DataContext = _mainViewModel;
                        _mainView.Show(); 

                        // Close LoginView and display MainView
                        Application.Current.MainWindow.Close();
                        Application.Current.MainWindow = _mainView;
                    });
                }
                else
                {
                    MyMessageBox.ShowDialog("Username or password is not correct.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                executeButton = true;
            }
        }

        private void ConvertPassToMD5()
        {
            converted_password = Constant.convertPassToMD5(Password);
        }
        #endregion
    }
}
