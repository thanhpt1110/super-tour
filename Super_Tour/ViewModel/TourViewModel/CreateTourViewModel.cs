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
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Entity.Migrations;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Super_Tour.ViewModel.TourViewModel;

namespace Super_Tour.ViewModel
{
    internal class CreateTourViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel;
        private MainTourViewModel _mainTourViewModel;
        private string _tourName = null;
        private decimal _tourPrice = 0;
        private int _totalDay = 0;
        private int _totalNight = 0;
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
        public ICommand SaveCommnand { get; }
        public ICommand AddADayCommand { get; }
        public ICommand AddProvinceCommand { get; }
        public ICommand RemoveProvinceCommand { get; }
        #endregion

        #region Constructor
        public CreateTourViewModel(MainTourViewModel mainTourViewModel, MainViewModel mainViewModel)
        {
            // Create command
            SaveCommnand = new RelayCommand(ExecuteCreateTourCommand);
            AddProvinceCommand = new RelayCommand(ExecuteAddProvince);
            RemoveProvinceCommand = new RelayCommand(ExecuteRemoveProvince);
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);

            // Create objects
            db = SUPER_TOUR.db;
            _mainViewModel = mainViewModel;
            this._mainTourViewModel = mainTourViewModel;
            _listSelectedProvinces = new List<Province>();
            _listProvinces = new ObservableCollection<Province>();
            _selectedProvinceList = new ObservableCollection<string>();
            _dateActivityList = new ObservableCollection<DateActivity>();
            LoadProvinces();
        }
        #endregion

        #region Province
        private void LoadProvinces()
        {
            foreach (Province Province in Get_Api_Address.getProvinces())
            {
                ListProvinces.Add(Province);
            }
        }

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

        #region Grid day's schedule class
        public class GridActivity : ObservableObject
        {
            public TOUR_DETAILS Tour_detail { get; set; }
            private DateTime _timeOfPacakge;
            private string _packageName;

            public string PackageName 
            { 
                get { return _packageName; }
                set 
                { 
                    _packageName = value; 
                    OnPropertyChanged(nameof(PackageName));
                } 
            }
            
            public DateTime TimeOfPackage
            {
                get => _timeOfPacakge; 
                set
                {
                    _timeOfPacakge=value;
                    OnPropertyChanged(nameof(TimeOfPackage));
                }
            }
        }
        #endregion

        #region Execute Add day
        private void ExecuteAddADayCommand(object obj)
        {
            int currentDay = DateActivityList.Count + 1;
            DateActivity dateActivity = new DateActivity(currentDay, this);
            DateActivityList.Add(dateActivity);
            TotalDay = currentDay;
            TotalNight = currentDay - 1;
        }
        #endregion

        #region Date Actitvity class
        public class DateActivity : ObservableObject
        {
            #region Declare Date activity variable
            private SUPER_TOUR db = null;
            private int _dateOrder;
            private bool _isUpdate = false;
            private TOUR _tour = null;
            private ObservableCollection<GridActivity> _morningTourDetail;
            private ObservableCollection<GridActivity> _afternoonTourDetail;
            private ObservableCollection<GridActivity> _eveningTourDetail;
            private CreateTourViewModel _createTourViewModel;
            private UpdateTourViewModel _updateTourViewModel;
            private DetailTourViewModel _detailTourViewModel;
            #endregion

            #region Declare binding
            public string DateOrder 
            { 
                get => $"Lịch trình ngày {_dateOrder}"; 
            }

            public ObservableCollection<GridActivity> MorningActivities
            {
                get => _morningTourDetail;
                set
                {
                    _morningTourDetail = value;
                    OnPropertyChanged(nameof(MorningActivities));
                }
            }

            public ObservableCollection<GridActivity> AfternoonActivities 
            { 
                get => _afternoonTourDetail; 
                set 
                { 
                    _afternoonTourDetail = value; 
                    OnPropertyChanged(nameof(AfternoonActivities)); 
                } 
            }

            public ObservableCollection<GridActivity> EveningActivities 
            { 
                get => _eveningTourDetail; 
                set 
                { _eveningTourDetail = value; 
                    OnPropertyChanged(nameof(EveningActivities));
                } 
            }
            #endregion

            #region Command
            public ICommand AddPackageToTourMorningCommand { get; private set; }
            public ICommand DeletePackageMorningCommand { get; private set; }
            public ICommand AddPackageToTourAfternoonCommand { get; private set; }
            public ICommand DeletePackageAfternoonCommand { get; private set; }
            public ICommand AddPackageToTourEveningCommand { get; private set; }
            public ICommand DeletePackageEveningCommand { get; private set; }
            public int DateOrder1 { get; }
            public object Value { get; }
            public DetailTourViewModel DetailTourViewModel { get; }
            public bool V { get; }
            public TOUR Tour { get; }
            #endregion

            #region Constructor
            public DateActivity(int dateOrder, CreateTourViewModel createTourViewModel = null, UpdateTourViewModel updateTourViewModel = null, DetailTourViewModel detailTourViewModel = null, bool isUpdate = false,TOUR tour=null)
            {
                db = SUPER_TOUR.db;
                _dateOrder = dateOrder;
                _tour = tour;
                _isUpdate = isUpdate;
                _createTourViewModel = createTourViewModel; 
                _updateTourViewModel = updateTourViewModel;
                _detailTourViewModel = detailTourViewModel;
                _morningTourDetail = new ObservableCollection<GridActivity>();
                _afternoonTourDetail = new ObservableCollection<GridActivity>();
                _eveningTourDetail = new ObservableCollection<GridActivity>();
                AddPackageToTourMorningCommand = new RelayCommand(ExecuteAddPackageToTourMorningCommand);
                DeletePackageMorningCommand = new RelayCommand(ExecuteDeletePacakgeMorningCommand);
                AddPackageToTourAfternoonCommand = new RelayCommand(ExecuteAddPackageToTourAfternoonCommand);
                DeletePackageAfternoonCommand = new RelayCommand(ExecuteDeletePacakgeAfternoonCommand);
                AddPackageToTourEveningCommand = new RelayCommand(ExecuteAddPackageToTourEveningCommand);
                DeletePackageEveningCommand = new RelayCommand(ExecuteDeletePacakgeEveningCommand);
            }

            public DateActivity(int dateOrder, object value, DetailTourViewModel detailTourViewModel, bool v, TOUR tour)
            {
                DateOrder1 = dateOrder;
                Value = value;
                DetailTourViewModel = detailTourViewModel;
                V = v;
                Tour = tour;
            }
            #endregion

            #region Delete package in Tour
            private void ExecuteDeletePacakgeMorningCommand(object obj)
            {
                GridActivity tour_detail = obj as GridActivity;
                _morningTourDetail.Remove(tour_detail);
            }
            private void ExecuteDeletePacakgeAfternoonCommand(object obj)
            {
                GridActivity tour_detail = obj as GridActivity;
                _afternoonTourDetail.Remove(tour_detail);
            }
            private void ExecuteDeletePacakgeEveningCommand(object obj)
            {
                GridActivity tour_detail = obj as GridActivity;
                _eveningTourDetail.Remove(tour_detail);               
            }
            #endregion

            #region Add package to Tour
            private void ExecuteAddPackageToTourMorningCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();      
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_morningTourDetail, _isUpdate, _tour);

                // Calculate old price
                decimal oldMorningPrice = SumActivityPrice(_morningTourDetail);
                addPackageToTourView.ShowDialog();

                // Calculate price 
                decimal newMorningPrice = SumActivityPrice(_morningTourDetail);
                if (_isUpdate)
                    _updateTourViewModel.ReloadPriceWith(oldMorningPrice, newMorningPrice);
                else
                    _createTourViewModel.ReloadPriceWith(oldMorningPrice, newMorningPrice);
            }
            private void ExecuteAddPackageToTourAfternoonCommand(object obj)    
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_afternoonTourDetail, _isUpdate, _tour);

                // Calculate old price
                decimal oldAfternoonPrice = SumActivityPrice(_afternoonTourDetail);
                addPackageToTourView.ShowDialog();

                // Calculate price 
                decimal newAfternoonPrice = SumActivityPrice(_afternoonTourDetail);
                if (_isUpdate)
                    _updateTourViewModel.ReloadPriceWith(oldAfternoonPrice, newAfternoonPrice);
                else
                    _createTourViewModel.ReloadPriceWith(oldAfternoonPrice, newAfternoonPrice);
            }
            private void ExecuteAddPackageToTourEveningCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                ObservableCollection<GridActivity> tempEvening = new ObservableCollection<GridActivity>(_eveningTourDetail);
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_eveningTourDetail, _isUpdate, _tour);

                // Calculate old price
                decimal oldEveningPrice = SumActivityPrice(_eveningTourDetail);
                addPackageToTourView.ShowDialog();

                // Calculate price 
                decimal newEveningPrice = SumActivityPrice(_eveningTourDetail);
                if (_isUpdate)
                {
                    if (tempEvening.Count() == 0 && tempEvening != _eveningTourDetail)
                        _updateTourViewModel.TotalNight++;
                    _updateTourViewModel.ReloadPriceWith(oldEveningPrice, newEveningPrice);
                }
                else
                {
                    if (tempEvening.Count() == 0 && tempEvening != _eveningTourDetail)
                        _createTourViewModel.TotalNight++;
                    _createTourViewModel.ReloadPriceWith(oldEveningPrice, newEveningPrice);
                }
            }

            private decimal SumActivityPrice(ObservableCollection<GridActivity> activityDetails)
            {
                decimal totalActivityPrice = 0;
                PACKAGE currentPackage = null;
                foreach(GridActivity item in activityDetails)
                {
                    currentPackage = db.PACKAGEs.Find(item.Tour_detail.Id_Package);
                    totalActivityPrice += currentPackage.Price;
                }
                return totalActivityPrice;
            }
            #endregion
        }
        #endregion

        #region Reload price 
        public void ReloadPriceWith(decimal oldPrice, decimal newPrice)
        {
            try
            {
                decimal currentPrice = TourPrice;
                currentPrice -= oldPrice;
                currentPrice += newPrice;
                TourPrice = currentPrice;
            }
            catch (Exception ex) 
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Check data modified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_tourName) || string.IsNullOrEmpty(_tourPrice.ToString()) || SelectedProvinceList.Count == 0
                || _dateActivityList.Count == 0)
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        #region Perform add new tour
        private async void ExecuteCreateTourCommand(object obj)
        {
            try
            {
                // Save data to DB in TOUR table
                TOUR tour = new TOUR();
                tour.Name_Tour = _tourName;
                tour.TotalDay = TotalDay;
                tour.TotalNight = TotalNight;
                tour.PlaceOfTour = "";
                tour.Status_Tour = "Available";
                tour.PriceTour = TourPrice;
                foreach(string province in _selectedProvinceList)
                {
                    tour.PlaceOfTour = tour.PlaceOfTour + province + "|";
                }
                db.TOURs.Add(tour);
                await this.db.SaveChangesAsync();

                // Save data to DB in TOUR_DETAILS table
                int dateOrder = 1;
                int IdTour = this.db.TOURs.Max(p => p.Id_Tour);
                foreach (DateActivity dateActivity in _dateActivityList)
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
                    dateOrder++;
                }
                await db.SaveChangesAsync();

                // Synchronyze real-time DB
                MainTourViewModel.TimeTour = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTourViewModel.TimeTour);
                
                // Process UI event
                MyMessageBox.ShowDialog("Add new tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainTourViewModel;
                _mainTourViewModel.ReloadAfterCreateNewTour(tour);
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
    }
}
