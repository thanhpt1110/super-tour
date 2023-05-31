using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateTravelViewModel : ObservableObject
    {
        private MainViewModel _mainViewModel;
        private MainTravelViewModel _mainTravelViewModel;
        private List<TOUR> _listToursSearching = null;
        private List<TOUR> _listToursOriginal = null;
        private ObservableCollection<DataGridTour> _listDataGridTour;
        private ObservableCollection<string> _listSearchFilterBy;
        private DataGridTour _selectedItem = null;
        private SUPER_TOUR db = null;
        private TOUR _tour = null;
        private string _startLocation;
        private DateTime _selectedDateTime = DateTime.Now.Date;
        private string _maxTicket;
        private bool _executeCommand = true;
        private string _selctedDiscount;
        private ObservableCollection<string> _listDiscount;
        #region Declare binding

        public string SelectedDiscount
        {
            get
            {
                return _selctedDiscount;
            }
            set
            {
                _selctedDiscount = value;
                OnPropertyChanged(nameof(SelectedDiscount));
            }
        }
        public ObservableCollection<string> ListDiscount
        {
            get
            {
                return _listDiscount;
            }
            set
            {
                _listDiscount = value;
                OnPropertyChanged(nameof(ListDiscount));
            }
        }
        public DataGridTour SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        public ObservableCollection<DataGridTour> ListDataGridTour
        {
            get
            {
                return _listDataGridTour;
            }
            set
            {
                _listDataGridTour = value;
                OnPropertyChanged(nameof(ListDataGridTour));
            }
        }
        public ObservableCollection<string> ListSearchFilterBy
        {
            get
            {
                return _listSearchFilterBy;
            }
            set
            {
                _listSearchFilterBy = value;
                OnPropertyChanged(nameof(ListDataGridTour));
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

        #endregion
        #region Command
        public ICommand OpenSelectTourForTravelViewCommand { get; }
        public ICommand SaveTourCommand { get; }
        #endregion
        #region Constructor
        public CreateTravelViewModel(MainViewModel mainViewModel, MainTravelViewModel mainTravelViewModel)
        {
            db = SUPER_TOUR.db;
            _listDataGridTour = new ObservableCollection<DataGridTour>();
            TOUR tour;
            this._mainViewModel = mainViewModel;
            LoadDiscount();
            InitTour();
            SaveTourCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommnad);
            _mainTravelViewModel = mainTravelViewModel;
        }
        #endregion
        #region Init discount
        private void LoadDiscount()
        {
            _listDiscount = new ObservableCollection<string>();
            _listDiscount.Add("5%");
            _listDiscount.Add("10%");
            _listDiscount.Add("15%");
            _listDiscount.Add("20%");
            _listDiscount.Add("25%");
            _listDiscount.Add("30%");
            _listDiscount.Add("35%");
            _listDiscount.Add("40%");
            _listDiscount.Add("45%");
            _listDiscount.Add("50%");
        }
        #endregion

        #region init list tour

        private async void InitTour()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listToursOriginal = db.TOURs.ToList();
                            LoadData();
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
        private void LoadData()
        {
            foreach(TOUR tour in _listToursOriginal)
            {
                decimal SumPrice = tour.TOUR_DETAILs
              .Where(p => p.Id_Tour == tour.Id_Tour)
              .Sum(p => p.PACKAGE.Price);
                _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
            }
        }
        #endregion
        #region Save Travel
        private bool CanExecuteSaveCommnad(object obj)
        {
            return _executeCommand;
        }
        private async void ExecuteSaveCommand(object obj)
        {

            if (SelectedItem == null || string.IsNullOrEmpty(_maxTicket) || string.IsNullOrEmpty(_selctedDiscount) || string.IsNullOrEmpty(StartLocation))
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
/*            if (!int.TryParse(_selctedDiscount, out discount))
            {
                MyMessageBox.ShowDialog("Discount must be in number.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }*/
/*            else if (discount > 100)
            {
                MyMessageBox.ShowDialog("Discount must be under 80 percents.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }*/
            try
            {
                _executeCommand = false;
                TRAVEL travel = new TRAVEL();
                travel.Id_Travel = 1;
                travel.Id_Tour = _selectedItem.Tour.Id_Tour;
                travel.Discount = int.Parse(_selctedDiscount.Remove(_selctedDiscount.Length-1));
                travel.MaxTicket = maxTicket;
                travel.StartLocation = _startLocation;
                travel.RemainingTicket = maxTicket;
                travel.StartDateTimeTravel = _selectedDateTime;
                db.TRAVELs.Add(travel);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Add new travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                await _mainTravelViewModel.ReloadDataAsync();
                _mainViewModel.setFirstChild("");
                _mainViewModel.Caption = "Travel";
                _mainViewModel.CurrentChildView = _mainTravelViewModel;
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
        #endregion

        
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
