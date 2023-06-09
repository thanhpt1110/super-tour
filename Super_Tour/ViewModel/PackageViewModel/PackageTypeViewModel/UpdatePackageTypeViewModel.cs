﻿using MySqlX.XDevAPI;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Super_Tour.ViewModel
{
    internal class UpdatePackageTypeViewModel : ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private TYPE_PACKAGE temp = null;
        private bool _isDataModified = false;
        private string _description = null;
        private string _packageTypeName = null;
        private string table = "UPDATE_TYPEPACKAGE";
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
        public RelayCommand UpdatePackageCommand { get; }
        #endregion

        #region Constructor
        public UpdatePackageTypeViewModel(TYPE_PACKAGE tmp)
        {
            UpdatePackageCommand = new RelayCommand(ExecuteUpdateNewCommand);
            db = SUPER_TOUR.db;
            this.temp = tmp;
            _description = tmp.Description;
            _packageTypeName = tmp.Name_Type;
        }
        #endregion

        #region Check data modified
        private void checkDataModified()
        {
            if (string.IsNullOrEmpty(PackageTypeName) || (Description == temp.Description && PackageTypeName == temp.Name_Type))
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion

        private void ExecuteUpdateNewCommand(object obj)
        {
            try
            {
                // Save to DB
                temp.Description = _description;
                temp.Name_Type = _packageTypeName;
                db.TYPE_PACKAGEs.AddOrUpdate(temp);
                db.SaveChanges();

                // Synchronize data to real time DB
                MainPackageTypeViewModel.TimePackageType = DateTime.Now;
                UPDATE_CHECK.NotifyChange(table, MainPackageTypeViewModel.TimePackageType);

                // Process UI event
                MyMessageBox.ShowDialog("Update type package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                // Find view to close
                UpdatePackageTypeView updatePackageTypeView = null;
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is UpdatePackageTypeView)
                    {
                        updatePackageTypeView = window as UpdatePackageTypeView;
                        break;
                    }
                }
                updatePackageTypeView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
    }
}
