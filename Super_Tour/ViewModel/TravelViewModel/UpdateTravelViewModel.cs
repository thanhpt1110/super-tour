using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using static Super_Tour.ViewModel.MainTourViewModel;

namespace Super_Tour.ViewModel
{
    internal class UpdateTravelViewModel:ObservableObject
    {
        #region Declare variable
        private MainViewModel _mainViewModel;
        private MainTravelViewModel _mainTravelViewModel;
        private List<TOUR> _listToursSearching = null;
        private List<TOUR> _listToursOriginal = null;
        private ObservableCollection<DataGridTour> _listDataGridTour;
        private ObservableCollection<string> _listSearchFilterBy;
        private DataGridTour _selectedItem = null;
        private int maxTicketInt;
        private SUPER_TOUR db = null;
        private TOUR _tour = null;
        private string _startLocation;
        private DateTime _selectedDateTime = DateTime.Now.Date;
        private string _maxTicket;
        private bool _executeCommand = true;
        private string _selctedDiscount;
        private ObservableCollection<string> _listDiscount;
        private TRAVEL _selectedTravel=null;
        private string _searchType = "";
        private bool _onSearching = false;
        private string _selectedFilter;
        #endregion
        #region Declare binding
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
                if (string.IsNullOrEmpty(value))
                    _maxTicket = value;
                else
                {
                    if (int.TryParse(value, out maxTicketInt))
                    {
                        _maxTicket = value;
                        //CheckDataModified();
                    }
                    else
                        MaxTicket = _maxTicket;
                }
                OnPropertyChanged(nameof(MaxTicket));
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
        public ICommand SaveUpdateCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand SelectedFilterCommand { get; }
        #endregion
        #region Constructor
        public UpdateTravelViewModel(TRAVEL travel,MainTravelViewModel mainTravelViewModel, MainViewModel mainViewModel)
        {
            db = SUPER_TOUR.db;
            _selectedTravel = travel;
            _mainTravelViewModel = mainTravelViewModel;
            _mainViewModel = mainViewModel;
            _listDataGridTour = new ObservableCollection<DataGridTour>();
            MaxTicket = travel.MaxTicket.ToString();
            SaveUpdateCommand = new RelayCommand(ExecuteSave, canExecuteSave);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            InitTour();
            InitDiscount();
            InitPage();
            generateFilterItem();
        }
        #endregion
        #region Init page
       
        private void InitPage()
        {
            TOUR tour = _selectedTravel.TOUR;
            decimal SumPrice = tour.TOUR_DETAILs
              .Where(p => p.Id_Tour == tour.Id_Tour)
              .Sum(p => p.PACKAGE.Price);
            _selectedItem =  new DataGridTour() { Tour = tour, TotalPrice = SumPrice };
            StartLocation = _selectedTravel.StartLocation;
            SelectedDateTime = _selectedTravel.StartDateTimeTravel;
            MaxTicket = _selectedTravel.MaxTicket.ToString();
            SelectedDiscount = _selectedTravel.Discount.ToString() + "%";
        }
        #endregion
        #region Init discount
        private void InitDiscount()
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
            _listDataGridTour.Clear();
            if (_onSearching)
            {
                switch (_selectedFilter)
                {
                    case "Tour Name":
                        SearchByName();
                        break;
                    case "Tour Place":
                        SearchByPlace();
                        break;

                }
                if(_listToursSearching.Count != 0)
                foreach (TOUR tour in _listToursSearching)
                {
                    decimal SumPrice = tour.TOUR_DETAILs
                  .Where(p => p.Id_Tour == tour.Id_Tour)
                  .Sum(p => p.PACKAGE.Price);
                    _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
                }
            }
            else
            {
                foreach (TOUR tour in _listToursOriginal)
                {
                    decimal SumPrice = tour.TOUR_DETAILs
                  .Where(p => p.Id_Tour == tour.Id_Tour)
                  .Sum(p => p.PACKAGE.Price);
                    _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
                }
            }
        }
        #endregion
        #region Save Command
        private async void ExecuteSave(object obj)
        {
            if ( string.IsNullOrEmpty(_maxTicket) || string.IsNullOrEmpty(_selctedDiscount) || string.IsNullOrEmpty(_selectedTravel.StartLocation))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }

            int numberOfTourists = db.BOOKINGs
            .Where(b => b.Id_Travel == _selectedTravel.Id_Travel)
            .SelectMany(b => b.TOURISTs) // Kết hợp tất cả các danh sách du khách trong các booking thành một danh sách du khách duy nhất
            .Count(); // Đếm số lượng du khách

            if (int.Parse(_maxTicket) < numberOfTourists)
            {
                MyMessageBox.ShowDialog("Couldn't set booking max ticket lower than number of tourists", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                _executeCommand = false;
                _selectedTravel.Id_Tour = _selectedItem.Tour.Id_Tour;
                _selectedTravel.Discount = int.Parse(_selctedDiscount.Remove(_selctedDiscount.Length - 1));
                _selectedTravel.MaxTicket = int.Parse(_maxTicket);
                _selectedTravel.StartLocation = _startLocation;
                _selectedTravel.StartDateTimeTravel = _selectedDateTime;
                db.TRAVELs.AddOrUpdate(_selectedTravel);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Add new travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                await _mainTravelViewModel.ReloadDataAsync();
                _mainViewModel.setFirstChild("");
                _mainViewModel.Caption = "Travel";
                _mainViewModel.CurrentChildView = _mainTravelViewModel;
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
        private bool canExecuteSave(object obj)
        {
            return _executeCommand;
        }
        #endregion
        #region Searching
        private void ExecuteSelectFilter(object obj)
        {
            SearchType = "";
            _onSearching = false;
            LoadData();
        }
        private void ExecuteSearchTour(object obj)
        {
            _onSearching = true;
            LoadData();
        }
        private void SearchByName()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listToursSearching = _listToursOriginal.Where(p => p.Name_Tour.Contains(_searchType)).ToList();
        }

        private void SearchByPlace()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listToursSearching = _listToursOriginal.Where(p => p.PlaceOfTour.Contains(_searchType)).ToList();

        }
        private void generateFilterItem()
        {
            _listSearchFilterBy = new ObservableCollection<string>();
            _listSearchFilterBy.Add("Tour Name");
            _listSearchFilterBy.Add("Tour Place");
            SelectedFilter = "Tour Name";
        }
        #endregion
    }
}
