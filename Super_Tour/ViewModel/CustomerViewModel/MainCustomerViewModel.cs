using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
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
    internal class MainCustomerViewModel: ObservableObject
    {
        #region Declare variable 
        private SUPER_TOUR db = null;
        public static DateTime TimeCustomer;
        private UPDATE_CHECK _tracker = null;
        private List<CUSTOMER> _listOriginalCustomer = null;
        private List<CUSTOMER> _ListCustomerSearching = null;
        private List<CUSTOMER> _listCustomerDatagrid = null;
        private ObservableCollection<CUSTOMER> _listCustomers = null;
        private CUSTOMER _selectedItem = null;
        private CUSTOMER temp = null;
        private DispatcherTimer _timer = null;
        private string _searchCustomer = "";
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
        private string table = "UPDATE_CUSTOMER";
        #endregion

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

        public string SearchCustomer
        {
            get { return _searchCustomer; }
            set
            {
                _searchCustomer = value;
                OnPropertyChanged(nameof(SearchCustomer));
            }
        }

        public CUSTOMER SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<CUSTOMER> ListCustomers
        {
            get
            {
                return _listCustomers;
            }
            set
            {
                _listCustomers = value;
                OnPropertyChanged(nameof(ListCustomers));
            }
        }
        #endregion

        #region Command
        public ICommand OpenCreateCustomerViewCommand { get; }
        public ICommand UpdateCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor 
        public MainCustomerViewModel()
        {
            db = SUPER_TOUR.db;
            _listCustomers = new ObservableCollection<CUSTOMER>();
            OpenCreateCustomerViewCommand = new RelayCommand(ExecuteOpenCreateCustomerViewCommand);
            OnSearchTextChangedCommand = new RelayCommand(SearchCustomerName);
            UpdateCustomerCommand = new RelayCommand(ExecuteUpdateCustomer);
            DeleteCustomerCommand = new RelayCommand(ExecuteDeleteCustomer);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            LoadDataAsync();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
        }
        #endregion

        #region Update data on datagrid
        private void RefreshDatagrid()
        {
            int index = ListCustomers.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListCustomers.RemoveAt(index);
                ListCustomers.Insert(index, temp);
            }
        }
        #endregion

        #region Load data asycn
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
                            _listOriginalCustomer = db.CUSTOMERs.ToList();
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
                        if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeCustomer)
                        {
                            TimeCustomer = (DateTime.Parse(_tracker.DateTimeUpdate));
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _listOriginalCustomer = db.CUSTOMERs.ToList();
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
        private void ExecuteOpenCreateCustomerViewCommand(object obj)
        {
            try
            {
                if (temp == null)
                    temp = new CUSTOMER();
                CreateCustomerView createCustomerView = new CreateCustomerView();
                createCustomerView.DataContext = new CreateCustomerViewModel(temp);
                createCustomerView.ShowDialog();
                _listOriginalCustomer = db.CUSTOMERs.ToList();
                ReloadData();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update
        private void ExecuteUpdateCustomer(object obj)
        {
            try
            {
                UpdateCustomerView updateView = new UpdateCustomerView();
                updateView.DataContext = new UpdateCustomerViewModel(SelectedItem);
                updateView.ShowDialog();
                RefreshDatagrid();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Delete
        private void ExecuteDeleteCustomer(object obj)
        {
            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Save data on database
                    db.CUSTOMERs.Remove(SelectedItem);
                    db.SaveChanges();

                    // Synchronize data to real-time database 
                    TimeCustomer = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimeCustomer);

                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    // Process UI event
                    _listOriginalCustomer.Remove(SelectedItem);
                    ReloadData();
                }
            }
            catch (Exception)
            {
                MyMessageBox.ShowDialog("The package could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Search
        private void SearchCustomerName(object obj)
        {
            if (string.IsNullOrEmpty(_searchCustomer))
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

        #region Custom datagrid by page
        private List<CUSTOMER> GetData(List<CUSTOMER> ListCustomer, int startIndex, int endIndex)
        {
            try
            {
                return ListCustomer.OrderBy(m => m.Id_Customer).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<CUSTOMER> ListCustomers)
        {
            try
            {
                this._totalResult = ListCustomers.Count();
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

                _listCustomerDatagrid = GetData(ListCustomers, this._startIndex, this._endIndex);
                _listCustomers.Clear();
                foreach (CUSTOMER customer in _listCustomerDatagrid)
                {
                    _listCustomers.Add(customer);
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
                LoadDataByPage(_ListCustomerSearching);
            else
                LoadDataByPage(_listOriginalCustomer);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_ListCustomerSearching);
            else
                LoadDataByPage(_listOriginalCustomer);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                _ListCustomerSearching = _listOriginalCustomer.Where(p => p.Name_Customer.ToLower().Contains(_searchCustomer.ToLower())).ToList();
                LoadDataByPage(_ListCustomerSearching);
            }
            else
                LoadDataByPage(_listOriginalCustomer);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
