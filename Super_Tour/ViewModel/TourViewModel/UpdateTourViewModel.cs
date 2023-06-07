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
using static Super_Tour.ViewModel.CreateTourViewModel;

namespace Super_Tour.ViewModel
{
    internal class UpdateTourViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel;
        private MainTourViewModel _mainTourViewModel;
        private string _tourName = null;
        private string _tourPrice = "0";
        private int _totalDay = 0;
        private int _totalNight = 0;
        private TOUR _tour = null;
        private Province _selectedProvince = null;
        private List<Province> _listSelectedProvinces = null;
        private ObservableCollection<Province> _listProvinces;
        private ObservableCollection<string> _selectedProvinceList;
        private ObservableCollection<DateActivity> _dateActivityList;
        private bool _isDataModified = false;
        private string table = "UPDATE_TOUR";
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
                CheckDataModified();
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
                CheckDataModified();
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
                    CheckDataModified();
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
                CheckDataModified();
            }
        }

        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

        public ObservableCollection<DateActivity> DateActivityList
        {
            get => _dateActivityList;
            set
            {
                _dateActivityList = value;
                CheckDataModified();
                OnPropertyChanged(nameof(DateActivityList));
            }
        }
        #endregion

        #region Command
        public ICommand SaveCommand { get; }
        public ICommand AddADayCommand { get; }
        public ICommand AddProvinceCommand { get; }
        public ICommand RemoveSelectedProvinceCommand { get; }
        #endregion

        #region Constructor
        public UpdateTourViewModel() 
        {

        }
        public UpdateTourViewModel(TOUR tour, MainTourViewModel mainTourViewModel, MainViewModel mainViewModel)
        {
            // Create command
            SaveCommand= new RelayCommand(ExecuteCreateTourCommand);
            AddProvinceCommand = new RelayCommand(ExecuteAddProvince);
            RemoveSelectedProvinceCommand = new RelayCommand(ExecuteRemoveProvince);
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);

            // Create objects
            db = SUPER_TOUR.db;
            _tour = tour;
            _mainViewModel = mainViewModel;
            _mainTourViewModel = mainTourViewModel;
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
            // Load selected province
            _selectedProvinceList =  new ObservableCollection<string>(_tour.PlaceOfTour.Split('|').ToList());
            SelectedProvinceList.RemoveAt(_selectedProvinceList.Count - 1);
            
            // Load provinces to ComboBox
            _listProvinces = new ObservableCollection<Province>(Get_Api_Address.getProvinces());
            foreach(string nameProvince in _selectedProvinceList)
            {
                Province province = _listProvinces.Where(p => p.name == nameProvince).SingleOrDefault();
                _listSelectedProvinces.Add(province);
                ListProvinces.Remove(province);                    
            }

            // Load tour name and price
            TourName = _tour.Name_Tour;
            TourPrice = _tour.PriceTour.ToString();
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
                        DateActivity dateActivity = new DateActivity(dateOrder, null, this, true, _tour);
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
                    TotalDay = _dateActivityList.Count;
                    TotalNight = _dateActivityList.Count - 1;
                });
            });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Province
        private void ExecuteAddProvince(object obj)
        {
            if (SelectedProvince != null)
            {
                SelectedProvinceList.Add(SelectedProvince.name);
                _listSelectedProvinces.Add(SelectedProvince);
                ListProvinces.Remove(SelectedProvince);
                SelectedProvince = null;
            }
        }

        private void ExecuteRemoveProvince(object obj)
        {
            // Get removed province name
            string removedProvinceName = obj as string;
            Province removedProvince = _listSelectedProvinces.SingleOrDefault(p => p.name == removedProvinceName);
            SelectedProvinceList.Remove(removedProvinceName);
            _listSelectedProvinces.Remove(removedProvince);

            // Add this province again to comboBox
            ListProvinces.Add(removedProvince);

            // Sort list again and Display in comboBox
            List<Province> newList = ListProvinces.OrderBy(p => p.name).ToList();
            ListProvinces.Clear();
            foreach (Province province in newList)
            {
                ListProvinces.Add(province);
            }
        }
        #endregion

        #region Execute add day
        private void ExecuteAddADayCommand(object obj)
        {
            int currentDay = DateActivityList.Count + 1;
            DateActivity dateActivity = new DateActivity(currentDay, null, this, true, _tour);
            DateActivityList.Add(dateActivity);
            TotalDay = currentDay;
            TotalNight = currentDay - 1;
        }
        #endregion

        #region Execute Update Tour
        private async void ExecuteCreateTourCommand(object obj)
        {
            try
            {
                // Save data to DB in TOUR table
                _tour.Name_Tour = TourName;
                _tour.TotalDay = _totalDay;
                _tour.TotalNight = _totalNight;
                _tour.PlaceOfTour = "";
                _tour.PriceTour = decimal.Parse(TourPrice);
                foreach (string province in _selectedProvinceList)
                {
                    _tour.PlaceOfTour = _tour.PlaceOfTour + province + "|";
                }
                db.TOURs.AddOrUpdate(_tour);

                // Save data to DB in TOUR_DETAILS table
                int dateOrder = 1;
                int IdTour = _tour.Id_Tour;
                List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                foreach (DateActivity dateActivity in _dateActivityList)
                {
                    if (listTourDetail.Where(p => p.Date_Order_Package == dateOrder).ToList().Count == 0)
                    {
                        foreach (GridActivity activity in dateActivity.MorningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Id_Tour = IdTour;
                            tourDetail.Date_Order_Package = dateOrder;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            tourDetail.Session = Constant.MORNING;
                            db.TOUR_DETAILs.Add(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.AfternoonActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Id_Tour = IdTour;
                            tourDetail.Date_Order_Package = dateOrder;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            tourDetail.Session = Constant.AFTERNOON;
                            db.TOUR_DETAILs.Add(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.EveningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Id_Tour = IdTour;
                            tourDetail.Date_Order_Package = dateOrder;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            tourDetail.Session = Constant.EVENING;
                            db.TOUR_DETAILs.Add(tourDetail);
                        }
                    }
                    else
                    {
                        foreach (GridActivity activity in dateActivity.MorningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Session = Constant.MORNING;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            tourDetail.Date_Order_Package = dateOrder;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.AfternoonActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Session = Constant.AFTERNOON;
                            tourDetail.Date_Order_Package = dateOrder;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.EveningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Session = Constant.EVENING;
                            tourDetail.Date_Order_Package = dateOrder;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        List<TOUR_DETAILS> tOUR_DETAILs = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour && p.Date_Order_Package == dateOrder).ToList();
                        
                        // Xử lý khi xóa bớt package có sẵn trong Tour details 
                        while (dateActivity.MorningActivities.Count < tOUR_DETAILs.Where(p => p.Session == Constant.MORNING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.AfternoonActivities.Count < tOUR_DETAILs.Where(p => p.Session == Constant.AFTERNOON).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.AFTERNOON).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.EveningActivities.Count < tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                    }
                    dateOrder++;
                }
                await db.SaveChangesAsync();

                // Synchronyze real-time DB
                MainTourViewModel.TimeTour = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTourViewModel.TimeTour);

                // Process UI event
                MyMessageBox.ShowDialog("Update tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainTourViewModel;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Reload price 
        public void ReloadPriceWith(decimal oldPrice, decimal newPrice)
        {
            decimal currentPrice = decimal.Parse(TourPrice);
            currentPrice -= oldPrice;
            currentPrice += newPrice;
            TourPrice = currentPrice.ToString();
        }
        #endregion

        #region CheckDataModified
        private void CheckDataModified()
        {
            if ((string.IsNullOrEmpty(TourName) || string.IsNullOrEmpty(TourPrice) || SelectedProvinceList.Count == 0
                || _dateActivityList.Count == 0) || _tour.Name_Tour == TourName && _tour.PriceTour == decimal.Parse(TourPrice))
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
