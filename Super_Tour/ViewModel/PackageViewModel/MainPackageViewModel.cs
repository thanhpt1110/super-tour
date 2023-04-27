using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Org.BouncyCastle.Asn1.Mozilla;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    class PackageDataGrid
    {
        private PACKAGE _package;
        public PACKAGE Package
        {
            get { return _package; }
            set
            {
                _package = value;
            }
        }
        private string _namePackageType;
        public string NamePackageType {
            get { return _namePackageType; }
            set
            {
                _namePackageType = value;
            }
        }
    }
    internal class MainPackageViewModel : ObservableObject
    {
        public ICommand OpenCreatePackageViewCommand { get; }
        private ObservableCollection<PackageDataGrid> _listPackagesDataGrid;
        public ObservableCollection<PackageDataGrid> ListPackagesDataGrid
        {
            get
            {
                return _listPackagesDataGrid;
            }
            set
            {
                _listPackagesDataGrid = value;
                OnPropertyChanged(nameof(ListPackagesDataGrid));
            }
        }
        private DispatcherTimer timer = new DispatcherTimer();
        private List<PACKAGE> listOriginalPackage;
        private SUPER_TOUR db = new SUPER_TOUR();
        public MainPackageViewModel()
        {
            OpenCreatePackageViewCommand = new RelayCommand(ExecuteOpenCreatePackageViewCommand);
            _listPackagesDataGrid = new ObservableCollection<PackageDataGrid>();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            LoadPackageDataAsync();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    List<PACKAGE> updatePackage = db.PACKAGEs.ToList();
                    if(!listOriginalPackage.SequenceEqual(updatePackage))
                    {
                        _listPackagesDataGrid.Clear();
                        listOriginalPackage = updatePackage;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (PACKAGE package in listOriginalPackage)
                            {
                                TYPE_PACKAGE type = db.TYPE_PACKAGEs.Find(package.Id_Type_Package);
                                _listPackagesDataGrid.Add(new PackageDataGrid() { Package = package, NamePackageType = type.Name_Type });
                            }
                        });                    }

                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private async Task LoadPackageDataAsync()
        {
            try
            {
                await Task.Run(() =>
            {

                listOriginalPackage = db.PACKAGEs.ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (PACKAGE package in listOriginalPackage)
                    {
                        TYPE_PACKAGE type = db.TYPE_PACKAGEs.Find(package.Id_Type_Package);
                        _listPackagesDataGrid.Add(new PackageDataGrid() { Package = package, NamePackageType = type.Name_Type });
                    }
                });

            });
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void ExecuteOpenCreatePackageViewCommand(object obj)
        {
            try
            {
                CreatePackageView createPackageView = new CreatePackageView();
                createPackageView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


        }
    } 
}
