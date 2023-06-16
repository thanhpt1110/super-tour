using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using Super_Tour.ViewModel.TourViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Super_Tour.ViewModel.MainTourViewModel;

namespace Super_Tour.ViewModel
{
    internal class CreateTravelViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel;
        private MainTravelViewModel _mainTravelViewModel;
        private string _searchType = "";
        private string _selectedFilter = "Tour Name";
        private List<TOUR> _listToursSearching = null;
        private List<TOUR> _listToursOriginal = null;
        private ObservableCollection<TOUR> _listTours = null;
        private TOUR _selectedItem = null;
        private string _startLocation = null;
        private DateTime _selectedDateTime = DateTime.Now.Date;
        private string _maxTicket;
        private ObservableCollection<string> _listDiscount;
        private string _selctedDiscount;
        private bool _isDataModified = false;
        private bool _onSearching = false;
        private string table = "UPDATE_TRAVEL";
        #endregion

        #region Declare binding
        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

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
                CheckDataModified();
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

        public string MaxTicket
        {
            get { return _maxTicket; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _maxTicket = value;
                    OnPropertyChanged(nameof(MaxTicket));
                    CheckDataModified();
                }
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
                CheckDataModified();
            }
        }

        public TOUR SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                CheckDataModified();
            }
        }

        public ObservableCollection<TOUR> ListTours
        {
            get { return _listTours; }
            set
            {
                _listTours = value;
                OnPropertyChanged(nameof(ListTours));
            }
        }

        public string SelectedFilter
        {
            get
            {
                return _selectedFilter;
            }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        public string SearchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                _searchType = value;
                OnPropertyChanged(nameof(SearchType));
            }
        }
        #endregion

        #region Command
        public ICommand SaveCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand SelectedFilterCommand { get; }
        public ICommand ViewTourDetailCommand { get; }
        #endregion

        #region Constructor
        public CreateTravelViewModel(MainViewModel mainViewModel, MainTravelViewModel mainTravelViewModel)
        {
            // Create objects
            db = SUPER_TOUR.db;
            _listTours = new ObservableCollection<TOUR>();
            _mainTravelViewModel = mainTravelViewModel;
            this._mainViewModel = mainViewModel;

            // Load UI
            LoadDiscount();
            LoadTour();
                
            // Create command
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            ViewTourDetailCommand = new RelayCommand(ExecuteViewTourDetailCommand);
        }
        #endregion

        #region Load window
        private void LoadDiscount()
        {
            _listDiscount = new ObservableCollection<string>
            {
                "0%",
                "5%",
                "10%",
                "15%",
                "20%",
                "25%",
                "30%",
                "35%",
                "40%",
                "45%",
                "50%"
            };
        }

        private async void LoadTour()
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
                            ReloadData();
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

        private void ReloadData()
        {
            _listTours.Clear();
            if (_onSearching)
            {
                switch(_selectedFilter)
                {
                    case "Tour Name":
                        SearchByName();
                        break;
                    case "Tour Place":
                        SearchByPlace();
                        break;
                }    
                foreach(TOUR tour in _listToursSearching)
                {
                    ListTours.Add(tour);
                }
            }
            else
            {
                foreach(TOUR tour in _listToursOriginal)
                {
                    ListTours.Add(tour);
                }
            }
        }
        #endregion

        #region Search
        private void ExecuteSelectFilter(object obj)
        {
            SearchType = "";
            _onSearching = false;
            ReloadData();
        }

        private void ExecuteSearchTour(object obj)
        {
            if (string.IsNullOrEmpty(_searchType))
            {
                _onSearching = false;
                ReloadData();
            }
            else
            {
                _onSearching = true;
                ReloadData();
            }
        }

        private void SearchByName()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listToursSearching = _listToursOriginal.Where(p => p.Name_Tour.ToLower().Contains(_searchType.ToLower())).ToList();
        }

        private void SearchByPlace()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listToursSearching = _listToursOriginal.Where(p => p.PlaceOfTour.ToLower().Contains(_searchType.ToLower())).ToList();

        }
        #endregion

        #region View tour detail
        private void ExecuteViewTourDetailCommand(object obj)
        {
            try
            {
                if (SelectedItem != null)
                {
                    DetailTourView detailTourView = new DetailTourView();
                    detailTourView.DataContext = new DetailTourViewModel(SelectedItem);
                    detailTourView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Perform add new Travel
        private async void ExecuteSaveCommand(object obj)
        {
            int maxTicket = int.Parse(_maxTicket);
            try
            {
                // Save data to DB in TRAVEL table
                TRAVEL travel = new TRAVEL();
                travel.Id_Tour = _selectedItem.Id_Tour;
                travel.StartLocation = _startLocation;
                travel.StartDateTimeTravel = _selectedDateTime;
                travel.MaxTicket = maxTicket;
                travel.Discount = int.Parse(_selctedDiscount.Remove(_selctedDiscount.Length-1));
                travel.RemainingTicket = maxTicket;
                db.TRAVELs.Add(travel);
                await db.SaveChangesAsync();

                // Synchronyze real-time DB
                MainTravelViewModel.TimeTravel = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTravelViewModel.TimeTravel);

                // Process UI event
                MyMessageBox.ShowDialog("Add new travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainTravelViewModel;
                _mainTravelViewModel.ReloadAfterCreateTravel(travel);
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Check data modified
        private void CheckDataModified()
        {
            if (SelectedItem == null || string.IsNullOrEmpty(_maxTicket)
                || string.IsNullOrEmpty(_selctedDiscount) || string.IsNullOrEmpty(StartLocation))
                IsDataModified = false;
            else 
                IsDataModified = true;
        }
        #endregion
    }
}
