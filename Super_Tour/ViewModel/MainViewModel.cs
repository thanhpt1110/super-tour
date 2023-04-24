using FontAwesome.Sharp;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace Super_Tour.ViewModel
{
    internal class MainViewModel: ObservableObject
    {
        //Fields
        private ObservableObject _currentChildView;
        private string _caption;
        private IconChar _icon;
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
        public RelayCommand ShowStatisticViewCommand { get; }
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
            ShowStatisticViewCommand = new RelayCommand(ExecuteShowStatisticViewCommand);
            ShowAccountViewCommand = new RelayCommand(ExecuteShowAccountViewCommand);
            ShowTechnicalHelpViewCommand = new RelayCommand(ExecuteShowTechnicalHelpViewCommand);
            CurrentChildView = new DashBoardViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void ExecuteShowTechnicalHelpViewCommand(object obj)
        {
            CurrentChildView = new TechnicalHelpViewModel();
            Caption = "Technical Help";
            Icon = IconChar.QuestionCircle;
        }

        private void ExecuteShowDashboardViewCommand(object obj)
        {
            CurrentChildView = new DashBoardViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }
        private void ExecuteShowTravelViewCommand(object obj)
        {
            CurrentChildView = new MainTravelViewModel();
            Caption = "Travel";
            Icon = IconChar.Plane;
        }
        private void ExecuteShowBookingViewCommand(object obj)
        {
            CurrentChildView = new MainBookingViewModel();
            Caption = "Booking";
            Icon = IconChar.Hand;
        }
        private void ExecuteShowCustomerViewCommand(object obj)
        {
            CurrentChildView = new MainCustomerViewModel();
            Caption = "Customer";
            Icon = IconChar.AddressBook;
        }
        private void ExecuteShowTicketViewCommand(object obj)
        {
            CurrentChildView = new MainTicketViewModel();
            Caption = "Ticket";
            Icon = IconChar.Ticket;
        }
        private void ExecuteShowTourViewCommand(object obj)
        {
            CurrentChildView = new MainTourViewModel();
            Caption = "Tour";
            Icon = IconChar.CalendarPlus;
        }
        private void ExecuteShowPackageViewCommand(object obj)
        {
            CurrentChildView = new MainPackageViewModel();
            Caption = "Package";
            Icon = IconChar.BagShopping;
        }
        private void ExecuteShowStatisticViewCommand(object obj)
        {
            Caption = "Statistic";
            Icon = IconChar.Database;
        }
        private void ExecuteShowAccountViewCommand(object obj)
        {
            CurrentChildView = new MainAccountViewModel();
            Caption = "Manage Account";
            Icon = IconChar.AddressCard;
        }
    }
}
