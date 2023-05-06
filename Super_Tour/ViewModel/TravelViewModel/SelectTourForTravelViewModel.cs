using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Super_Tour.View;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using Super_Tour.Model;
using System.Windows.Threading;
using Super_Tour.CustomControls;
using System.Windows;
using System.Threading;

namespace Super_Tour.ViewModel
{
    internal class SelectTourForTravelViewModel: ObservableObject
    {
        //Test
        //End test
        private List<TOUR> _listToursOriginal;
        private List<TOUR> _listSearchTour;
        private readonly object _locker = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<DataGridTour> _listDataGridTour;
        private string _searchTour="";
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<string> _listSearchFilterBy;
        private DispatcherTimer timer = new DispatcherTimer();
        private string _selectedFilter = "Name";
        private TOUR _tour;

        public string SelectedFilter
        {
            get { return _selectedFilter; }
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

        public string SearchTour
        {
            get
            {
                return _searchTour;
            }
            set
            {
                _searchTour = value;
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
        public ICommand ViewTourDetail { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand SelectedFilterCommand { get; }
        public ICommand SelectTourCommand { get; }
        public SelectTourForTravelViewModel(TOUR tour)
        {

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            _listSearchTour = new List<TOUR>();
            timer.Tick += Timer_Tick; ;
            _listDataGridTour = new ObservableCollection<DataGridTour>();
            _listSearchFilterBy = new ObservableCollection<string>();
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            ViewTourDetail = new RelayCommand(ExecuteViewDetailTour);
            SelectTourCommand = new RelayCommand(ExecuteSelectTourCommand);
            _tour = tour;
            generateFilterItem();
            LoadTourDataAsync();
        }
        private void ExecuteSelectTourCommand(object obj)
        {
            DataGridTour dataGridTour = obj as DataGridTour;
            _tour = dataGridTour.Tour;
            SelectTourForTravelView selectTourForTravelView = null;
            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine(window.ToString());
                if (window is SelectTourForTravelView)
                {
                    selectTourForTravelView = window as SelectTourForTravelView;
                    break;
                }
            }
            selectTourForTravelView.Close();
        }
        private void ExecuteViewDetailTour(object obj)
        {

        }
        private void ExecuteSelectFilter(object obj)
        {
            SearchTour = "";
        }
        private void ExecuteSearchTour(object obj)
        {

        }
        private void LoadGrid(List<TOUR> listTour)
        {
            _listDataGridTour.Clear();
            foreach (TOUR tour in listTour)
            {
                decimal SumPrice = 0;
                if (db.TOUR_DETAILs.Where(p => p.Id_Tour == tour.Id_Tour).ToList().Count == 0)
                {
                    _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
                    continue;
                }
                foreach (TOUR_DETAILS tour_detail in db.TOUR_DETAILs.Where(p => p.Id_Tour == tour.Id_Tour).ToList())
                {
                    SumPrice += db.PACKAGEs.Find(tour_detail.Id_Package).Price;
                }
                _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
            }
        }
        private void SearchByName()
        {
                this._listSearchTour = _listToursOriginal.Where(p => p.Name_Tour.Contains(SearchTour)).ToList();
                LoadGrid(_listSearchTour);  
        }
        private void SearchByPlace()
        {
            
            this._listSearchTour= _listToursOriginal.Where(p=>p.PlaceOfTour.Contains(SearchTour)).ToList();
            LoadGrid(_listSearchTour);
        }
        private void generateFilterItem()
        {
            _listSearchFilterBy.Add("Name");
            _listSearchFilterBy.Add("Place");
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    List<TOUR> Updatetours = db.TOURs.ToList();
                    if (!Updatetours.SequenceEqual(_listToursOriginal))
                    {
                        _listToursOriginal = Updatetours;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (string.IsNullOrEmpty(_searchTour))
                                LoadGrid(_listToursOriginal);
                            LoadGrid(_listSearchTour);

                        });
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }
        public async Task LoadTourDataAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    _listToursOriginal = db.TOURs.ToList();
                    _listSearchTour.CopyTo(_listToursOriginal.ToArray());
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _listDataGridTour.Clear();
                        foreach (TOUR tour in _listToursOriginal)
                        {
                            decimal SumPrice = 0;
                            if (db.TOUR_DETAILs.Where(p => p.Id_Tour == tour.Id_Tour).ToList().Count == 0)
                            {
                                _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
                                continue;
                            }
                            foreach (TOUR_DETAILS tour_detail in db.TOUR_DETAILs.Where(p => p.Id_Tour == tour.Id_Tour).ToList())
                            {
                                SumPrice += db.PACKAGEs.Find(tour_detail.Id_Package).Price;
                            }
                            _listDataGridTour.Add(new DataGridTour() { Tour = tour, TotalPrice = SumPrice });
                        }
                    });
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
        }
    }
}
