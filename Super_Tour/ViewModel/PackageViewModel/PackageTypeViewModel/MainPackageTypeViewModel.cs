using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;
using Super_Tour.Model;
using System.Windows;
using System.Threading;
using System.Data.Entity;
using System.Windows.Threading;
using Super_Tour.CustomControls;
using System.Windows.Markup;
using Org.BouncyCastle.Crypto.Tls;
using System.IO.Packaging;
using Org.BouncyCastle.Asn1.Cms;
using System.Diagnostics;

namespace Super_Tour.ViewModel
{
    internal class MainPackageTypeViewModel : ObservableObject
    {
        private SUPER_TOUR db = new SUPER_TOUR();
        private List<TYPE_PACKAGE> _listTypePackageOriginal; // Data gốc
        private List<TYPE_PACKAGE> _listTypePackageSearching; // Data lúc mà có người search
        private ObservableCollection<TYPE_PACKAGE> _listTypePackages = new ObservableCollection<TYPE_PACKAGE>();
        private DispatcherTimer timer = new DispatcherTimer();
        private string _searchType="";
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

        public string SearchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                _searchType = value;
                OnPropertyChanged(nameof(SearchType));
            }
        }
        public ObservableCollection<TYPE_PACKAGE> ListTypePackages
        {
            get { return _listTypePackages; }
            set
            {
                _listTypePackages = value;
                OnPropertyChanged(nameof(ListTypePackages));
            } 
        }
        #endregion

        // End Test
        public ICommand OpenCreatePackageTypeViewCommand { get;private set; }
        public ICommand DeletePackageInDataGridView { get;private set; }
        public ICommand OnSearchTextChangedCommand { get; private set; }
        public ICommand UpdatePackageCommand { get;private set; }
        public ICommand GoToPreviousPageCommand { get; private set; }
        public ICommand GoToNextPageCommand { get; private set; }

        public  MainPackageTypeViewModel() 
        {
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
            DeletePackageInDataGridView = new RelayCommand(ExecuteDeletePackageCommand);
            UpdatePackageCommand = new RelayCommand(UpdatePackage);
            GoToPreviousPageCommand = new RelayCommand(ExcecuteGoToPreviousPageCommand);
            GoToNextPageCommand = new RelayCommand(ExcecuteGoToNextPageCommand);
            OnSearchTextChangedCommand = new RelayCommand(SearchCommand);
            LoadDataAsync();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
        }

        private void UpdatePackage(object obj)
        {
            timer.Stop();
            TYPE_PACKAGE type_package = obj as TYPE_PACKAGE;
            UpdatePackageTypeView view = new UpdatePackageTypeView();
            view.DataContext = new UpdatePackageTypeViewModel(type_package);
            view.ShowDialog();
            LoadDataAsync();
        }

        private void SearchCommand(object obj)
        {
            if (string.IsNullOrEmpty(_searchType))
            {
                _onSearching = false;
                ReloadData(_listTypePackageOriginal);
            }
            else
            {
                _onSearching = true;
                this._currentPage = 1;
                _listTypePackageSearching = _listTypePackageOriginal.Where(p => p.Name_Type.StartsWith(_searchType)).ToList();
                ReloadData(_listTypePackageSearching);
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await CheckDataPerSecondAsync();
        }

        private async Task CheckDataPerSecondAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                    db = new SUPER_TOUR();
                    var myEntities = await db.TYPE_PACKAGEs.ToListAsync();
                    // Kiểm tra dữ liệu có được cập nhật chưa
                    if (!myEntities.SequenceEqual(_listTypePackageOriginal))
                    { 
                        // Dữ liệu đã được cập nhật
                        // Thực hiện các xử lý cập nhật dữ liệu trong ứng dụng của bạn
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTypePackageOriginal = myEntities;
                            if (_onSearching)
                            {
                                _listTypePackageSearching = _listTypePackageOriginal.Where(p => p.Name_Type.StartsWith(_searchType)).ToList();
                                ReloadData(_listTypePackageSearching);
                            }
                            else
                                ReloadData(_listTypePackageOriginal);
                        });
                    }

                });
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private async Task LoadDataAsync()
        {
            try
            {
                int flag = 0;
                await Task.Run(() =>
                {
                    try
                    {
                        if (db != null)
                        {
                            db.Dispose();
                        }
                        db = new SUPER_TOUR();
                        _listTypePackageOriginal = db.TYPE_PACKAGEs.ToList();
                        _listTypePackageSearching = _listTypePackageOriginal.Where(p => p.Name_Type.StartsWith(_searchType)).ToList();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_onSearching)
                                ReloadData(_listTypePackageSearching);
                            else
                                ReloadData(_listTypePackageOriginal);
                        });
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                        flag = 1;
                    }
                });
                if(flag==0)
                    timer.Start();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }  
        private async void ExecuteDeletePackageCommand(object obj)
        {
            try
            {
                TYPE_PACKAGE type_package = obj as TYPE_PACKAGE;
                timer.Stop();
                TYPE_PACKAGE type_packageFind = await db.TYPE_PACKAGEs.FindAsync(type_package.Id_Type_Package);
                if(db.PACKAGEs.Where(p=>p.Id_Type_Package==type_packageFind.Id_Type_Package).ToList().Count>0)
                {
                    MyMessageBox.ShowDialog("The package type could not be deleted.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                if (type_packageFind != null)
                {
                    MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                    if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                    {
                        db.TYPE_PACKAGEs.Remove(type_packageFind);
                        await db.SaveChangesAsync();
                        MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                        LoadDataAsync();
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
        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            try
            {
                CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
                timer.Stop();
                createPackageTypeView.ShowDialog();
                LoadDataAsync();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private List<TYPE_PACKAGE> GetData(List<TYPE_PACKAGE> ListTypePackage,int startIndex, int endIndex)
        {
            try
            {
                return ListTypePackage.OrderBy(m => m.Id_Type_Package).Skip(startIndex).Take(endIndex-startIndex).ToList();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void LoadDataByPage(List<TYPE_PACKAGE> typePackages)
        {
            try
            {
                this._totalResult = typePackages.Count();
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
                    if (this._totalPage < _currentPage )
                        _currentPage = this._totalPage;
                    this._startIndex = (this._currentPage - 1) * 13;
                    this._endIndex = Math.Min(this._startIndex + 13, _totalResult);
                }

                List<TYPE_PACKAGE> ListTypePackage = GetData(typePackages, this._startIndex, this._endIndex);
                _listTypePackages.Clear();
                foreach (TYPE_PACKAGE typePackage in ListTypePackage)
                {
                    _listTypePackages.Add(typePackage);
                }
            }
            catch(Exception ex)
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
                LoadDataByPage(_listTypePackageSearching);
            else
                LoadDataByPage(_listTypePackageOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ExcecuteGoToNextPageCommand(object obj)
        {
            if (this._currentPage < this._totalPage)
                ++this._currentPage;
            if (_onSearching)
                LoadDataByPage(_listTypePackageSearching);
            else
                LoadDataByPage(_listTypePackageOriginal);

            setButtonAndPage();
            setResultNumber();
        }

        private void ReloadData(List<TYPE_PACKAGE> typePackages)
        {
            LoadDataByPage(typePackages);
            setButtonAndPage();
            setResultNumber();
        }
    }
}
