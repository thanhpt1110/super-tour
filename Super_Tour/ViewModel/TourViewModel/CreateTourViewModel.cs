using Student_wpf_application.ViewModels.Command;
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

namespace Super_Tour.ViewModel
{
    internal class CreateTourViewModel: ObservableObject
    {
        //Real not test anymore
        public class DateActivity:ObservableObject
        {
            private int _dateID;
            private ObservableCollection<string> _morningActivitiesText;
            private ObservableCollection<string> _afternoonActivitiesText;
            private ObservableCollection<string> _eveningActivitiesText;
            private List<TOUR_DETAILS> _morningTourDetail;
            private List<TOUR_DETAILS> _afternoonTourDetail;
            private List<TOUR_DETAILS> _eveningTourDetail;
            private SUPER_TOUR db = new SUPER_TOUR();
            public ICommand AddPackageToTourMorningCommand { get; private set; }
            public ICommand AddPackageToTourAfternoonCommand { get; private set; }
            public ICommand AddPackageToTourEveningCommand { get; private set; }

            public DateActivity(int dateID)
            {
                this._dateID=dateID;
                generateCommand();
            }
            private void generateCommand()
            {
                _morningActivitiesText=new ObservableCollection<string>();
                _afternoonActivitiesText = new ObservableCollection<string>();
                _eveningActivitiesText = new ObservableCollection<string>();

                _morningTourDetail = new List<TOUR_DETAILS>();
                _afternoonTourDetail = new List<TOUR_DETAILS>();
                _eveningTourDetail = new List<TOUR_DETAILS>();
                AddPackageToTourMorningCommand = new RelayCommand(ExecuteAddPackageToTourMorningCommand);
                AddPackageToTourAfternoonCommand = new RelayCommand(ExecuteAddPackageToTourAfternoonCommand);
                AddPackageToTourEveningCommand = new RelayCommand(ExecuteAddPackageToTourEveningCommand);
            }
            private void ExecuteAddPackageToTourMorningCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_morningTourDetail);
                addPackageToTourView.ShowDialog();
                MorningActivities.Clear();
                foreach(TOUR_DETAILS tourDetail in _morningTourDetail)
                {
                    MorningActivities.Add(db.PACKAGEs.Find(tourDetail.Id_Package).Name_Package + $"- Ngày {DateID}");
                }
            }
            private void ExecuteAddPackageToTourAfternoonCommand(object obj)
            {
                MessageBox.Show("Afternoon");

                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_afternoonTourDetail);
                addPackageToTourView.ShowDialog();
                _afternoonActivitiesText.Clear();
                foreach (TOUR_DETAILS tourDetail in _afternoonTourDetail)
                {
                    _afternoonActivitiesText.Add(db.PACKAGEs.Find(tourDetail.Id_Package).Name_Package + $"- Ngày {DateID}");
                }
            }
            private void ExecuteAddPackageToTourEveningCommand(object obj)
            {
                //MessageBox.Show("Evening");
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_eveningTourDetail);
                addPackageToTourView.ShowDialog();
                _eveningActivitiesText.Clear();
                foreach (TOUR_DETAILS tourDetail in _eveningTourDetail)
                {
                    _eveningActivitiesText.Add(db.PACKAGEs.Find(tourDetail.Id_Package).Name_Package + $"- Ngày {DateID}");
                }
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
            public ObservableCollection<string> MorningActivities { get => _morningActivitiesText; set { _morningActivitiesText = value;
                    OnPropertyChanged(nameof(MorningActivities)); } }
            public ObservableCollection<string> AfternoonActivities { get => _afternoonActivitiesText; set { _afternoonActivitiesText = value; OnPropertyChanged(nameof(AfternoonActivities)); } }
            public ObservableCollection<string> EveningActivities { get => _eveningActivitiesText; set { _eveningActivitiesText = value; OnPropertyChanged(nameof(EveningActivities)); } }

        }




        // End test
        private string _nameTour;
        private ObservableCollection<DateActivity> _dateActivityList;
        private int _totalDay;
        private int _totalNight;
        private City _selectedCity;
        private ObservableCollection<City> _listCities;

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
        private void LoadCities()
        {
            ListCities = new ObservableCollection<City>();
            foreach(City city in Get_Api_Address.getCities())
            {
                ListCities.Add(city);
            }
        }
        // End test
        public ICommand AddADayCommand { get; }
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

            // End Test
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
        }

        private void ExecuteAddADayCommand(object obj)
        {
            DateActivity dateActivity = new DateActivity(DateActivityList.Count + 1);
            DateActivityList.Add(dateActivity);
        }
    }
}
