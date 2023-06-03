using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Student_wpf_application.ViewModels.Command;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Super_Tour.ViewModel
{
    internal class AddTouristViewModel : ObservableObject
    {
        #region Declare variable 
        private string _touristName = null;
        private bool _enterName = false;
        private int _order = 0;
        private TOURIST _newTourist = null;
        private List<TOURIST> _listCurrentTourist = null;
        private ObservableCollection<TOURIST> _listTourists;
        private TOURIST _selectedTourist = null;
        #endregion

        #region Declare binding
        public int Order
        {
            get { return _order; }
            set
            {
                _order = value;
                OnPropertyChanged(nameof(Order));   
            }
        }

        public bool EnterName
        {
            get { return _enterName; }
            set
            {
                _enterName = value;
                OnPropertyChanged(nameof(EnterName));
            }
        }

        public string TouristName
        {
            get { return _touristName; }
            set 
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _touristName = value;
                    OnPropertyChanged(nameof(TouristName));
                    CheckDataModified();
                }
            }
        }

        public ObservableCollection<TOURIST> ListTourists
        {
            get => _listTourists;
            set
            {
                _listTourists = value;
                OnPropertyChanged(nameof(ListTourists));
            }
        }

        public TOURIST SelectedTourist
        {
            get => _selectedTourist;
            set
            {
                _selectedTourist = value;
                OnPropertyChanged(nameof(SelectedTourist));
            }
        }
        #endregion

        #region Check data modified
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(_touristName))
                EnterName = false;
            else
                EnterName = true;
        }
        #endregion

        #region Command
        public ICommand SaveTouristCommand { get; }
        public ICommand DeleteTouristCommnand { get; }
        public ICommand AddTouristCommand { get;}
        #endregion

        #region Constructor
        public AddTouristViewModel(List<TOURIST> listTourist)
        {
            // Create objects
            this._listCurrentTourist = listTourist;
            ListTourists = new ObservableCollection<TOURIST>(_listCurrentTourist);

            // Create command
            SaveTouristCommand = new RelayCommand(ExecuteSaveTouristCommand);
            DeleteTouristCommnand = new RelayCommand(ExecuteDeleteTouristCommand);
            AddTouristCommand = new RelayCommand(ExecuteAddTouristCommand);
        }
        #endregion

        #region Execute add tourist 
        private void ExecuteAddTouristCommand(object obj)
        {
            Order = ListTourists.Count;
            _newTourist = new TOURIST();
            _newTourist.Name_Tourist = _touristName;
            _newTourist.Id_Tourist = Order;
            ListTourists.Add(_newTourist);
            _touristName = null;
        }
        #endregion

        #region Execute delete tourist 
        private void ExecuteDeleteTouristCommand(object obj)
        {
            ListTourists.Remove(_selectedTourist);
            Order = ListTourists.Count;
        }
        #endregion

        #region Perform save tourists
        public void ExecuteSaveTouristCommand(object obj)
        {
            _listCurrentTourist.Clear();
            foreach(TOURIST tourist in ListTourists)
            {
                _listCurrentTourist.Add(tourist);
            }

            // Process Ui event
            AddTouristView addTouristView = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window is AddTouristView)
                {
                    addTouristView = window as AddTouristView;
                    break;
                }
            }
            addTouristView.Close();
        }
        #endregion
    }
}
