using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainTicketViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        public static DateTime TimeTicket;
        private UPDATE_CHECK _tracker;
        private DispatcherTimer _timer = null;
        private List<TICKET> _listOriginalTicket = null;
        private List<TICKET> _listTicketSearching = null;
        private List<TICKET> _listTicketDatagrid = null;
        private ObservableCollection<TICKET> _listTickets = null;
        private TICKET _selectedItem = null;
        private TICKET temp = null;
        private string _searchTicket = "";
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
        private string table = "UPDATE_TICKET";
        private string _selectedFilter = "ID Number";
        #endregion

        #region Declare binding
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

        public string SearchTicket
        {
            get { return _searchTicket; }
            set
            {
                _searchTicket = value;
                OnPropertyChanged(nameof(SearchTicket));
            }
        }

        public TICKET SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<TICKET> ListTickets
        {
            get
            {
                return _listTickets;
            }
            set
            {
                _listTickets = value;
                OnPropertyChanged(nameof(ListTickets));
            }
        }
        #endregion

        #region Command
        public ICommand SelectedFilterCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand UpdateTicketCommand { get; }
        public ICommand PrintTicketCommand { get; }
        public ICommand OpenTicket { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public MainTicketViewModel() 
        {
            // Create objects 
            db = SUPER_TOUR.db;
            _listTickets = new ObservableCollection<TICKET>();

            // Create Command 
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            UpdateTicketCommand = new RelayCommand(ExecuteUpdateTicket);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchBooking);
            PrintTicketCommand = new RelayCommand(ExecutePrintTicket);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);

            LoadDataAsync();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += _timer_Tick;
        }
        #endregion

        #region Update on data on datagrid event
        private void RefreshDatagrid()
        {
            int index = ListTickets.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListTickets.RemoveAt(index);
                ListTickets.Insert(index, temp);
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
                        _listOriginalTicket = db.TICKETs.ToList();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
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
        private async void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        _tracker = UPDATE_CHECK.getTracker(table);
                        if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeTicket)
                        {
                            TimeTicket = (DateTime.Parse(_tracker.DateTimeUpdate));
                            _listOriginalTicket = db.TICKETs.ToList();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
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

        #region Update
        private void ExecuteUpdateTicket(object obj)
        {
            if(SelectedItem != null)
            {
                MyMessageBox.ShowDialog("Are you sure you want to update this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    if (SelectedItem.Status == "Paid")
                        SelectedItem.Status = "Canceled";
                    else
                        SelectedItem.Status = "Paid";
                    RefreshDatagrid();
                    db.SaveChanges();

                    MyMessageBox.ShowDialog("Update ticket's status succesful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);

                    // Synchronize real-time data
                    TimeTicket = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimeTicket);
                }
            }    
        }
        #endregion

        #region Print Ticket
        private void ExecutePrintTicket(object obj)
        {
            if (SelectedItem != null)
            {
                if (_selectedItem.Status == "Canceled")
                {
                    MyMessageBox.ShowDialog("Can not print canceled ticket!", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                ExportedTicketView exportedTicketView = new ExportedTicketView();
                exportedTicketView.DataContext = new ExportedTicketViewModel(_selectedItem);
                exportedTicketView.ShowDialog();
            }
        }
        #endregion

        #region Search
        private void ExecuteSelectFilter(object obj)
        {
            SearchTicket = "";
            _onSearching = false;
            ReloadData();
        }

        private void ExecuteSearchBooking(object obj)
        {
            if (string.IsNullOrEmpty(_searchTicket))
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

        private void SearchByIdNumber()
        {
            if (_listOriginalTicket == null || _listOriginalTicket.Count == 0)
                return;
            this._listTicketSearching = _listOriginalTicket.Where(p => p.TOURIST.BOOKING.CUSTOMER.IdNumber.Contains(_searchTicket)).ToList();
        }

        private void SearchByPhoneNumber()
        {
            if (_listOriginalTicket == null || _listOriginalTicket.Count == 0)
                return;
            this._listTicketSearching = _listOriginalTicket.Where(p => p.TOURIST.BOOKING.CUSTOMER.PhoneNumber.Contains(_searchTicket)).ToList();
        }
        #endregion

        #region Custom display data grid
        private List<TICKET> GetData(List<TICKET> ListTicket, int startIndex, int endIndex)
        {
            try
            {
                return ListTicket.OrderBy(m => m.Id_Ticket).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<TICKET> ListTicket)
        {
            try
            {
                this._totalResult = ListTicket.Count();
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

                _listTicketDatagrid = GetData(ListTicket, this._startIndex, this._endIndex);
                _listTickets.Clear();
                foreach (TICKET TICKET in _listTicketDatagrid)
                {
                    _listTickets.Add(TICKET);
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
                LoadDataByPage(_listTicketSearching);
            else
                LoadDataByPage(_listOriginalTicket);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTicketSearching);
            else
                LoadDataByPage(_listOriginalTicket);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                switch (_selectedFilter)
                {
                    case "ID Number":
                        SearchByIdNumber();
                        break;
                    case "Phone Number":
                        SearchByPhoneNumber();
                        break;
                }
                LoadDataByPage(_listTicketSearching);
            }
            else
                LoadDataByPage(_listOriginalTicket);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
