using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
namespace Super_Tour.ViewModel
{
    internal class UpdateTravelViewModel:ObservableObject
    {
        private TRAVEL _travel;
        private bool _executeSave=true;
        private string _maxTicket;
        private string _discount;
        private SUPER_TOUR db = new SUPER_TOUR();
        public string MaxTicket
        {
            get { return _maxTicket; }
            set
            {
                _maxTicket = value;
                OnPropertyChanged(nameof(MaxTicket));
            }
        }
        public string Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                OnPropertyChanged(nameof(Discount));
            }
        }
        public TRAVEL Travel
        {
            get { return _travel; }
            set { _travel = value;
                OnPropertyChanged(nameof(Travel));
            }
        }
        public ICommand SaveUpdateCommand { get; }
        public ICommand OpenSelectTourForTravelViewCommand { get; }
        public UpdateTravelViewModel(TRAVEL travel)
        {
            Travel = new TRAVEL(travel);
            MaxTicket = travel.MaxTicket.ToString();
            Discount = travel.Discount.ToString();
            SaveUpdateCommand = new RelayCommand(ExecuteSave, canExecuteSave);
        }
        private async void ExecuteSave(object obj)
        {
            if ( string.IsNullOrEmpty(_maxTicket) || string.IsNullOrEmpty(_discount) || string.IsNullOrEmpty(_travel.StartLocation))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            int maxTicket;
            int discount;
            if (!int.TryParse(_maxTicket, out maxTicket))
            {
                MyMessageBox.ShowDialog("Max ticket must be in number.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(_discount, out discount))
            {
                MyMessageBox.ShowDialog("Discount must be in number.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            else if (discount > 100)
            {
                MyMessageBox.ShowDialog("Discount must be under 80 percents.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                _executeSave = false;
                _travel.Discount = discount;
                _travel.MaxTicket = maxTicket;
                _travel.RemainingTicket = maxTicket;
                db.TRAVELs.AddOrUpdate(_travel);
                await db.SaveChangesAsync();
            
                MyMessageBox.ShowDialog("Update travel successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                /*UpdateTravelView updateTravelView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is UpdateTravelView)
                    {
                        updateTravelView = window as UpdateTravelView;
                        break;
                    }
                }
                updateTravelView.Close();*/
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeSave = true;
            }
        }
        private bool canExecuteSave(object obj)
        {
            return _executeSave;
        }
        /*private void ExecuteOpenSelectTourForTravelViewCommand(object obj)
        {
            SelectTourForTravelView selectTourForTravelView = new SelectTourForTravelView();
            selectTourForTravelView.DataContext = new SelectTourForTravelViewModel(Travel.TOUR);
            selectTourForTravelView.ShowDialog();
        }*/
    }
}
