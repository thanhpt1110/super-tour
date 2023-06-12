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
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class DetailCustomerViewModel: ObservableObject
    {
        #region Declare variable
        private ObservableCollection<Province> _listProvince = null;
        private ObservableCollection<District> _listDistrict = null;
        private CUSTOMER _selectedItem = null;
        private string _idNumber = null;
        private string _phoneNumber = null;
        private string _selectedGender = null;
        private string _name = null;
        private Province _selectedProvince = null;
        private District _selectedDistrict = null;
        private List<Province> ListProvinces = null;
        private List<District> ListDistricts = null;
        #endregion

        #region Declare binding
        public string IdNumber
        {
            get { return _idNumber; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _idNumber = value;
                    OnPropertyChanged(nameof(IdNumber));
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
            }
        }

        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged(nameof(SelectedDistrict));
            }
        }

        public Province SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
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

        #region Constructor
        public DetailCustomerViewModel()
        {

        }

        public DetailCustomerViewModel(CUSTOMER customer)
        {
            this._selectedItem = customer;

            // Create object
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
    }
}
