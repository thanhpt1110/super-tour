using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Threading;
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

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdateBooking;
        private SUPER_TOUR db = null;
        private ObservableCollection<TravelStatistic> _travelStatisticList;
        private int _totalTravel;
        private int _totalBooking;
        private int _totalCancelBooking;
        private TravelBookingDate _top1TravelBookingDate;
        private TravelBookingDate _top2TravelBookingDate;
        private TravelBookingDate _top3TravelBookingDate;
        private DateTime _startDate;
        private DateTime _endDate;
        private SeriesCollection _bookingSeries;
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
        public SeriesCollection BookingSeries
        {
            get { return _bookingSeries; }
            set
            {
                _bookingSeries = value;
                OnPropertyChanged(nameof(BookingSeries));
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
            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;


            lastUpdateBooking = DateTime.Now;
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
            TotalBooking = db.BOOKINGs.Count();

            TotalTravel = (
                from booking in db.BOOKINGs
                join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                where ticket.Status != "Cancel" && booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
                select booking.Id_Travel
            ).Distinct().Count();

            TotalCancelBooking = (
                from booking in db.BOOKINGs
                join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                where ticket.Status == "Cancel" && booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
                select booking.Id_Travel
            ).Distinct().Count();

            var top3Dates = (from booking in db.BOOKINGs
                             where booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
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

            var sqlQuery = @"SELECT TOUR.Name_Tour AS TravelName, COUNT(BOOKING.Id_Booking) AS TotalBooking, SUM(BOOKING.TotalPrice) AS TotalRevenue
                            FROM TRAVEL
                            JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                            JOIN BOOKING ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                            GROUP BY TOUR.Name_Tour";

            var result = db.Database.SqlQuery<TravelStatistic>(sqlQuery);
            Console.WriteLine(result);

            TravelStatisticList.Clear();
            foreach (var item in result)
            {
                // Tạo một đối tượng TravelStatistic từ kết quả truy vấn
                TravelStatistic travelStatistic = new TravelStatistic(item.TravelName, item.TotalBooking, item.TotalRevenue);

                // Thêm đối tượng TravelStatistic vào ObservableCollection
                TravelStatisticList.Add(travelStatistic);
            }


            BookingSeries = new SeriesCollection();
            Labels = new List<string>();

            var bookingCounts = db.BOOKINGs
                 .Where(b => b.Status == "Paid" && b.Booking_Date >= StartDate && b.Booking_Date <= EndDate) // Lọc theo trạng thái xác nhận (tuỳ vào yêu cầu của bạn)
                 .GroupBy(b => b.Booking_Date) // Nhóm booking theo ngày đặt tour
                 .Select(g => new { Date = g.Key, Count = g.Count() }) // Chọn ngày và số lượng khách hàng
                 .OrderBy(item => item.Date) // Sắp xếp theo ngày tăng dần
                 .ToList();


            // Xử lý dữ liệu để hiển thị trên biểu đồ
            var values = new ChartValues<int>();

            foreach (var item in bookingCounts)
            {
                values.Add(item.Count);
                Labels.Add(item.Date.ToString("dd/MM/yyyy")); // Hàm GetFormattedDate để định dạng ngày theo yêu cầu của bạn
            }

            // Tạo Series và thêm vào SeriesCollection
            var series = new ColumnSeries
            {
                Title = "Number of Customers",
                Values = values
            };
            BookingSeries.Add(series);

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
                    if (DateTime.Parse(_trackerBooking.DateTimeUpdate) > lastUpdateBooking)
                    {
                        lastUpdateBooking = (DateTime.Parse(_trackerBooking.DateTimeUpdate));
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
