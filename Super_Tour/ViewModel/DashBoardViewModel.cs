using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
        private string _mostFrequentlyTour = null;
        private string _totalBooking = null;
        private int _unpaidBooking = 0;
        private DispatcherTimer _timer = null;
        private DateTime _customerDashboard;
        private DateTime _ticketDashboard;
        private DateTime _bookingDashboard;
        private DateTime _travelDashboard;
        private UPDATE_CHECK _trackerCustomer = null;
        private UPDATE_CHECK _trackerTicket = null;
        private UPDATE_CHECK _trackerTravel = null;
        private UPDATE_CHECK _trackerBooking = null;
        #endregion

        #region Declare binding
        public int UnpaidBooking
        {
            get { return _unpaidBooking; }
            set
            {
                _unpaidBooking = value;
                OnPropertyChanged(nameof(UnpaidBooking));
            }
        }

        public string TotalBooking
        {
            get { return _totalBooking; }
            set
            {
                _totalBooking = value;
                OnPropertyChanged(nameof(TotalBooking));    
            }
        }

        public string MostFrequentlyTour
        {
            get { return _mostFrequentlyTour;}
            set
            {
                _mostFrequentlyTour = value;
                OnPropertyChanged(nameof(MostFrequentlyTour));
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
        public ICommand GoToCustomerStatisticCommand { get; }
        public ICommand GoToTravelStatisticCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public DashBoardViewModel(MainViewModel mainViewModel) 
        {
            // Create objects
            db = SUPER_TOUR.db;
            _mainViewModel = mainViewModel;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            Timer.Tick += Timer_Tick;

            // Objects to sync
            _customerDashboard = DateTime.Now;
            _travelDashboard = DateTime.Now;
            _ticketDashboard = DateTime.Now;
            _bookingDashboard = DateTime.Now;
                
            // Create command
            GoToCustomerManagementCommand = new RelayCommand(ExecuteGoToCustomerManagement);
            GoToTravelManagementCommand = new RelayCommand(ExecuteGoToTravelManagement);
            GoToBookingManagementCommand = new RelayCommand(ExecuteGoToBookingManagement);
            GoToRevenueStatisticCommand = new RelayCommand(ExecuteRevenueStatistic);
            GoToTravelStatisticCommand = new RelayCommand(ExecuteTravelStatistic);
            GoToCustomerStatisticCommand = new RelayCommand(ExecuteCustomerStatistic);
            LoadUI();
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
                     if (DateTime.Parse(_trackerCustomer.DateTimeUpdate) > _customerDashboard)
                     {
                         _customerDashboard = (DateTime.Parse(_trackerCustomer.DateTimeUpdate));
                         System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         {
                             LoadCustomerData();
                         });
                     }
                     // Compare ticket
                     _trackerTicket = UPDATE_CHECK.getTracker("UPDATE_TICKET");
                     if (DateTime.Parse(_trackerTicket.DateTimeUpdate) > _ticketDashboard)
                     {
                         _ticketDashboard = (DateTime.Parse(_trackerTicket.DateTimeUpdate));
                         System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         {
                             LoadTicketData();
                         });
                     }
                     // Compare booking
                     _trackerBooking = UPDATE_CHECK.getTracker("UPDATE_BOOKING");
                     if (DateTime.Parse(_trackerBooking.DateTimeUpdate) > _bookingDashboard)
                     {
                         _bookingDashboard = (DateTime.Parse(_trackerBooking.DateTimeUpdate));
                         System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         {
                             LoadBookingData();
                         });
                     }
                     // Compare travel
                     _trackerTravel = UPDATE_CHECK.getTracker("UPDATE_TRAVEL");
                     if (DateTime.Parse(_trackerTravel.DateTimeUpdate) > _travelDashboard)
                     {
                         _travelDashboard = (DateTime.Parse(_trackerTravel.DateTimeUpdate));
                         System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         {
                             LoadTravelData();
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

        #region Load UI 
        public void LoadUI()
        {
            LoadCustomerData();
            LoadTravelData();
            LoadBookingData();
            LoadTicketData();           
        }
        #endregion

        #region Load CUSTOMER data
        private void LoadCustomerData()
        {
            // Total customer
            TotalCustomer = db.CUSTOMERs.Count();
        }
        #endregion

        #region Load TICKET data
        private void LoadTicketData()
        {
            // Total ticket
            TotalTicket = db.TICKETs.Count();
        }
        #endregion

        #region Load BOOKING data
        private void LoadBookingData()
        {
            // Unpaid booking
            UnpaidBooking = db.BOOKINGs.Where(p => p.Status == "Unpaid").Count();

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

            // Total booking
            TotalBooking = db.BOOKINGs.Count().ToString();
        }
        #endregion

        #region Load TRAVEL data
        private void LoadTravelData()
        {
            // Total travel
            TotalTravel = db.TRAVELs.Count();

            // Most freq tour
            var tour = db.TRAVELs.GroupBy(b => b.Id_Tour).OrderByDescending(g => g.Count()).FirstOrDefault();
            if (tour != null)
            {
                int idTour = tour.Key;
                var mostTour = db.TOURs.FirstOrDefault(c => c.Id_Tour == idTour);
                MostFrequentlyTour = mostTour.Name_Tour;
            }
            else
                MostFrequentlyTour = "Not available!";
        }
        #endregion

        #region Button navigation
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

        private void ExecuteCustomerStatistic(object obj)
        {
            _mainViewModel.GoToCustomertatistic();
        }

        private void ExecuteTravelStatistic(object obj)
        {
            _mainViewModel.GoToTravelStatistic();
        }
        #endregion

    }
}
