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
    class GridTravel
    {
        public TRAVEL Travel { get; set; }
        public string nameTour { get; set; }
    }
    internal class MainTravelViewModel: ObservableObject
    {
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<GridTravel> _listObservableGridTravel;
        private DispatcherTimer timer = new DispatcherTimer();
        private List<TRAVEL> _listOriginalTravel;
        private string _searchItem;
        private string _selectedFilter;
        private ObservableCollection<string> _listFilter;
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

        public string SearchItem
        {
            get { return _searchItem; }
            set { _searchItem = value;
                OnPropertyChanged(nameof(SearchItem));
            }
        }

        public ObservableCollection<GridTravel> ListObservableTravel
        { get { return _listObservableGridTravel; }
           set
            { 
                _listObservableGridTravel = value;
                OnPropertyChanged(nameof(ListObservableTravel));
            }
        }
        public ICommand OpenCreateTravelViewCommand { get; }
        public ICommand DeleteTravelCommnand { get; }
        public ICommand SearchTravelCommand { get; }
        public ICommand UpdateTravelCommand { get; }
        public MainTravelViewModel() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick; 
            OpenCreateTravelViewCommand = new RelayCommand(ExecuteOpenCreateTravelViewCommand);
            this._listObservableGridTravel = new ObservableCollection<GridTravel>();
            LoadFilter();
            SearchTravelCommand = new RelayCommand(ExecuteSearchTravel);
            DeleteTravelCommnand = new RelayCommand(ExecuteDeleteTravel);
            UpdateTravelCommand = new RelayCommand(ExecuteUpdateCommand);
            LoadTourDataAsync();
        }
        private async void ExecuteUpdateCommand(object obj)
        {

        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    List<TRAVEL> Updatetours = db.TRAVELs.ToList();
                    if (!Updatetours.SequenceEqual(_listOriginalTravel))
                    {
                        _listOriginalTravel = Updatetours;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listObservableGridTravel.Clear();
                            foreach (TRAVEL travel in _listOriginalTravel)
                            {
                                string nameTour = db.TOURs.Find(travel.Id_Tour).Name_Tour;
                                _listObservableGridTravel.Add(new GridTravel() { Travel = travel, nameTour = nameTour });
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }
        private async void ExecuteDeleteTravel(object obj)
        {

            try
            {
                GridTravel gridTravel = obj as GridTravel;
                TRAVEL travel = gridTravel.Travel;
                timer.Stop();
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
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                timer.Start();
            }
        }
        public async Task LoadTourDataAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    _listOriginalTravel = db.TRAVELs.ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _listObservableGridTravel.Clear();
                        foreach (TRAVEL travel in _listOriginalTravel)
                        {
                            TOUR tour = db.TOURs.Find(travel.Id_Tour);
                            _listObservableGridTravel.Add(new GridTravel() { Travel = travel, nameTour = tour.Name_Tour });
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
        private void ExecuteOpenCreateTravelViewCommand(object obj)
        {
            CreateTravelView createTravelView = new CreateTravelView();
            createTravelView.ShowDialog();
        }

        private void ExecuteSearchTravel(object obj)
        {
            switch(_selectedFilter)
            {
            }
        }
        private void LoadFilter()
        {
            SelectedFilter = "Tour name";
            _listFilter = new ObservableCollection<string>();
            _listFilter.Add("Tour name");
            _listFilter.Add("Place");
        }
    }
}
