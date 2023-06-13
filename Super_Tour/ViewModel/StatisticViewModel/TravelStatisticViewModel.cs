using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Super_Tour.ViewModel.CustomerStatisticViewModel;

namespace Super_Tour.ViewModel
{
    internal class TravelStatisticViewModel: ObservableObject
    {
        #region Declare TravelBookingDate Class

        public class TravelBookingDate: ObservableObject
        {
            private string _date;
            private int _count;
            public TravelBookingDate()
            {

            }
            public TravelBookingDate(string date, int count)
            {
                Date = date;
                Count = count;
            }

            public string Date 
            { 
                get => _date;
                set
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
            public int Count 
            { 
                get => _count; 
                set 
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        #endregion

        #region Declare variable
        private SUPER_TOUR db = null;
        private ObservableCollection<TravelStatistic> _travelStatisticList;
        private int _totalTravel;
        private int _totalBooking;
        private int _totalCancelBooking;
        private TravelBookingDate _top1TravelBookingDate;
        private TravelBookingDate _top2TravelBookingDate;
        private TravelBookingDate _top3TravelBookingDate;
        #endregion

        #region Declare binding

        public ObservableCollection<TravelStatistic> TravelStatisticList
        {
            get => _travelStatisticList;
            set {
                _travelStatisticList = value;
                OnPropertyChanged(nameof(TravelStatisticList));
            }
        }
        public int TotalTravel
        {
            get => _totalTravel;
            set
            {
                _totalTravel = value;
                OnPropertyChanged(nameof(TotalTravel));
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
        public int TotalCancelBooking 
        { 
            get => _totalCancelBooking;
            set
            {
                _totalCancelBooking = value;
                OnPropertyChanged(nameof(TotalCancelBooking));
            }
        }

        public TravelBookingDate Top1TravelBookingDate
        {
            get => _top1TravelBookingDate;
            set
            {
                _top1TravelBookingDate = value;
                OnPropertyChanged(nameof(Top1TravelBookingDate));
            }
        }
        public TravelBookingDate Top2TravelBookingDate
        {
            get => _top2TravelBookingDate;
            set
            {
                _top2TravelBookingDate = value;
                OnPropertyChanged(nameof(Top2TravelBookingDate));
            }
        }
        public TravelBookingDate Top3TravelBookingDate 
        { 
            get => _top3TravelBookingDate; 
            set 
            {
                _top3TravelBookingDate = value;
                OnPropertyChanged(nameof(Top3TravelBookingDate));
            }
        }

        #endregion

        #region Constructor
        public TravelStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            _top1TravelBookingDate = new TravelBookingDate();
            _top2TravelBookingDate = new TravelBookingDate();
            _top3TravelBookingDate = new TravelBookingDate();
            TravelStatisticList = new ObservableCollection<TravelStatistic>();
            LoadDataAsync();
        }
        #endregion

        #region Load Data
        private async Task LoadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TotalBooking = db.BOOKINGs.Count();

                            TotalTravel = (
                                from booking in db.BOOKINGs
                                join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                                join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                                where ticket.Status != "Cancel"
                                select booking.Id_Travel
                            ).Distinct().Count();

                            TotalCancelBooking = (
                                from booking in db.BOOKINGs
                                join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                                join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                                where ticket.Status == "Cancel"
                                select booking.Id_Travel
                            ).Distinct().Count();

                            var top3Dates = (from booking in db.BOOKINGs
                                             group booking by booking.Booking_Date into g
                                             orderby g.Count() descending
                                             select new
                                             {
                                                 Count = g.Count(),
                                                 Date = g.Key,
                                             }).Take(3).ToList()
                                             .Select(x => new TravelBookingDate
                                             {
                                                 Count = x.Count,
                                                 Date = x.Date.ToString("dd-MM-yyyy"),
                                             }).ToList();

                            Top1TravelBookingDate = top3Dates.ElementAtOrDefault(0);
                            Top2TravelBookingDate = top3Dates.ElementAtOrDefault(1);
                            Top3TravelBookingDate = top3Dates.ElementAtOrDefault(2);

                            var sqlQuery = @"
                                SELECT TOUR.Name_Tour AS TravelName, COUNT(BOOKING.Id_Booking) AS TotalBooking, SUM(BOOKING.TotalPrice) AS TotalRevenue
                                FROM TRAVEL
                                JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                                JOIN BOOKING ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                                GROUP BY TOUR.Name_Tour
                                ";

                            var result = db.Database.SqlQuery<TravelStatistic>(sqlQuery);
                            Console.WriteLine(result);
                            foreach (var item in result)
                            {
                                // Tạo một đối tượng TravelStatistic từ kết quả truy vấn
                                TravelStatistic travelStatistic = new TravelStatistic(item.TravelName, item.TotalBooking, item.TotalRevenue);

                                // Thêm đối tượng TravelStatistic vào ObservableCollection
                                TravelStatisticList.Add(travelStatistic);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
    #endregion
    }
}
