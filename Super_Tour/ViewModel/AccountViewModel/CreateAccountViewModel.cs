using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateAccountViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private ACCOUNT _selectedItem = null;
        private string _accountName = null; 
        private string _email = null;
        private string _username = null;
        private string _password = null;
        private string _selectedService;
        private bool _isDataModified = false;
        #endregion

        #region Declare binding
        public bool IsDataModified
        {
            get { return _isDataModified; }
            set 
            { 
                _isDataModified = value; 
                OnPropertyChanged(nameof(IsDataModified));      
            }
        }

        public string AccountName
        {
            get { return _accountName; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _accountName = value;
                    OnPropertyChanged(nameof(_accountName));
                    CheckDataModified();
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetterOrDigit(c)))
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                    CheckDataModified();
                }
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetterOrDigit(c)))
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                    CheckDataModified();
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                CheckDataModified();
            }
        }
        
        public string SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                OnPropertyChanged(nameof(SelectedService));
                CheckDataModified();    
            }
        }
        #endregion

        #region Command
        public ICommand SaveCommand { get; }
        #endregion

        #region Constructor
        public CreateAccountViewModel()
        {
            
        }

        public CreateAccountViewModel(ACCOUNT account)
        {
            db = SUPER_TOUR.db;
            this._selectedItem = account; 

            //Create object
            SaveCommand = new RelayCommand(ExecuteCreateAccountCommand);
        }
        #endregion

        #region Check data is modified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_accountName) || string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_username)
                || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_selectedService))
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        #region Perform add new account
        private void ExecuteCreateAccountCommand(object obj)
        {
            try
            {
                // Check email data format 
                if (!ValidateDataFormat.CheckGmailFormat(_email))
                {
                    MyMessageBox.ShowDialog("Please write the correct email format!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }

                // Save data to DB
                _selectedItem.Account_Name = _accountName;
                _selectedItem.Email = _email;
                _selectedItem.Username = _username;
                _selectedItem.Password = Constant.convertPassToMD5(_password);
                _selectedItem.Service = _selectedService;   
                db.ACCOUNTs.Add(_selectedItem);
                db.SaveChanges();
                _selectedItem.Id_Account = db.ACCOUNTs.Max(p => p.Id_Account);

                // Synchronize data to real-time database

                // Process UI events
                MyMessageBox.ShowDialog("Add new account successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                CreateAccountView createAccountView = null;
                foreach (Window window in Application.Current.Windows)
                {
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
            }
        }
        #endregion
    }
}
