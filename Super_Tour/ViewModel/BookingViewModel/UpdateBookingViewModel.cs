using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class UpdateBookingViewModel: ObservableObject
    {
        private CUSTOMER _customer;
        private MainViewModel _mainViewModel;
        private MainBookingViewModel mainBookingViewModel;
        private TRAVEL _travel;
        private ObservableCollection<TOURIST> _tourists;
        private string _searchTravel;
        private Province _selectedCity;
        private District _selectedDistrict;
        private ObservableCollection<Province> _listCities;
        private ObservableCollection<District> _listDistricts;
        private BOOKING _booking;
        private ObservableCollection<string> _listGender;
        
        public CUSTOMER Customer { get => _customer; set { _customer = value; OnPropertyChanged(nameof(Customer)); } }
        public TRAVEL Travel { get => _travel; set { _travel = value; OnPropertyChanged(nameof(Travel)); } }
        public ObservableCollection<TOURIST> Tourists { get => _tourists; set { _tourists = value; OnPropertyChanged(nameof(Tourists)); } }
        public string SearchTravel { get => _searchTravel; set { _searchTravel = value; OnPropertyChanged(nameof(SearchTravel)); } }
        public Province SelectedCity { get => _selectedCity; set { _selectedCity = value; OnPropertyChanged(nameof(SelectedCity)); } }
        public District SelectedDistrict { get => _selectedDistrict; set { _selectedDistrict = value; OnPropertyChanged(nameof(SelectedDistrict)); } }
        public ObservableCollection<Province> ListCities { get => _listCities; set { _listCities = value; OnPropertyChanged(nameof(ListCities)); } }
        public ObservableCollection<District> ListDistricts { get => _listDistricts; set { _listDistricts = value;  OnPropertyChanged(nameof(ListDistricts)); } }
        public ICommand UpdateBookingCommand { get; }
        public ICommand AutoFillInformationCommand { get; }
        public ICommand OpenAddTouristForBookingViewCommand { get; }
        public ICommand SelectedCityCommand { get; }
        public ObservableCollection<string> ListGender { get => _listGender; set { _listGender = value; OnPropertyChanged(nameof(ListGender)); } }

        public UpdateBookingViewModel(BOOKING booking, MainViewModel mainViewModel, MainBookingViewModel mainBookingViewModel)
        {
            _booking = booking;
            SelectedCityCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            LoadData();
            _mainViewModel = mainViewModel;
            this.mainBookingViewModel = mainBookingViewModel;
        }
        private void LoadData()
        {
            Travel = _booking.TRAVEL;
            Tourists = new ObservableCollection<TOURIST>(_booking.TOURISTs);
            _listGender = new ObservableCollection<string>();
            Customer = _booking.CUSTOMER;
            loadGender();
            LoadProvinces();
        }
        private void ExecuteSelectedProvinceComboBox(object obj)
        {
            try
            {
                _listDistricts.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistricts.Add(district);
                }
                _selectedDistrict = null;
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
    }
}
