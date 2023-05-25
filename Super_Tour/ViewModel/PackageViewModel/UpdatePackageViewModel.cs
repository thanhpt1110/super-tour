using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.View;
using System.Windows;
using Firebase.Storage;
using System.Data.Entity.Migrations;

namespace Super_Tour.ViewModel
{
    internal class UpdatePackageViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private PACKAGE _selectedItem = null;
        private Province _selectedProvince = null;
        private District _selectedDistrict = null;
        private BitmapImage _selectedImage = null;
        private TYPE_PACKAGE _selectedTypePackage = null;
        private ObservableCollection<Province> _listProvince = null;
        private ObservableCollection<District> _listDistrict = null;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage = null;
        private List<Province> ListProvinces = null;
        private List<District> ListDistricts = null;
        private List<TYPE_PACKAGE> ListTypePackages = null;
        private FirebaseStorage firebaseStorage = null;
        private string _packageName = null;
        private string _description = null;
        private string _price = null;
        private bool _isDataModified = false;
        private string _imagePath = null;
        private bool _isNewImage = false;
        #endregion

        #region Declare binding
        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
                CheckDataModified();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string PackageName
        {
            get
            {
                return _packageName;
            }
            set
            {
                _packageName = value;
                OnPropertyChanged(nameof(PackageName));
                CheckDataModified();
            }
        }

        public BitmapImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
                CheckDataModified();
            }
        }

        public TYPE_PACKAGE SelectedTypePackage
        {
            get
            {
                return _selectedTypePackage;
            }
            set
            {
                _selectedTypePackage = value;
                OnPropertyChanged(nameof(SelectedTypePackage));
                CheckDataModified();
            }
        }

        public ObservableCollection<TYPE_PACKAGE> ListTypePackage
        {
            get
            {
                return _listTypePackage;
            }
            set
            {
                _listTypePackage = value;
                OnPropertyChanged(nameof(ListTypePackage));
            }
        }

        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged(nameof(SelectedDistrict));
                CheckDataModified();
            }
        }

        public Province SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
                CheckDataModified();
            }
        }

        public ObservableCollection<District> ListDistrict
        {
            get
            {
                return _listDistrict;
            }
            set
            {
                _listDistrict = value;
                OnPropertyChanged(nameof(ListDistrict));
            }
        }

        public ObservableCollection<Province> ListProvince
        {
            get
            {
                return _listProvince;
            }
            set
            {
                _listProvince = value;
                OnPropertyChanged(nameof(ListProvince));
            }
        }
        #endregion

        #region Command
        public ICommand SelectedProvinceCommand { get; }
        public ICommand OpenPictureCommand { get; }
        public ICommand UpdatePackageCommand { get; }
        #endregion

        #region Constructor
        public UpdatePackageViewModel()
        {
        }

        public UpdatePackageViewModel(PACKAGE Package)
        {
            db = SUPER_TOUR.db;
            this._selectedItem = Package;

            // Create object
            SelectedProvinceCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            OpenPictureCommand = new RelayCommand(ExecuteOpenImage);
            UpdatePackageCommand = new RelayCommand(ExecuteUpdatePackage);
            _listProvince = new ObservableCollection<Province>();
            _listDistrict = new ObservableCollection<District>();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>();
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");


            // Load data from existed item
            LoadProvinces();
            _selectedProvince = _listProvince.Where(p => p.codename == _selectedItem.Id_Province).FirstOrDefault();
            _selectedDistrict = Get_Api_Address.getDistrict(_selectedProvince).Where(p => p.codename == _selectedItem.Id_District).FirstOrDefault();
            LoadDistrict();
            LoadPackageType();
            _selectedTypePackage = _selectedItem.TYPEPACKAGE;
            _selectedImage = getInageOnline();
            _packageName = _selectedItem.Name_Package;
            _description = _selectedItem.Description_Package;
            _price = _selectedItem.Price.ToString();
        }
        #endregion

        #region Province
        private void LoadProvinces()
        {
            try
            {
                ListProvinces = Get_Api_Address.getProvinces();
                foreach (Province Province in ListProvinces)
                {
                    _listProvince.Add(Province);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region District
        private void ExecuteSelectedProvinceComboBox(object obj)
        {
            _selectedDistrict = null;
            LoadDistrict();
        }

        private void LoadDistrict()
        {
            try
            {
                _listDistrict.Clear();
                ListDistricts = Get_Api_Address.getDistrict(_selectedProvince);
                foreach (District District in ListDistricts)
                {
                    _listDistrict.Add(District);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Package type
        private void LoadPackageType()
        {
            try
            {
                ListTypePackages = db.TYPE_PACKAGEs.ToList();
                foreach (TYPE_PACKAGE TypePackage in ListTypePackages)
                {
                    _listTypePackage.Add(TypePackage);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Image
        private BitmapImage getInageOnline()
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(_selectedItem.Image_Package);
                    using (MemoryStream memoryStream = new MemoryStream(data))
                    {
                        // Đọc ảnh từ MemoryStream và gán vào đối tượng BitmapImage
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return null;
            }
        }

        private void ExecuteOpenImage(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";
            dialog.Title = "Select Image";
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _imagePath = dialog.FileName;
                // Đọc hình ảnh từ tệp được chọn và lưu vào SelectedImage
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                SelectedImage = image;
                _isNewImage = true;
            }
        }

        private async Task<string> UploadImg()
        {
            if (_isNewImage)
            {
                await firebaseStorage.Child("Images").Child("Package" + _selectedItem.Id_Package.ToString()).DeleteAsync();
                Stream img = new FileStream(_imagePath, FileMode.Open, FileAccess.Read);
                var image = await firebaseStorage.Child("Images").Child("Package" + _selectedItem.Id_Package.ToString()).PutAsync(img);
                return image;
            }
            return _selectedItem.Image_Package;
        }
        #endregion

        #region Check data modified
        private void CheckDataModified()
        {
            if (_selectedProvince == null || _selectedDistrict == null || _selectedImage == null || _selectedTypePackage == null ||
                string.IsNullOrEmpty(_packageName) || string.IsNullOrEmpty(_price)
                || (_selectedProvince.codename == _selectedItem.Id_Province && _selectedDistrict.codename == _selectedItem.Id_District &&
                    _selectedTypePackage.Id_Type_Package == _selectedItem.Id_Type_Package && _packageName == _selectedItem.Name_Package &&
                    _price == _selectedItem.Price.ToString() && _isNewImage == false && _description == _selectedItem.Description_Package.ToString()))
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        #region Perform update action
        private async void ExecuteUpdatePackage(object obj)
        {
            try
            {
                _selectedItem.Id_Type_Package = _selectedTypePackage.Id_Type_Package;
                _selectedItem.Name_Package = _packageName;
                _selectedItem.Id_Province = _selectedProvince.codename;
                _selectedItem.Id_District = _selectedDistrict.codename;
                _selectedItem.Description_Package = _description;
                _selectedItem.Price = decimal.Parse(_price);
                _selectedItem.Image_Package = await UploadImg();
                db.PACKAGEs.AddOrUpdate(_selectedItem);
                await db.SaveChangesAsync();

                MyMessageBox.ShowDialog("Update package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);

                // Find view to close 
                UpdatePackageView createPackageView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is UpdatePackageView)
                    {
                        createPackageView = window as UpdatePackageView;
                        break;
                    }
                }
                createPackageView.Close();
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
    }
}
