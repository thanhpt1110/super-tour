using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateTourViewModel: ObservableObject
    {
        //Test
        public class DateActivity
        {
            private string _dateID;
            private List<string> _morningActivities;
            private List<string> _afternoonActivities;
            private List<string> _eveningActivities;
            public ICommand AddPackageToTourCommand { get; }
            public DateActivity()
            {
                AddPackageToTourCommand = new RelayCommand(ExecuteAddPackageToTourCommand);
            }
            private void ExecuteAddPackageToTourCommand(object obj)
            {
                AddPackageToTourView addPackageToTourView = new AddPackageToTourView();
                addPackageToTourView.ShowDialog();
            }
            public DateActivity(string dateID, List<string> morningActivities, List<string> afternoonActivities, List<string> eveningActivities)
            {
                AddPackageToTourCommand = new RelayCommand(ExecuteAddPackageToTourCommand);
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
        private ObservableCollection<DateActivity> _dateActivityList;
        public ObservableCollection<DateActivity> DateActivityList
        {
            get => _dateActivityList;
            set
            {
                _dateActivityList = value;
                OnPropertyChanged(nameof(DateActivityList));
            }
        }
        // End test
        public ICommand AddADayCommand { get; }
        public CreateTourViewModel()
        {
            // Test 
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
            // End Test
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
        }

        private void ExecuteAddADayCommand(object obj)
        {
            List<string> morAct = new List<string>();
            List<string> afterAct = new List<string>();
            List<string> nightAct = new List<string>();
            DateActivity dateActivity = new DateActivity("Date ID",morAct,afterAct,nightAct);
            DateActivityList.Add(dateActivity);
        }
    }
}
