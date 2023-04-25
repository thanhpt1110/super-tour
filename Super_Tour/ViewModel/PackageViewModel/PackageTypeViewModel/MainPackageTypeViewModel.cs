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

namespace Super_Tour.ViewModel
{
    internal class MainPackageTypeViewModel : ObservableObject
    {
        private SUPER_TOUR db = new SUPER_TOUR();
        public class PackageTypeTest{
            private string _packageTypeID;
            private string _packageTypeName;
            private string _description;

            public string PackageTypeID { get => _packageTypeID; set => _packageTypeID = value; }
            public string PackageTypeName { get => _packageTypeName; set => _packageTypeName = value; }
            public string Description { get => _description; set => _description = value; }
            public PackageTypeTest() { 
            }
            public PackageTypeTest(string packageTypeID, string packageTypeName, string description)
            {
                this.PackageTypeID = packageTypeID;
                this.PackageTypeName = packageTypeName;
                this.Description = description;
            }
        }
        private ObservableCollection<TYPE_PACKAGE> _listTypePackages;

        public ObservableCollection<TYPE_PACKAGE> ListTypePackages
        {
            get { return _listTypePackages; }
            set
            {
                _listTypePackages = value;
                OnPropertyChanged(nameof(ListTypePackages));
            } 
        }
        // End Test
        public ICommand OpenCreatePackageTypeViewCommand { get;private set; }
        public MainPackageTypeViewModel() {
            _listTypePackages = new ObservableCollection<TYPE_PACKAGE>();
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
            GetAllPackage();
        }
        private void GetAllPackage()
        {
            _listTypePackages.Clear();
            List<TYPE_PACKAGE> ListTypePackage = db.TYPE_PACKAGEs.ToList();
            foreach(TYPE_PACKAGE typePackage in ListTypePackage)
            {
                _listTypePackages.Add(typePackage);
            }    
        }

        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
            createPackageTypeView.ShowDialog();
            GetAllPackage();
        }
    }
}
