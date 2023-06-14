using LiveCharts;
using LiveCharts.Wpf;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Super_Tour.ViewModel.TravelStatisticViewModel;

namespace Super_Tour.ViewModel
{
    internal class RevenueStatisticViewModel : ObservableObject
    {
        #region Declare RevenueDate class
        public class RevenueDate: ObservableObject
        {
            private string _date;
            private decimal _revenue;
            public RevenueDate()
            {

            }
            public RevenueDate(string date, decimal revenue)
            {
                Date = date;
                Revenue = revenue;
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
            public decimal Revenue 
            { 
                get => _revenue;
                set
                {
                    _revenue = value;
                    OnPropertyChanged(nameof(Revenue));
                }
            }
        }
        #endregion

        #region Declare variable

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerTicket = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdateBooking;
        private DateTime lastUpdateTicket;
        private SUPER_TOUR db = null;
        private ObservableCollection<TravelStatistic> _travelStatisticList;
        private decimal _totalRevenue;
        private decimal _totalCancelMoney;
        private decimal _totalTourist;
        private RevenueDate _top1RevenueDate;
        private RevenueDate _top2RevenueDate;
        private RevenueDate _top3RevenueDate;
        private DateTime _startDate;
        private DateTime _endDate;
        private SeriesCollection _revenueSeries;
        private List<string> _labels;

        #endregion

        #region Declare binding
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                LoadChart();
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                LoadChart();
            }
        }
        public SeriesCollection RevenueSeries
        {
            get { return _revenueSeries; }
            set
            {
                _revenueSeries = value;
                OnPropertyChanged(nameof(RevenueSeries));
            }
        }

