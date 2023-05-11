using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Data.Entity.Migrations;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class UpdatePackageTypeViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private bool _execute = true;
        private string _description;
        private string _typePackageName;
        private TYPE_PACKAGE _typePackage;
        #endregion

        #region Declare binding
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string TypePackageName
        {
            get => _typePackageName;
            set
            {
                _typePackageName = value;
                OnPropertyChanged(nameof(TypePackageName));
            }
        }
        #endregion

        #region Command
        public RelayCommand UpdatePackageCommand { get; }
        #endregion

        public UpdatePackageTypeViewModel(TYPE_PACKAGE typePackage)
        {
            db = MainViewModel.db;
            UpdatePackageCommand = new RelayCommand(ExecuteUpdateNewCommand, canExecuteUpdateNew);
            _typePackage = typePackage;
            _description = typePackage.Description;
            _typePackageName = typePackage.Name_Type;
        }

        private bool canExecuteUpdateNew(object obj)
        {
            return _execute;
        }

        private async void ExecuteUpdateNewCommand(object obj)
        {
            if(string.IsNullOrEmpty(_description) || string.IsNullOrEmpty(_typePackageName))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            if (_description != _typePackage.Description || _typePackageName != _typePackage.Name_Type)
            {
                try
                {
                    _execute = false;
                    MyMessageBox.ShowDialog("Are you sure about the information this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                    if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                    {

                        _typePackage.Description = _description;
                        _typePackage.Name_Type = _typePackageName;
                        db.TYPE_PACKAGEs.AddOrUpdate(_typePackage);
                        await db.SaveChangesAsync();
                        MyMessageBox.ShowDialog("Update type package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                        
                        // Find view to close
                        UpdatePackageTypeView updatePackageTypeView = null;
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is UpdatePackageTypeView)
                            {
                                updatePackageTypeView = window as UpdatePackageTypeView;
                                break;
                            }
                        }
                        updatePackageTypeView.Close();
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
                finally
                {
                    _execute = true;
                }
            }
        }
    }
}
