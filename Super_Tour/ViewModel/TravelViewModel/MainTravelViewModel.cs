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
        private ObservableCollection<TRAVEL> _listObservableTravel;
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

        public ObservableCollection<TRAVEL> ListObservableTravel
        { get { return _listObservableTravel; }
           set
            {
                _listObservableTravel = value;
                OnPropertyChanged(nameof(ListObservableTravel));
            }
        }
        public ICommand OpenCreateTravelViewCommand { get; }
        public ICommand DeleteTravelCommand { get; }
        public ICommand SearchTravelCommand { get; }
        public ICommand UpdateTravelCommand { get; }
        public MainTravelViewModel() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick; 
            OpenCreateTravelViewCommand = new RelayCommand(ExecuteOpenCreateTravelViewCommand);
            this._listObservableTravel = new ObservableCollection<TRAVEL>();
            LoadFilter();
            SearchTravelCommand = new RelayCommand(ExecuteSearchTravel);
            DeleteTravelCommand = new RelayCommand(ExecuteDeleteTravel);
            UpdateTravelCommand = new RelayCommand(ExecuteUpdateCommand);
            LoadTourDataAsync();
        }
        private async void ExecuteUpdateCommand(object obj)
        {
            TRAVEL travel = obj as TRAVEL;
            UpdateTravelView view = new UpdateTravelView();
            timer.Stop();
            view.DataContext = new UpdateTravelViewModel(travel);
            view.ShowDialog();
            LoadTourDataAsync();
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
        private async void ExecuteDeleteTravel(object obj)
        {

            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    TRAVEL travel = obj as TRAVEL;
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
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    _listObservableTravel.Remove(TravelFind);
                    _listOriginalTravel.Remove(TravelFind);
                }
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
