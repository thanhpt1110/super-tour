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
using Org.BouncyCastle.Asn1.X509;
using Super_Tour.ViewModel.TourViewModel;

namespace Super_Tour.ViewModel
{
    internal class UpdateTravelViewModel:ObservableObject
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
        private TRAVEL _selectedTravel = null;
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
        public UpdateTravelViewModel(TRAVEL travel,MainTravelViewModel mainTravelViewModel, MainViewModel mainViewModel)
        {
            // Create objects
            db = SUPER_TOUR.db;
            _selectedTravel = travel;
            _mainTravelViewModel = mainTravelViewModel;
            _mainViewModel = mainViewModel;
            _listTours = new ObservableCollection<TOUR>();

            // Load UI
            LoadDiscount();
            LoadTour();
            LoadExistedTravelInformation();

            // Create command
            SaveCommand = new RelayCommand(ExecuteSave);
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

        private void LoadExistedTravelInformation()
        {
            SelectedItem = _selectedTravel.TOUR;
            StartLocation = _selectedTravel.StartLocation;
            SelectedDateTime = _selectedTravel.StartDateTimeTravel;
            MaxTicket = _selectedTravel.MaxTicket.ToString();
            SelectedDiscount = _selectedTravel.Discount.ToString() + "%";
        }

        private void ReloadData()
        {
            _listTours.Clear();
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
                foreach (TOUR tour in _listToursSearching)
                {
                    ListTours.Add(tour);
                }
            }
            else
            {
                foreach (TOUR tour in _listToursOriginal)
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
            this._listToursSearching = _listToursOriginal.Where(p => p.Name_Tour.Contains(_searchType)).ToList();
        }

        private void SearchByPlace()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listToursSearching = _listToursOriginal.Where(p => p.PlaceOfTour.Contains(_searchType)).ToList();

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

        #region Perform update travel 
        private async void ExecuteSave(object obj)
        {
            int maxTicket = int.Parse(_maxTicket);

            // Get number of tourists that have booked this travel
            int numberOfTourists = db.BOOKINGs
            .Where(b => b.Id_Travel == _selectedTravel.Id_Travel)
            .SelectMany(b => b.TOURISTs) // Kết hợp tất cả các danh sách du khách trong các booking thành một danh sách du khách duy nhất
            .Count(); // Đếm số lượng du khách

            if (maxTicket < numberOfTourists)
            {
                MyMessageBox.ShowDialog("Couldn't set travel's max ticket less than number of tourists have booked!", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                // Save data to DB in TRAVEL table
                _selectedTravel.Id_Tour = _selectedItem.Id_Tour;
                _selectedTravel.StartLocation = _startLocation;
                _selectedTravel.StartDateTimeTravel = _selectedDateTime;
                _selectedTravel.MaxTicket = maxTicket;
                _selectedTravel.Discount = int.Parse(_selctedDiscount.Remove(_selctedDiscount.Length - 1));
                _selectedTravel.RemainingTicket = maxTicket;
                db.TRAVELs.AddOrUpdate(_selectedTravel);
                await db.SaveChangesAsync();

                // Synchronyze real-time DB
                MainTravelViewModel.TimeTravel = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainTravelViewModel.TimeTravel);

                // Process UI event
                MyMessageBox.ShowDialog("Update travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainTravelViewModel;
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
                || string.IsNullOrEmpty(_selctedDiscount) || string.IsNullOrEmpty(StartLocation) ||
                _selectedTravel.Id_Tour == SelectedItem.Id_Tour && _selectedTravel.MaxTicket.ToString() == _maxTicket
                && _selectedTravel.Discount.ToString() == _selctedDiscount && _selectedTravel.StartLocation == StartLocation
                && _selectedTravel.StartDateTimeTravel == _selectedDateTime)
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
