using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Super_Tour.Ultis.Api_Address;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace Super_Tour.ViewModel
{
    internal class CreatePackageViewModel : ObservableObject
    {
        #region Declare private variable
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<City> _listCity;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage;
        private ObservableCollection<District> _listDistrict;
        private City _selectedCity;
        private District _selectedDistrict;
        private TYPE_PACKAGE _selectedTypePackage; // Danh
        private List<TYPE_PACKAGE> listOriginalTYpePackage; // Danh sách listPackage
        private BitmapImage _selectedImage = null; // Ảnh mặc định
        private PACKAGE package;
        private string _namePackage = "Phuc Binh";
        private string _description;
        private string _price;

        #endregion
        #region Declare public variable
        public string Price
        {
            get { return _price; }
            set
            {
                if (Regex.IsMatch(value, "^[0-9]*$"))
                {
                    _price = value;
                }
                else
                {
                    // Hiển thị thông báo lỗi tại đây
                }
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
        private byte[] convertImageToByteArray(BitmapImage image)
        {
            byte[] imageBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                imageBytes = stream.ToArray();
            }
            return imageBytes;
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
        #endregion
        public CreatePackageViewModel()
        {
            //package = new PACKAGE();
            _listCity = new ObservableCollection<City>();
            CreateNewPackageCommand = new RelayCommand(ExecuteCreatePackageCommand);
            SelectedCityCommand = new RelayCommand(ExecuteSelectedCityComboBox);
             OpenPictureCommand = new RelayCommand(ExecuteOpenImage);
            _listDistrict = new ObservableCollection<District>();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>();
            LoadPackageType();
            LoadProvinces();
        }
        private void ExecuteOpenImage(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";
            dialog.Title = "Chọn ảnh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
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
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p=>p.name).ToList();
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
                cities = cities.OrderBy(p=>p.name).ToList();
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
                if(_selectedCity== null || _selectedDistrict==null || _selectedImage==null || _selectedTypePackage == null || string.IsNullOrEmpty(_namePackage ))
                {
                    MessageBox.Show("Please enter all information", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                SUPER_TOUR db = new SUPER_TOUR();
                package = new PACKAGE();
                package.Id_Package = 1;
                package.Id_Type_Package = _selectedTypePackage.Id_Type_Package;
                package.Name_Package = _namePackage;
                package.Id_Province = _selectedCity.codename;
                package.Id_District = _selectedDistrict.codename;
                package.Description_Package = _description;
                package.Price=decimal.Parse(_price);
                _selectedImage.DecodePixelHeight = (int)(_selectedImage.PixelHeight*0.1);
                _selectedImage.DecodePixelWidth = (int)(_selectedImage.PixelWidth*0.1);
                package.Image_Package = convertImageToByteArray(_selectedImage);

                db.PACKAGEs.Add(package);
                await  db.SaveChangesAsync();
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                //db.PACKAGEs.Remove(_package);
                Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        } 
    }
}
