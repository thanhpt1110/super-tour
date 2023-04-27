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

namespace Super_Tour.ViewModel
{
    internal class CreatePackageViewModel : ObservableObject
    {
        private PACKAGE _package;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<City> _listCity;
        private ObservableCollection<District> _listDistrict;
        private City _selectedCity;
        private District _selectedDistrict;
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

        public PACKAGE Package
        {
            get
            {
                return _package;
            }
            set
            {
                _package = value;
                OnPropertyChanged(nameof(Package));
            }
        }
        public ICommand SelectedCityCommand { get; }
        public ICommand CreateNewPackageCommand { get; }
        public CreatePackageViewModel()
        {
            _package = new PACKAGE();
            _listCity = new ObservableCollection<City>();
            CreateNewPackageCommand = new RelayCommand(ExecuteCreatePackageCommand);
            SelectedCityCommand = new RelayCommand(ExecuteLoadDistrict);
            _listDistrict = new ObservableCollection<District>();

            LoadProvinces();

        }
        private void ExecuteLoadDistrict(object obj)
        {
            try
            {
                _listDistrict.Clear();
                foreach (District district in Get_Api_Address.getDistrict(_selectedCity))
                {
                    _listDistrict.Add(district);
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
                foreach (City city in Get_Api_Address.getCities())
                {
                    _listCity.Add(city);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        }
        private void ExecuteCreatePackageCommand(object obj)
        {
            try
            {
                //Console.WriteLine("hj");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        } 
    }
}
