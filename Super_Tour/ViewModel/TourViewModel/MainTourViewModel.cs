using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainTourViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        public static DateTime TimeTour;
        private MainViewModel mainViewModel;
        private UPDATE_CHECK _tracker = null;
        private List<TOUR> _listTourSearching = null;
        private List<TOUR> _listTourOriginal = null;
        private List<TOUR> _listTourDatagrid = null;
        private List<TOUR_DETAILS> _listTourDetails = null;
        private ObservableCollection<TOUR> _listTours = null;
        private TOUR _selectedItem = null;
        private TOUR _temp = null;
        private DispatcherTimer _timer = null;
        private string _searchTour = "";
        private string _selectedFilter = "Tour Name";
        private int _currentPage = 1;
        private int _totalPage;
        private string _pageNumberText = null;
        private bool _enableButtonNext;
        private bool _enableButtonPrevious;
        private bool _onSearching = false;
        private string _resultNumberText = null;
        private int _startIndex;
        private int _endIndex;
        private int _totalResult;
        private string table = "UPDATE_TOUR";
        #endregion

        #region Declare binding
        public TOUR SelectedItem
        {
            get { return _selectedItem; }
            set 
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set 
            { 
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        public string ResultNumberText
        {
            get { return _resultNumberText; }
            set
            {
                _resultNumberText = value;
                OnPropertyChanged(nameof(ResultNumberText));
            }
        }

        public string PageNumberText
        {
            get
            {
                return _pageNumberText;
            }
            set
            {
                _pageNumberText = value;
                OnPropertyChanged(nameof(PageNumberText));
            }
        }

        public bool EnableButtonNext
        {
            get
            {
                return _enableButtonNext;
            }
            set
            {
                _enableButtonNext = value;
                OnPropertyChanged(nameof(EnableButtonNext));
            }
        }

        public bool EnableButtonPrevious
        {
            get
            {
                return _enableButtonPrevious;
            }
            set
            {
                _enableButtonPrevious = value;
                OnPropertyChanged(nameof(EnableButtonPrevious));
            }
        }

        public string SearchTour
        {
            get
            {
                return _searchTour;
            }
            set
            {
                _searchTour = value;
                OnPropertyChanged(nameof(SearchTour));
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
        #endregion

        #region Command
        public ICommand OpenCreateTourViewCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommnand { get; }
        public ICommand OnSearchTextChangedCommand { get;}
        public ICommand SelectedFilterCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public MainTourViewModel(MainViewModel mainViewModel) 
        {
            this.mainViewModel = mainViewModel;
            db = SUPER_TOUR.db;
            _listTours = new ObservableCollection<TOUR>();
            OpenCreateTourViewCommand = new RelayCommand(ExecuteOpenCreateTourViewCommand);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            UpdateTourCommand = new RelayCommand(ExecuteUpdateTourCommand);
            DeleteTourCommnand = new RelayCommand(ExecuteDeleteTour);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            LoadDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick;
        }
        #endregion

        #region Update data on datagrid
        public void RefreshDatagrid()
        {
            int index = ListTours.IndexOf(SelectedItem);
            _temp = SelectedItem;
            if (index != -1)
            {
                ListTours.RemoveAt(index);
                ListTours.Insert(index, _temp);
            }
        }
        #endregion

        #region Load data async
        private async Task LoadDataAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTourOriginal = db.TOURs.ToList();
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
        #endregion

        #region Check data per second
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await ReloadDataAsync();
        }

        public void ReloadAfterCreateNewTour(TOUR tour)
        {
            try
            {
                _listTourOriginal.Add(tour);
                ListTours.Add(tour);
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            };
        }

        public async Task ReloadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    _tracker = UPDATE_CHECK.getTracker(table);
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeTour)
                    {
                        TimeTour = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTourOriginal = db.TOURs.ToList();
                            ReloadData();
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

        #region Insert
        private void ExecuteOpenCreateTourViewCommand(object obj)
        {
            try
            {
                CreateTourViewModel createTourViewModel = new CreateTourViewModel(this, mainViewModel);
                mainViewModel.CurrentChildView = createTourViewModel;
                mainViewModel.setFirstChild("Add Tour");
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update      
        private void ExecuteUpdateTourCommand(object obj)
        {
            try
            {
                UpdateTourViewModel updateTourViewModel = new UpdateTourViewModel(_selectedItem, this, mainViewModel);
                mainViewModel.CurrentChildView = updateTourViewModel;
                mainViewModel.setFirstChild("Update Tour");
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Delete
        private async void ExecuteDeleteTour(object obj)
        {
            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Delete all Tour details first
                    _listTourDetails = db.TOUR_DETAILs.Where(p => p.Id_Tour == SelectedItem.Id_Tour).ToList();
                    foreach (TOUR_DETAILS tour_detail in _listTourDetails)
                    {
                        db.TOUR_DETAILs.Remove(tour_detail);
                    }

                    // Delete that tour and Save to data
                    db.TOURs.Remove(db.TOURs.Find(SelectedItem.Id_Tour));
                    await db.SaveChangesAsync();

                    // Synchronize real-time data
                    UPDATE_CHECK.NotifyChange(table, TimeTour);
                    TimeTour = DateTime.Now;

                    // Process UI event
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    _listTourOriginal.Remove(SelectedItem);
                    ReloadData();   
                }
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

        #region Search
        private void ExecuteSelectFilter(object obj)
        {
            SearchTour = "";
            _onSearching = false;
            ReloadData();
        }

        private void ExecuteSearchTour(object obj)
        {
            if (string.IsNullOrEmpty(_searchTour))
            {
                _onSearching = false;
                ReloadData();
            }
            else
            {
                _onSearching = true;
                this._currentPage = 1;
                ReloadData();
            }
        }

        private void SearchByName()
        {
            if (_listTourOriginal == null || _listTourOriginal.Count == 0)
                return;
            this._listTourSearching = _listTourOriginal.Where(p => p.Name_Tour.ToLower().Contains(_searchTour.ToLower())).ToList();
        }

        private void SearchByPlace()
        {
            if (_listTourOriginal == null || _listTourOriginal.Count == 0)
                return;
            this._listTourSearching = _listTourOriginal.Where(p => p.PlaceOfTour.ToLower().Contains(_searchTour.ToLower())).ToList();
        }
        #endregion

        #region Custom display data grid
        private List<TOUR> GetData(List<TOUR> ListTours, int startIndex, int endIndex)
        {
            try
            {
                return ListTours.OrderBy(m => m.Id_Tour).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<TOUR> ListTours)
        {
            try
            {
                this._totalResult = ListTours.Count();
                if (_totalResult == 0)
                {
                    _startIndex = -1;
                    _endIndex = 0;
                    _totalPage = 1;
                    _currentPage = 1;
                }
                else
                {
                    this._totalPage = (int)Math.Ceiling((double)_totalResult / 13);
                    if (this._totalPage < _currentPage)
                        _currentPage = this._totalPage;
                    this._startIndex = (this._currentPage - 1) * 13;
                    this._endIndex = Math.Min(this._startIndex + 13, _totalResult);
                }

                _listTourDatagrid = GetData(ListTours, this._startIndex, this._endIndex);
                _listTours.Clear();
                foreach (TOUR tour in _listTourDatagrid)
                {
                    _listTours.Add(tour);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private void setResultNumber()
        {
            ResultNumberText = $"Showing {this._startIndex + 1} - {this._endIndex} of {this._totalResult} results";
        }

        private void setButtonAndPage()
        {
            if (this._currentPage < this._totalPage)
            {
                EnableButtonNext = true;
                if (this._currentPage == 1)
                    EnableButtonPrevious = false;
                else
                    EnableButtonPrevious = true;
            }
            if (this._currentPage == this._totalPage)
            {
                if (this._currentPage == 1)
                {
                    EnableButtonPrevious = false;
                    EnableButtonNext = false;
                }
                else
                {
                    EnableButtonPrevious = true;
                    EnableButtonNext = false;
                }
            }
            PageNumberText = $"Page {this._currentPage} of {this._totalPage}";
        }

        private void ExecuteGoToPreviousPageCommand(object obj)
        {
            if (this._currentPage > 1)
                --this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTourSearching);
            else
                LoadDataByPage(_listTourOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTourSearching);
            else
                LoadDataByPage(_listTourOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
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
                LoadDataByPage(_listTourSearching);
            }
            else
                LoadDataByPage(_listTourOriginal);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
