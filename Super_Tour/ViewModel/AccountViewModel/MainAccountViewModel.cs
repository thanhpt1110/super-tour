using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace Super_Tour.ViewModel
{
    internal class MainAccountViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        public static DateTime TimeAccount;
        private UPDATE_CHECK _tracker = null;
        private List<ACCOUNT> _listAccountOriginal = null;
        private List<ACCOUNT> _listAccountSearching = null;
        private List<ACCOUNT> _listAccountDatagrid = null;
        private ObservableCollection<ACCOUNT> _listAccounts = null;
        private ACCOUNT _selectedItem = null;
        private ACCOUNT temp = null;
        private DispatcherTimer _timer = null;
        private string _searchUserName = "";
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
        private string table = "UPDATE_ACCOUNT";
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

        public string SearchUserName
        {
            get { return _searchUserName; }
            set
            {
                _searchUserName = value;
                OnPropertyChanged(nameof(SearchUserName));
            }
        }

        public ACCOUNT SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<ACCOUNT> ListAccounts
        {
            get
            {
                return _listAccounts;
            }
            set
            {
                _listAccounts = value;
                OnPropertyChanged(nameof(ListAccounts));
            }
        }
        #endregion

        #region Command
        public ICommand OpenCreateAccountViewCommand { get; }
        public ICommand DeleteAccountCommand { get; private set; }
        public ICommand OnSearchTextChangedCommand { get; private set; }
        public ICommand UpdateAccountCommand { get; private set; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public MainAccountViewModel()
        {
            db = SUPER_TOUR.db;
            _listAccounts = new ObservableCollection<ACCOUNT>();
            OpenCreateAccountViewCommand = new RelayCommand(ExecuteOpenCreateAccountViewCommand);
            DeleteAccountCommand = new RelayCommand(ExecuteDeleteAccountCommand);
            UpdateAccountCommand = new RelayCommand(ExecuteUpdateAccountCommand);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            OnSearchTextChangedCommand = new RelayCommand(SearchCommand);
            TimeAccount = DateTime.Now;
            LoadDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            Timer.Tick += Timer_Tick;
        }
        #endregion

        #region Update data on datagrid
        private void RefreshDatagrid()
        {
            int index = ListAccounts.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListAccounts.RemoveAt(index);
                ListAccounts.Insert(index, temp);
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
                            _listAccountOriginal = db.ACCOUNTs.ToList();
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

        private async Task ReloadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    _tracker = UPDATE_CHECK.getTracker(table);
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeAccount)
                    {
                        TimeAccount = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listAccountOriginal = db.ACCOUNTs.ToList();
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
        private void ExecuteOpenCreateAccountViewCommand(object obj)
        {
            try
            {
                if (temp == null)
                    temp = new ACCOUNT();
                CreateAccountView createAccountView = new CreateAccountView();
                createAccountView.DataContext = new CreateAccountViewModel(temp);
                createAccountView.ShowDialog();
                _listAccountOriginal = db.ACCOUNTs.ToList();
                ReloadData();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update
        private void ExecuteUpdateAccountCommand(object obj)
        {
            UpdateAccountView updateAccountView = new UpdateAccountView();
            updateAccountView.DataContext = new UpdateAccountViewModel(SelectedItem);
            updateAccountView.ShowDialog();
            RefreshDatagrid();
        }
        #endregion

        #region Delete
        private void ExecuteDeleteAccountCommand(object obj)
        {
            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Save data on database
                    db.ACCOUNTs.Remove(SelectedItem);
                    db.SaveChanges();

                    // Synchronize data to real-time database 
                    TimeAccount = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimeAccount);

                    // Process UI event
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    _listAccountOriginal.Remove(SelectedItem);
                    ReloadData();
                }
            }
            catch (Exception)
            {
                MyMessageBox.ShowDialog("The account could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Search
        private void SearchCommand(object obj)
        {
            if (string.IsNullOrEmpty(_searchUserName))
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

        #region Custom display data grid
        private List<ACCOUNT> GetData(List<ACCOUNT> ListAccount, int startIndex, int endIndex)
        {
            try
            {
                return ListAccount.OrderBy(m => m.Id_Account).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<ACCOUNT> ListAccounts)
        {
            try
            {
                this._totalResult = ListAccounts.Count();
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

                _listAccountDatagrid = GetData(ListAccounts, this._startIndex, this._endIndex);
                _listAccounts.Clear();
                foreach (ACCOUNT typePackage in _listAccountDatagrid)
                {
                    _listAccounts.Add(typePackage);
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
                LoadDataByPage(_listAccountSearching);
            else
                LoadDataByPage(_listAccountOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listAccountSearching);
            else
                LoadDataByPage(_listAccountOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                _listAccountSearching = _listAccounts.Where(p => p.Username.ToLower().Contains(_searchUserName.ToLower())).ToList();
                LoadDataByPage(_listAccountSearching);
            }
            else
                LoadDataByPage(_listAccountOriginal);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}