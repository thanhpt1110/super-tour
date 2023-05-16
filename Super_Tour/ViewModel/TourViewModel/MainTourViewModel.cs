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
        private List<TOUR> _listSearchTour;
        private List<TOUR> _listToursOriginal;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<DataGridTour> _listDataGridTour;
        private string _searchTour = null;
        private SUPER_TOUR db = null;
        private ObservableCollection<string> _listSearchFilterBy;
        private DispatcherTimer _timer = null;
        private string _selectedFilter = "Name";
        private Visibility _isLoading = Visibility.Hidden;
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

        #endregion

        #region Command
        public ICommand OpenCreateTourViewCommand { get; }
        public ICommand OnSearchTextChangedCommand { get;}
        public ICommand SelectedFilterCommand { get; }
        public ICommand DeleteTourCommnand { get; }
        public ICommand UpdateTourCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainTourViewModel() 
        {
            db = new SUPER_TOUR();
            _listDataGridTour = new ObservableCollection<DataGridTour>();
            _listSearchFilterBy= new ObservableCollection<string>();
            OpenCreateTourViewCommand = new RelayCommand(ExecuteOpenCreateTourViewCommand);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTour);
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            DeleteTourCommnand = new RelayCommand(ExecuteDeleteTour);
            UpdateTourCommand = new RelayCommand(ExecuteUpdateTourCommand);
            LoadTourDataAsync();
            generateFilterItem();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick;
        }

        private void ExecuteUpdateTourCommand(object obj)
        {
            Timer.Stop();
            DataGridTour dataGridTour = (DataGridTour)obj;
            TOUR tour = dataGridTour.Tour;
            UpdateTourView view = new UpdateTourView();
            view.DataContext = new UpdateTourViewModel(tour);
            view.ShowDialog();
            LoadTourDataAsync();
        }

        private void ExecuteSelectFilter(object obj)
        {
            SearchTour = "";
        }

        private void ExecuteSearchTour(object obj)
        {
            switch(_selectedFilter)
            {

            }    
        }

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

        private void generateFilterItem()
        {
            _listSearchFilterBy.Add("Name");
            _listSearchFilterBy.Add("Place");
        }

        private void SearchByName()
        {
            if (_listToursOriginal == null || _listToursOriginal.Count == 0)
                return;
            this._listSearchTour = _listToursOriginal.Where(p => p.Name_Tour.Contains(_searchTour)).ToList();
            LoadGrid(_listSearchTour);
        }

        private void SearchByPlace()
        {
            this._listSearchTour = _listToursOriginal.Where(p => p.PlaceOfTour.Contains(SearchTour)).ToList();
            LoadGrid(_listSearchTour);
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {

/*                        if (db != null)
                        {
                            db.Dispose();
                        }
                        db = new SUPER_TOUR();*/
                        List<TOUR> Updatetours = db.TOURs.ToList();
                        db.Entry(Updatetours).Reload();

                        if (!Updatetours.SequenceEqual(_listToursOriginal))
                        {
                            _listToursOriginal = Updatetours;
                            Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadGrid(_listToursOriginal);

                        });
                        }
                    
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }

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
            catch(Exception ex)
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
            CreateTourView createTourView = new CreateTourView();
            createTourView.ShowDialog();
        }
        public async Task LoadTourDataAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                    db = new SUPER_TOUR();
                    _listToursOriginal = await db.TOURs.ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadGrid(_listToursOriginal);
                    });
                    Timer.Start();
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
        }
    }
}
