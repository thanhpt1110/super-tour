using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

namespace Super_Tour.ViewModel
{
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
        private ObservableCollection<PACKAGE> _listPackages;
        public ObservableCollection<PACKAGE> ListPackages
        {
            get
            {
                return _listPackages;
            }
            set
            {
                _listPackages = value;
                OnPropertyChanged(nameof(ListPackages));
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
            _listPackages = new ObservableCollection<PACKAGE>();
            timer.Interval = TimeSpan.FromSeconds(3);
            OnSearchTextChangedCommand = new RelayCommand(SearchPackage);
            timer.Tick += Timer_Tick;
            LoadPackageDataAsync();
        }
        void LoadGrid(List<PACKAGE> listPackage)
        {
            _listPackages.Clear();
            foreach(PACKAGE packgage in listPackage)
            {
                _listPackages.Add(packgage);
            }    
        }
        private async void ExecuteDeletePackage(object obj)
        {
            try
            {
                PACKAGE package = obj as PACKAGE;
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
                        LoadGrid(listOriginalPackage);
                    }

                }
                else
                {
                    MyMessageBox.ShowDialog("The package could not be found.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
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
            PACKAGE package = obj as PACKAGE;
            UpdatePackageView view = new UpdatePackageView();
            view.DataContext = new UpdatePackageViewModel(package);
            view.ShowDialog();
        }
        private void SearchPackage(object obj)
        {
            List<PACKAGE> list;
            if(string.IsNullOrEmpty(_searchPackageName))
            {
                list = listOriginalPackage;

            }    
            else
            {
                list = listOriginalPackage.Where(p => p.Name_Package.StartsWith(_searchPackageName)).ToList();
            }
            LoadGrid(list);
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
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
                        List<PACKAGE> updatePackage = db.PACKAGEs.ToList();
                        if (!listOriginalPackage.SequenceEqual(updatePackage))
                        {
                            listOriginalPackage = updatePackage;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                SearchPackage(null);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private async Task LoadPackageDataAsync()
        {
            try
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
                    listOriginalPackage = await db.PACKAGEs.ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadGrid(listOriginalPackage);
                    });
                }
                catch(Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
                timer.Start();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
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
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }


        }

        
    } 
}
