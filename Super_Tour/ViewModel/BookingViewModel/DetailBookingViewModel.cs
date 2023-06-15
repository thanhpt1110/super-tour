using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using Super_Tour.ViewModel.TourViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Super_Tour.ViewModel.BookingViewModel
{
    internal class DetailBookingViewModel: ObservableObject
    {
        #region Declare variable 
        private SUPER_TOUR db = null;

        // Passing objects
        private BOOKING _booking = null;
        private CUSTOMER _customer = null;
        private TRAVEL _travel = null;

        // Travel information
        private string _travelName = null;
        private string _startLocation = null;
        private DateTime _selectedDateTime = DateTime.Now;
        private string _maxTicket = null;
        private string _selectedDiscount = null;
        private ObservableCollection<string> _listDiscount;
        private decimal _totalPrice;
        // Customer information
        private string _idNumber;
        private string _customerName;
        private string _selectedGender;
        private string _phoneNumber;
        private List<TOURIST> _tourists = null;
        private Province _selectedProvince;
        private District _selectedDistrict;
        private ObservableCollection<District> _listDistricts = null;
        private ObservableCollection<Province> _listProvinces = null;
        #endregion

        #region Declare binding
        #region Travel information
        public string TravelName
        {
            get { return _travelName; }
            set
            {
                _travelName = value;
                OnPropertyChanged(nameof(TravelName));  
            }
        }

        public string StartLocation
        {
            get { return _startLocation; }
            set
            {
                _startLocation = value;
                OnPropertyChanged(nameof(StartLocation));
            }
        }

        public DateTime SelectedDateTime
        {
            get { return _selectedDateTime; }
            set
            {
                _selectedDateTime = value;
                OnPropertyChanged(nameof(SelectedDateTime));
            }
        }

        public string MaxTicket
        {
            get { return _maxTicket; }
            set
            {
                _maxTicket = value;
                OnPropertyChanged(nameof(MaxTicket));
            }
        }

        public ObservableCollection<string> ListDiscount
        {
            get
            {
                return _listDiscount;
            }
            set
            {
                _listDiscount = value;
                OnPropertyChanged(nameof(ListDiscount));
            }
        }

        public string SelectedDiscount
        {
            get { return _selectedDiscount; }
            set
            {
                _selectedDiscount = value;
                OnPropertyChanged(nameof(SelectedDiscount));
            }
        }
        #endregion

        #region Customer information
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

        public string CustomerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _customerName = value;
                    OnPropertyChanged(nameof(CustomerName));
                }
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

        public Province SelectedProvince
        {
            get
            {
                return _selectedProvince;
            }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
            }
        }

        public ObservableCollection<District> ListDistricts
        {
            get
            {
                return _listDistricts;
            }
            set
            {
                _listDistricts = value;
                OnPropertyChanged(nameof(ListDistricts));
            }
        }

        public ObservableCollection<Province> ListProvinces
        {
            get
            {
                return _listProvinces;
            }
            set
            {
                _listProvinces = value;
                OnPropertyChanged(nameof(ListProvinces));
            }
        }
        #endregion

        #endregion

        #region Command
        public ICommand OpenAddTouristForBookingViewCommand { get; private set; }
        public ICommand ViewDetailScheduleCommand { get; private set; }
        
        #endregion

        #region Constructor
        public DetailBookingViewModel(BOOKING booking) 
        {
            // Create objects
            _booking = booking;
            _travel = _booking.TRAVEL;
            _customer = booking.CUSTOMER;
            _listDistricts = new ObservableCollection<District>();

            // Create command
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
            ViewDetailScheduleCommand = new RelayCommand(ExecuteViewDetailScheduleCommand);

            // Load data from existed objects
            LoadTravelInformation();
            LoadCustomerInformation();
        }
        #endregion

        #region Load data from existed objects
        private void LoadTravelInformation()
        {
            _travelName = _travel.TOUR.Name_Tour;
            _startLocation = _travel.StartLocation;
            _selectedDateTime = _travel.StartDateTimeTravel;
            _maxTicket = _travel.MaxTicket.ToString();
            _listDiscount = new ObservableCollection<string>
            {
                "5%",
                "10%",
                "15%",
                "20%",
                "25%",
                "30%",
                "35%",
                "40%",
                "45%",
                "50%"
            };
            _selectedDiscount = _travel.Discount.ToString() + "%";
        }

        private void LoadCustomerInformation()
        {
            _phoneNumber = _customer.PhoneNumber;
            _customerName = _customer.Name_Customer;
            _idNumber = _customer.IdNumber;
            _selectedGender = _customer.Gender;
            LoadProvinces();
            _selectedProvince = _listProvinces.Where(p => p.codename == _customer.Id_Province).FirstOrDefault();
            LoadDistrict();
            _selectedDistrict = Get_Api_Address.getDistrict(_selectedProvince).Where(p => p.codename == _customer.Id_District).FirstOrDefault();
        }

        private void LoadDistrict()
        {
            try
            {
                _listDistricts.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedProvince);
                foreach (District district in districts)
                {
                    _listDistricts.Add(district);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private void LoadProvinces()
        {
            try
            {
                List<Province> provinces = Get_Api_Address.getProvinces();
                _listProvinces = new ObservableCollection<Province>(provinces);
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region View detail schedule
        private void ExecuteViewDetailScheduleCommand(object obj)
        {
            try
            {
                if (_travel != null)
                {
                    DetailTourView detailTourView = new DetailTourView();
                    detailTourView.DataContext = new DetailTourViewModel(_travel.TOUR);
                    detailTourView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region View tourist detail
        private void ExecuteOpenAddTouristForBookingViewCommand(object obj)
        {
            try
            {
                if (_booking != null)
                {
                    DetailTouristView view = new DetailTouristView();
                    view.DataContext = new DetailTouristViewModel(_booking.TOURISTs.ToList());
                    view.ShowDialog();
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
