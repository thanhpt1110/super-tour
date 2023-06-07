using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;

namespace Super_Tour.ViewModel
{
    internal class DashBoardViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel = null;
        private static DateTime _timeDashboard;
        private UPDATE_CHECK _tracker = null;
        private int _totalCustomer = 0;
        private string _mostFrequentlyBookingCustomer = null;
        private string _totalRevenue = null;
        private int _totalTicket = 0;
        private int _totalTravel = 0;
        private string _mostFrequentlyProvince = null;
        private string _totalBooking = null;
        private DispatcherTimer Timer = null;
        private string table = "UPDATE_DASHBOARD";
        #endregion

        #region Declare binding
        public string TotalBooking
        {
            get { return _totalBooking; }
            set
            {
                _totalBooking = value;
                OnPropertyChanged(nameof(TotalBooking));    
            }
        }

        public string MostFrequentlyProvince
        {
            get { return _mostFrequentlyProvince;}
            set
            {
                _mostFrequentlyProvince = value;
                OnPropertyChanged(nameof(MostFrequentlyProvince));
            }
        }

        public int TotalTravel
        {
            get { return _totalTravel; }
            set
            {
                _totalTravel = value;
                OnPropertyChanged(nameof(TotalTravel)); 
            }
        }

        public int TotalTicket
        {
            get { return _totalTicket; }
            set
            {
                _totalTicket = value;
                OnPropertyChanged(nameof(TotalTicket));
            }
        }

        public string TotalRevenue
        {
            get { return _totalRevenue; }
            set
            {
                _totalRevenue = value;
                OnPropertyChanged(nameof(TotalRevenue));    
            }
        }

        public string MostFrequentlyBookingCustomer
        {
            get { return _mostFrequentlyBookingCustomer;}
            set
            {
                _mostFrequentlyBookingCustomer = value;
                OnPropertyChanged(nameof(MostFrequentlyBookingCustomer));
            }
        }

        public int TotalCustomer
        {
            get { return _totalCustomer; }
            set
            {
                _totalCustomer = value;
                OnPropertyChanged(nameof(TotalCustomer));
            }
        }
        #endregion

        #region Command
        public ICommand GoToCustomerManagementCommand { get; }
        public ICommand GoToTravelManagementCommand { get; }
        public ICommand GoToBookingManagementCommand { get; }
        public ICommand GoToRevenueStatisticCommand { get; }
        #endregion

        #region Constructor
        public DashBoardViewModel(MainViewModel mainViewModel) 
        {
            // Create objects
            db = SUPER_TOUR.db;
            _mainViewModel = mainViewModel;
            _timeDashboard = DateTime.Now;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            //Timer.Tick += Timer_Tick;

            // Create command
            GoToCustomerManagementCommand = new RelayCommand(ExecuteGoToCustomerManagement);
            GoToTravelManagementCommand = new RelayCommand(ExecuteGoToTravelManagement);
            GoToBookingManagementCommand = new RelayCommand(ExecuteGoToBookingManagement);
            GoToRevenueStatisticCommand = new RelayCommand(ExecuteRevenueStatistic);

            LoadUI();
        }

        #region Check data persecond
       /* private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    _tracker = UPDATE_CHECK.getTracker(table);
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > _timeDashboard)
                    {
                        _timeDashboard = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }*/
        #endregion

        #region Load UI 
        public void LoadUI()
        {
            // Total customer
            TotalCustomer = db.CUSTOMERs.Count();

            //Most freq customer
            var result = db.BOOKINGs.GroupBy(b => b.Id_Customer_Booking).OrderByDescending(g => g.Count()).FirstOrDefault();
            if (result != null)
            {
                int idCustomer = result.Key;
                int numBookings = result.Count();
                // lấy thông tin khách hàng có số booking lớn nhất
                var customer = db.CUSTOMERs.FirstOrDefault(c => c.Id_Customer == idCustomer);
                MostFrequentlyBookingCustomer = customer.Name_Customer;
            }
            else
                MostFrequentlyBookingCustomer = "0";

            // Total revenue
            if (db.BOOKINGs.Count() > 0)
            {
                decimal totalPriceSum = db.BOOKINGs.Sum(b => b.TotalPrice);
                TotalRevenue = totalPriceSum.ToString("#,#") + " VND";
            }
            else 
                TotalRevenue = "0 VND";

            // Total ticket
            TotalTicket = db.TICKETs.Count();   

            // Total travel
            TotalTravel =  db.TRAVELs.Count();

            // Most freq province

            // Total booking
            TotalBooking = db.BOOKINGs.Count().ToString();
        }
        #endregion

        #region Button naviagation
        private void ExecuteGoToCustomerManagement(object obj)
        {
            _mainViewModel.GoToCustomerManagement();
        }

        private void ExecuteGoToBookingManagement(object obj)
        {
            _mainViewModel.GoToBookingManagement(); 
        }

        private void ExecuteGoToTravelManagement(object obj)
        {
            _mainViewModel.GoToTravelManagement();
        }

        private void ExecuteRevenueStatistic(object obj)
        {
            _mainViewModel.GoToRevenueStatistic();
        }
        #endregion
        #endregion
    }
}
