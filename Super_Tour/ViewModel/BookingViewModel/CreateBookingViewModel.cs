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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateBookingViewModel: ObservableObject
    {
        private TRAVEL _travel;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<City> _listCities;
        private ObservableCollection<TOURIST> _tourists;
        private string _selectedGender;
        private District _selectedDistrict;
        private ObservableCollection<District> _listDistrict;
        private City _selectedCity;
        private CUSTOMER _customer;
        private ObservableCollection<string> _listGender=new ObservableCollection<string>();

        public ObservableCollection<string> ListGender { 
            get
            { return _listGender; 
            }}

        public string SelectedGender { 
            get { return _selectedGender; }
            set {
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
        public City SelectedCity
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

        public ObservableCollection<City> ListCities
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

        public TRAVEL Travel
        {
            get { return _travel; }
            set
            {
                _travel = value;
                OnPropertyChanged(nameof(Travel));
            }
        }
        public ObservableCollection<TOURIST> Tourists 
        { 
            get => _tourists;
            set
            {
                _tourists = value;
                OnPropertyChanged(nameof(Tourists));
            }
        }
        public ICommand OpenSelectTravelForBookingViewCommand { get; }
        public ICommand SelectedCityCommand { get; }
        public ICommand OpenAddTouristForBookingViewCommand { get; }
        public ICommand CreateNewBookingCommand { get; }
        
        public CreateBookingViewModel()
        {
            //Tourists = new ObservableCollection<TOURIST>();
            _customer = new CUSTOMER();
            _listGender = new ObservableCollection<string>();
            _tourists = new ObservableCollection<TOURIST>();
            OpenSelectTravelForBookingViewCommand = new RelayCommand(ExecuteOpenSelectTravelForBookingViewCommand);
            SelectedCityCommand = new RelayCommand(ExecuteSelectedCityComboBox);
            _listCities = new ObservableCollection<City>();
            _listDistrict = new ObservableCollection<District>();
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
            loadGender();
            LoadProvinces();
        }
        private void LoadProvinces()
        {
            try
            {
                List<City> cities = Get_Api_Address.getCities();
                cities = cities.OrderBy(p => p.name).ToList();
                foreach (City city in cities)
                {
                    _listCities.Add(city);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private void loadGender()
        {
            _listGender.Add("Female");
            _listGender.Add("Male");
        }
        private async void ExecuteCreateNewBookingCommand(object obj)
        {
            if (SelectedCity == null || SelectedDistrict == null || string.IsNullOrEmpty(_selectedGender)
                || string.IsNullOrEmpty(_customer.IdNumber)
                || string.IsNullOrEmpty(_customer.PhoneNumber)
                || string.IsNullOrEmpty(_customer.Name_Customer)
                || string.IsNullOrEmpty(_customer.Address)
                || _travel == null
                || _tourists.Count == 0)
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            Customer.Id_Customer = 1;
            Customer.Province = _selectedCity.name;
            Customer.District = _selectedDistrict.name;
            Customer.Gender = _selectedGender;
            db.CUSTOMERs.Add(Customer);
            await db.SaveChangesAsync();
            BOOKING booking = new BOOKING();
            booking.Status = "Payed";
            booking.Id_Travel = _travel.Id_Travel;
            booking.Id_Customer_Booking = db.CUSTOMERs.Max(p => p.Id_Customer);
            booking.TotalPrice = _travel.TOUR.PriceTour* _tourists.Count;
            booking.Booking_Date = DateTime.Now;
            booking.Id_Booking = 1;
            db.BOOKINGs.Add(booking);
            await db.SaveChangesAsync();
/*            foreach(TOURIST tourist in _tourists)
            {
                tourist.
            }    */
        }
        private void ExecuteSelectedCityComboBox(object obj)
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
        private void ExecuteOpenAddTouristForBookingViewCommand(object obj)
        {
            AddTouristView view = new AddTouristView();
            view.DataContext = new AddTouristViewModel(Tourists);
            view.ShowDialog();
        }
        private void ExecuteOpenSelectTravelForBookingViewCommand(object obj)
        {
            _travel = new TRAVEL();
            SelectTravelForBookingView selectTravelForBookingView = new SelectTravelForBookingView();
            selectTravelForBookingView.DataContext = new SelectTravelForBookingViewModel(_travel);
            selectTravelForBookingView.ShowDialog();
            Travel = db.TRAVELs.Find(_travel.Id_Travel);
            Console.WriteLine("a");
        }
    }
}
