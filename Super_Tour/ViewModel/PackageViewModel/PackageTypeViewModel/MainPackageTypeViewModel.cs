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
        public ICommand DeletePackageInDataGridView { get;private set; }
        public MainPackageTypeViewModel() {
            _listTypePackages = new ObservableCollection<TYPE_PACKAGE>();
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
            DeletePackageInDataGridView = new RelayCommand(ExecuteDeletePackageCommand);
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
        private void ExecuteDeletePackageCommand(object obj)
        {
            try
            {
                TYPE_PACKAGE type_package = obj as TYPE_PACKAGE;
                db.TYPE_PACKAGEs.Remove(type_package);
                db.SaveChangesAsync();
                _listTypePackages.Remove(type_package);
                MessageBox.Show("Delete Package Type successful", "Success", MessageBoxButton.OK);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Exclamation);
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
