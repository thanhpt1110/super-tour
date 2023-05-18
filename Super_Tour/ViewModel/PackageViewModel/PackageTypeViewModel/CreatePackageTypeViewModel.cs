using FireSharp;
using FireSharp.Interfaces;
using MySqlX.XDevAPI;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Linq;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class CreatePackageTypeViewModel : ObservableObject
    {
        #region Declare variale 
        private SUPER_TOUR db = null;
        private TYPE_PACKAGE temp = null;
        private string table = "UPDATE_TYPEPACKAGE";
        private bool _isDataModified = false;
        private string _description = null;
        private string _packageTypeName = null;
        #endregion

        #region Declare binding
        public bool IsDataModified
        {
            get => _isDataModified;
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
                checkDataModified();
            }
        }

        public string PackageTypeName
        {
            get => _packageTypeName;
            set
            {
                _packageTypeName = value;
                OnPropertyChanged(nameof(PackageTypeName));
                checkDataModified();
            }
        }
        #endregion

        #region Command
        public RelayCommand CreateNewPackageCommand { get; }
        #endregion

        private void checkDataModified()
        {
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(PackageTypeName))
                IsDataModified = false;
            else
                IsDataModified = true;
        }

        public CreatePackageTypeViewModel(TYPE_PACKAGE tmp)
        {
            CreateNewPackageCommand = new RelayCommand(execute_CreateNewType_Package);
            db = SUPER_TOUR.db;
            this.temp = tmp;
        }

        public void execute_CreateNewType_Package(object obj)
        {
            try
            {
                // Save data to DB
                temp.Name_Type = _packageTypeName;
                temp.Description = _description;
                db.TYPE_PACKAGEs.Add(temp);
                db.SaveChanges();
                temp.Id_Type_Package = db.TYPE_PACKAGEs.Max(p=>p.Id_Type_Package);

                // // Synchronize data to real time DB
                MainPackageTypeViewModel.TimePackageType = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainPackageTypeViewModel.TimePackageType);

                // Process UI event
                MyMessageBox.ShowDialog("Add type package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                // Find view to close
                CreatePackageTypeView createPackageTypeView = null;
                foreach (Window window in Application.Current.Windows)
                {
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
                db.TYPE_PACKAGEs.Remove(temp);
            }
        }
    }
}
