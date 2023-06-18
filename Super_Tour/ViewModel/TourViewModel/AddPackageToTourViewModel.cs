using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using Super_Tour.ViewModel.PackageViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Super_Tour.ViewModel.CreateTourViewModel;

namespace Super_Tour.ViewModel
{

    internal class AddPackageToTourViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private TYPE_PACKAGE _selectedTypePackage;
        private PACKAGE _selectedItem = null;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage;
        private List<PACKAGE> _listAvailablePackage;
        private Province _selectedProvince;
        private List<Province> _listProvince;
        private ObservableCollection<Province> _listProvinces;
        private ObservableCollection<PACKAGE> _observableListAvailablePackage;
        private ObservableCollection<PACKAGE> _observableListSelectedPackage;
        private TOUR _tour;
        private ObservableCollection<GridActivity> _listTourDetail;
        private PACKAGE _package = null;
        private bool _isDataModified = false;
        private int _noRowInSelectedDatagrid = 0;
        #endregion

        #region Declare binding
        public PACKAGE SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public Province SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));    
            }
        }

        public ObservableCollection<Province> ListProvinces
        {
            get { return _listProvinces; }
            set
            {
                _listProvinces = value;
                OnPropertyChanged(nameof(ListProvinces));   
            }
        }

        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));  
            }
        }

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

        private int NoRowInSelectedDatagrid
        {
            get { return _noRowInSelectedDatagrid; }
            set
            {
                _noRowInSelectedDatagrid = value;
                if (_noRowInSelectedDatagrid > 0)
                    IsDataModified = true;
                else 
                    IsDataModified = false; 
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
        public ObservableCollection<PACKAGE> ObservableListSelectedPackage
        {
            get
            {
                return _observableListSelectedPackage;
            }
            set
            {
                _observableListSelectedPackage = value;
                OnPropertyChanged(nameof(ObservableListSelectedPackage));
            }
        }
        #endregion

        #region Command
        public ICommand CreateNewPacakgeCommand { get; }
        public ICommand SavePackageCommand { get; }
        public ICommand AddAvailablePackageCommand { get; }
        public ICommand DeleteSelectedPackageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SearchProvinceCommand { get; }  
        public ICommand ViewPackageDetailCommand { get; }
        #endregion

        #region Constructor
        public AddPackageToTourViewModel() 
        {

        }

        public AddPackageToTourViewModel(ObservableCollection<GridActivity> listTourDetail, bool isUpdate = false, TOUR tour = null)
        {
            // Pass object from prev class
            _tour = tour;
            _listTourDetail = listTourDetail;

            // Create Object and Command
            db = SUPER_TOUR.db;
            _observableListSelectedPackage = new ObservableCollection<PACKAGE>();
            AddAvailablePackageCommand = new RelayCommand(ExecuteAddNewPacakge);
            DeleteSelectedPackageCommand = new RelayCommand(ExecuteDeletePickedPackage);
            CreateNewPacakgeCommand = new RelayCommand(ExecuteCreatePackgeCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            SearchProvinceCommand = new RelayCommand(ExecuteSearchProvinceCommand);
            if (isUpdate)
                SavePackageCommand = new RelayCommand(ExecuteUpdatePackage);
            else
                SavePackageCommand = new RelayCommand(ExecuteSavePackage);
            ViewPackageDetailCommand = new RelayCommand(ExecuteViewPackageDetailCommand);
            // Load combo box TypePackage
            LoadTypePackage();

            // Load combo box Province 
            LoadProvinces();

            // Load datagrid Available Pacakage
            LoadAvailablePackage();
        }
        #endregion

        #region Load view
        private void LoadTypePackage()
        {
            List<TYPE_PACKAGE> package =  db.TYPE_PACKAGEs.OrderBy(p => p.Name_Type).ToList();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>(package);

            // Add option load all Type package
            TYPE_PACKAGE All = new TYPE_PACKAGE();
            All.Name_Type = "All";
            All.Id_Type_Package = -1;
            _listTypePackage.Insert(0, All);
            SelectedTypePackage = All;
        }

        private void LoadProvinces()
        {
            _listProvince = Get_Api_Address.getProvinces();
            _listProvinces = new ObservableCollection<Province>(_listProvince);

            // Add option load all Province
            Province All = new Province();
            All.name = "All";
            All.codename = "-1";
            _listProvinces.Insert(0, All);
            SelectedProvince = All;
        }

        private async Task LoadAvailablePackage()
        {
            await Task.Run(() =>
            {
                try
                {
                    _listAvailablePackage = db.PACKAGEs.ToList();
                    ObservableListAvailablePackage = new ObservableCollection<PACKAGE>(_listAvailablePackage);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_listTourDetail.Count > 0)
                        {
                            foreach (GridActivity item in _listTourDetail)
                            {
                                PACKAGE package = db.PACKAGEs.Find(item.Tour_detail.Id_Package);
                                ObservableListSelectedPackage.Add(package);
                                ObservableListAvailablePackage.Remove(package);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }

        private void LoadWithSearch()
        {
            // Process query
            List<PACKAGE> listSearchType = null;

            // Search by Province first
            if (_selectedProvince.codename == "-1")
                listSearchType = _listAvailablePackage;
            else
                listSearchType = _listAvailablePackage.Where(p => p.Id_Province.Contains(_selectedProvince.codename)).ToList();
            // Search by Type Package 
            if (_selectedTypePackage.Id_Type_Package != -1)
                listSearchType = _listAvailablePackage.Where(p => p.Id_Type_Package == _selectedTypePackage.Id_Type_Package).ToList();

            ObservableListAvailablePackage.Clear();
            // Load UI
            foreach (PACKAGE package in listSearchType)
            {
                Province province = Get_Api_Address.getProvinces().First(p => p.codename == package.Id_Province);
                package.ProvinceName = province.name;
                ObservableListAvailablePackage.Add(package);
            }
            foreach (PACKAGE package in _observableListSelectedPackage)
            {
                Province province = Get_Api_Address.getProvinces().First(p => p.codename == package.Id_Province);
                package.ProvinceName = province.name;
                ObservableListAvailablePackage.Remove(package);
            }
        }
        #endregion

        #region Search
        private void ExecuteSearchCommand(object obj)
        {
            LoadWithSearch();
        }

        private void ExecuteSearchProvinceCommand(object obj)
        {
            LoadWithSearch();   
        }
        #endregion

        #region View package detail
        private void ExecuteViewPackageDetailCommand(object obj)
        {
            try
            {
                if (SelectedItem != null)
                {
                    DetailPackageView detailView = new DetailPackageView();
                    detailView.DataContext = new DetailPackageViewModel(SelectedItem);
                    detailView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Add packgage to Tour
        public void ExecuteAddNewPacakge(object obj)
        {
            PACKAGE package = obj as PACKAGE;
            ObservableListAvailablePackage.Remove(package);
            ObservableListSelectedPackage.Add(package);
            NoRowInSelectedDatagrid = ObservableListSelectedPackage.Count;
        }

        // If that package does not exist -> create new package
        #region Process Package event
        private void ExecuteCreatePackgeCommand(object obj)
        {
            CreatePackageView view = new CreatePackageView();
            if (_package == null)
                _package = new PACKAGE();
            view.DataContext = new CreatePackageViewModel(_package);
            view.ShowDialog();
            ReloadPackage(); 
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
                        // Remove item in Available that has been selected
                        foreach (PACKAGE item in ObservableListSelectedPackage)
                        {
                            ObservableListAvailablePackage.Remove(item);
                        }

                        if (_selectedTypePackage != null)
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
        #endregion
        #endregion

        #region Delete package from Tour
        public void ExecuteDeletePickedPackage(object obj)
        {
            PACKAGE package = obj as PACKAGE;
            ObservableListSelectedPackage.Remove(package);
            ObservableListAvailablePackage.Add(package);
            NoRowInSelectedDatagrid = ObservableListSelectedPackage.Count;
        }
        #endregion

        #region Update package in Tour using UpdateTourViewModel
        public void ExecuteUpdatePackage(object obj)
        {
            try
            {
                if (_listTourDetail.Count <= ObservableListSelectedPackage.Count)
                {
                    int i = 0;
                    foreach (PACKAGE package in ObservableListSelectedPackage)
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
                        if (ObservableListSelectedPackage.Count <= i)
                            break;
                        gridActivity.Tour_detail.Id_Package = ObservableListSelectedPackage[i].Id_Package;
                        gridActivity.PackageName = ObservableListSelectedPackage[i].Name_Package;
                        i++;
                    }
                    for (; i < _listTourDetail.Count; i++)
                    {
                        _listTourDetail.RemoveAt(i);
                        i--;
                    }
                }

                // Process UI event
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
            }
        }
        #endregion

        #region Save package in Tour using CreateTourViewModel
        public void ExecuteSavePackage(object obj)
        {
            try
            {
                _listTourDetail.Clear();
                foreach (PACKAGE package in ObservableListSelectedPackage)
                {
                    TOUR_DETAILS tour_detail = new TOUR_DETAILS();
                    tour_detail.Id_Package = package.Id_Package;
                    string namePacakge = db.PACKAGEs.Find(tour_detail.Id_Package).Name_Package;
                    _listTourDetail.Add(new GridActivity() { Tour_detail = tour_detail, PackageName = namePacakge });
                }
                // Process UI event
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
            }
        }
        #endregion
    }
}
