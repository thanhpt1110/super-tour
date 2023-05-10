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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class MainLoginViewModel: ObservableObject
    {
        private bool _isViewVisible = true;
        private string _username;
        private string _password;
        private bool executeButton = true;
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
            
            LoginCommand = new RelayCommand(Login, canExecute);
            CommandForgotPassword = new RelayCommand(MoveToForgotPass);
        }

        public void MoveToForgotPass(object a)
        {
            ForgotPass_EmailView view = new ForgotPass_EmailView();
            view.Show();
            Application.Current.MainWindow.Hide();
        }
        public bool canExecute(object a)
        {
            return executeButton;
        }
        public async void Login(object a)
        {
            // Thực hiện đăng nhập và kiểm tra thông tin người dùng
            // Sử dụng Entity Framework để truy vấn cơ sở dữ liệu
            // Nếu thông tin đăng nhập hợp lệ, chuyển đến trang chính
            // Nếu không, hiển thị thông báo lỗi
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
                ACCOUNT user=null;
               await Task.Run(()=> {
                   try
                   {
                       user = db.ACCOUNTs.FirstOrDefault(u => u.Username == Username && u.Password == converted_password);
                   }
                   catch (Exception ex)
                   {
                       MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                   }
               });

                // Nếu thông tin đăng nhập hợp lệ
                if (user != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsViewVisible = false;
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
