using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateAccountViewModel:ObservableObject
    {
        private string _username;
        private string _password;
        private string _accountName;
        private int _selectedPriority=0;
        private string _selectedService;
        private List<int> _listPriority;
        private List<string> _listServices;
        private SUPER_TOUR db = new SUPER_TOUR();
        private bool _executeSave = true;
        private string _email;
        public int SelectedPriority
        {
            get { return _selectedPriority; }
            set
            {
                _selectedPriority=value;
                OnPropertyChanged(nameof(SelectedPriority));
            }
        }
        public string SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                OnPropertyChanged(nameof(SelectedService));
            }
        }
        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                OnPropertyChanged(nameof(_accountName));
            }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public List<int> ListPriority
        {
            get { return _listPriority; }
        }
        public List<string> ListServices
        {
            get { return _listServices; }
        }
        public string Username
        {
            get { return _username; }
            set { _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public ICommand CreateAccountCommand { get; }
        public CreateAccountViewModel(ACCOUNT temp)
        {
            _listPriority = new List<int>();
            _listServices = new List<string>();
            generatePriority();
            generateService();
            CreateAccountCommand = new RelayCommand(ExecuteCreateAccountCommand, canExecuteCreateAccountCommand);
        }
        private bool canExecuteCreateAccountCommand(object obj)
        {
            return _executeSave;
        }
        private async void ExecuteCreateAccountCommand(object obj)
        {
            if(string.IsNullOrEmpty(_password)||string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_selectedService) || _selectedPriority==0)
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                _executeSave = false;
                ACCOUNT account = new ACCOUNT();
                account.Email = "a@gmail.com";
                account.Service = _selectedService;
                account.Priority = _selectedPriority;
                account.Id_Account = 1;
                account.Password = Constant.convertPassToMD5(_password);
                account.Username = _username;
                db.ACCOUNTs.Add(account);
                await db.SaveChangesAsync();
                CreateAccountView createAccountView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    //Console.WriteLine(window.ToString());
                    if (window is CreateAccountView)
                    {
                        createAccountView = window as CreateAccountView;
                        break;
                    }
                }
                createAccountView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeSave = true;
            }
        }
        private void generatePriority()
        {
            _listPriority.Add(1);
            _listPriority.Add(2);
            _listPriority.Add(3);
        }
        private void generateService()
        {
            _listServices.Add("Admin");
            _listServices.Add("Manager");
            _listServices.Add("Employee");
        }
    }
}
