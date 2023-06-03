using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Firebase.Storage;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainBookingViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        public static DateTime TimeBooking;
        private UPDATE_CHECK _tracker;
        private MainViewModel mainViewModel;
        private List<BOOKING> _listOriginalBooking = null;
        private List<BOOKING> _listBookingsSearching = null;
        private List<BOOKING> _listBookingDatagrid = null;
        private ObservableCollection<BOOKING> _listBookings = null;
        private BOOKING _selectedItem = null;
        private BOOKING temp = null;
        private DispatcherTimer _timer = null;
        private FirebaseStorage firebaseStorage = null;
        private string _searchBooking = "";
        private int _currentPage = 1;
        private int _totalPage;
        private string _pageNumberText;
        private bool _enableButtonNext;
        private bool _enableButtonPrevious;
        private bool _onSearching = false;
        private string _resultNumberText;
        private int _startIndex;
        private int _endIndex;
        private int _totalResult;
        private string table = "UPDATE_PACKAGE";
        #endregion

        #region Declare binding 
        #region Declare binding
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

        public string SearchBooking
        {
            get { return _searchBooking; }
            set
            {
                _searchBooking = value;
                OnPropertyChanged(nameof(SearchBooking));
            }
        }

        public BOOKING SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<BOOKING> ListBookings
        {
            get
            {
                return _listBookings;
            }
            set
            {
                _listBookings = value;
                OnPropertyChanged(nameof(ListBookings));
            }
        }
        #endregion

        #region Command
        public ICommand OpenCreateBookingViewCommand { get; }
        public ICommand UpdateBookingViewCommand { get; }
        public ICommand DeleteBookingViewCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion
        #region Constructor
        public MainBookingViewModel(MainViewModel mainViewModel) 
        {
            this.mainViewModel = mainViewModel;
            db = SUPER_TOUR.db;
            _listBookings = new ObservableCollection<BOOKING>();
            OpenCreateBookingViewCommand = new RelayCommand(ExecuteOpenCreateBookingViewCommand);
            UpdateBookingViewCommand = new RelayCommand(ExecuteUpdateBooking);
            DeleteBookingViewCommand = new RelayCommand(ExecuteDeleteBooking);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchBooking);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);

            //LoadBookingDataAsync();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += Timer_Tick;
        }
        #endregion
        #region Update on data on datagrid event
        private void RefreshDatagrid()
        {
            int index = ListBookings.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListBookings.RemoveAt(index);
                ListBookings.Insert(index, temp);
            }
        }
        #endregion
        #region Load data async
        private async Task LoadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listOriginalBooking = db.BOOKINGs.ToList();
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
        public void ReloadAfterCreateBooking(BOOKING booking)
        {
            try
            {
                _listOriginalBooking.Add(booking);
                ListBookings.Add(booking);
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        _tracker = UPDATE_CHECK.getTracker(table);
                        if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeBooking)
                        {
                            TimeBooking = (DateTime.Parse(_tracker.DateTimeUpdate));
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _listOriginalBooking = db.BOOKINGs.ToList();
                                ReloadData();
                            });
                        }
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
        #region Insert
        private void ExecuteOpenCreateBookingViewCommand(object obj)
        {
            try
            {
                CreateBookingViewModel createBookingViewModel = new CreateBookingViewModel();
                mainViewModel.CurrentChildView = createBookingViewModel;
                mainViewModel.setFirstChild("Add Booking");
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
        #region Update
        private async void ExecuteUpdateBooking(object obj)
        {
            try
            {
                UpdateBookingViewModel updateBoooingViewModel = new UpdateBookingViewModel(_selectedItem, mainViewModel, this);
                mainViewModel.CurrentChildView = updateBoooingViewModel;
                mainViewModel.setFirstChild("Update Booking");
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion
        #region Delete
        private async void ExecuteDeleteBooking(object obj)
        {

            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Delete that booking and Save to data
                    List<TOURIST> listTourist = _selectedItem.TOURISTs.ToList();
                    TRAVEL travel = _selectedItem.TRAVEL;
                    travel.RemainingTicket += listTourist.Count;
                    db.TRAVELs.AddOrUpdate(travel);
                    db.TOURISTs.RemoveRange(listTourist);
                    db.BOOKINGs.Remove(_selectedItem);   
                    db.SaveChanges();
                    // Synchronize real-time data
                    UPDATE_CHECK.NotifyChange(table, TimeBooking);
                    TimeBooking = DateTime.Now;
                    // Process UI event
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    _listOriginalBooking.Remove(SelectedItem);
                    ReloadData();
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion
        #region Search
        private void ExecuteSearchBooking(object obj)
        {
            if (string.IsNullOrEmpty(_searchBooking))
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
        #endregion
        private List<BOOKING> GetData(List<BOOKING> ListBooking, int startIndex, int endIndex)
        {
            try
            {
                return ListBooking.OrderBy(m => m.Id_Booking).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<BOOKING> ListBooking)
        {
            try
            {
                this._totalResult = ListBooking.Count();
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

                _listBookingDatagrid = GetData(ListBooking, this._startIndex, this._endIndex);
                _listBookings.Clear();
                foreach (BOOKING booking in _listBookingDatagrid)
                {
                    _listBookings.Add(booking);
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
                LoadDataByPage(_listBookingsSearching);
            else
                LoadDataByPage(_listOriginalBooking);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listBookingsSearching);
            else
                LoadDataByPage(_listOriginalBooking);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
/*                switch (_selectedFilter)
                {
                    case "Tour Name":
                        SearchByName();
                        break;
                    case "Tour Place":
                        SearchByPlace();
                        break;
                }*/
                LoadDataByPage(_listBookingsSearching);
            }
            else
                LoadDataByPage(_listOriginalBooking);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
