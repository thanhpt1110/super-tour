using Student_wpf_application.ViewModels.Command;
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
        public class DateActivity
        {
            private string _dateID;
            private List<string> _morningActivities;
            private List<string> _afternoonActivities;
            private List<string> _eveningActivities;
            public ICommand AddPackageToTourMorningCommand { get; private set; }
            public ICommand AddPackageToTourAfternoonCommand { get; private set; }
            public ICommand AddPackageToTourEveningCommand { get; private set; }

            public DateActivity()
            {

                generateCommand();
            }
            private void generateCommand()
            {
                AddPackageToTourMorningCommand = new RelayCommand(ExecuteAddPackageToTourMorningCommand);
                AddPackageToTourAfternoonCommand = new RelayCommand(ExecuteAddPackageToTourAfternoonCommand);
                AddPackageToTourEveningCommand = new RelayCommand(ExecuteAddPackageToTourEveningCommand);
            }
            private void ExecuteAddPackageToTourMorningCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(Constant.MORNING);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourAfternoonCommand(object obj)
            {
                MessageBox.Show("Afternoon");

                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(Constant.AFTERNOON);
                addPackageToTourView.ShowDialog();
            }
            private void ExecuteAddPackageToTourEveningCommand(object obj)
            {
                //MessageBox.Show("Evening");
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.DataContext = new AddPackageToTourViewModel(Constant.EVENING);

                addPackageToTourView.ShowDialog();
            }
            public DateActivity(string dateID, List<string> morningActivities, List<string> afternoonActivities, List<string> eveningActivities)
            {
                generateCommand();
                _dateID = dateID;
                _morningActivities = morningActivities;
                _afternoonActivities = afternoonActivities;
                _eveningActivities = eveningActivities;
            }
            public string DateID { get => _dateID; set => _dateID = value; }
            public List<string> MorningActivities { get => _morningActivities; set => _morningActivities = value; }
            public List<string> AfternoonActivities { get => _afternoonActivities; set => _afternoonActivities = value; }
            public List<string> EveningActivities { get => _eveningActivities; set => _eveningActivities = value; }

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
        public CreateTourViewModel()
        {
            // Test 

            LoadCities();
            //SelectedCity = Get_Api_Address.getCities().Where(p => p.code == 1).SingleOrDefault();
            DateActivityList = new ObservableCollection<DateActivity>();
            List<string> morAct1 = new List<string>();
            List<string> afterAct1 = new List<string>();
            List<string> nightAct1 = new List<string>();
            morAct1.Add("Hoạt động sáng 1 - Ngày 1");
            morAct1.Add("Hoạt động sáng 2 - Ngày 1");
            morAct1.Add("Hoạt động sáng 3 - Ngày 1");
            morAct1.Add("Hoạt động sáng 4 - Ngày 1");
            morAct1.Add("Hoạt động sáng 5 - Ngày 1");
            afterAct1.Add("Hoạt động chiều 1 - Ngày 1");
            afterAct1.Add("Hoạt động chiều 2 - Ngày 1");
            afterAct1.Add("Hoạt động chiều 3 - Ngày 1");
            nightAct1.Add("Hoạt động tối 1 - Ngày 1");
            nightAct1.Add("Hoạt động tối 2 - Ngày 1");
            nightAct1.Add("Hoạt động tối 3 - Ngày 1");
            DateActivity dateActivity1 = new DateActivity("Lịch trình ngày 1", morAct1, afterAct1, nightAct1);
            DateActivityList.Add(dateActivity1);

            List<string> morAct2 = new List<string>();
            List<string> afterAct2 = new List<string>();
            List<string> nightAct2 = new List<string>();
            morAct2.Add("Hoạt động sáng 1 - Ngày 2");
            morAct2.Add("Hoạt động sáng 2 - Ngày 2");
            morAct2.Add("Hoạt động sáng 3 - Ngày 2");
            afterAct2.Add("Hoạt động chiều 1 - Ngày 2");
            afterAct2.Add("Hoạt động chiều 2 - Ngày 2");
            afterAct2.Add("Hoạt động chiều 3 - Ngày 2");
            nightAct2.Add("Hoạt động tối 1 - Ngày 2");
            nightAct2.Add("Hoạt động tối 2 - Ngày 2");
            nightAct2.Add("Hoạt động tối 3 - Ngày 2");
            DateActivity dateActivity2 = new DateActivity("Lịch trình ngày 2", morAct2, afterAct2, nightAct2);
            DateActivityList.Add(dateActivity2);

            List<string> morAct3 = new List<string>();
            List<string> afterAct3 = new List<string>();
            List<string> nightAct3 = new List<string>();
            morAct3.Add("Hoạt động sáng 1 - Ngày 3");
            morAct3.Add("Hoạt động sáng 2 - Ngày 3");
            morAct3.Add("Hoạt động sáng 3 - Ngày 3");
            afterAct3.Add("Hoạt động chiều 1 - Ngày 3");
            afterAct3.Add("Hoạt động chiều 2 - Ngày 3");
            afterAct3.Add("Hoạt động chiều 3 - Ngày 3");
            nightAct3.Add("Hoạt động tối 1 - Ngày 3");
            nightAct3.Add("Hoạt động tối 2 - Ngày 3");
            nightAct3.Add("Hoạt động tối 3 - Ngày 3");
            DateActivity dateActivity3 = new DateActivity("Lịch trình ngày 3", morAct3, afterAct3, nightAct3);
            DateActivityList.Add(dateActivity3);
            TotalNight = (DateActivityList.Count - 1);
            TotalDay = DateActivityList.Count;

            // End Test
        }
    }
}
