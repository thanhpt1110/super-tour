﻿using System;
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
        private MainViewModel mainViewModel;
        private SUPER_TOUR db;
        private ObservableCollection<TRAVEL> _listObservableTravel;
        private DispatcherTimer _timer = null;  
        private List<TRAVEL> _listOriginalTravel;
        private string _searchItem;
        private string _selectedFilter;
        private ObservableCollection<string> _listFilter;

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
        #endregion

        #region Command 
        public ICommand OpenCreateTravelViewCommand { get; }
        public ICommand DeleteTravelCommand { get; }
        public ICommand SearchTravelCommand { get; }
        public ICommand UpdateTravelCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainTravelViewModel(MainViewModel mainViewModel) 
        {
            this.mainViewModel = mainViewModel;
            LoadFilter();
            OpenCreateTravelViewCommand = new RelayCommand(ExecuteOpenCreateTravelViewCommand);
            this._listObservableTravel = new ObservableCollection<TRAVEL>();
            SearchTravelCommand = new RelayCommand(ExecuteSearchTravel);
            DeleteTravelCommand = new RelayCommand(ExecuteDeleteTravel);
            UpdateTravelCommand = new RelayCommand(ExecuteUpdateCommand);
            LoadTourDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick; 
        }
        private async void ExecuteUpdateCommand(object obj)
        {
            TRAVEL travel = obj as TRAVEL;
            UpdateTravelViewModel updateTravelViewModel = new UpdateTravelViewModel(travel);
            Timer.Stop();
            mainViewModel.CurrentChildView = updateTravelViewModel;
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
                Timer.Start();
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
                    Timer.Start();
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
        }
        private void ExecuteOpenCreateTravelViewCommand(object obj)
        {
            CreateTravelViewModel createTravelViewModel = new CreateTravelViewModel(mainViewModel);
            mainViewModel.CurrentChildView = createTravelViewModel;
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
