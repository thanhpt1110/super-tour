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

namespace Super_Tour.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        //Fields
        // Tạo một cache mới với tên "myCache"
        //private MemoryCache myCache;
        public static ObservableObject CurrentChild;
        private ObservableObject _currentChildView;
        private readonly IMemoryCache _cache;
        private string _caption;
        private IconChar _icon;
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

        public ObservableObject CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
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
        //Command
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
            _dashBoardViewModel = new DashBoardViewModel();
            CurrentChildView = _dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void ExecuteShowTechnicalHelpViewCommand(object obj)
        {
            if (_technicalHelpViewModel == null)
                _technicalHelpViewModel = new TechnicalHelpViewModel();
            CurrentChildView = _technicalHelpViewModel;
            CurrentChild=_technicalHelpViewModel;
            Caption = "Technical Help";
            Icon = IconChar.QuestionCircle;
        }

        private void ExecuteShowDashboardViewCommand(object obj)
        {
            if (_dashBoardViewModel == null)
                _dashBoardViewModel = new DashBoardViewModel();
            CurrentChildView = _dashBoardViewModel;
            CurrentChild=_dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void ExecuteShowTravelViewCommand(object obj)
        {
            if (_mainTravelViewModel == null)
                _mainTravelViewModel = new MainTravelViewModel();
            CurrentChildView = _mainTravelViewModel;
            CurrentChild=_mainTravelViewModel;
            Caption = "Travel";
            Icon = IconChar.Plane;
        }

        private void ExecuteShowBookingViewCommand(object obj)
        {
            if (_mainBookingViewModel == null)
                _mainBookingViewModel= new MainBookingViewModel();
            CurrentChildView = _mainBookingViewModel;
            CurrentChild=_mainBookingViewModel;
            Caption = "Booking";
            Icon = IconChar.Hand;
        }
        private void ExecuteShowCustomerViewCommand(object obj)
        {
            if (_mainCustomerViewModel == null)
                _mainCustomerViewModel = new MainCustomerViewModel();
            CurrentChildView = _mainCustomerViewModel;
            CurrentChild=_mainCustomerViewModel;
            Caption = "Customer";
            Icon = IconChar.AddressBook;
        }
        private void ExecuteShowTicketViewCommand(object obj)
        {
            if (_mainTicketViewModel == null)
                _mainTicketViewModel = new MainTicketViewModel();
            CurrentChildView = _mainTicketViewModel;
            CurrentChild=_mainTicketViewModel;
            Caption = "Ticket";
            Icon = IconChar.Ticket;
        }
        private void ExecuteShowTourViewCommand(object obj)
        {
            if (_mainTourViewModel == null)
                _mainTourViewModel = new MainTourViewModel();
            CurrentChildView = _mainTourViewModel;
            CurrentChild=_mainTourViewModel;
            Caption = "Tour";
            Icon = IconChar.CalendarPlus;
        }
        private void ExecuteShowPackageViewCommand(object obj)
        {
            if (_mainPackageViewModel == null)
                _mainPackageViewModel = new MainPackageViewModel();
            CurrentChildView = _mainPackageViewModel;
            CurrentChild=_mainPackageViewModel;
            Caption = "Package";
            Icon = IconChar.BagShopping;
        }
        private void ExecuteShowPackageTypeViewCommand(object obj)
        {
            if (_mainPackageTypeViewModel == null)
                _mainPackageTypeViewModel = new MainPackageTypeViewModel();
            CurrentChildView = _mainPackageTypeViewModel;
            CurrentChild=_mainPackageTypeViewModel;
            Caption = "Package Type";
            Icon = IconChar.BagShopping;
        }
        private void ExecuteShowCustomerStatisticViewCommand(object obj)
        {
            Caption = "Customer Statistic";
            Icon = IconChar.Database;
        }
        private void ExecuteShowRevenueStatisticViewCommand(object obj)
        {
            Caption = "Revenue Statistic";
            Icon = IconChar.Database;
        }
        private void ExecuteShowTravelStatisticViewCommand(object obj)
        {
            Caption = "Travel Statistic";
            Icon = IconChar.Database;
        }
        private void ExecuteShowAccountViewCommand(object obj)
        {
            if (_mainAccountViewModel == null)
                _mainAccountViewModel = new MainAccountViewModel();
            CurrentChildView = _mainAccountViewModel;
            CurrentChild=_mainAccountViewModel;
            Caption = "Manage Account";
            Icon = IconChar.AddressCard;
        }
    }
    }
