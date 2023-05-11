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
        #region Declare variable 
        private string _searchPackageName="";
        private FirebaseStorage firebaseStorage;
        private ObservableCollection<PACKAGE> _listPackages;
        private DispatcherTimer timer = new DispatcherTimer();
        private List<PACKAGE> listOriginalPackage;
        private List<PACKAGE> listPackagesSearching;
        private SUPER_TOUR db = new SUPER_TOUR();
        private int _currentPage = 1;
        private int _totalPage;
        private string _pageNumberText;
        private bool _enableButtonNext;
        private bool _enableButtonPrevious;
        private bool _onSearching = false;
        private string _resultNumberText;
        private int _startIndex;
        private int _endIndex;
        private int _totalResult;
        #endregion

        #region Declare binding
        public string ResultNumberText
        {
            get { return _resultNumberText; }
            set
            {
                _resultNumberText = value;
                OnPropertyChanged(nameof(ResultNumberText));
            }
        }

        public string PageNumberText
        {
            get
            {
                return _pageNumberText;
            }
            set
            {
                _pageNumberText = value;
                OnPropertyChanged(nameof(PageNumberText));
            }
        }

        public bool EnableButtonNext
        {
            get
            {
                return _enableButtonNext;
            }
            set
            {
                _enableButtonNext = value;
                OnPropertyChanged(nameof(EnableButtonNext));
            }
        }

        public bool EnableButtonPrevious
        {
            get
            {
                return _enableButtonPrevious;
            }
            set
            {
                _enableButtonPrevious = value;
                OnPropertyChanged(nameof(EnableButtonPrevious));
            }
        }

        public string SearchPackageName
        {
            get { return _searchPackageName; }
            set
            {
                _searchPackageName = value;
                OnPropertyChanged(nameof(SearchPackageName));
            }
        }

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
        #endregion

        // Command 
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand OpenCreatePackageViewCommand { get; }
        public ICommand DeletePackageCommand { get; }
        public ICommand UpdatePackageViewCommand { get; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }

        public MainPackageViewModel()
        {
            OpenCreatePackageViewCommand = new RelayCommand(ExecuteOpenCreatePackageViewCommand);
            DeletePackageCommand = new RelayCommand(ExecuteDeletePackage);
            UpdatePackageViewCommand = new RelayCommand(ExecuteUpdatePackage);
            OnSearchTextChangedCommand = new RelayCommand(SearchPackage);
            GoToPreviousPageCommand = new RelayCommand(ExcecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExcecuteGoToNextPageCommand);
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");
            _listPackages = new ObservableCollection<PACKAGE>();
            LoadPackageDataAsync();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
        }

        /* void LoadGrid(List<PACKAGE> listPackage)
        {
            _listPackages.Clear();
            foreach(PACKAGE packgage in listPackage)
            {
                _listPackages.Add(packgage);
            }    
        }*/

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
                        await firebaseStorage.Child("Images").Child("Package" + packageFind.Id_Package.ToString()).DeleteAsync();
                        db.PACKAGEs.Remove(packageFind);
                        await db.SaveChangesAsync();
                        MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                        /*listOriginalPackage = db.PACKAGEs.ToList();
                        ReloadData(listOriginalPackage);*/
                        LoadPackageDataAsync();
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

        private void ExecuteUpdatePackage(object obj)
        {
            timer.Stop();
            PACKAGE package = obj as PACKAGE;
            UpdatePackageView view = new UpdatePackageView();
            view.DataContext = new UpdatePackageViewModel(package);
            view.ShowDialog();
            LoadPackageDataAsync();
        }

        private void SearchPackage(object obj)
        {
            if (string.IsNullOrEmpty(_searchPackageName))
            {
                _onSearching = false;
                ReloadData(listOriginalPackage);
            }
            else
            {
                _onSearching = true;
                this._currentPage = 1;
                listPackagesSearching = listOriginalPackage.Where(p => p.Name_Package.StartsWith(_searchPackageName)).ToList();
                ReloadData(listPackagesSearching);
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (MainViewModel.CurrentChild is MainPackageViewModel)
                        {
                            if (db != null)
                            {
                                db.Dispose();
                            }
                            db = new SUPER_TOUR();
                            var updatePackage = await db.PACKAGEs.ToListAsync();
                            if (!updatePackage.SequenceEqual(listOriginalPackage))
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    listOriginalPackage = updatePackage;
                                    if (_onSearching)
                                    {
                                        listPackagesSearching = listOriginalPackage.Where(p => p.Name_Package.StartsWith(_searchPackageName)).ToList();
                                        ReloadData(listPackagesSearching);
                                    }
                                    else
                                        ReloadData(listOriginalPackage);
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private async Task LoadPackageDataAsync()
        {
            try
            {
                int flag = 0;
                await Task.Run(async () =>
                {
                    try
                    {
                        if (db != null)
                        {
                            db.Dispose();
                        }
                        db = new SUPER_TOUR();
                        listOriginalPackage = db.PACKAGEs.ToList();
                        listPackagesSearching = listOriginalPackage.Where(p => p.Name_Package.StartsWith(_searchPackageName)).ToList();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_onSearching)
                                ReloadData(listPackagesSearching);
                            else
                                ReloadData(listOriginalPackage);
                        });
                    }
                    catch (Exception ex)
                    {
                        flag = 1;
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                    }
                });
                if (flag == 0)
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
                LoadPackageDataAsync();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        #region Custom datagrid by page
        private List<PACKAGE> GetData(List<PACKAGE> ListPackage, int startIndex, int endIndex)
        {
            try
            {
                return ListPackage.OrderBy(m => m.Id_Type_Package).Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<PACKAGE> packages)
        {
            try
            {
                this._totalResult = packages.Count();
                if (_totalResult == 0)
                {
                    _startIndex = -1;
                    _endIndex = 0;
                    _totalPage = 1;
                    _currentPage = 1;
                }
                else
                {
                    this._totalPage = (int)Math.Ceiling((double)_totalResult / 13);
                    if (this._totalPage < _currentPage)
                        _currentPage = this._totalPage;
                    this._startIndex = (this._currentPage - 1) * 13;
                    this._endIndex = Math.Min(this._startIndex + 13, _totalResult);
                }

                List<PACKAGE> ListPackage = GetData(packages, this._startIndex, this._endIndex);
                _listPackages.Clear();
                foreach (PACKAGE package in ListPackage)
                {
                    _listPackages.Add(package);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private void setResultNumber()
        {
            ResultNumberText = $"Showing {this._startIndex + 1} - {this._endIndex} of {this._totalResult} results";
        }

        private void setButtonAndPage()
        {
            if (this._currentPage < this._totalPage)
            {
                EnableButtonNext = true;
                if (this._currentPage == 1)
                    EnableButtonPrevious = false;
                else
                    EnableButtonPrevious = true;
            }
            if (this._currentPage == this._totalPage)
            {
                if (this._currentPage == 1)
                {
                    EnableButtonPrevious = false;
                    EnableButtonNext = false;
                }
                else
                {
                    EnableButtonPrevious = true;
                    EnableButtonNext = false;
                }
            }
            PageNumberText = $"Page {this._currentPage} of {this._totalPage}";
        }

        private void ExcecuteGoToPreviousPageCommand(object obj)
        {
            if (this._currentPage > 1)
                --this._currentPage;
            if (_onSearching)
                LoadDataByPage(listPackagesSearching);
            else
                LoadDataByPage(listOriginalPackage);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExcecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(listPackagesSearching);
            else
                LoadDataByPage(listOriginalPackage);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData(List<PACKAGE> packages)
        {
            LoadDataByPage(packages);
            setButtonAndPage();
            setResultNumber();
        }
        #endregion
    }
}