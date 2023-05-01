using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Super_Tour.View;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace Super_Tour.ViewModel
{
    internal class SelectTourForTravelViewModel: ObservableObject
    {
        //Test
        public class Tour
        {
            private string _tourID;
            private string _tourName;
            private int _totalDay;
            private int _totalNight;
            private decimal _price;
            public Tour(string tourID, string tourName, int totalDay, int totalNight, decimal price)
            {
                TourID = tourID;
                TourName = tourName;
                TotalDay = totalDay;
                TotalNight = totalNight;
                Price = price;
            }

            public string TourID { get => _tourID; set => _tourID = value; }
            public string TourName { get => _tourName; set => _tourName = value; }
            public int TotalDay { get => _totalDay; set => _totalDay = value; }
            public int TotalNight { get => _totalNight; set => _totalNight = value; }
            public decimal Price { get => _price; set => _price = value; }
        }
        private ObservableCollection<Tour> _tours;
        public ObservableCollection<Tour> Tours 
        { 
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }
        //End test
        public SelectTourForTravelViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            Tour tour = new Tour("1", "Tour 1", 3, 2, 2000000);
            Tours.Add(tour);
            tour = new Tour("2", "Tour 2", 4, 3, 3000000);
            Tours.Add(tour);
            tour = new Tour("3", "Tour 3", 2, 3, 2000000);
            Tours.Add(tour);
        }
    }
}
