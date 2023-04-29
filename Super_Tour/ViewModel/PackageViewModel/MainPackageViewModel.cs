using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Firebase.Storage;
using MaterialDesignThemes.Wpf;
using Org.BouncyCastle.Asn1.Mozilla;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using Super_Tour.View.PackageView;

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
        private string _searchPackageName;
        private FirebaseStorage firebaseStorage;
        public string SearchPackageName
        {
            get { return _searchPackageName; }
            set
            {
                _searchPackageName = value;
                OnPropertyChanged(nameof(SearchPackageName));
            }
        }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand OpenCreatePackageViewCommand { get; }
        public ICommand DeletePackageCommand { get; }
        public ICommand UpdatePackageViewCommand { get; }
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
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");
            OpenCreatePackageViewCommand = new RelayCommand(ExecuteOpenCreatePackageViewCommand);
            DeletePackageCommand = new RelayCommand(ExecuteDeletePackage);
            UpdatePackageViewCommand = new RelayCommand(ExecuteUpdatePackage);
            _listPackagesDataGrid = new ObservableCollection<PackageDataGrid>();
            timer.Interval = TimeSpan.FromSeconds(3);
            OnSearchTextChangedCommand = new RelayCommand(SearchPackage);
            timer.Tick += Timer_Tick;
            LoadPackageDataAsync();
        }

        private async void ExecuteDeletePackage(object obj)
        {
            try
            {
                PackageDataGrid packageDataGrid = obj as PackageDataGrid;
                PACKAGE package = packageDataGrid.Package;
                timer.Stop();
                PACKAGE packageFind = await db.PACKAGEs.FindAsync(package.Id_Package);
                if (db.TOUR_DETAILs.Where(p => p.Id_Package == packageFind.Id_Package).ToList().Count > 0)
                {
                    MyMessageBox.ShowDialog("The package could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                if (packageFind != null)
                {
                    MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                    if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                    {
                        await firebaseStorage.Child("Images").Child("Package"+packageFind.Id_Package.ToString()).DeleteAsync();
                        db.PACKAGEs.Remove(packageFind);
                        await db.SaveChangesAsync();
                        MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                        listOriginalPackage = db.PACKAGEs.ToList();
                        _listPackagesDataGrid.Clear();
                        foreach (PACKAGE package1 in listOriginalPackage)
                        {
                            TYPE_PACKAGE type = db.TYPE_PACKAGEs.Find(package1.Id_Type_Package);
                            _listPackagesDataGrid.Add(new PackageDataGrid() { Package = package1, NamePackageType = type.Name_Type });
                        }
                    }

                }
                else
                {
                    MyMessageBox.ShowDialog("The package type could not be found.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                timer.Start();
            }
        }
        private async void ExecuteUpdatePackage(object obj)
        {
            PackageDataGrid packageDataGrid = obj as PackageDataGrid;
            PACKAGE package = packageDataGrid.Package;
            UpdatePackageView view = new UpdatePackageView();
            view.DataContext = new UpdatePackageViewModel(package);
            view.ShowDialog();
        }
        private void SearchPackage(object obj)
        {
            _listPackagesDataGrid.Clear();
            List<PACKAGE> list;
            if(string.IsNullOrEmpty(_searchPackageName))
            {
                list = listOriginalPackage;

            }    
            else
            {
                list = listOriginalPackage.Where(p => p.Name_Package.StartsWith(_searchPackageName)).ToList();
            }
            foreach (PACKAGE package in list)
            {
                TYPE_PACKAGE type = db.TYPE_PACKAGEs.Find(package.Id_Type_Package);
                _listPackagesDataGrid.Add(new PackageDataGrid() { Package = package, NamePackageType = type.Name_Type });
            }
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
                        listOriginalPackage = updatePackage;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_listPackagesDataGrid.Count != 0)
                                _listPackagesDataGrid.Clear();
                            foreach (PACKAGE package in listOriginalPackage)
                            {
                                TYPE_PACKAGE type = db.TYPE_PACKAGEs.Find(package.Id_Type_Package);
                                _listPackagesDataGrid.Add(new PackageDataGrid() { Package = package, NamePackageType = type.Name_Type });
                            }
                        });                    
                    }

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
