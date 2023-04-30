using Firebase.Storage;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Super_Tour.ViewModel
{
    internal class CreatePackageViewModel : ObservableObject
    {
        #region Declare private variable
        private FirebaseStorage firebaseStorage;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<City> _listCity;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage;
        private ObservableCollection<District> _listDistrict;
        private City _selectedCity;
        private string _imagePath;
        private District _selectedDistrict;
        private TYPE_PACKAGE _selectedTypePackage; // Danh
        private List<TYPE_PACKAGE> listOriginalTYpePackage; // Danh sách listPackage
        private BitmapImage _selectedImage = null; // Ảnh mặc định
        private PACKAGE package;
        private string _namePackage;
        private string _description;
        private decimal _price;
        private bool _execute = true;
        #endregion
        #region Declare public variable
        public decimal Price
        {
            get { return _price; }
            set
            {
                    _price = value;
                OnPropertyChanged(nameof(Price));
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
        public string NamePackage
        {
            get
            {
                return _namePackage;
            }
            set
            {
                _namePackage = value;
                OnPropertyChanged(nameof(NamePackage));
            }
        }
        public BitmapImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
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
            }
        }
        public City SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
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

        public ObservableCollection<City> ListCity
        {
            get
            {
                return _listCity;
            }
            set
            {
                _listCity = value;
                OnPropertyChanged(nameof(ListCity));
            }
        }
        public ICommand SelectedCityCommand { get; }
        public ICommand CreateNewPackageCommand { get; }
        public ICommand OpenPictureCommand { get; }
        public ICommand TextChangeOnlyNum { get; }
        #endregion
        public CreatePackageViewModel()
        {
            //package = new PACKAGE();
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");
            _listCity = new ObservableCollection<City>();
            CreateNewPackageCommand = new RelayCommand(ExecuteCreatePackageCommand, CanExecuteCreateNewPackage);
            SelectedCityCommand = new RelayCommand(ExecuteSelectedCityComboBox);
            OpenPictureCommand = new RelayCommand(ExecuteOpenImage);
            //TextChangeOnlyNum = new RelayCommand(ExecuteMyCommand, CanExecuteTextChangeOnlyNum);
            _listDistrict = new ObservableCollection<District>();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>();
            LoadPackageType();
            LoadProvinces();
        }
/*        private void ExecuteMyCommand(object parameter)
        {
            string input = parameter as string;
            if (ValidateNumericInput(input))
            {
                // Nếu giá trị nhập vào là số, set giá trị của MyProperty bằng giá trị đó
                Price = input;
            }
            else
            {
                // Nếu giá trị nhập vào không phải là số, không làm gì cả
            }
        }*/
        private bool CanExecuteTextChangeOnlyNum(object parameter)
        {
            // Kiểm tra xem người dùng đã nhập dữ liệu hợp lệ vào TextBox chưa
            return ValidateNumericInput(parameter as string);
        }

        private bool ValidateNumericInput(string input)
        {
            // Kiểm tra xem ký tự được nhập vào có phải là số không
            double result;
            return Double.TryParse(input, out result);
        }
        private bool CanExecuteCreateNewPackage(object obj)
        {
            return _execute;
        }
        private void ExecuteOpenImage(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";
            dialog.Title = "Chọn ảnh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _imagePath = dialog.FileName;
                // Đọc hình ảnh từ tệp được chọn và lưu vào SelectedImage
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                SelectedImage = image;
            }
        }
        private void ExecuteSelectedCityComboBox(object obj)
        {
            try
            {
                _listDistrict.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistrict.Add(district);
                }
                _selectedDistrict = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void LoadPackageType()
        {
            try
            {
                listOriginalTYpePackage = db.TYPE_PACKAGEs.ToList();
                // _listTypePackage.Clear();
                foreach (TYPE_PACKAGE type in listOriginalTYpePackage)
                {
                    _listTypePackage.Add(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        private void LoadProvinces()
        {
            try
            {
                List<City> cities = Get_Api_Address.getCities();
                cities = cities.OrderBy(p => p.name).ToList();
                foreach (City city in cities)
                {
                    _listCity.Add(city);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        }
        private async void ExecuteCreatePackageCommand(object obj)
        {
            try
            {
                if (_selectedCity == null || _selectedDistrict == null || _selectedImage == null || _selectedTypePackage == null || string.IsNullOrEmpty(_namePackage))
                {
                    MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                _execute = false;
                SUPER_TOUR db = new SUPER_TOUR();
                package = new PACKAGE();
                package.Id_Package = db.PACKAGEs.ToList().Count==0 ? 100000: db.PACKAGEs.Max(p=>p.Id_Package)+1;
                package.Id_Type_Package = _selectedTypePackage.Id_Type_Package;
                package.Name_Package = _namePackage;
                package.Id_Province = _selectedCity.codename;
                package.Id_District = _selectedDistrict.codename;
                package.Description_Package = _description;
                package.Price = _price;
                package.Image_Package = await UploadImg();
                db.PACKAGEs.Add(package);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Add new package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                CreatePackageView createPackageView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is CreatePackageView)
                    {
                        createPackageView = window as CreatePackageView;
                        break;
                    }
                }
                createPackageView.Close();
            }
            catch (Exception ex)
            {
                //db.PACKAGEs.Remove(_package);
                Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
            finally
            {
                _execute = true;
            }
        }
        private async Task<string> UploadImg()
        {
            Stream img = new FileStream(_imagePath, FileMode.Open, FileAccess.Read);
            var image = await firebaseStorage.Child("Images").Child("Package" + package.Id_Package.ToString()).PutAsync(img);
            return image;

        }
    }

}
