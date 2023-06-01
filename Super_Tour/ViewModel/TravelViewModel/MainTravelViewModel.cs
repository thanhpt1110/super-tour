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

    internal class MainTravelViewModel: ObservableObject
    {
        private SUPER_TOUR db;
        private MainViewModel mainViewModel;
        public static DateTime TimeTravel;
        private UPDATE_CHECK _tracker = null;
        private ObservableCollection<string> _listFilter;
        private List<TRAVEL> _listTravelOriginal = null; // Data gốc
        private List<TRAVEL> _listTravelSearching = null; // Data lúc mà có người search
        private List<TRAVEL> _listTravelDataGrid = null; // Data để đổ vào datagrid 
        private ObservableCollection<TRAVEL> _listTravels = null; // Data binding của Datagrid
        private TRAVEL _selectedItem = null;
        private TRAVEL temp = null;
        private DispatcherTimer _timer = null;
        private ObservableCollection<string> _listSearchFilterBy;
        private string _selectedFilter = "";
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
        private string table = "UPDATE_TRAVEL";
        #region Declare binding
        public ObservableCollection<string> ListFilter
        {
            get { return _listFilter; }
            set { _listFilter = value;
                OnPropertyChanged(nameof(ListFilter));
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
        public ObservableCollection<string> ListSearchFilterBy
        {
            get
            {
                return _listSearchFilterBy;
            }
            set
            {
                _listSearchFilterBy = value;
                OnPropertyChanged(nameof(ListSearchFilterBy));
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

        public TRAVEL SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<TRAVEL> ListTravels
        {
            get { return _listTravels ; }
            set
            {
                _listTravels = value;
                OnPropertyChanged(nameof(ListTravels));
            }
        }
        #endregion

        #region Command 
        public ICommand OpenCreateTravelViewCommand { get; }
        public ICommand DeleteTravelCommand { get; }
        public ICommand SearchTravelCommand { get; }
        public ICommand UpdateTravelCommand { get; }
        public ICommand SelectedFilterCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainTravelViewModel(MainViewModel mainViewModel) 
        {
            this.mainViewModel = mainViewModel;
            db = SUPER_TOUR.db;
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            OpenCreateTravelViewCommand = new RelayCommand(ExecuteOpenCreateTravelViewCommand);
            SearchTravelCommand = new RelayCommand(ExecuteSearchTravel);
            DeleteTravelCommand = new RelayCommand(ExecuteDeleteTravel);
            UpdateTravelCommand = new RelayCommand(ExecuteUpdateCommand);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            this._listTravels = new ObservableCollection<TRAVEL>();
            generateFilterItem();
            LoadDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick; 
        }
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
                            _listTravelOriginal = db.TRAVELs.ToList();
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

        public async Task ReloadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    _tracker = UPDATE_CHECK.getTracker(table);
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeTravel)
                    {
                        TimeTravel = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTravelOriginal = db.TRAVELs.ToList();
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
        #region Update
        private async void ExecuteUpdateCommand(object obj)
        {
            TRAVEL travel = obj as TRAVEL;
            UpdateTravelViewModel updateTravelViewModel = new UpdateTravelViewModel(travel,this,mainViewModel);
            mainViewModel.CurrentChildView = updateTravelViewModel;
            mainViewModel.setFirstChild("Update Travel");
        }
        #endregion
        #region Delete
        private async void ExecuteDeleteTravel(object obj)
        {

            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    TRAVEL travel = obj as TRAVEL;
                    Timer.Stop();
                    TRAVEL TravelFind = await db.TRAVELs.FindAsync(travel.Id_Travel);
                    if (TravelFind == null)
                    {
                        MyMessageBox.ShowDialog("The tour could not be found.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                        return;
                    }
                    if (db.BOOKINGs.Where(p => p.Id_Travel == TravelFind.Id_Travel).ToList().Count > 0)
                    {
                        MyMessageBox.ShowDialog("The tour could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                        return;
                    }

                    db.TRAVELs.Remove(TravelFind);
                    await db.SaveChangesAsync();
                    TimeTravel = DateTime.Now;
                    UPDATE_CHECK.NotifyChange(table, TimeTravel);
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    _listTravels.Remove(TravelFind);
                    _listTravelOriginal.Remove(TravelFind);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                Timer.Start();
            }
        }
        #endregion
        #region Insert
        private void ExecuteOpenCreateTravelViewCommand(object obj)
        {
            CreateTravelViewModel createTravelViewModel = new CreateTravelViewModel(mainViewModel,this);
            mainViewModel.CurrentChildView = createTravelViewModel;
            mainViewModel.setFirstChild("Add Travel");
        }
        #endregion
        #region Searching
        private void ExecuteSelectFilter(object obj)
        {
            SearchType = "";
            _onSearching = false;
            ReloadData();
        }
        private void ExecuteSearchTravel(object obj)
        {
            _onSearching = true;
            ReloadData();
        }
        private void SearchByName()
        {
            if (_listTravelOriginal == null || _listTravelOriginal.Count == 0)
                return;
                this._listTravelSearching = _listTravelOriginal.Where(p => p.TOUR.Name_Tour.Contains(_searchType)).ToList();
        }

        private void SearchByPlace()
        {
            if (_listTravelOriginal == null || _listTravelOriginal.Count == 0)
                return;
            this._listTravelSearching = _listTravelOriginal.Where(p => p.TOUR.PlaceOfTour.Contains(_searchType)).ToList();

        }
        private void generateFilterItem()
        {
            _listSearchFilterBy = new ObservableCollection<string>();
            _listSearchFilterBy.Add("Tour Name");
            _listSearchFilterBy.Add("Tour Place");
            SelectedFilter = "Tour Name";
        }
        #endregion
        #region Custom display data grid
        private List<TRAVEL> GetData(List<TRAVEL> ListTravel, int startIndex, int endIndex)
        {
            try
            {
                return ListTravel.OrderBy(m => m.Id_Travel).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<TRAVEL> ListTravels)
        {
            try
            {
                this._totalResult = ListTravels.Count();
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

                _listTravelDataGrid = GetData(ListTravels, this._startIndex, this._endIndex);
                _listTravels.Clear();
                foreach (TRAVEL travel in _listTravelDataGrid)
                {
                    _listTravels.Add(travel);
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
                LoadDataByPage(_listTravelSearching);
            else
                LoadDataByPage(_listTravelOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTravelSearching);
            else
                LoadDataByPage(_listTravelOriginal);

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
                LoadDataByPage(_listTravelSearching);
            }
            else
                LoadDataByPage(_listTravelOriginal);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}
