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
    internal class MainViewModel: ObservableObject
    {
        //Fields
        private ObservableObject _currentChildView;
        private readonly IMemoryCache _cache;
        private string _caption;
        private IconChar _icon;
        private List<object> _formViewModel = new List<object>();
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
            CurrentChildView = new DashBoardViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void ExecuteShowTechnicalHelpViewCommand(object obj)
        {
            TechnicalHelpViewModel  technicalHelpViewModel= null;
            foreach (var form in _formViewModel)
            {
                if (form is TechnicalHelpViewModel)
                {
                    technicalHelpViewModel = form as TechnicalHelpViewModel;
                    break;
                }
            }    
            if(technicalHelpViewModel==null)
            {
                technicalHelpViewModel = new TechnicalHelpViewModel();
                _formViewModel.Add(technicalHelpViewModel);
            }    
            CurrentChildView = technicalHelpViewModel;
            Caption = "Technical Help";
            Icon = IconChar.QuestionCircle;
        }

        private void ExecuteShowDashboardViewCommand(object obj)
        {
            DashBoardViewModel dashBoardViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is DashBoardViewModel)
                {
                    dashBoardViewModel = form as DashBoardViewModel;
                    break;
                }
            }
            if (dashBoardViewModel == null)
            {
                dashBoardViewModel = new DashBoardViewModel();
                _formViewModel.Add(dashBoardViewModel);
            }
            CurrentChildView = dashBoardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }
        private void ExecuteShowTravelViewCommand(object obj)
        {
            MainTravelViewModel mainTravelViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainTravelViewModel)
                {
                    mainTravelViewModel = form as MainTravelViewModel;
                    break;
                }
            }
            if (mainTravelViewModel == null)
            {
                mainTravelViewModel = new MainTravelViewModel();
                _formViewModel.Add(mainTravelViewModel);
            }
            CurrentChildView = mainTravelViewModel;
            Caption = "Travel";
            Icon = IconChar.Plane;
        }
        private void ExecuteShowBookingViewCommand(object obj)
        {
            MainBookingViewModel mainBookingViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainBookingViewModel)
                {
                    mainBookingViewModel = form as MainBookingViewModel;
                    break;
                }
            }
            if (mainBookingViewModel == null)
            {
                mainBookingViewModel = new MainBookingViewModel();
                _formViewModel.Add(mainBookingViewModel);
            }
            CurrentChildView = mainBookingViewModel;
            Caption = "Booking";
            Icon = IconChar.Hand;
        }
        private void ExecuteShowCustomerViewCommand(object obj)
        {
            MainCustomerViewModel mainCustomerViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainCustomerViewModel)
                {
                    mainCustomerViewModel = form as MainCustomerViewModel;
                    break;
                }
            }
            if (mainCustomerViewModel == null)
            {
                mainCustomerViewModel = new MainCustomerViewModel();
                _formViewModel.Add(mainCustomerViewModel);
            }
            CurrentChildView = mainCustomerViewModel;
            Caption = "Customer";
            Icon = IconChar.AddressBook;
        }
        private void ExecuteShowTicketViewCommand(object obj)
        {
            MainTicketViewModel mainTicketViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainTicketViewModel)
                {
                    mainTicketViewModel = form as MainTicketViewModel;
                    break;
                }
            }
            if (mainTicketViewModel == null)
            {
                mainTicketViewModel = new MainTicketViewModel();
                _formViewModel.Add(mainTicketViewModel);
            }
            CurrentChildView = mainTicketViewModel;
            Caption = "Ticket";
            Icon = IconChar.Ticket;
        }
        private void ExecuteShowTourViewCommand(object obj)
        {
            MainTourViewModel mainTourViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainTourViewModel)
                {
                    mainTourViewModel = form as MainTourViewModel;
                    break;
                }
            }
            if (mainTourViewModel == null)
            {
                mainTourViewModel = new MainTourViewModel();
                _formViewModel.Add(mainTourViewModel);
            }
            CurrentChildView = mainTourViewModel;
            Caption = "Tour";
            Icon = IconChar.CalendarPlus;
        }
        private void ExecuteShowPackageViewCommand(object obj)
        {
            MainPackageViewModel mainPackageViewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainPackageViewModel)
                {
                    mainPackageViewModel = form as MainPackageViewModel;
                    break;
                }
            }
            if (mainPackageViewModel == null)
            {
                mainPackageViewModel = new MainPackageViewModel();
                _formViewModel.Add(mainPackageViewModel);
            }
            CurrentChildView = mainPackageViewModel;
            Caption = "Package";
            Icon = IconChar.BagShopping;
        }
        private void ExecuteShowPackageTypeViewCommand(object obj)
        {
            MainPackageTypeViewModel viewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainPackageTypeViewModel)
                {
                    viewModel = form as MainPackageTypeViewModel;
                    break;
                }
            }
            if (viewModel == null)
            {
                viewModel = new MainPackageTypeViewModel();
                _formViewModel.Add(viewModel);
            }
            CurrentChildView =   viewModel;
            Caption = "Package Type";
            Icon = IconChar.BagShopping;
        }
        private void ExecuteShowCustomerStatisticViewCommand(object obj)
        {
            Caption = "Customer Statistic";
            Icon = IconChar.Database;
            CustomerStatisticViewModel viewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is CustomerStatisticViewModel)
                {
                    viewModel = form as CustomerStatisticViewModel;
                    break;
                }
            }
            if (viewModel == null)
            {
                viewModel = new CustomerStatisticViewModel();
                _formViewModel.Add(viewModel);
            }
            CurrentChildView = viewModel;
            Caption = "Customer Statistic";
            Icon = IconChar.BagShopping;
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
            MainAccountViewModel viewModel = null;
            foreach (var form in _formViewModel)
            {
                if (form is MainAccountViewModel)
                {
                    viewModel = form as MainAccountViewModel;
                    break;
                }
            }
            if (viewModel == null)
            {
                viewModel = new MainAccountViewModel();
                _formViewModel.Add(viewModel);
            }
            CurrentChildView = viewModel;
/*            CurrentChildView = new MainAccountViewModel();*/
            Caption = "Manage Account";
            Icon = IconChar.AddressCard;
        }
    }
}
