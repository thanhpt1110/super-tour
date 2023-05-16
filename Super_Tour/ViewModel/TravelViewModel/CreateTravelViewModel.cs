using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateTravelViewModel : ObservableObject
    {
        private MainViewModel mainViewModel;
        private TOUR _tour = null;
        private string _startLocation;
        private SUPER_TOUR db = new SUPER_TOUR();
        private DateTime _selectedDateTime = DateTime.Now.Date;
        private string _maxTicket;
        private bool _executeCommand = true;
        private string _discount;
        public string Discount
        {
            get
            {
                return _discount;
            }
            set
            {
                _discount = value;
                OnPropertyChanged(nameof(Discount));
            }
        }
        public string MaxTicket
        {
            get { return _maxTicket; }
            set
            {
                _maxTicket = value;
                OnPropertyChanged(nameof(_maxTicket));
            }
        }
        public DateTime SelectedDateTime
        {
            get { return _selectedDateTime; }
            set
            {
                _selectedDateTime = value;
                OnPropertyChanged(nameof(SelectedDateTime));
            }
        }
        public string StartLocation
        {
            get { return _startLocation; }
            set
            {
                _startLocation = value;
                OnPropertyChanged(nameof(StartLocation));
            }
        }
        public TOUR Tour
        {
            get { return _tour; }
            set
            {
                _tour = value;
                OnPropertyChanged(nameof(Tour));
            }
        }
        public ICommand OpenSelectTourForTravelViewCommand { get; }
        public ICommand SaveTourCommand { get; }
        public CreateTravelViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            _tour = new TOUR();
            SaveTourCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommnad);
        }
        private bool CanExecuteSaveCommnad(object obj)
        {
            return _executeCommand;
        }
        private async void ExecuteSaveCommand(object obj)
        {

            if (_tour == null || string.IsNullOrEmpty(_maxTicket) || string.IsNullOrEmpty(_discount) || string.IsNullOrEmpty(StartLocation))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            int maxTicket;
            int discount;
            if (!int.TryParse(_maxTicket, out maxTicket))
            {
                MyMessageBox.ShowDialog("Max ticket must be in number.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(_discount, out discount))
            {
                MyMessageBox.ShowDialog("Discount must be in number.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            else if (discount > 100)
            {
                MyMessageBox.ShowDialog("Discount must be under 80 percents.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                _executeCommand = false;
                TRAVEL travel = new TRAVEL();
                travel.Id_Travel = 1;
                travel.Id_Tour = _tour.Id_Tour;
                travel.Discount = discount;
                travel.MaxTicket = maxTicket;
                travel.StartLocation = _startLocation;
                travel.RemainingTicket = maxTicket;
                travel.StartDateTimeTravel = _selectedDateTime;
                db.TRAVELs.Add(travel);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Add new travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                CreateTravelView createTravelView = null;
                /*foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is CreateTourView)
                    {
                        createTravelView = window as CreateTravelView;
                        break;
                    }
                }
                createTravelView.Close();*/
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeCommand = true;
            }
        }
        /*private void ExecuteOpenSelectTourForTravelViewCommand(object obj)
        {
            SelectTourForTravelView selectTourForTravelView = new SelectTourForTravelView();
            _tour = new TOUR();
            selectTourForTravelView.DataContext = new SelectTourForTravelViewModel(_tour);
            selectTourForTravelView.ShowDialog();
            Tour = db.TOURs.Find(Tour.Id_Tour);
        }*/
    }
}
