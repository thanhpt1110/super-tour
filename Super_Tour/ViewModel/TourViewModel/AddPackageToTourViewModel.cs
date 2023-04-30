using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class AddPackageToTourViewModel: ObservableObject
    {
        private TOUR_DETAILS _tour_detail;
        private List<PACKAGE> _listAvailablePackage;
        private List<PACKAGE> _listAlreadyPickedPackage;
        private SUPER_TOUR db = new SUPER_TOUR();
        private ObservableCollection<PACKAGE> _observableListAvailablePackage;
        private ObservableCollection<PACKAGE> _observableListPickedPackage;
        public ObservableCollection<PACKAGE> ObservableListAvailablePackage
        {
            get
            {
                return _observableListAvailablePackage;
            }
            set
            {
                _observableListAvailablePackage = value;
                OnPropertyChanged(nameof(ObservableListAvailablePackage));
            }
        }
        public ObservableCollection<PACKAGE> ObservableListPickedPackage
        {
            get
            {
                return _observableListPickedPackage;
            }
            set
            {
                _observableListPickedPackage = value;
                OnPropertyChanged(nameof(ObservableListPickedPackage));
            }
        }
        public AddPackageToTourViewModel() 
        {

        }
        public AddPackageToTourViewModel(string session)
        {
            _tour_detail = new TOUR_DETAILS();
            LoadAvailablePackage();
        }
        private async Task LoadAvailablePackage()
        {
            await Task.Run(() =>
            {
                try {
                    _listAvailablePackage=db.PACKAGEs.ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _observableListAvailablePackage = new ObservableCollection<PACKAGE>(_listAvailablePackage);
                    });
                }
                catch(Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            });
        }
    }
}
