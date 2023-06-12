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
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class DetailTravelViewModel : ObservableObject
    {
        #region Declare variable

        // Passing objects
        private TRAVEL _travel = null;
        private TOUR _tour = null;

        // Travel information
        private string _tourName = null;
        private string _startLocation = null;
        private DateTime _selectedDateTime = DateTime.Now;
        private string _maxTicket = null;
        private string _selectedDiscount = null;
        private ObservableCollection<string> _listDiscount;

        // Tour information
        private string _tourPrice = "0";
        private int _totalDay = 0;
        private int _totalNight = 0;
        private Province _selectedProvince = null;
        private List<Province> _listSelectedProvinces = null;
        private ObservableCollection<Province> _listProvinces;
        private ObservableCollection<string> _selectedProvinceList;
        #endregion

        #region Declare binding 
        #region Travel information
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

        #region Tour information
        public Province SelectedProvince
        {
            get => _selectedProvince;
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
            }
        }

        public ObservableCollection<string> SelectedProvinceList
        {
            get => _selectedProvinceList;
            set
            {
                _selectedProvinceList = value;
                OnPropertyChanged(nameof(SelectedProvinceList));
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

        public int TotalDay
        {
            get
            {
                return _totalDay;
            }
            set
            {
                _totalDay = value;
                OnPropertyChanged(nameof(TotalDay));
            }
        }

        public int TotalNight
        {
            get
            {
                return _totalNight;
            }
            set
            {
                _totalNight = value;
                OnPropertyChanged(nameof(TotalNight));
            }
        }

        public string TourPrice
        {
            get { return _tourPrice; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _tourPrice = value;
                    OnPropertyChanged(nameof(TourPrice));
                }
            }
        }

        public string TourName
        {
            get { return _tourName; }
            set
            {
                _tourName = value;
                OnPropertyChanged(nameof(TourName));
            }
        }
        #endregion
        #endregion

        #region Command
        public ICommand ViewDetailScheduleCommand { get; private set; }
        #endregion

        #region Constructor
        public DetailTravelViewModel()
        {

        }

        public DetailTravelViewModel(TRAVEL travel)
        {
            // Create objects
            _travel = travel;
            _tour = travel.TOUR;
            _listSelectedProvinces = new List<Province>();
            _listProvinces = new ObservableCollection<Province>();

            // Create command
            ViewDetailScheduleCommand = new RelayCommand(ExecuteViewDetailScheduleCommand);

            // Load UI
            LoadTourInformation();
            LoadTravelInformation();
        }
        #endregion

        #region Load page from existed item
        private void LoadTourInformation()
        {
            try
            {
                // Load selected province
                _selectedProvinceList = new ObservableCollection<string>(_tour.PlaceOfTour.Split('|').ToList());
                SelectedProvinceList.RemoveAt(_selectedProvinceList.Count - 1);

                // Load provinces to ComboBox
                _listProvinces = new ObservableCollection<Province>(Get_Api_Address.getProvinces());
                foreach (string nameProvince in _selectedProvinceList)
                {
                    Province province = _listProvinces.Where(p => p.name == nameProvince).SingleOrDefault();
                    _listSelectedProvinces.Add(province);
                }

                // Load basic tour information
                TourName = _tour.Name_Tour;
                TourPrice = _tour.PriceTour.ToString();
                TotalDay = _tour.TotalDay;
                TotalNight = _tour.TotalNight;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private void LoadTravelInformation()
        {
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
        #endregion

        #region View detail schedule
        private void ExecuteViewDetailScheduleCommand(object obj)
        {
            try
            {
                if (_tour != null)
                {
                    DetailTourView detailTourView = new DetailTourView();
                    detailTourView.DataContext = new DetailTourViewModel(_tour);
                    detailTourView.ShowDialog();
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