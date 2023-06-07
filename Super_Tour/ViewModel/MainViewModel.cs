using FontAwesome.Sharp;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Super_Tour.Ultis;
using Microsoft.Extensions.Caching.Memory;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;
using System.Data.Entity.Migrations.Builders;
using Super_Tour.View;
using System.Windows;
using System.Windows.Media.Imaging;
using Super_Tour.CustomControls;
using System.Windows.Documents;

namespace Super_Tour.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        #region Declare variable
        public static ObservableObject _currentChildView;
        private string _nextChildCaption1;
        private bool _haveOneChild = false;
        private string _caption;
        private bool _visibilityAccount = false;
        private bool _visibilityTour = false;
        private bool _visibilityTravel = false;
        private bool _visibilityPackage = false;
        private bool _visibilityPackageType = false;
        private IconChar _icon;
        private Brush childCaptionColor;
        private Brush nextChildCaption1Color;
        private TechnicalHelpViewModel _technicalHelpViewModel = null;
        private DashBoardViewModel _dashBoardViewModel = null;
        private MainTravelViewModel _mainTravelViewModel = null;
        private MainBookingViewModel _mainBookingViewModel = null;
        private MainCustomerViewModel _mainCustomerViewModel = null;
        private MainTicketViewModel _mainTicketViewModel = null;
        private MainTourViewModel _mainTourViewModel = null;
        private MainPackageViewModel _mainPackageViewModel = null;
        private MainPackageTypeViewModel _mainPackageTypeViewModel = null;
        private MainAccountViewModel _mainAccountViewModel = null;
        private CustomerStatisticViewModel _customerStatisticViewModel = null;
        private RevenueStatisticViewModel _revenueStatisticViewModel = null;
        private TravelStatisticViewModel _travelStatisticViewModel = null;
        private DispatcherTimer _timer = null;
        private ACCOUNT _currentUser = null;
        private bool _customerChecked = false;
        private bool _travelChecked = false;
        private bool _revenueChecked = false;
        private bool _bookingChecked = false;
        #endregion

        #region Declare binding
        public bool BookingChecked
        {
            get { return _bookingChecked; }
            set
            {
                _bookingChecked = value;
                OnPropertyChanged(nameof(BookingChecked));  
            }
        }

        public bool RevenueChecked
        {
            get { return _revenueChecked; }
            set
            {
                _revenueChecked = value;
                OnPropertyChanged(nameof(RevenueChecked));
            }
        }

        public bool TravelChecked
        {
            get { return _travelChecked; }
            set
            {
                _travelChecked = value;
                OnPropertyChanged(nameof(TravelChecked));
            }
        }

        public bool CustomerChecked
        {
            get { return _customerChecked; }
            set
            {
                _customerChecked = value;
                OnPropertyChanged(nameof(CustomerChecked));
            }
        }

        public bool VisibilityTour
        {
            get { return _visibilityTour; }
            set
            {
                _visibilityTour = value;
                OnPropertyChanged(nameof(VisibilityTour));  
            }
        }
        
        public bool VisibilityTravel
        {
            get { return _visibilityTravel; }
            set
            {
                _visibilityTravel = value;
                OnPropertyChanged(nameof(VisibilityTravel));
            }
        }

        public bool VisibilityPackage
        {
            get { return _visibilityPackage; }
            set
            {
                _visibilityPackage = value;
                OnPropertyChanged(nameof(VisibilityPackage));
            }
        }

        public bool VisibilityPackageType
        {
            get { return _visibilityPackageType; }
            set
            {
                _visibilityPackage = value;
                OnPropertyChanged(nameof(VisibilityPackageType));
            }
        }

        public bool VisibilityAccount
        {
            get { return _visibilityAccount; }
            set 
            { 
                _visibilityAccount = value; 
                OnPropertyChanged(nameof(VisibilityAccount));   
            }
        }

        public ACCOUNT CurrentUser
        {
            get { return _currentUser; }
            set 
            { 
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                ReloadNewLogin();
            }
        }

        public ObservableObject CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }
        public string NextChildCaption1
        {
            get
            {
                return _nextChildCaption1;
            }
            set
            {
                _nextChildCaption1 = value;
                OnPropertyChanged(nameof(NextChildCaption1));
            }
        }
        public bool HaveOneChild
        {
            get => _haveOneChild;
            set
            {
                _haveOneChild = value;
                OnPropertyChanged(nameof(HaveOneChild));
            }
        }
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public Brush ChildCaptionColor
        {
            get => childCaptionColor;
            set
            {
                childCaptionColor = value;
                OnPropertyChanged(nameof(ChildCaptionColor));
            }
        }
        public Brush NextChildCaption1Color
        {
            get => nextChildCaption1Color;
            set
            {
                nextChildCaption1Color = value;
                OnPropertyChanged(nameof(NextChildCaption1Color));
            }
        }
        #endregion

        #region Command
        public RelayCommand ShowDashboardViewCommand { get; }
        public RelayCommand ShowTravelViewCommand { get; }
        public RelayCommand ShowBookingViewCommand { get; }
        public RelayCommand ShowCustomerViewCommand { get; }
        public RelayCommand ShowTicketViewCommand { get; }
        public RelayCommand ShowTourViewCommand { get; }
        public RelayCommand ShowPackageViewCommand { get; }
        public RelayCommand ShowPackageTypeViewCommand { get; }
        public RelayCommand ShowStatisticViewCommand { get; }
        public RelayCommand ShowCustomerStatisticViewCommand { get; }
        public RelayCommand ShowRevenueStatisticViewCommand { get; }
        public RelayCommand ShowTravelStatisticViewCommand { get; }
        public RelayCommand ShowAccountViewCommand { get; }
        public RelayCommand ShowTechnicalHelpViewCommand { get; }
        public ICommand BackToPreviousChildCommand { get; }
        public ICommand SignOutCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            ShowDashboardViewCommand = new RelayCommand(ExecuteShowDashboardViewCommand);
            ShowTravelViewCommand = new RelayCommand(ExecuteShowTravelViewCommand);
            ShowBookingViewCommand = new RelayCommand(ExecuteShowBookingViewCommand);
            ShowCustomerViewCommand = new RelayCommand(ExecuteShowCustomerViewCommand);
            ShowTicketViewCommand = new RelayCommand(ExecuteShowTicketViewCommand);
            ShowTourViewCommand = new RelayCommand(ExecuteShowTourViewCommand);
            ShowPackageViewCommand = new RelayCommand(ExecuteShowPackageViewCommand);
            ShowPackageTypeViewCommand = new RelayCommand(ExecuteShowPackageTypeViewCommand);
            ShowCustomerStatisticViewCommand = new RelayCommand(ExecuteShowCustomerStatisticViewCommand);
            ShowRevenueStatisticViewCommand = new RelayCommand(ExecuteShowRevenueStatisticViewCommand);
            ShowTravelStatisticViewCommand = new RelayCommand(ExecuteShowTravelStatisticViewCommand);
            ShowAccountViewCommand = new RelayCommand(ExecuteShowAccountViewCommand);
            ShowTechnicalHelpViewCommand = new RelayCommand(ExecuteShowTechnicalHelpViewCommand);
            BackToPreviousChildCommand = new RelayCommand(ExecuteBackToPreviousChildCommand);
            SignOutCommand = new RelayCommand(ExecuteSignOutCommand);
            _dashBoardViewModel = new DashBoardViewModel(this);
            removeFirstChild();
            CurrentChildView = _dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        #endregion

        private void ExecuteBackToPreviousChildCommand(object obj)
        {
            removeFirstChild();
            if (Caption == "Travel")
            {
                CurrentChildView = _mainTravelViewModel;
            }
            else if(Caption == "Booking")
            {
                CurrentChildView = _mainBookingViewModel;
            }
            else if(Caption == "Tour")
            {
                CurrentChildView = _mainTourViewModel;
            }
        }

        public void setFirstChild(string nextChildCaption1)
        {
            HaveOneChild = true;
            this.NextChildCaption1 = nextChildCaption1;
            ChildCaptionColor = new SolidColorBrush(Color.FromRgb(130, 136, 143));
            NextChildCaption1Color = new SolidColorBrush(Color.FromRgb(29, 36, 46));
        }

        public void removeFirstChild()
        {
            HaveOneChild = false;
            this.NextChildCaption1 = "";
            ChildCaptionColor = new SolidColorBrush(Color.FromRgb(29, 36, 46));
        }

        #region Check timer tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Dashboard
            /*if (_dashBoardViewModel != null)
            {
                if (CurrentChildView == _dashBoardViewModel)
                    _dashBoardViewModel.Timer.Start();
                else    
                    _dashBoardViewModel.Timer.Stop();
            }*/

            // Travel
            if (_mainTravelViewModel != null)
            {
                if (CurrentChildView == _mainTravelViewModel) 
                    _mainTravelViewModel.Timer.Start();
                else 
                    _mainTravelViewModel.Timer.Stop();
            }   

            // Booking
            if (_mainBookingViewModel != null)
            {
                if (CurrentChildView == _mainBookingViewModel)
                    _mainBookingViewModel.Timer.Start();
                else
                    _mainBookingViewModel.Timer.Stop();
            }

            // Customer
            if (_mainCustomerViewModel != null)
            {
                if (CurrentChildView == _mainCustomerViewModel)
                    _mainCustomerViewModel.Timer.Start();
                else
                    _mainCustomerViewModel.Timer.Stop();
            }

            // Ticket
            if (_mainTicketViewModel != null)
            {
                if (CurrentChildView == _mainTicketViewModel)
                    _mainTicketViewModel.Timer.Start();
                else
                    _mainTicketViewModel.Timer.Stop();
            }

            // Tour
            if (_mainTourViewModel != null)
            {
                if (CurrentChildView == _mainTourViewModel)
                    _mainTourViewModel.Timer.Start();
                else
                    _mainTourViewModel.Timer.Stop();
            }
                
            // Package
            if (_mainPackageViewModel != null)
            {
                if (CurrentChildView == _mainPackageViewModel)
                    _mainPackageViewModel.Timer.Start();
                else 
                    _mainPackageViewModel.Timer.Stop();
            }

            // Package Type
            if (_mainPackageTypeViewModel != null)
            {
                if (CurrentChildView == _mainPackageTypeViewModel)
                    _mainPackageTypeViewModel.Timer.Start();
                else
                    _mainPackageTypeViewModel.Timer.Stop();
            }

            // Account
            if (_mainAccountViewModel != null)
            {
                if (CurrentChildView == _mainAccountViewModel)
                    _mainAccountViewModel.Timer.Start();
                else
                    _mainAccountViewModel.Timer.Stop();
            }
        }
        #endregion

        #region Execute command to set CurrentChildView
        private void ExecuteShowTechnicalHelpViewCommand(object obj)
        {
            if (_technicalHelpViewModel == null)
                _technicalHelpViewModel = new TechnicalHelpViewModel();
            CurrentChildView = _technicalHelpViewModel;
            Caption = "Technical Help";
            Icon = IconChar.QuestionCircle;
            removeFirstChild();
        }

        private void ExecuteShowDashboardViewCommand(object obj)
        {
            if (_dashBoardViewModel == null)
                _dashBoardViewModel = new DashBoardViewModel(this);
            CurrentChildView = _dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
            removeFirstChild();
        }

        private void ExecuteShowTravelViewCommand(object obj)
        {
            if (_mainTravelViewModel == null)
                _mainTravelViewModel = new MainTravelViewModel(this);
            CurrentChildView = _mainTravelViewModel;
            Caption = "Travel";
            Icon = IconChar.Plane;
            removeFirstChild();
        }

        private void ExecuteShowBookingViewCommand(object obj)
        {
            if (_mainBookingViewModel == null)
                _mainBookingViewModel= new MainBookingViewModel(this);
            CurrentChildView = _mainBookingViewModel;
            Caption = "Booking";
            Icon = IconChar.Hand;
            removeFirstChild();
        }
        private void ExecuteShowCustomerViewCommand(object obj)
        {
            if (_mainCustomerViewModel == null)
                _mainCustomerViewModel = new MainCustomerViewModel();
            CurrentChildView = _mainCustomerViewModel;
            Caption = "Customer";
            Icon = IconChar.AddressBook;
            removeFirstChild();
        }
        private void ExecuteShowTicketViewCommand(object obj)
        {
            if (_mainTicketViewModel == null)
                _mainTicketViewModel = new MainTicketViewModel();
            CurrentChildView = _mainTicketViewModel;
            Caption = "Ticket";
            Icon = IconChar.Ticket;
            removeFirstChild();
        }
        private void ExecuteShowTourViewCommand(object obj)
        {
            if (_mainTourViewModel == null)
                _mainTourViewModel = new MainTourViewModel(this);
            CurrentChildView = _mainTourViewModel;
            Caption = "Tour";
            Icon = IconChar.CalendarPlus;
            removeFirstChild();
        }
        private void ExecuteShowPackageViewCommand(object obj)
        {
            if (_mainPackageViewModel == null)
                _mainPackageViewModel = new MainPackageViewModel();
            CurrentChildView = _mainPackageViewModel;
            Caption = "Package";
            Icon = IconChar.BagShopping;
            removeFirstChild();
        }
        private void ExecuteShowPackageTypeViewCommand(object obj)
        {
            if (_mainPackageTypeViewModel == null)
                _mainPackageTypeViewModel = new MainPackageTypeViewModel();
            CurrentChildView = _mainPackageTypeViewModel;
            Caption = "Package Type";
            Icon = IconChar.BagShopping;
            removeFirstChild();
        }
        private void ExecuteShowCustomerStatisticViewCommand(object obj)
        {
            if(_customerStatisticViewModel == null)
                _customerStatisticViewModel = new CustomerStatisticViewModel();
            CurrentChildView = _customerStatisticViewModel;
            Caption = "Customer Statistic";
            Icon = IconChar.Database;
            removeFirstChild();
        }
        private void ExecuteShowRevenueStatisticViewCommand(object obj)
        {
            if(_revenueStatisticViewModel == null)
                _revenueStatisticViewModel = new RevenueStatisticViewModel();
            CurrentChildView = _revenueStatisticViewModel;
            Caption = "Revenue Statistic";
            Icon = IconChar.Database;
            removeFirstChild();
        }
        private void ExecuteShowTravelStatisticViewCommand(object obj)
        {
            Caption = "Travel Statistic";
            Icon = IconChar.Database;
            removeFirstChild();
        }
        private void ExecuteShowAccountViewCommand(object obj)
        {
            if (_mainAccountViewModel == null)
                _mainAccountViewModel = new MainAccountViewModel();
            CurrentChildView = _mainAccountViewModel;
            Caption = "Manage Account";
            Icon = IconChar.AddressCard;
            removeFirstChild();
        }
        #endregion

        #region Log out 
        private void ExecuteSignOutCommand(object obj)
        {
            MyMessageBox.ShowDialog("Are you sure you want to sign out?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
            if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
            {
                MyApp.CurrentUser = null;

                // Close MainView and display LoginView
                var loginView = new LoginView();
                loginView.Show();
                Application.Current.MainWindow.Close();
                BitmapSource icon = new BitmapImage(new Uri("pack://application:,,,/Super_Tour;component/Images/Logo.ico"));
                Application.Current.MainWindow = loginView;
                Application.Current.MainWindow.Icon = icon;
            }
        }

        private void ReloadNewLogin()
        {
            // Set Dashboard UI as Initial view when login new account
            CurrentChildView = _dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
            removeFirstChild();
            SetPriority();
        }

        private void SetPriority()
        {
            if (CurrentUser.Service == "Admin")
            {
                VisibilityAccount = true;
                _visibilityPackageType = true;
                VisibilityPackage = true;
                VisibilityTour = true;
                VisibilityTravel = true;
            }
            if (CurrentUser.Service == "Manager")
            {
                VisibilityAccount = false;
                _visibilityPackageType = true;
                VisibilityPackage = true;
                VisibilityTour = true;
                VisibilityTravel = true;
            }
            if (CurrentUser.Service == "Employee")
            {
                VisibilityAccount = false;
                _visibilityPackageType = false;
                VisibilityPackage = false;
                VisibilityTour = false;
                VisibilityTravel = false;
            }
        }
        #endregion

        #region Open new window by Dashboard
        public void GoToCustomerManagement()
        {
            ExecuteShowCustomerViewCommand(null);
            CustomerChecked = true;
        }

        public void GoToTravelManagement()
        {
            ExecuteShowTravelViewCommand(null);
            TravelChecked = true;
        }

        public void GoToBookingManagement()
        {
            ExecuteShowBookingViewCommand(null);
            BookingChecked = true;
        }

        public void GoToRevenueStatistic()
        {
            ExecuteShowRevenueStatisticViewCommand(null);
            RevenueChecked = true;
        }
        #endregion
    }
}
