using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class UpdateCustomerViewModel : ObservableObject   
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private ObservableCollection<Province> _listProvince = null;
        private ObservableCollection<District> _listDistrict = null;
        private CUSTOMER _selectedItem = null;
        private string _idNumber = null;
        private string _phoneNumber = null;
        private string _selectedGender = null;
        private string _name = null;
        private bool _isDataModified = false;
        private Province _selectedProvince = null;
        private District _selectedDistrict = null;
        private List<Province> ListProvinces = null;
        private List<District> ListDistricts = null;
        private string table = "UPDATE_CUSTOMER";
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
                _selectedGender = value;
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
        #endregion        

        #region Constructor
        public UpdateCustomerViewModel()
        {

        }

        public UpdateCustomerViewModel(CUSTOMER customer)
        {
            db = SUPER_TOUR.db;
            this._selectedItem = customer;

            // Create object
            SaveCommand = new RelayCommand(AddNewCustomer);
            SelectedProvinceCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            _listProvince = new ObservableCollection<Province>();
            _listDistrict = new ObservableCollection<District>();

            // Load data from existed item
            // These fill can not be null
            _phoneNumber = _selectedItem.PhoneNumber;
            _selectedGender = _selectedItem.Gender;
            _name = _selectedItem.Name_Customer;

            // These fill can be null
            LoadProvinces();
            if (_selectedItem.Id_Province != null && _selectedItem.Id_District != null)
            {
                _selectedProvince = _listProvince.Where(p => p.codename == _selectedItem.Id_Province).FirstOrDefault();
                _selectedDistrict = Get_Api_Address.getDistrict(_selectedProvince).Where(p => p.codename == _selectedItem.Id_District).FirstOrDefault();
                LoadDistrict();
            }
            if (_selectedItem.IdNumber != null) 
                _idNumber = _selectedItem.IdNumber;
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
            if ((string.IsNullOrEmpty(_phoneNumber) || string.IsNullOrEmpty(_name) ||
                _selectedGender == null || _phoneNumber == _selectedItem.PhoneNumber &&
                _name == _selectedItem.Name_Customer && _selectedGender == _selectedItem.Gender)
                && (_selectedItem.IdNumber != null && _selectedItem.IdNumber == IdNumber) 
                && (_selectedItem.Id_Province != null && _selectedItem.Id_Province == _selectedProvince.codename)
                && (_selectedItem.Id_District != null && _selectedItem.Id_District == _selectedDistrict.codename))
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        #region Perform update customer
        public void AddNewCustomer(object a)
        {
            try
            {
                // Save data to DB
                _selectedItem.PhoneNumber = _phoneNumber;
                _selectedItem.Name_Customer = _name;
                _selectedItem.Gender = _selectedGender;

                if (IdNumber != null)
                    _selectedItem.IdNumber = _idNumber;
                if (_selectedItem.Id_Province != null && _selectedItem.Id_District != null)
                {
                    _selectedItem.Id_Province = _selectedProvince.codename;
                    _selectedItem.Id_District = _selectedDistrict.codename;
                }
                db.CUSTOMERs.AddOrUpdate(_selectedItem);
                db.SaveChanges();

                // Synchronize data to real time DB
                MainCustomerViewModel.TimeCustomer = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainCustomerViewModel.TimeCustomer);

                // Process UI events
                MyMessageBox.ShowDialog("Update customer successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                UpdateCustomerView updateCustomerView = null; ;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is DetailCustomerView)
                    {
                        updateCustomerView = window as UpdateCustomerView;
                        break;
                    }
                }
                updateCustomerView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
