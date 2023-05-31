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

namespace Super_Tour.ViewModel
{
    internal class CreateTourViewModel: ObservableObject
    {
        #region Grid Activity class
        public class GridActivity:ObservableObject
        {
            private DateTime _timeOfPacakge;
            public TOUR_DETAILS Tour_detail { get; set; }
            private string _packageName;
            public string PackageName { get { return _packageName; } set { _packageName = value; OnPropertyChanged(nameof(PackageName)); } }
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
        #region Date Actitvity class
        public class DateActivity:ObservableObject
        {
            #region Declare Date activity varibale
            private int _dateID;
            private bool _isUpdate = false;
            private TOUR _tour = null;
            private ObservableCollection<GridActivity> _morningTourDetail;
            private ObservableCollection<GridActivity> _afternoonTourDetail;
            private ObservableCollection<GridActivity> _eveningTourDetail;
            private SUPER_TOUR db = new SUPER_TOUR();
            #endregion
            #region Declare binding
            public string DateID { get => $"Lịch trình ngày {_dateID}"; }
            public ObservableCollection<GridActivity> MorningActivities
            {
                get => _morningTourDetail; set
                {
                    _morningTourDetail = value;
                    OnPropertyChanged(nameof(MorningActivities));
                }
            }
            public ObservableCollection<GridActivity> AfternoonActivities { get => _afternoonTourDetail; set { _afternoonTourDetail = value; OnPropertyChanged(nameof(AfternoonActivities)); } }
            public ObservableCollection<GridActivity> EveningActivities { get => _eveningTourDetail; set { _eveningTourDetail = value; OnPropertyChanged(nameof(EveningActivities)); } }
            #endregion
            #region Command
            public ICommand DeletePackageMorningCommand { get; private set; }
            public ICommand DeletePackageAfternoonCommand { get; private set; }
            public ICommand DeletePackageEveningCommand { get; private set; }
            public ICommand AddPackageToTourMorningCommand { get; private set; }
            public ICommand AddPackageToTourAfternoonCommand { get; private set; }
            public ICommand AddPackageToTourEveningCommand { get; private set; }
            #endregion
            #region Constructor

            public DateActivity(int dateID,bool isUpdate=false,TOUR tour=null)
            {
                this._dateID=dateID;
                this._tour = tour;
                this._isUpdate=isUpdate;
                generateCommand();
            }
            public DateActivity(int dateID, List<string> morningActivities, List<string> afternoonActivities, List<string> eveningActivities)
            {
                generateCommand();
                _dateID = dateID;
            }

            #endregion
            #region init command
            private void generateCommand()
            {
                _morningTourDetail = new ObservableCollection<GridActivity>();
                _afternoonTourDetail = new ObservableCollection<GridActivity>();
                _eveningTourDetail = new ObservableCollection<GridActivity>();
                DeletePackageAfternoonCommand = new RelayCommand(ExecuteDeletePacakgeAfternoonCommand);
                DeletePackageEveningCommand = new RelayCommand(ExecuteDeletePacakgeEveningCommand);

                DeletePackageMorningCommand = new RelayCommand(ExecuteDeletePacakgeMorningCommand);
                AddPackageToTourMorningCommand = new RelayCommand(ExecuteAddPackageToTourMorningCommand);
                AddPackageToTourAfternoonCommand = new RelayCommand(ExecuteAddPackageToTourAfternoonCommand);
                AddPackageToTourEveningCommand = new RelayCommand(ExecuteAddPackageToTourEveningCommand);
            }
            #endregion
            #region Delete
            private void ExecuteDeletePacakgeMorningCommand(object obj)
            {
                MessageBox.Show("DeleteMorning");
/*                TOUR_DETAILS tour_detail = obj as TOUR_DETAILS;
                _morningTourDetail.Remove(tour_detail);*/
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
            #region Add
            private void ExecuteAddPackageToTourMorningCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();               
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_morningTourDetail, _isUpdate, _tour);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourAfternoonCommand(object obj)
            {
                MessageBox.Show("Afternoon");

                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_afternoonTourDetail, _isUpdate, _tour);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourEveningCommand(object obj)
            {
                //MessageBox.Show("Evening");
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_eveningTourDetail, _isUpdate, _tour);
                addPackageToTourView.ShowDialog();
            }
            #endregion
           
        }

