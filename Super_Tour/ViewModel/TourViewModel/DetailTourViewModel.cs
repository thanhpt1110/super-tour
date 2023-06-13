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
using static Super_Tour.ViewModel.CreateTourViewModel;
using System.Windows.Input;
using System.Data.Entity.Migrations;
using System.Windows;

namespace Super_Tour.ViewModel.TourViewModel
{
    internal class DetailTourViewModel: ObservableObject
    {
        #region Declare varibale
        private SUPER_TOUR db = null;
        private TOUR _tour = null;
        private string _tourName = null;
        private decimal _tourPrice = 0;
        private int _totalDay = 0;
        private int _totalNight = 0;
        private Province _selectedProvince = null;
        private List<Province> _listSelectedProvinces = null;
        private ObservableCollection<Province> _listProvinces;
        private ObservableCollection<string> _selectedProvinceList;
        private ObservableCollection<DateActivity> _dateActivityList;
        #endregion

        #region Declare binding
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

        public decimal TourPrice
        {
            get { return _tourPrice; }
            set
            {
                string stringValue = value.ToString();
                if (string.IsNullOrEmpty(stringValue) || stringValue.All(char.IsDigit))
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

        public ObservableCollection<DateActivity> DateActivityList
        {
            get => _dateActivityList;
            set
            {
                _dateActivityList = value;
                OnPropertyChanged(nameof(DateActivityList));
            }
        }
        #endregion

        #region Constructor
        public DetailTourViewModel()
        {

        }

        public DetailTourViewModel(TOUR tour)
        {
            // Create objects
            _tour = tour;
            db = SUPER_TOUR.db;

            _listSelectedProvinces = new List<Province>();
            _listProvinces = new ObservableCollection<Province>();
            _dateActivityList = new ObservableCollection<DateActivity>();
            LoadTourInformation();
            LoadTourDetailsInformation();
        }
        #endregion

        #region Load page from existed Item
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
                TourPrice = _tour.PriceTour;
                TotalDay = _tour.TotalDay;
                TotalNight = _tour.TotalNight;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private async Task LoadTourDetailsInformation()
        {
            try
            {
                await Task.Run(() => {
                    List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                    Application.Current.Dispatcher.Invoke(() => {
                        int dateOrder = 1;
                        while (listTourDetail.Where(p => p.Date_Order_Package == dateOrder).ToList().Count > 0)
                        {
                            List<TOUR_DETAILS> dateNum = listTourDetail.Where(p => p.Date_Order_Package == dateOrder).ToList();
                            DateActivity dateActivity = new DateActivity(dateOrder, null, null, this, true, _tour);
                            foreach (TOUR_DETAILS date in dateNum)
                            {
                                GridActivity gridActivity = new GridActivity() { Tour_detail = date, TimeOfPackage = DateTime.Now.Date.Add(date.Start_Time_Package), PackageName = db.PACKAGEs.Find(date.Id_Package).Name_Package };
                                // Switch case to add following Session
                                if (date.Session == Constant.MORNING)
                                    dateActivity.MorningActivities.Add(gridActivity);
                                else if (date.Session == Constant.AFTERNOON)
                                    dateActivity.AfternoonActivities.Add(gridActivity);
                                else
                                    dateActivity.EveningActivities.Add(gridActivity);
                            }
                            _dateActivityList.Add(dateActivity);
                            dateOrder++;
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
