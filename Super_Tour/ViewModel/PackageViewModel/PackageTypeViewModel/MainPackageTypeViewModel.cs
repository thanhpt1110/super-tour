using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;
using Super_Tour.Model;
using System.Windows;
using System.Data.Entity;
using System.Windows.Threading;
using Super_Tour.CustomControls;

namespace Super_Tour.ViewModel
{
    internal class MainPackageTypeViewModel : ObservableObject
    {
        #region Declare variable 
        private SUPER_TOUR db = null;
        public static DateTime TimePackageType;
        private UPDATE_CHECK _tracker = null;  
        private List<TYPE_PACKAGE> _listTypePackageOriginal = null; // Data gốc
        private List<TYPE_PACKAGE> _listTypePackageSearching = null; // Data lúc mà có người search
        private List<TYPE_PACKAGE> _listTypePackageDatagrid = null; // Data để đổ vào datagrid 
        private ObservableCollection<TYPE_PACKAGE> _listTypePackages = null ; // Data binding của Datagrid
        private TYPE_PACKAGE _selectedItem = null;
        private TYPE_PACKAGE temp = null;
        private DispatcherTimer _timer = null;
        private string _searchType = "";
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
        private string table= "UPDATE_TYPEPACKAGE";
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

        public TYPE_PACKAGE SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<TYPE_PACKAGE> ListTypePackages
        {
            get { return _listTypePackages; }
            set
            {
                _listTypePackages = value;
                OnPropertyChanged(nameof(ListTypePackages));
            }   
        }
        #endregion

        #region Command
        public ICommand OpenCreatePackageTypeViewCommand { get;private set; }
        public ICommand DeletePackageInDataGridView { get;private set; }
        public ICommand OnSearchTextChangedCommand { get; private set; }
        public ICommand UpdatePackageCommand { get;private set; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        #region Constructor
        public MainPackageTypeViewModel() 
        {
            db = SUPER_TOUR.db;
            _listTypePackages = new ObservableCollection<TYPE_PACKAGE>();
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
            DeletePackageInDataGridView = new RelayCommand(ExecuteDeletePackageCommand);
            UpdatePackageCommand = new RelayCommand(UpdatePackage);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            OnSearchTextChangedCommand = new RelayCommand(SearchCommand);
            TimePackageType = DateTime.Now;
            LoadDataAsync();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += Timer_Tick;
        }
        #endregion

        #region Update on data on datagrid event
        private void RefreshDatagrid()
        {
            int index = ListTypePackages.IndexOf(SelectedItem);
            temp = SelectedItem;
            if (index != -1)
            {
                ListTypePackages.RemoveAt(index);
                ListTypePackages.Insert(index, temp);
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
                            _listTypePackageOriginal = db.TYPE_PACKAGEs.ToList();
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
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > TimePackageType)
                    {
                        TimePackageType = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTypePackageOriginal = db.TYPE_PACKAGEs.ToList();
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
        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            try
            {
                if (temp == null)
                    temp = new TYPE_PACKAGE();
                CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
                createPackageTypeView.DataContext = new CreatePackageTypeViewModel(temp);
                createPackageTypeView.ShowDialog();
                _listTypePackageOriginal = db.TYPE_PACKAGEs.ToList();
                ReloadData();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update
        private void UpdatePackage(object obj)
        {
            UpdatePackageTypeView updatePackageView = new UpdatePackageTypeView();
            updatePackageView.DataContext = new UpdatePackageTypeViewModel(SelectedItem);
            updatePackageView.ShowDialog();
            RefreshDatagrid();
        }
        #endregion

        #region Delete
        private void ExecuteDeletePackageCommand(object obj)
        {
            try
            {
                if(SelectedItem.PACKAGEs.Count>0)
                {
                    MyMessageBox.ShowDialog("You couldn't delete this type packages.", "Warning", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Warning);
                    return;
                }    
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    // Save data on database
                    db.TYPE_PACKAGEs.Remove(SelectedItem);
                    db.SaveChanges();

                    // Synchronize data to real-time database 
                    TimePackageType = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimePackageType);

                    // Process UI event
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);    
                    _listTypePackageOriginal.Remove(SelectedItem);
                    ReloadData();
                }
            }
            catch (Exception)
            {
                MyMessageBox.ShowDialog("The package type could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Search
        private void SearchCommand(object obj)
        {
            if (string.IsNullOrEmpty(_searchType))
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
        private List<TYPE_PACKAGE> GetData(List<TYPE_PACKAGE> ListTypePackage,int startIndex, int endIndex)
        {
            try
            {
                return ListTypePackage.OrderBy(m => m.Id_Type_Package).Skip(startIndex).Take(endIndex-startIndex).ToList();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<TYPE_PACKAGE> ListTypePackages)
        {
            try
            {
                this._totalResult = ListTypePackages.Count();
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
                    if (this._totalPage < _currentPage )
                        _currentPage = this._totalPage;
                    this._startIndex = (this._currentPage - 1) * 13;
                    this._endIndex = Math.Min(this._startIndex + 13, _totalResult);
                }

                _listTypePackageDatagrid = GetData(ListTypePackages, this._startIndex, this._endIndex);
                _listTypePackages.Clear();
                foreach (TYPE_PACKAGE typePackage in _listTypePackageDatagrid)
                {
                    _listTypePackages.Add(typePackage);
                }
            }
            catch(Exception ex)
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
                LoadDataByPage(_listTypePackageSearching);
            else
                LoadDataByPage(_listTypePackageOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTypePackageSearching);
            else
                LoadDataByPage(_listTypePackageOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                _listTypePackageSearching = _listTypePackageOriginal.Where(p => p.Name_Type.ToLower().Contains(_searchType.ToLower())).ToList();
                LoadDataByPage(_listTypePackageSearching);
            }
            else
                LoadDataByPage(_listTypePackageOriginal);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
