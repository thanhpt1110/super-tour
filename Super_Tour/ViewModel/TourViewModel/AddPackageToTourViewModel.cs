using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Super_Tour.ViewModel.CreateTourViewModel;
using static Super_Tour.ViewModel.SelectTourForTravelViewModel;

namespace Super_Tour.ViewModel
{

    internal class AddPackageToTourViewModel: ObservableObject
    {
        private TYPE_PACKAGE _selectedTypePackage;
        private List<PACKAGE> _listAvailablePackage;
        private bool _executeSave=true;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<PACKAGE> _observableListAvailablePackage;
        private ObservableCollection<PACKAGE> _observableListPickedPackage;
        private ObservableCollection<GridActivity> _listTourDetail;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage;
        private TOUR _tour;

        public ObservableCollection<TYPE_PACKAGE> ListTypePackage
        {
            get { return _listTypePackage; }
            set {
                _listTypePackage = value;
                OnPropertyChanged(nameof(ListTypePackage));
            }
        }

        public TYPE_PACKAGE SelectedTypePackage
        {
            get => _selectedTypePackage;
            set
            {
                _selectedTypePackage = value;
                OnPropertyChanged(nameof(SelectedTypePackage));
            }
        }
        public ObservableCollection<PACKAGE> ObservableListAvailablePackage
        {
            get
            {
                return _observableListAvailablePackage;
            }
            set
            {
                _observableListAvailablePackage = value;
                OnPropertyChanged(nameof(ObservableListAvailablePackage));
            }
        }
        public ObservableCollection<PACKAGE> ObservableListPickedPackage
        {
            get
            {
                return _observableListPickedPackage;
            }
            set
            {
                _observableListPickedPackage = value;
                OnPropertyChanged(nameof(ObservableListPickedPackage));
            }
        }
        public ICommand AddAvailablePackageCommand { get; }
        public ICommand DeletePickedPackageCommand { get; }
        public ICommand SavePackageCommand { get; }
        public ICommand CreateNewPacakgeCommand { get; }
        public ICommand SearchCommand { get; }
        public AddPackageToTourViewModel() 
        {
            AddAvailablePackageCommand = new RelayCommand(ExecuteAddNewPacakge);
            DeletePickedPackageCommand = new RelayCommand(ExecuteDeletePickedPackage);
            SavePackageCommand = new RelayCommand(ExecuteSavePackage, CanExecuteSavePackage);
            CreateNewPacakgeCommand = new RelayCommand(ExecuteCreatePackgeCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
        }
        private void LoadTypePackage()
        {
            List<TYPE_PACKAGE> package =  db.TYPE_PACKAGEs.ToList();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>(package);
        }
        private void ExecuteSearchCommand(object obj)
        {
            /*            List<PACKAGE> listPackageSearch = _listAvailablePackage.Where(p => p.Id_Type_Package == _selectedTypePackage.Id_Type_Package).ToList();
                        _listAvailablePackage.Clear();
                        foreach(PACKAGE package in listPackageSearch)
                        {
                            _listAvailablePackage.Add(package);
                        }    */
            LoadWithSearch();
        }
        private void LoadWithSearch()
        {
            List<PACKAGE> listSearchType = this._listAvailablePackage.Where(p => p.Id_Type_Package == _selectedTypePackage.Id_Type_Package).ToList();
            ObservableListAvailablePackage.Clear();
            foreach (PACKAGE package in listSearchType)
            {
                ObservableListAvailablePackage.Add(package);
            }
            foreach (PACKAGE package in _observableListPickedPackage)
            {
                ObservableListAvailablePackage.Remove(package);
            }
        }
        private void ExecuteCreatePackgeCommand(object obj)
        {
            CreatePackageView view = new CreatePackageView();
            view.ShowDialog();
            ReloadPackage(); 
        }
        public void ExecuteUpdatePackage(object obj)
        {
            try
            {
                _executeSave = false;

                if (_listTourDetail.Count <= _observableListPickedPackage.Count)
            {
                int i = 0;
                foreach (PACKAGE package in ObservableListPickedPackage)
                {
                    if (i >= _listTourDetail.Count)
                    {
                        TOUR_DETAILS tour_detail = new TOUR_DETAILS();
                        tour_detail.Id_Package = package.Id_Package;
                        tour_detail.Id_Tour = _tour.Id_Tour;
                        string namePacakge = db.PACKAGEs.Find(tour_detail.Id_Package).Name_Package;
                        tour_detail.Id_TourDetails = 1;
                        _listTourDetail.Add(new GridActivity() { Tour_detail = tour_detail, PackageName = namePacakge });
                    }
                    else
                    {
                        _listTourDetail[i].Tour_detail.Id_Package = package.Id_Package;
                        _listTourDetail[i].PackageName = package.Name_Package;

                    }
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach (GridActivity gridActivity in _listTourDetail)
                {
                    if (ObservableListPickedPackage.Count <= i)
                        break;
                    gridActivity.Tour_detail.Id_Package = ObservableListPickedPackage[i].Id_Package;
                    gridActivity.PackageName = ObservableListPickedPackage[i].Name_Package;
                    i++;
                }
                for(;i< _listTourDetail.Count;i++)
                {
                    _listTourDetail.RemoveAt(i);
                    i--;
                }    
            }
            AddPackageToTourView addPackageToTourView = null;
            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine(window.ToString());
                if (window is AddPackageToTourView)
                {
                    addPackageToTourView = window as AddPackageToTourView;
                    break;
                }
            }
            addPackageToTourView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
            finally
            {
                _executeSave = true;
            }
        }
        public void ExecuteSavePackage(object obj)
        {
            try
            {
                _executeSave = false;

                    _listTourDetail.Clear();
                    foreach (PACKAGE package in ObservableListPickedPackage)
                    {
                        TOUR_DETAILS tour_detail = new TOUR_DETAILS();
                        tour_detail.Id_Package = package.Id_Package;
                        string namePacakge = db.PACKAGEs.Find(tour_detail.Id_Package).Name_Package;
                        tour_detail.Id_TourDetails = 1;
                        _listTourDetail.Add(new GridActivity() { Tour_detail = tour_detail, PackageName = namePacakge });
                    }
                
                AddPackageToTourView addPackageToTourView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is AddPackageToTourView)
                    {
                        addPackageToTourView = window as AddPackageToTourView;
                        break;
                    }
                }
                addPackageToTourView.Close();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
            finally
            {
                _executeSave = true;
            }
        }
        public bool CanExecuteSavePackage(object obj)
        {
            return _executeSave;
        }
        public AddPackageToTourViewModel(ObservableCollection<GridActivity> listTourDetail,bool isUpdate,TOUR tour=null)
        {
            _tour = tour;
            _listTourDetail = listTourDetail;
            _observableListPickedPackage = new ObservableCollection<PACKAGE>();
            AddAvailablePackageCommand = new RelayCommand(ExecuteAddNewPacakge);
            if(isUpdate)
                SavePackageCommand = new RelayCommand(ExecuteUpdatePackage, CanExecuteSavePackage);
            else
            SavePackageCommand = new RelayCommand(ExecuteSavePackage, CanExecuteSavePackage);
            DeletePickedPackageCommand = new RelayCommand(ExecuteDeletePickedPackage);
            CreateNewPacakgeCommand = new RelayCommand(ExecuteCreatePackgeCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            LoadTypePackage();
            LoadAvailablePackage();
        }
        public void ExecuteDeletePickedPackage(object obj)
        {
            PACKAGE package = obj as PACKAGE;
            ObservableListPickedPackage.Remove(package);
            ObservableListAvailablePackage.Add(package);
        }
        public void ExecuteAddNewPacakge(object obj)
        {
            PACKAGE package = obj as PACKAGE;
            ObservableListAvailablePackage.Remove(package);
            ObservableListPickedPackage.Add(package);
        }
        private async Task LoadAvailablePackage()
        {
            await Task.Run(() =>
            {
                try {
                    _listAvailablePackage=db.PACKAGEs.ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ObservableListAvailablePackage = new ObservableCollection<PACKAGE>(_listAvailablePackage);
                        if(_listTourDetail.Count>0)
                        {

                            foreach(GridActivity item in _listTourDetail)
                            {
                                PACKAGE package = db.PACKAGEs.Find(item.Tour_detail.Id_Package);
                                ObservableListAvailablePackage.Remove(package);
                                ObservableListPickedPackage.Add(package);
                            }
                        }
                    });
                }
                catch(Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }
        private async Task ReloadPackage()
        {
            await Task.Run(() =>
            {
                try
                {
                    _listAvailablePackage = db.PACKAGEs.ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ObservableListAvailablePackage = new ObservableCollection<PACKAGE>(_listAvailablePackage);

                            foreach (PACKAGE item in ObservableListPickedPackage)
                            {
                                ObservableListAvailablePackage.Remove(item);                            
                            }
                         if(_selectedTypePackage!=null)
                        {
                            LoadWithSearch();
                        }    
                    }
                            
                    );
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }
    }
}
