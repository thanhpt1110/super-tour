using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Org.BouncyCastle.Crypto.Digests.SkeinEngine;
using static System.Net.Mime.MediaTypeNames;

namespace Super_Tour.ViewModel
{
    internal class CreateCustomerViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private ObservableCollection<Province> _listProvince = null;
        private ObservableCollection<District> _listDistrict = null;
        private CUSTOMER _selectedItem = null;
        private string _idNumber = null;
        private string _phoneNumber = null ;
        private string _name = null;
        private string _selectedGender = null;
        private bool _isDataModified = false;
        private Province _selectedProvince = null;
        private District _selectedDistrict = null;
        private List<Province> ListProvinces = null;
        private List<District> ListDistricts = null;
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

        public string IdNumber
        {
            get { return _idNumber; }
            set
            {   
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                { 
                    _idNumber = value;
                    OnPropertyChanged(nameof(IdNumber));
                    CheckDataModified();
                }
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit)) 
                { 
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                    CheckDataModified();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    CheckDataModified();
                }
            }
        }

        public string SelectedGender
        {
            get { return _selectedGender; }
            set
            {
                _selectedGender  = value;
                OnPropertyChanged(nameof(SelectedGender));
                CheckDataModified();
            }
        }

        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged(nameof(SelectedDistrict));
                CheckDataModified();
            }
        }

        public Province SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
                CheckDataModified();
            }
        }

        public ObservableCollection<District> ListDistrict
        {
            get { return _listDistrict; }
            set
            {
                _listDistrict = value;
                OnPropertyChanged(nameof(ListDistrict));
            }
        }

        public ObservableCollection<Province> ListProvince
        {
            get { return _listProvince; }
            set
            {
                _listProvince = value;
                OnPropertyChanged(nameof(ListProvince));
            }
        }
        #endregion

        #region Command
        public ICommand SaveCommand { get; }
        public ICommand SelectedProvinceCommand { get; }
        public ICommand PreviewTextInputCommand { get; set; }
        #endregion        

        #region Constructor
        public CreateCustomerViewModel()
        {
                
        }

        public CreateCustomerViewModel(CUSTOMER customer)
        {
            db = SUPER_TOUR.db;
            this._selectedItem = customer;

            // Create object
            SaveCommand = new RelayCommand(AddNewCustomer);
            SelectedProvinceCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            _listProvince = new ObservableCollection<Province>(); 
            _listDistrict = new ObservableCollection<District>();   
            LoadProvinces();
        }
        #endregion

        #region Province
        private void LoadProvinces()
        {
            try
            {
                ListProvinces = Get_Api_Address.getProvinces();
                foreach (Province Province in ListProvinces)
                {
                    _listProvince.Add(Province);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region District
        private void ExecuteSelectedProvinceComboBox(object obj)
        {
            _selectedDistrict = null;
            LoadDistrict();
        }

        private void LoadDistrict()
        {
            try
            {
                _listDistrict.Clear();
                ListDistricts = Get_Api_Address.getDistrict(_selectedProvince);
                foreach (District district in ListDistricts)
                {
                    _listDistrict.Add(district);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
        }
        #endregion

        #region Check data is modified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_idNumber) || string.IsNullOrEmpty(_phoneNumber)|| string.IsNullOrEmpty(_name) ||
                _selectedGender == null || _selectedProvince == null || _selectedDistrict == null)
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        #region Perform add new customer
        public void AddNewCustomer(object a)
        {
            try
            {
                // Save data to DB
                _selectedItem.IdNumber = _idNumber;
                _selectedItem.Name_Customer = _name;
                _selectedItem.PhoneNumber = _phoneNumber;
                _selectedItem.Gender = _selectedGender;
                _selectedItem.Id_Province = _selectedProvince.codename;
                _selectedItem.Id_District = _selectedDistrict.codename;
                db.CUSTOMERs.Add(_selectedItem);
                db.SaveChanges();
                _selectedItem.Id_Customer = db.CUSTOMERs.Max(p => p.Id_Customer);

                // Synchronize data to real-time database

                // Process UI events
                MyMessageBox.ShowDialog("Add new customer successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                CreateCustomerView createCustomerView = null; ;
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is CreateCustomerView)
                    {
                        createCustomerView = window as CreateCustomerView;
                        break;
                    }
                }
                createCustomerView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
