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
    internal class CreateBookingViewModel: ObservableObject
    {
        private TRAVEL _travel;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<Province> _listCities;
        private ObservableCollection<TOURIST> _tourists;
        private string _selectedGender;
        private District _selectedDistrict;
        private ObservableCollection<District> _listDistrict;
        private Province _selectedCity;
        private CUSTOMER _customer;
        private bool _execute = true;
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
        public ICommand AutoFillInformationCommand { get; }


        public CreateBookingViewModel()
        {
            //Tourists = new ObservableCollection<TOURIST>();
            _customer = new CUSTOMER();
            _customer.Id_Customer = 0;
            _listGender = new ObservableCollection<string>();
            _tourists = new ObservableCollection<TOURIST>();
            SelectedCityCommand = new RelayCommand(ExecuteSelectedCityComboBox);
            _listDistrict = new ObservableCollection<District>();
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
            AutoFillInformationCommand = new RelayCommand(ExecuteAutoFillInformationCommand);
            CreateNewBookingCommand = new RelayCommand(ExecuteCreateNewBookingCommand, CanExecuteCreateNewBooking);
            loadGender();
            LoadProvinces();
        }
        private void ExecuteAutoFillInformationCommand(object obj)
        {
            CUSTOMER customer = db.CUSTOMERs.FirstOrDefault(p => p.IdNumber == _customer.IdNumber);
            if(customer!=null)
            {
                Customer = customer;
                SelectedCity = _listCities.Where(p=>p.codename== customer.Province).First();
                SelectedGender = customer.Gender;
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistrict.Add(district);
                }
                SelectedDistrict = _selectedCity.districts.Where(p => p.codename == customer.District).FirstOrDefault();
            }    
        }
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
        private void loadGender()
        {
            _listGender.Add("Female");
            _listGender.Add("Male");
        }
        private bool CanExecuteCreateNewBooking(object obj)
        {
            return _execute;
        }
        private async void ExecuteCreateNewBookingCommand(object obj)
        {
            try
            {
                _execute = false;
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
                Customer.Province = _selectedCity.codename;
                Customer.District = _selectedDistrict.codename;
                Customer.Gender = _selectedGender;
                db.CUSTOMERs.AddOrUpdate(_customer);
                await db.SaveChangesAsync();
                BOOKING booking = new BOOKING();
                booking.Status = "Payed";
                booking.Id_Travel = _travel.Id_Travel;
                booking.Id_Customer_Booking = db.CUSTOMERs.Max(p => p.Id_Customer);
                booking.TotalPrice = _travel.TOUR.PriceTour * _tourists.Count;
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
                /*CreateBookingView view = null;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is CreateBookingView)
                    {
                        view = window as CreateBookingView;
                        break;
                    }
                }
                view.Close();*/
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
        
    }
}
