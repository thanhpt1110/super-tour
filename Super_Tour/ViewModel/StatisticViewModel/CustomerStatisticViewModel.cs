using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Super_Tour.ViewModel
{
    internal class CustomerStatisticViewModel: ObservableObject
    {
        #region Define CustomerStatisticClass

        public class CustomerStatistic: ObservableObject
        {
            private string _customerName;
            private int _totalBooking;
            private decimal _totalRevenue;
            public CustomerStatistic()
            {
                // Constructor mặc định
            }
            public CustomerStatistic(string customerName, int totalBooking, decimal totalRevenue)
            {
                CustomerName = customerName;
                TotalBooking = totalBooking;
                TotalRevenue = totalRevenue;
            }

            public string CustomerName 
            { 
                get => _customerName;
                set
                {
                    _customerName = value;
                    OnPropertyChanged(nameof(CustomerName));
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

        #endregion

        #region Define CustomerChart Class
        public class CustomerChart: ObservableObject
        {
            private DateTime _bookingDate;
            private int _customerCount;

            public CustomerChart()
            {

            }
            public CustomerChart(DateTime bookingDate, int customerCount)
            {
                _bookingDate = bookingDate;
                _customerCount = customerCount;
            }

            public DateTime BookingDate
            { 
                get => _bookingDate;
                set
                {
                    _bookingDate = value;
                    OnPropertyChanged(nameof(BookingDate));
                }
            }
            public int CustomerCount
            {
                get => _customerCount;
                set
                {
                    _customerCount = value;
                    OnPropertyChanged(nameof(CustomerCount));
                }
            }
        }


        #endregion

        #region Declare variable

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerCustomer = null;
        private UPDATE_CHECK _trackerTicket = null;
        private UPDATE_CHECK _trackerTravel = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdate;
        private ObservableCollection<CustomerStatistic> _customerStatisticList;
        private SUPER_TOUR db = null;
        private int _totalCustomer;
        private int _totalReBookingCustomer;
        private int _totalTicket;
        private DateTime _startDate;

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

        private DateTime _endDate;
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
        private SeriesCollection _customerSeries;
        private List<string> _labels;

        public SeriesCollection CustomerSeries
        {
            get { return _customerSeries; }
            set
            {
                _customerSeries = value;
                OnPropertyChanged(nameof(CustomerSeries));
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
        public int TotalCustomer 
        { 
            get => _totalCustomer;
            set
            {
                _totalCustomer = value;
                OnPropertyChanged(nameof(TotalCustomer));
            }
        }
        public int TotalReBookingCustomer 
        { 
            get => _totalReBookingCustomer;
            set
            {
                _totalReBookingCustomer = value;
                OnPropertyChanged(nameof(TotalReBookingCustomer));
            }
        }
        public int TotalTicket 
        { 
            get => _totalTicket;
            set
            {
                _totalTicket = value;
               OnPropertyChanged(nameof(TotalTicket));
            }
        }

        public ObservableCollection<CustomerStatistic> CustomerStatisticList 
        { 
            get => _customerStatisticList;
            set
            {
                _customerStatisticList = value;
                OnPropertyChanged(nameof(CustomerStatisticList));
            }
        }

        #endregion

        #region Constructor
        public CustomerStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            CustomerStatisticList = new ObservableCollection<CustomerStatistic>();

            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;

            lastUpdate = DateTime.Now;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            Timer.Tick += Timer_Tick;
            

            LoadDataAsync();

        }
        #endregion

        #region LoadData
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
        #endregion

        #region LoadChart
        private void LoadChart()
        {
            TotalCustomer = db.CUSTOMERs.Count();
            TotalReBookingCustomer = db.BOOKINGs.GroupBy(bd => bd.Id_Customer_Booking).Count(g => g.Count() > 1);
            TotalTicket = db.TICKETs.Count();
            var startDateParam = new SqlParameter("@StartDate", StartDate);
            var endDateParam = new SqlParameter("@EndDate", EndDate);
            
            var sqlQuery = @"
                            SELECT CUSTOMER.Name_Customer AS CustomerName,
                                COUNT(BOOKING.Id_Booking) AS TotalBooking,
                                SUM(BOOKING.TotalPrice * (1 - TRAVEL.Discount / 100))  AS TotalRevenue
                                FROM CUSTOMER
                                JOIN BOOKING ON CUSTOMER.ID_Customer = BOOKING.Id_Customer_Booking
                                JOIN TRAVEL ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                                JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                                WHERE BOOKING.Status = 'Paid'
                                GROUP BY CUSTOMER.Name_Customer";


            var result = db.Database.SqlQuery<CustomerStatistic>(sqlQuery);
            Console.WriteLine(result);

            CustomerStatisticList.Clear();  
            foreach (var item in result)
            {
                // Tạo một đối tượng CustomerStatistic từ kết quả truy vấn
                CustomerStatistic customerStatistic = new CustomerStatistic(item.CustomerName, item.TotalBooking, item.TotalRevenue);

                // Thêm đối tượng CustomerStatistic vào ObservableCollection
                CustomerStatisticList.Add(customerStatistic);
            }

            //Load Chart Data
            // Khởi tạo SeriesCollection và Labels
            CustomerSeries = new SeriesCollection();
            Labels = new List<string>();


            var customerCounts = db.BOOKINGs
                 .Where(b => b.Booking_Date >= StartDate && b.Booking_Date <= EndDate)
                 .GroupBy(b => b.Booking_Date)
                 .Select(g => new { Date = g.Key, Count = g.Select(b => b.Id_Customer_Booking).Distinct().Count() })
                 .OrderBy(item => item.Date)
                 .ToList();

            // Xử lý dữ liệu để hiển thị trên biểu đồ
            var values = new ChartValues<int>();

            foreach (var item in customerCounts)
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


            CustomerSeries.Add(series);
        }
        #endregion

        #region Check data persecond
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Compare customer
                    _trackerCustomer = UPDATE_CHECK.getTracker("UPDATE_CUSTOMER");
                    if (DateTime.Parse(_trackerCustomer.DateTimeUpdate) > lastUpdate)
                    {
                        lastUpdate = (DateTime.Parse(_trackerCustomer.DateTimeUpdate));
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
