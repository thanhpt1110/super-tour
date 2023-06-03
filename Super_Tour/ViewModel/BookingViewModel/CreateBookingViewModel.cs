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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateBookingViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel = null;
        private MainBookingViewModel _mainBookingViewModel = null;
        private TRAVEL _selectedTravel = null;
        private CUSTOMER _customer = null;
        private ObservableCollection<Province> _listCities = null;
        private ObservableCollection<TOURIST> _tourists = null;
        private ObservableCollection<District> _listDistrict = null;
        private Province _selectedCity;
        private District _selectedDistrict;
        private string _selectedFilter = "Tour Name";
        private bool _isDataModified = false;
        private bool _onSearching = false;
        private string _idNumber;
        private string _nameCustomer;
        private string _phoneNumber;
        private string _selectedGender;
        private bool _execute = true;
        private string _searchTravel;
        private string table = "UPDATE_BOOKING";
        #endregion
        #region Declare binding
        public string IdNumber
        {
            get
            {
                return _idNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _idNumber = value;
                    CheckDataModified();
                    OnPropertyChanged(nameof(IdNumber));
                }
            }
        }
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }
        public string NameCustomer
        {
            get
            {
                return _nameCustomer;
            }
            set
            {
                _nameCustomer = value;
                CheckDataModified();
                OnPropertyChanged(nameof(NameCustomer));
            }
        }
        public string SelectedFilter
        {
            get
            {
                return _selectedFilter;
            }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }
        public string SearchTravel
        {
            get
            {
                return _searchTravel;
            }
            set
            {
                _searchTravel = value;
                OnPropertyChanged(nameof(SearchTravel));
            }
        }
        public bool IsDataModified
        {
            get 
            {
                return _isDataModified; 
            }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

        public string SelectedGender
        {            
            get
            {
                return _selectedGender; 
            }
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
        public Province SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }
        public ObservableCollection<District> ListDistrict
        {
            get
            {
                return _listDistrict;
            }
            set
            {
                _listDistrict = value;
                OnPropertyChanged(nameof(ListDistrict));
            }
        }

        public ObservableCollection<Province> ListCities
        {
            get
            {
                return _listCities;
            }
            set
            {
                _listCities = value;
                OnPropertyChanged(nameof(ListCities));
            }
        }
        public CUSTOMER Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public TRAVEL SelectedTravel
        {
            get 
            {
                return _selectedTravel; 
            }
            set
            {
                _selectedTravel = value;
                OnPropertyChanged(nameof(SelectedTravel));
            }
        }
        public ObservableCollection<TOURIST> Tourists
        {
            get
            {
              return  _tourists;
            }
            set
            {
                _tourists = value;
                OnPropertyChanged(nameof(Tourists));
            }
        }
        #endregion
        #region Command
        public ICommand OpenSelectTravelForBookingViewCommand { get; }
        public ICommand SelectedCityCommand { get; }
        public ICommand OpenAddTouristForBookingViewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AutoFillInformationCommand { get; }
        #endregion
        #region Constructor
        public CreateBookingViewModel()
        {          
            _tourists = new ObservableCollection<TOURIST>();
            _listDistrict = new ObservableCollection<District>();
            _customer = new CUSTOMER();
            SelectedCityCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
            AutoFillInformationCommand = new RelayCommand(ExecuteAutoFillInformationCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            LoadProvinces();
        }
        #endregion
        #region Auto fill Customer
        private void ExecuteAutoFillInformationCommand(object obj)
        {
            CUSTOMER customer = db.CUSTOMERs.FirstOrDefault(p => p.IdNumber == _customer.IdNumber);
            if(customer!=null)
            {
                Customer = customer;
                SelectedCity = _listCities.Where(p=>p.codename== customer.Id_Province).First();
                SelectedGender = customer.Gender;
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistrict.Add(district);
                }
                SelectedDistrict = _selectedCity.districts.Where(p => p.codename == customer.Id_District).FirstOrDefault();
            }    
        }
        #endregion
        #region Load window
        private void LoadProvinces()
        {
            try
            {
                List<Province> cities = Get_Api_Address.getProvinces();
                cities = cities.OrderBy(p => p.name).ToList();
                _listCities = new ObservableCollection<Province>(cities);

            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
        #region Perform add new Booking
        
        private async void ExecuteSaveCommand(object obj)
        {
            try
            {
                if (SelectedCity == null || SelectedDistrict == null || string.IsNullOrEmpty(_selectedGender)
                    || string.IsNullOrEmpty(_customer.IdNumber)
                    || string.IsNullOrEmpty(_customer.PhoneNumber)
                    || string.IsNullOrEmpty(_customer.Name_Customer)
                    || string.IsNullOrEmpty(_customer.Address)
                    || _selectedTravel == null
                    || _tourists.Count == 0)
                {
                    MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                Customer.Id_Province = _selectedCity.codename;
                Customer.Id_District = _selectedDistrict.codename;
                Customer.Gender = _selectedGender;
                db.CUSTOMERs.AddOrUpdate(_customer);
                await db.SaveChangesAsync();
                BOOKING booking = new BOOKING();
                booking.Status = "Payed";
                booking.Id_Travel = _selectedTravel.Id_Travel;
                booking.Id_Customer_Booking = db.CUSTOMERs.Max(p => p.Id_Customer);
                booking.TotalPrice = _selectedTravel.TOUR.PriceTour * _tourists.Count;
                booking.Booking_Date = DateTime.Now;
                booking.Id_Booking = 1;
                db.BOOKINGs.Add(booking);
                await db.SaveChangesAsync();
                foreach (TOURIST tourist in _tourists)
                {
                    tourist.Id_Tourist = 1;
                    tourist.Id_Booking = db.BOOKINGs.Max(p => p.Id_Booking);
                    db.TOURISTs.Add(tourist);
                }
                await db.SaveChangesAsync();
                // Synchronyze real-time DB
                MainTravelViewModel.TimeTravel = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTravelViewModel.TimeTravel);

                // Process UI event
                MyMessageBox.ShowDialog("Add new travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainBookingViewModel;
                _mainBookingViewModel.ReloadAfterCreateBooking(booking);
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _execute = true;
            }
        }
        #endregion
        #region Select Province
        private void ExecuteSelectedProvinceComboBox(object obj)
        {
            try
            {
                _listDistrict.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistrict.Add(district);
                }
                _selectedDistrict = null;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
        #region Add new Tourist
        private void ExecuteOpenAddTouristForBookingViewCommand(object obj)
        {
            AddTouristView view = new AddTouristView();
            view.DataContext = new AddTouristViewModel(Tourists);
            view.ShowDialog();
        }
        #endregion
        #region Check data modified
        private void CheckDataModified()
        {
            if (
                SelectedCity == null 
                || SelectedDistrict == null 
                || string.IsNullOrEmpty(IdNumber)
                || string.IsNullOrEmpty(PhoneNumber) 
                || string.IsNullOrEmpty(NameCustomer)
                || string.IsNullOrEmpty(_selectedGender)
                )
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
