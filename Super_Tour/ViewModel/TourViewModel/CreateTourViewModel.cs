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
          public class GridActivity
        {
            public TOUR_DETAILS Tour_detail { get; set; }
            public string PackageName { get; set; }

        }
        public class DateActivity:ObservableObject
        {
            private int _dateID;
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

            public DateActivity(int dateID)
            {
                this._dateID=dateID;
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
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_morningTourDetail);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourAfternoonCommand(object obj)
            {
                MessageBox.Show("Afternoon");

                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_afternoonTourDetail);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourEveningCommand(object obj)
            {
                //MessageBox.Show("Evening");
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(_eveningTourDetail);
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
        private string _nameTour;
        private ObservableCollection<DateActivity> _dateActivityList;
        private int _totalDay;
        private int _totalNight;
        private City _selectedCity;
        public ICommand DeletePackageMorningCommand { get; private set; }

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
