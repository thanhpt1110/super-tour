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
    class DataGridTour
    {
        private TOUR _tour;

        public TOUR Tour
        {
            get { return _tour; }
            set { _tour = value; }
        }
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; }
        }
    }

    internal class MainTourViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel mainViewModel;
        public static DateTime TimeTour;
        private UPDATE_CHECK _tracker = null;
        private List<TOUR> _listToursSearching = null;
        private List<TOUR> _listToursOriginal = null;
        private List<TOUR> _listTourDatagrid = null;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<DataGridTour> _listDataGridTour;
        private ObservableCollection<string> _listSearchFilterBy;
        private DispatcherTimer _timer = null;
        private DataGridTour _selectedItem = null;
        private DataGridTour temp = null;
        private Visibility _isLoading = Visibility.Hidden;
        private string _searchType = "";
        private string _selectedFilter = "Name";
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
        public Visibility IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set { _selectedFilter = value;
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
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Delay(TimeSpan.FromSeconds(0.5), _cancellationTokenSource.Token).ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                    {
                        if (_selectedFilter == "Name")
                            SearchByName();
                        else
                            SearchByPlace();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                OnPropertyChanged(nameof(SearchType));
            }
        }
        public DataGridTour SelectedItem
        {
            get { return _selectedItem; }
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

        #endregion

        #region Command
        public ICommand OpenCreateTourViewCommand { get; }
        public ICommand OnSearchTextChangedCommand { get;}
        public ICommand SelectedFilterCommand { get; }
        public ICommand DeleteTourCommnand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion
        #region Constructor
        public MainTourViewModel(MainViewModel mainViewModel) 
        {
            this.mainViewModel = mainViewModel;
            db = SUPER_TOUR.db;
            _listDataGridTour = new ObservableCollection<DataGridTour>();
            _listSearchFilterBy= new ObservableCollection<string>();
            OpenCreateTourViewCommand = new RelayCommand(ExecuteOpenCreateTourViewCommand);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            DeleteTourCommnand = new RelayCommand(ExecuteDeleteTour);
            UpdateTourCommand = new RelayCommand(ExecuteUpdateTourCommand);
            GoToPreviousPageCommand = new RelayCommand(ExecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExecuteGoToNextPageCommand);
            LoadDataAsync();
            generateFilterItem();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick;
        }
        #endregion

        #region Execute Command
        private void ExecuteUpdateTourCommand(object obj)
        {
            DataGridTour dataGridTour = (DataGridTour)obj;
            TOUR tour = dataGridTour.Tour;
            UpdateTourViewModel updateTourViewModel = new UpdateTourViewModel(tour, this, mainViewModel);
            mainViewModel.CurrentChildView = updateTourViewModel;
            mainViewModel.setFirstChild("Update Tour");
            
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
                    if (DateTime.Parse(_tracker.DateTimeUpdate) > TimeTour)
                    {
                        TimeTour = (DateTime.Parse(_tracker.DateTimeUpdate));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listToursOriginal = db.TOURs.ToList();
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
                _listDataGridTour.Clear();
                foreach (TOUR tour in _listTourDatagrid)
                {
                    decimal SumPrice = tour.TOUR_DETAILs
                .Where(p => p.Id_Tour == tour.Id_Tour)
                .Sum(p => p.PACKAGE.Price);
                    _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
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
                LoadDataByPage(_listToursSearching);
            else
                LoadDataByPage(_listToursOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listToursSearching);
            else
                LoadDataByPage(_listToursOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData()
        {
            if (_onSearching)
            {
                _listToursSearching = _listToursOriginal.Where(p => p.Name_Tour.ToLower().Contains(_searchType.ToLower())).ToList();
                LoadDataByPage(_listToursSearching);
            }
            else
                LoadDataByPage(_listToursOriginal);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
        private async void ExecuteDeleteTour(object obj)
        {

            try
            {
                DataGridTour dataGridTour = obj as DataGridTour;
                TOUR tourMain = dataGridTour.Tour;
                Timer.Stop();
                TOUR TourFind = await db.TOURs.FindAsync(tourMain.Id_Tour);
                if (db.TRAVELs.Where(p => p.Id_Tour == TourFind.Id_Tour).ToList().Count > 0)
                {
                    MyMessageBox.ShowDialog("The tour could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                if (TourFind != null)
                {
                    MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                    if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                    {
                        using (SUPER_TOUR db = new SUPER_TOUR())
                        {
                            List<TOUR_DETAILS> tour_details = db.TOUR_DETAILs.Where(p => p.Id_Tour == TourFind.Id_Tour).ToList();
                            foreach (TOUR_DETAILS tour_detail in tour_details)
                            {
                                db.TOUR_DETAILs.Remove(tour_detail);
                            }
                            await db.SaveChangesAsync();
                            List<TOUR_DETAILS> tour_details1 = db.TOUR_DETAILs.Where(p => p.Id_Tour == TourFind.Id_Tour).ToList();
                            db.TOURs.Remove(db.TOURs.Find(TourFind.Id_Tour));
                            await db.SaveChangesAsync();

                            MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                            //_listToursOriginal = db.TOURs.AsNoTracking().ToList();
                            _listDataGridTour.Remove(dataGridTour);
                        }
                    }
                }
                else
                {
                    MyMessageBox.ShowDialog("The tour could not be found.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
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

        private void ExecuteOpenCreateTourViewCommand(object obj)
        {
            CreateTourViewModel createTourViewModel = new CreateTourViewModel(this,mainViewModel);
            mainViewModel.CurrentChildView = createTourViewModel;
            mainViewModel.setFirstChild("Add Tour");
        }
        private void ExecuteSelectFilter(object obj)
        {
            SearchType = "";
        }

        private void ExecuteSearchTour(object obj)
        {
/*            switch(_selectedFilter)
            {
                default:
                    return;
            }    */
        }
        private void SearchByName()
        {
/*            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listSearchTour = _listToursOriginal.Where(p => p.Name_Tour.Contains(_searchTour)).ToList();
            LoadGrid(_listSearchTour);*/
        }

        private void SearchByPlace()
        {
/*            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listSearchTour = _listToursOriginal.Where(p => p.PlaceOfTour.Contains(SearchTour)).ToList();
            LoadGrid(_listSearchTour);*/
        }
        private void generateFilterItem()
        {
            _listSearchFilterBy.Add("Name");
            _listSearchFilterBy.Add("Place");
        }
        #endregion
        #region Load đata
        private void LoadGrid(List<TOUR> listTour)
        {
            _listDataGridTour.Clear();
            foreach (TOUR tour in listTour)
            {
                decimal SumPrice = tour.TOUR_DETAILs
                .Where(p => p.Id_Tour == tour.Id_Tour)
                .Sum(p => p.PACKAGE.Price);
                _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
            }
        }       
        #endregion
    }
}
