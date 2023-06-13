using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.ViewModel
{    public class TravelStatistic : ObservableObject
    {
        private string _travelName;
        private int _totalBooking;
        private decimal _totalRevenue;
        public TravelStatistic()
        {

        }
        public TravelStatistic(string travelName, int totalBooking, decimal totalRevenue)
        {
            TravelName = travelName;
            TotalBooking = totalBooking;
            TotalRevenue = totalRevenue;
        }

        public string TravelName
        {
            get => _travelName;
            set
            {
                _travelName = value;
                OnPropertyChanged(nameof(TravelName));
            }
        }
        public int TotalBooking
        {
            get => _totalBooking;
            set
            {
                _totalBooking = value;
                OnPropertyChanged(nameof(TotalBooking));
            }
        }
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged(nameof(TotalRevenue));
            }
        }
    }
}