        public List<string> Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }
        public ObservableCollection<TravelStatistic> TravelStatisticList
        {
            get => _travelStatisticList;
            set
            {
                _travelStatisticList = value;
                OnPropertyChanged(nameof(TravelStatisticList));
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
        public decimal TotalCancelMoney 
        { 
            get => _totalCancelMoney;
            set
            {
                _totalCancelMoney = value;
                OnPropertyChanged(nameof(TotalCancelMoney));
            }
        }
        public decimal TotalTourist
        {
            get => _totalTourist;
            set
            {
                _totalTourist = value;
                OnPropertyChanged(nameof(TotalTourist));
            }
        }

        public RevenueDate Top1RevenueDate 
        { 
            get => _top1RevenueDate;
            set
            {
                _top1RevenueDate = value;
                OnPropertyChanged(nameof(Top1RevenueDate));
            }
        }
        public RevenueDate Top2RevenueDate 
        { 
            get => _top2RevenueDate;
            set
            {
                _top2RevenueDate = value;
                OnPropertyChanged(nameof(Top2RevenueDate));
            }
        }
        public RevenueDate Top3RevenueDate
        {
            get => _top3RevenueDate;
            set
            {
                _top3RevenueDate = value;
                OnPropertyChanged(nameof(Top3RevenueDate));
            }
        }

        #endregion

        #region Constructor

        public RevenueStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            TravelStatisticList = new ObservableCollection<TravelStatistic>();
            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;

            lastUpdateBooking = DateTime.Now;
            lastUpdateTicket = DateTime.Now;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            Timer.Tick += Timer_Tick;

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
                            LoadChart();
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
        private void LoadChart()
        {
            var sqlQueryTotalRevenue = @"
                                SELECT SUM(BOOKING.TotalPrice) 
                                FROM BOOKING
                                WHERE BOOKING.Status='Paid'";
            var totalRevenue = db.Database.SqlQuery<Decimal>(sqlQueryTotalRevenue);
            TotalRevenue = totalRevenue.ElementAtOrDefault(0);

            var sqlQueryTotalCancelMoney = @"SELECT SUM(TOUR.PriceTour) * COUNT(DISTINCT TOURIST.Id_Tourist)
                                FROM BOOKING
                                JOIN TOURIST ON TOURIST.Id_Booking = BOOKING.Id_Booking
                                JOIN TICKET ON TICKET.Id_Tourist = TOURIST.Id_Tourist
                                JOIN TRAVEL ON TRAVEL.Id_Travel = BOOKING.Id_Travel
                                JOIN TOUR ON TOUR.Id_Tour = TRAVEL.Id_Tour
                                WHERE TICKET.Status = 'Canceled'
                                GROUP BY BOOKING.Id_Booking";

            TotalCancelMoney = db.Database.SqlQuery<Decimal>(sqlQueryTotalCancelMoney).ElementAtOrDefault(0);


            TotalTourist = db.TOURISTs.Count();

            var sqlTravelStatisticQuery = @"
                                SELECT TOUR.Name_Tour AS TravelName, COUNT(BOOKING.Id_Booking) AS TotalBooking, SUM(BOOKING.TotalPrice) AS TotalRevenue
                                FROM TRAVEL
                                JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                                JOIN BOOKING ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                                GROUP BY TOUR.Name_Tour
                                ";

            var result = db.Database.SqlQuery<TravelStatistic>(sqlTravelStatisticQuery);

            TravelStatisticList.Clear();
            foreach (var item in result)
            {
                // Tạo một đối tượng CustomerStatistic từ kết quả truy vấn
                TravelStatistic travelStatistic = new TravelStatistic(item.TravelName, item.TotalBooking, item.TotalRevenue);

                // Thêm đối tượng CustomerStatistic vào ObservableCollection
                TravelStatisticList.Add(travelStatistic);
            }

            var top3Dates = (from booking in db.BOOKINGs
                             group booking by booking.Booking_Date into g
                             orderby g.Sum(x => x.TotalPrice) descending
                             select new
                             {
                                 Revenue = g.Sum(x => x.TotalPrice),
                                 Date = g.Key,
                             }).Take(3).ToList()
                             .Select(x => new RevenueDate
                             {
                                 Revenue = x.Revenue,
                                 Date = x.Date.ToString("dd-MM-yyyy"),
                             }).ToList();

            Top1RevenueDate = top3Dates.ElementAtOrDefault(0);
            Top2RevenueDate = top3Dates.ElementAtOrDefault(1);
            Top3RevenueDate = top3Dates.ElementAtOrDefault(2);

            RevenueSeries = new SeriesCollection();
            Labels = new List<string>();

            var revenueByDate = db.BOOKINGs
                 .Where(b => b.Booking_Date >= StartDate && b.Booking_Date <= EndDate)
                 .GroupBy(b => b.Booking_Date)
                 .Select(g => new { Date = g.Key, TotalMoney = g.Sum(b => b.TotalPrice) })
                 .ToList();


            var values = new ChartValues<decimal>();

            foreach (var item in revenueByDate)
            {
                values.Add(item.TotalMoney);
                Labels.Add(item.Date.ToString("dd/MM/yyyy"));

                
            }
            var series = new ColumnSeries
            {
                Title = "Revenue by day",
                Values = values
            };
            RevenueSeries.Add(series);

        }
        #endregion

        #region Check data persecond
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Compare Booking & Ticket
                    _trackerBooking = UPDATE_CHECK.getTracker("UPDATE_BOOKING");
                    _trackerTicket = UPDATE_CHECK.getTracker("UPDATE_TICKET");
                    if (DateTime.Parse(_trackerBooking.DateTimeUpdate) > lastUpdateBooking)
                    {
                        lastUpdateBooking = (DateTime.Parse(_trackerBooking.DateTimeUpdate));
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadDataAsync();
                        });
                    }
                    else if(DateTime.Parse(_trackerTicket.DateTimeUpdate) > lastUpdateTicket)
                    {
                        lastUpdateTicket = (DateTime.Parse(_trackerTicket.DateTimeUpdate));
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadDataAsync();
                        });
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
