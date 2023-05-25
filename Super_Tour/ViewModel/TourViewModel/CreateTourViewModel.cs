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

namespace Super_Tour.ViewModel
{
    internal class CreateTourViewModel: ObservableObject
    {
        //Real not test anymore
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
        public class DateActivity:ObservableObject
        {
            private int _dateID;
            private bool _isUpdate = false;
            private TOUR _tour = null;
            private ObservableCollection<GridActivity> _morningTourDetail;
            private ObservableCollection<GridActivity> _afternoonTourDetail;
            private ObservableCollection<GridActivity> _eveningTourDetail;
            private SUPER_TOUR db = new SUPER_TOUR();
            public ICommand DeletePackageMorningCommand { get; private set; }
            public ICommand DeletePackageAfternoonCommand { get; private set; }
            public ICommand DeletePackageEveningCommand { get; private set; }
            public ICommand AddPackageToTourMorningCommand { get; private set; }
            public ICommand AddPackageToTourAfternoonCommand { get; private set; }
            public ICommand AddPackageToTourEveningCommand { get; private set; }

            public DateActivity(int dateID,bool isUpdate=false,TOUR tour=null)
            {
                this._dateID=dateID;
                this._tour = tour;
                this._isUpdate=isUpdate;
                generateCommand();
            }
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
            public DateActivity(int dateID, List<string> morningActivities, List<string> afternoonActivities, List<string> eveningActivities)
            {
                generateCommand();
                _dateID = dateID;
/*                _morningActivitiesText = morningActivities;
                _afternoonActivitiesText = afternoonActivities;
                _eveningActivitiesText = eveningActivities;*/
            }

            public string DateID { get => $"Lịch trình ngày {_dateID}";   }
            public ObservableCollection<GridActivity> MorningActivities { get => _morningTourDetail; set { _morningTourDetail = value;
                    OnPropertyChanged(nameof(MorningActivities)); } }
            public ObservableCollection<GridActivity> AfternoonActivities { get => _afternoonTourDetail; set { _afternoonTourDetail = value; OnPropertyChanged(nameof(AfternoonActivities)); } }
            public ObservableCollection<GridActivity> EveningActivities { get => _eveningTourDetail; set { _eveningTourDetail = value; OnPropertyChanged(nameof(EveningActivities)); } }
        }




        // End test
        private ObservableCollection<string> _selectedProvinceList;
        private bool _executeSave=true;
        private SUPER_TOUR db = new SUPER_TOUR();
        private string _nameTour;
        private ObservableCollection<DateActivity> _dateActivityList;
        private int _totalDay;
        private int _totalNight;
        private string _selectedProvinces;
        private Province _selectedCity;
        public ICommand CreateTourCommnand { get; }

        private ObservableCollection<Province> _listCities;

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
                OnPropertyChanged(nameof(DateActivityList));
            }
        }
        public string NameTour
        {
            get { return _nameTour; }
            set
            {
                _nameTour = value;
                OnPropertyChanged(nameof(NameTour));
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
        private void LoadCities()
        {
            ListCities = new ObservableCollection<Province>();
            foreach(Province Province in Get_Api_Address.getProvinces())
            {
                ListCities.Add(Province);
            }
        }
        public string SelectedProvinces
        {
            get => _selectedProvinces;
            set
            {
                _selectedProvinces = value;
                OnPropertyChanged(nameof(SelectedProvinces));
            }
        }
        // End test
        public ICommand AddADayCommand { get; }
        public ObservableCollection<string> SelectedProvinceList
        {
            get => _selectedProvinceList;
            set
            {
                _selectedProvinceList = value;
                OnPropertyChanged(nameof(SelectedProvinceList));
            }
        }

        public CreateTourViewModel()
        {
            // Test 
            DateActivityList = new ObservableCollection<DateActivity>();
            LoadCities();
            if (DateActivityList.Count == 0)
                TotalNight = 0;
            else
            TotalNight = (DateActivityList.Count - 1);
            TotalDay = DateActivityList.Count;
            CreateTourCommnand = new RelayCommand(ExecuteCreateTourCommand, checkExecuteSave);
            // End Test
            // Test Province List
            SelectedProvinceList = new ObservableCollection<string>();
            SelectedProvinceList.Add("Thành phố Hồ Chí Minh");
            SelectedProvinceList.Add("Quảng Nam");
            SelectedProvinceList.Add("Bình Dương");
            SelectedProvinceList.Add("Thành phố Đà Nẵng");
            SelectedProvinceList.Add("Thành phố Đà Nẵng");
            SelectedProvinceList.Add("Thành phố Đà Nẵng");
            SelectedProvinceList.Add("Thành phố Đà Nẵng");
            // End test
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
        }
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
                tour.PlaceOfTour = "hcm";
                tour.Status_Tour = "Available";
                tour.PriceTour = 0;
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
                        tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
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
                        tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
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
                        tour.PriceTour += this.db.PACKAGEs.Find(tourDetail.Id_Package).Price;
                        db.TOUR_DETAILs.Add(tourDetail);
             
                    }
                    i++;
                }
                db.TOURs.AddOrUpdate(tour);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Add new tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
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
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeSave = true;
            }

        }
        private void ExecuteAddADayCommand(object obj)
        {
            DateActivity dateActivity = new DateActivity(DateActivityList.Count + 1);
            DateActivityList.Add(dateActivity);
            TotalNight = (DateActivityList.Count - 1);
            TotalDay = DateActivityList.Count;
        }
    }
}
