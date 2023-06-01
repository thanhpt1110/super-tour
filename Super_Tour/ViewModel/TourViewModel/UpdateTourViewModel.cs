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
        #region declare variable
        private TOUR _tour;
        private ObservableCollection<DateActivity> _listDateActitvities;
        private ObservableCollection<string> _selectedProvinceList;
        private ObservableCollection<Province> _listProvinces;
        private SUPER_TOUR db = new SUPER_TOUR();
        private string _nameTour;
        private string _price;
        private int _totalDay;
        private int _totalNight;
        private Province _selectedProvinces;
        private bool _executeSave = true;
        private int _priceInt;
        private bool _isDataModified = false;
        private List<Province> _listselectedProvinces;
        private MainTourViewModel _mainTourViewModel;
        private MainViewModel _mainViewModel;
        private string table = "UPDATE_TOUR";

        #endregion
        #region Declare binding
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
        public Province SelectedProvinces
        {
            get => _selectedProvinces;
            set
            {
                _selectedProvinces = value;
                CheckDataModified();
                OnPropertyChanged(nameof(SelectedProvinces));
            }
        }
        // End test
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
                CheckDataModified();
                OnPropertyChanged(nameof(ListProvinces));
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
        public string Price
        {
            get { return _price; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _price = value;
                else
                {
                    if (int.TryParse(value, out _priceInt))
                    {
                        _price = value;
                        CheckDataModified();
                    }
                    else
                        Price = _price;
                }
                OnPropertyChanged(nameof(Price));

            }
        }
        public string NameTour
        {
            get { return _nameTour; }
            set
            {
                _nameTour = value;
                CheckDataModified();
                OnPropertyChanged(nameof(NameTour));
            }
        }
        
        public ObservableCollection<DateActivity> ListDateAcitivities
        {
            get
            {
                return _listDateActitvities;
            }
            set
            {
                _listDateActitvities = value;
                CheckDataModified();
                OnPropertyChanged(nameof(ListDateAcitivities));
            }
        }
        #endregion
        #region Command
        public ICommand SaveUpdateTourCommand { get; }
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
            _tour = tour;
            _mainViewModel = mainViewModel;
            _mainTourViewModel = mainTourViewModel;
            AddProvinceCommand = new RelayCommand(ExecuteAddProvince);
            RemoveSelectedProvinceCommand = new RelayCommand(ExecuteRemoveProvince);
            SaveUpdateTourCommand= new RelayCommand(ExecuteCreateTourCommand);
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
            _selectedProvinceList = new ObservableCollection<string>();
            _listProvinces = new ObservableCollection<Province>();
            _listselectedProvinces = new List<Province>();
            _listDateActitvities = new ObservableCollection<DateActivity>();
            initPage();
            LoadPage();
        }
        #endregion
        #region init page
        private void initPage()
        {
            _selectedProvinceList =  new ObservableCollection<string>(_tour.PlaceOfTour.Split('|').ToList());
            SelectedProvinceList.RemoveAt(_selectedProvinceList.Count - 1);
            _listProvinces = new ObservableCollection<Province>(Get_Api_Address.getProvinces());
            foreach(string nameProvince in _selectedProvinceList)
            {
                Province province = _listProvinces.Where(p => p.name == nameProvince).SingleOrDefault();
                _listselectedProvinces.Add(province);
                ListProvinces.Remove(province);                    
            }
            NameTour = _tour.Name_Tour;
            Price = _tour.PriceTour.ToString();
        }
        private async Task LoadPage()
        {
            await Task.Run(() => {
                List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                Application.Current.Dispatcher.Invoke(() => {
                    int i = 1;
                    while (listTourDetail.Where(p => p.Date_Order_Package == i).ToList().Count > 0)
                    {
                        List<TOUR_DETAILS>
                        dateNum = listTourDetail.Where(p => p.Date_Order_Package == i).ToList();
                        DateActivity dateActivity = new DateActivity(i, true, _tour);
                        foreach (TOUR_DETAILS date in dateNum)
                        {
                            GridActivity gridActivity = new GridActivity() { Tour_detail = date, TimeOfPackage = DateTime.Now.Date.Add(date.Start_Time_Package), PackageName = db.PACKAGEs.Find(date.Id_Package).Name_Package };

                            if (date.Session == Constant.MORNING)
                            {
                                dateActivity.MorningActivities.Add(gridActivity);
                            }
                            else if (date.Session == Constant.AFTERNOON)
                            {
                                dateActivity.AfternoonActivities.Add(gridActivity);
                            }
                            else
                            {
                                dateActivity.EveningActivities.Add(gridActivity);
                            }
                        }
                        _listDateActitvities.Add(dateActivity);
                        i++;
                    }
                    TotalDay = _listDateActitvities.Count;
                    TotalNight = _listDateActitvities.Count - 1;
                });
            });
        }
        #endregion
        #region Execute province
        private void ExecuteAddProvince(object obj)
        {
            if (SelectedProvinces != null)
            {
                SelectedProvinceList.Add(SelectedProvinces.name);
                _listselectedProvinces.Add(SelectedProvinces);
                ListProvinces.Remove(SelectedProvinces);
                SelectedProvinces = null;
            }
        }
        private void ExecuteRemoveProvince(object obj)
        {
            string removedProvinceName = obj as string;
            Province removedProvince = _listselectedProvinces.SingleOrDefault(p => p.name == removedProvinceName);
            SelectedProvinceList.Remove(removedProvinceName);
            _listselectedProvinces.Remove(removedProvince);
            ListProvinces.Add(removedProvince);
            List<Province> newList = ListProvinces.OrderBy(p => p.name).ToList();
            ListProvinces.Clear();
            foreach (Province province in newList)
            {
                ListProvinces.Add(province);
            }
        }
        #endregion
        #region Execute Update Tour
        private async void ExecuteCreateTourCommand(object obj)
        {
            if (string.IsNullOrEmpty(_nameTour) || _listDateActitvities.Count == 0)
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                _executeSave = false;
                int i = 1;
                int IdTour = _tour.Id_Tour;
                _tour.PriceTour = 0;
                _tour.PlaceOfTour = "";
                foreach (string province in _selectedProvinceList)
                {
                    _tour.PlaceOfTour = _tour.PlaceOfTour + province + "|";
                }
                foreach (DateActivity dateActivity in _listDateActitvities)
                {
                    if (listTourDetail.Where(p => p.Date_Order_Package == i).ToList().Count == 0)
                    {
                            foreach (GridActivity activity in dateActivity.MorningActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Id_Tour = IdTour;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Session = Constant.MORNING;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                                db.TOUR_DETAILs.Add(tourDetail);
                            }
                            foreach (GridActivity activity in dateActivity.AfternoonActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Session = Constant.AFTERNOON;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Id_Tour = IdTour;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                                db.TOUR_DETAILs.Add(tourDetail);
                        }
                            foreach (GridActivity activity in dateActivity.EveningActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Id_Tour = IdTour;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Session = Constant.EVENING;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
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
                            tourDetail.Date_Order_Package = i;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.AfternoonActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Date_Order_Package = i;
                            tourDetail.Session = Constant.AFTERNOON;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.EveningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Date_Order_Package = i;
                            tourDetail.Session = Constant.EVENING;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                _tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        List<TOUR_DETAILS> tOUR_DETAILs= db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour && p.Date_Order_Package==i).ToList();
                        while (dateActivity.MorningActivities.Count< tOUR_DETAILs.Where(p=>p.Session==Constant.MORNING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.AfternoonActivities.Count < tOUR_DETAILs.Where(p=> p.Session == Constant.AFTERNOON).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.AFTERNOON).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.EveningActivities.Count < tOUR_DETAILs.Where(p=>p.Session == Constant.EVENING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                    }
                    i++;
                }
                _tour.TotalDay = _totalDay;
                _tour.TotalNight=_totalNight;
                db.TOURs.AddOrUpdate(_tour);
                await db.SaveChangesAsync();
                MainTourViewModel.TimeTour=DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTourViewModel.TimeTour);
                MyMessageBox.ShowDialog("Update tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                await _mainTourViewModel.ReloadDataAsync();
                _mainViewModel.CurrentChildView = _mainTourViewModel;
                _mainViewModel.setFirstChild("");
                /*UpdateTourView updateTourView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is UpdateTourView)
                    {
                        updateTourView = window as UpdateTourView;
                        break;
                    }
                }
                updateTourView.Close();*/
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeSave = true;
            }

        }
        #endregion
        #region Add new Day
        private void ExecuteAddADayCommand(object obj)
        {
            DateActivity dateActivity = new DateActivity(ListDateAcitivities.Count + 1);
            ListDateAcitivities.Add(dateActivity);
            TotalNight = (ListDateAcitivities.Count - 1);
            TotalDay = ListDateAcitivities.Count;
        }
        #endregion
        #region CheckDataModified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_nameTour) || string.IsNullOrEmpty(_price) || SelectedProvinceList.Count == 0
                || _listDateActitvities.Count == 0)
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
