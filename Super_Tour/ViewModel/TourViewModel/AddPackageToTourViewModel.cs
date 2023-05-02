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

namespace Super_Tour.ViewModel
{
    internal class AddPackageToTourViewModel: ObservableObject
    {
        private TOUR_DETAILS _tour_detail;
        private List<PACKAGE> _listAvailablePackage;
        private List<PACKAGE> _listAlreadyPickedPackage;
        private bool _executeSave=true;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<PACKAGE> _observableListAvailablePackage;
        private ObservableCollection<PACKAGE> _observableListPickedPackage;
        private List<TOUR_DETAILS> _listTourDetail;
        
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
        public AddPackageToTourViewModel() 
        {
            AddAvailablePackageCommand = new RelayCommand(ExecuteAddNewPacakge);
            DeletePickedPackageCommand = new RelayCommand(ExecuteDeletePickedPackage);
            SavePackageCommand = new RelayCommand(ExecuteSavePackage, CanExecuteSavePackage);
        }
        public void ExecuteSavePackage(object obj)
        {
            try
            {
                _executeSave = false;
                _listTourDetail.Clear();
                foreach(PACKAGE package in ObservableListPickedPackage)
                {
                    TOUR_DETAILS tour_detail = new TOUR_DETAILS();
                    tour_detail.Id_Package = package.Id_Package;
                    tour_detail.Id_TourDetails = 1;
                    _listTourDetail.Add(tour_detail);
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
        public AddPackageToTourViewModel(List<TOUR_DETAILS> listTourDetail)
        {
            _listTourDetail = listTourDetail;
            _observableListPickedPackage = new ObservableCollection<PACKAGE>();
            AddAvailablePackageCommand = new RelayCommand(ExecuteAddNewPacakge);
            SavePackageCommand = new RelayCommand(ExecuteSavePackage, CanExecuteSavePackage);
            DeletePickedPackageCommand = new RelayCommand(ExecuteDeletePickedPackage);
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
                            foreach(TOUR_DETAILS item in _listTourDetail)
                            {
                                PACKAGE package = db.PACKAGEs.Find(item.Id_Package);
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
    }
}