        #endregion
        #region Declare varibale
        // End test
        private List<Province> _listselectedProvinces;
        private ObservableCollection<string> _selectedProvinceList;
        private bool _executeSave=true;
        private SUPER_TOUR db = new SUPER_TOUR();
        private string _nameTour;
        private ObservableCollection<DateActivity> _dateActivityList;
        private int _totalDay;
        private int _totalNight;
        private Province _selectedProvinces;
        private MainTourViewModel mainTourViewModel;
        private string _price = null;
        private int _priceInt=0;
        private bool _isDataModified = false;
        private ObservableCollection<Province> _listProvinces;
        private string _selectRemoveProvince = null;
        private MainViewModel _mainViewModel;
        #endregion
        #region Declare binding
        public string SelectRemoveProvince
        {
            get
            {
                return _selectRemoveProvince;
            }
            set
            {
                _selectRemoveProvince = value;
                OnPropertyChanged(nameof(SelectRemoveProvince));
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
        public ObservableCollection<string> SelectedProvinceList
        {
            get => _selectedProvinceList;
            set
            {
                _selectedProvinceList = value;
                OnPropertyChanged(nameof(SelectedProvinceList));
            }
        }
        #endregion
        #region Command
        public ICommand CreateTourCommnand { get; }
        public ICommand AddADayCommand { get; }
        public ICommand AddProvinceCommand { get; }
        public ICommand RemoveProvinceCommand { get; }
        #endregion
        #region Constructor
        public CreateTourViewModel(MainTourViewModel mainTourViewModel, MainViewModel mainViewModel)
        {
            // Test 
            _mainViewModel = mainViewModel;
            this.mainTourViewModel = mainTourViewModel;
            DateActivityList = new ObservableCollection<DateActivity>();
            _listselectedProvinces = new List<Province>();
            LoadProvinces();
            if (DateActivityList.Count == 0)
                TotalNight = 0;
            else
            TotalNight = (DateActivityList.Count - 1);
            TotalDay = DateActivityList.Count;
            CreateTourCommnand = new RelayCommand(ExecuteCreateTourCommand, checkExecuteSave);
            AddProvinceCommand = new RelayCommand(ExecuteAddProvince);
            RemoveProvinceCommand = new RelayCommand(ExecuteRemoveProvince);
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
            SelectedProvinceList = new ObservableCollection<string>();
        }
        #endregion 
        #region init Page
        private void LoadProvinces()
        {
            ListProvinces = new ObservableCollection<Province>();
            foreach (Province Province in Get_Api_Address.getProvinces())
            {
                ListProvinces.Add(Province);
            }
        }
        #endregion
        #region Execute Provinces
        private void ExecuteRemoveProvince(object obj)
        {
            string removedProvinceName = obj as string;
            Province removedProvince = _listselectedProvinces.SingleOrDefault(p => p.name == removedProvinceName);
            SelectedProvinceList.Remove(removedProvinceName);
            _listselectedProvinces.Remove(removedProvince);
            ListProvinces.Add(removedProvince);
            List <Province> newList = ListProvinces.OrderBy(p => p.name).ToList();
            ListProvinces.Clear();
            foreach(Province province in newList)
            {
                ListProvinces.Add(province);
            }    
        }
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
        #endregion
        #region Create tour
        private bool checkExecuteSave(object obj)
        {
            return _executeSave;
        }
        private async void ExecuteCreateTourCommand(object obj)
        {
            if (string.IsNullOrEmpty(_nameTour) || DateActivityList.Count==0)
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                _executeSave = false;
                TOUR tour = new TOUR();
                tour.Id_Tour = 1;
                tour.Name_Tour = _nameTour;
                tour.TotalDay = TotalDay;
                tour.TotalNight = TotalNight;
                tour.PlaceOfTour = "";
                foreach(string province in _selectedProvinceList)
                {
                    tour.PlaceOfTour = tour.PlaceOfTour + province + "|";
                }
                tour.Status_Tour = "Available";
                tour.PriceTour = _priceInt;
                this.db.TOURs.Add(tour);
                int i = 1;
                await this.db.SaveChangesAsync();
                int IdTour = this.db.TOURs.Max(p => p.Id_Tour);
                foreach (DateActivity dateActivity in _dateActivityList)
                {

                    foreach (GridActivity activity in dateActivity.MorningActivities)
                    {
                        TOUR_DETAILS tourDetail = activity.Tour_detail;
                        tourDetail.Id_Tour = IdTour;
                        tourDetail.Date_Order_Package = i;
                        tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                        tourDetail.Id_TourDetails = 1;
                        tourDetail.Session = Constant.MORNING;
                        //tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
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
                        //tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
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
                        //tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                        db.TOUR_DETAILs.Add(tourDetail);
             
                    }
                    i++;
                }
                db.TOURs.AddOrUpdate(tour);
                await db.SaveChangesAsync();
                await mainTourViewModel.ReloadDataAsync();
                MyMessageBox.ShowDialog("Add new tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.setFirstChild("");
                _mainViewModel.Caption = "Tour";
                _mainViewModel.CurrentChildView = mainTourViewModel;
                /* CreateTourView createTourView = null;
                 foreach (Window window in Application.Current.Windows)
                 {
                     Console.WriteLine(window.ToString());
                     if (window is CreateTourView)
                     {
                         createTourView = window as CreateTourView;
                         break;
                     }
                 }
                 createTourView.Close();*/
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
        #region Execute Add day
        private void ExecuteAddADayCommand(object obj)
        {
            DateActivity dateActivity = new DateActivity(DateActivityList.Count + 1);
            DateActivityList.Add(dateActivity);
            TotalNight = (DateActivityList.Count - 1);
            TotalDay = DateActivityList.Count;
        }
        #endregion
        #region Check data modified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_nameTour) || string.IsNullOrEmpty(_price) || SelectedProvinceList.Count==0
                || _dateActivityList.Count==0)
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
