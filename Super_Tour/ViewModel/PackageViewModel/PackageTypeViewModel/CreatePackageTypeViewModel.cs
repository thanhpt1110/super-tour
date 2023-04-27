using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.View;
using System;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class CreatePackageTypeViewModel
    {
        SUPER_TOUR db = new SUPER_TOUR();
        private string _packageTypeName;
        private string _description;
        public string PackageTypeName { get => _packageTypeName; set => _packageTypeName = value; }
        public string Description { get => _description; set => _description = value; }
        public RelayCommand CreateNewPackageCommand { get; }
        public CreatePackageTypeViewModel()
        {
            CreateNewPackageCommand = new RelayCommand(execute_CreateNewType_Package);
        }
        public void execute_CreateNewType_Package(object obj)
        {
            if (string.IsNullOrEmpty(_packageTypeName) || string.IsNullOrEmpty(_description))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            TYPE_PACKAGE type_package = new TYPE_PACKAGE();
            try
            {
                type_package.Id_Type_Package = 1;
                type_package.Name_Type = _packageTypeName;
                type_package.Description = _description;
                db.TYPE_PACKAGEs.Add(type_package);
                db.SaveChangesAsync();
                CreatePackageTypeView createPackageTypeView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is CreatePackageTypeView)
                    {
                        createPackageTypeView = window as CreatePackageTypeView;
                        break;
                    }
                }
                createPackageTypeView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                db.TYPE_PACKAGEs.Remove(type_package);
            }
        }
    }
}
