using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
namespace Super_Tour.ViewModel
{
    internal class SelectTravelForBookingViewModel: ObservableObject
    {
        private SUPER_TOUR db;
        private TRAVEL _travel;
        private ObservableCollection<TRAVEL> _listObservableTravel;
        private DispatcherTimer timer = new DispatcherTimer();
        private List<TRAVEL> _listOriginalTravel;
        private string _searchItem;
        private string _selectedFilter;
        private ObservableCollection<string> _listFilter;
        public ObservableCollection<string> ListFilter
        {
            get { return _listFilter; }
            set
            {
                _listFilter = value;
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
            set
            {
                _searchItem = value;
                OnPropertyChanged(nameof(SearchItem));
            }
        }

        public ObservableCollection<TRAVEL> ListObservableTravel
        {
            get { return _listObservableTravel; }
            set
            {
                _listObservableTravel = value;
                OnPropertyChanged(nameof(ListObservableTravel));
            }
        }
        public ICommand SelectTravelForBookingCommand { get; }
        public SelectTravelForBookingViewModel(TRAVEL travel)
        {
            _travel=travel;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick; ;
            this._listObservableTravel = new ObservableCollection<TRAVEL>();
            SelectTravelForBookingCommand = new RelayCommand(ExecuteSelectTravelForBookingCommand);
            LoadFilter();
            LoadTourDataAsync();
        }
        private void ExecuteSelectTravelForBookingCommand(object obj)
        {
            TRAVEL pickedTravel = obj as TRAVEL;
            _travel.Id_Travel = pickedTravel.Id_Travel;
            SelectTravelForBookingView selectTravelForBookingView = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window is SelectTravelForBookingView)
                {
                    selectTravelForBookingView = window as SelectTravelForBookingView;
                    break;
                }
            }
            selectTravelForBookingView.Close();
        }
        private void LoadGrid(List<TRAVEL> listTravel)
        {
            _listObservableTravel.Clear();
            foreach (TRAVEL travel in _listOriginalTravel)
            {
                _listObservableTravel.Add(travel);
            }
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    db.Dispose();
                    db = new SUPER_TOUR();
                    List<TRAVEL> Updatetours = db.TRAVELs.ToList();
                    if (!Updatetours.SequenceEqual(_listOriginalTravel))
                    {
                        _listOriginalTravel = Updatetours;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadGrid(_listOriginalTravel);
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
                    if (db != null)
                    {
                        db.Dispose();
                    }
                    db = new SUPER_TOUR();
                    _listOriginalTravel = db.TRAVELs.ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadGrid(_listOriginalTravel);
                    });
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
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
