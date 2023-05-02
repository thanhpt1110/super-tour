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
    internal class CreateBookingViewModel: ObservableObject
    {
        public class Tourist
        {
            private string _id;
            private string _name;
            public Tourist(string id, string name)
            {
                Id = id;
                Name = name;
            }
            public string Id { get => _id; set => _id = value; }
            public string Name { get => _name; set => _name = value; }
        }
        private ObservableCollection<Tourist> _tourists;
        public ICommand OpenSelectTravelForBookingViewCommand { get; }
        private void ExecuteOpenSelectTravelForBookingViewCommand(object obj)
        {
            SelectTravelForBookingView selectTravelForBookingView = new SelectTravelForBookingView();
            selectTravelForBookingView.ShowDialog();
        }
        public ObservableCollection<Tourist> Tourists 
        { 
            get => _tourists;
            set
            {
                _tourists = value;
                OnPropertyChanged(nameof(Tourists));
            }
        }
        public CreateBookingViewModel()
        {
            OpenSelectTravelForBookingViewCommand = new RelayCommand(ExecuteOpenSelectTravelForBookingViewCommand);
            Tourists = new ObservableCollection<Tourist>();
            Tourist tourist = new Tourist("1", "Tourist 1");
            Tourists.Add(tourist);
            tourist = new Tourist("2", "Tourist 2");
            Tourists.Add(tourist);
            tourist = new Tourist("2", "Tourist 3");
            Tourists.Add(tourist);
        }
    }
}
