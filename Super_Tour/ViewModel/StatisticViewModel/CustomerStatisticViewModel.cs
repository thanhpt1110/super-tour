using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        #region Declare variable

        private ObservableCollection<CustomerStatistic> _customerStatisticList;
        private SUPER_TOUR db = null;
        private int _totalCustomer;
        private int _totalReBookingCustomer;
        private int _totalTicket;

        #endregion

        #region Declare binding
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
                            TotalCustomer = db.CUSTOMERs.Count();
                            TotalReBookingCustomer = db.BOOKINGs.GroupBy(bd => bd.Id_Customer_Booking).Count(g => g.Count() > 1);
                            TotalTicket = db.TICKETs.Count();

                            CustomerStatisticList = new ObservableCollection<CustomerStatistic>();
                            var sqlQuery = @"
                            SELECT CUSTOMER.Name_Customer AS CustomerName,
                                   COUNT(BOOKING.Id_Booking) AS TotalBooking,
                                   SUM(TOUR.PriceTour * TRAVEL.Discount / 100 * 
                                       (SELECT COUNT(*) FROM TOURIST WHERE TOURIST.Id_Booking = BOOKING.Id_Booking)) AS TotalRevenue
                            FROM CUSTOMER
                            JOIN BOOKING ON CUSTOMER.ID_Customer = BOOKING.Id_Customer_Booking
                            JOIN TRAVEL ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                            JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                            WHERE BOOKING.Status = 'Paid'
                            GROUP BY CUSTOMER.Name_Customer";


                            var result = db.Database.SqlQuery<CustomerStatistic>(sqlQuery);
                            Console.WriteLine(result);
                            foreach (var item in result)
                            {
                                // Tạo một đối tượng CustomerStatistic từ kết quả truy vấn
                                CustomerStatistic customerStatistic = new CustomerStatistic(item.CustomerName, item.TotalBooking, item.TotalRevenue);

                                // Thêm đối tượng CustomerStatistic vào ObservableCollection
                                CustomerStatisticList.Add(customerStatistic);
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
