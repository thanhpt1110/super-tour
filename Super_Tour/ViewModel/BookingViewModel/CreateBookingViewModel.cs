using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
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
        private TRAVEL _travel;
        private SUPER_TOUR db = new SUPER_TOUR();
       
        private ObservableCollection<TOURIST> _tourists;
        public ICommand OpenSelectTravelForBookingViewCommand { get; }
        public ICommand OpenAddTouristForBookingViewCommand { get; }

        public TRAVEL Travel
        {
            get { return _travel; }
            set
            {
                _travel = value;
                OnPropertyChanged(nameof(Travel));
            }
        }
        public ObservableCollection<TOURIST> Tourists 
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
            Tourists = new ObservableCollection<TOURIST>();
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
 /*           Tourist tourist = new Tourist("1", "Tourist 1");
            Tourists.Add(tourist);
            tourist = new Tourist("2", "Tourist 2");
            Tourists.Add(tourist);
            tourist = new Tourist("2", "Tourist 3");
            Tourists.Add(tourist);*/
        }
        private void ExecuteOpenAddTouristForBookingViewCommand(object obj)
        {
            AddTouristView view = new AddTouristView();
            view.DataContext = new AddTouristViewModel(Tourists);
            view.ShowDialog();
        }
        private void ExecuteOpenSelectTravelForBookingViewCommand(object obj)
        {
            _travel = new TRAVEL();
            SelectTravelForBookingView selectTravelForBookingView = new SelectTravelForBookingView();
            selectTravelForBookingView.DataContext = new SelectTravelForBookingViewModel(_travel);
            selectTravelForBookingView.ShowDialog();
            Travel = db.TRAVELs.Find(_travel.Id_Travel);
            
        }
    }
}
