using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using MySqlX.XDevAPI;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainPackageViewModel : ObservableObject
    {
        #region Declare variable 
        private SUPER_TOUR db = null;
        public static DateTime TimePackage;
        private UPDATE_CHECK _tracker = null;
        private List<PACKAGE> _listOriginalPackage = null;
        private List<PACKAGE> _listPackagesSearching = null;
        private List<PACKAGE> _listPackageDatagrid = null;
        private ObservableCollection<PACKAGE> _listPackages = null;
        private PACKAGE _selectedItem = null;
        private PACKAGE temp = null;
        private DispatcherTimer _timer = null;
        private FirebaseStorage firebaseStorage = null;
        private string _searchPackageName = "";
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

        public string SearchPackageName
        {
            get { return _searchPackageName; }
            set
            {
                _searchPackageName = value;
                OnPropertyChanged(nameof(SearchPackageName));
            }
        }

        public PACKAGE SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<PACKAGE> ListPackages
        {
            get
            {
                return _listPackages;
            }
            set
            {
                _listPackages = value;
                OnPropertyChanged(nameof(ListPackages));
            }
        }
        #endregion

        #region Command 
        public ICommand OpenCreatePackageViewCommand { get; }
        public ICommand UpdatePackageViewCommand { get; }
        public ICommand DeletePackageCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public MainPackageViewModel()
        {
            db = SUPER_TOUR.db;
            _listPackages = new ObservableCollection<PACKAGE>();
            OpenCreatePackageViewCommand = new RelayCommand(ExecuteOpenCreatePackageViewCommand);
            UpdatePackageViewCommand = new RelayCommand(ExecuteUpdatePackage);
            DeletePackageCommand = new RelayCommand(ExecuteDeletePackage);
            OnSearchTextChangedCommand = new RelayCommand(SearchPackage);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");
            LoadDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
        }
        #endregion

        #region Update on data on datagrid event
        private void RefreshDatagrid()
        {
            int index = ListPackages.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListPackages.RemoveAt(index);
                ListPackages.Insert(index, temp);
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
                            _listOriginalPackage = db.PACKAGEs.ToList();
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
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        _tracker = UPDATE_CHECK.getTracker(table);
                        if(DateTime.Parse(_tracker.DateTimeUpdate) > TimePackage)
                        {
                            TimePackage = (DateTime.Parse(_tracker.DateTimeUpdate));
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _listOriginalPackage = db.PACKAGEs.ToList();
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
        private void ExecuteOpenCreatePackageViewCommand(object obj)
        {
            try
            {
                if (temp == null)
                    temp = new PACKAGE();
                CreatePackageView createPackageView = new CreatePackageView();
                createPackageView.DataContext = new CreatePackageViewModel(temp);
                createPackageView.ShowDialog();
                _listOriginalPackage = db.PACKAGEs.ToList();
                ReloadData();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update
        private void ExecuteUpdatePackage(object obj)
        {
            try
            {
                UpdatePackageView updateView = new UpdatePackageView();
                updateView.DataContext = new UpdatePackageViewModel(SelectedItem);
                updateView.ShowDialog();
                RefreshDatagrid();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Delete
        private void ExecuteDeletePackage(object obj)
        {
            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Save data on database
                    firebaseStorage.Child("Images").Child("Package" + _selectedItem.Id_Package.ToString()).DeleteAsync();
                    db.PACKAGEs.Remove(SelectedItem);
                    db.SaveChanges();

                    // Synchronize data to real-time database 
                    TimePackage = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimePackage);

                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    // Process UI event
                    _listOriginalPackage.Remove(SelectedItem);
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
        private void SearchPackage(object obj)
        {
            if (string.IsNullOrEmpty(_searchPackageName))
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
        private List<PACKAGE> GetData(List<PACKAGE> ListPackage, int startIndex, int endIndex)
        {
            try
            {
                return ListPackage.OrderBy(m => m.Id_Package).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<PACKAGE> ListPackages)
        {
            try
            {
                this._totalResult = ListPackages.Count();
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

                _listPackageDatagrid = GetData(ListPackages, this._startIndex, this._endIndex);
                _listPackages.Clear();
                foreach (PACKAGE package in _listPackageDatagrid)
                {
                    _listPackages.Add(package);
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
                LoadDataByPage(_listPackagesSearching);
            else
                LoadDataByPage(_listOriginalPackage);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listPackagesSearching);
            else
                LoadDataByPage(_listOriginalPackage);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                _listPackagesSearching = _listOriginalPackage.Where(p => p.Name_Package.ToLower().Contains(_searchPackageName.ToLower())).ToList();
                LoadDataByPage(_listPackagesSearching);
            }
            else
                LoadDataByPage(_listOriginalPackage);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
