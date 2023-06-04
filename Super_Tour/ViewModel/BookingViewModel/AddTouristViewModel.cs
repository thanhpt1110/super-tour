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
        public class DatagridTourist
        {
            #region Declare variable
            private int _order;
            private TOURIST _tourist;
            #endregion

            public int Order
            {
                get { return _order; }
                set
                {
                    _order = value;
                }
            }

            public TOURIST Tourist
            {
                get { return _tourist; }
                set
                {
                    _tourist = value;
                }
            }
        }

        #region Declare variable 
        private string _touristName = null;
        private bool _enterName = false;
        private bool _isDataModified = false;
        private int _order = 0;
        private TOURIST _newTourist = null;
        private List<TOURIST> _listCurrentTourist = null;
        private List<TOURIST> _listUITourist = null;
        private ObservableCollection<DatagridTourist> _listTourists;
        private DatagridTourist _selectedTourist = null;
        #endregion

        #region Declare binding
        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));  
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
                    CheckEnterName();
                }
            }
        }

        public ObservableCollection<DatagridTourist> ListTourists
        {
            get => _listTourists;
            set
            {
                _listTourists = value;
                OnPropertyChanged(nameof(ListTourists));
            }
        }

        public DatagridTourist SelectedTourist
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
        private void CheckEnterName()
        {
            if (string.IsNullOrEmpty(_touristName))
                EnterName = false;
            else
                EnterName = true;
        }
        
        private void CheckDataModified()
        {
            if (_listCurrentTourist.SequenceEqual(_listUITourist.ToList()))
                IsDataModified = false;
            else 
                IsDataModified = true;
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
            _listUITourist = new List<TOURIST>(listTourist);
            ListTourists = new ObservableCollection<DatagridTourist>();

            // Create command
            SaveTouristCommand = new RelayCommand(ExecuteSaveTouristCommand);
            DeleteTouristCommnand = new RelayCommand(ExecuteDeleteTouristCommand);
            AddTouristCommand = new RelayCommand(ExecuteAddTouristCommand);

            // Load UI
            GenerateOrder();
        }
        #endregion

        private void GenerateOrder()
        {
            ListTourists.Clear();
            for (int i = 0; i < _listUITourist.Count; i++)
            {
                ListTourists.Add(new DatagridTourist { Order = i + 1, Tourist = _listUITourist[i] });
            }
        }

        #region Execute add tourist 
        private void ExecuteAddTouristCommand(object obj)
        {
            // Add new tourist to UI
            _newTourist = new TOURIST();
            _newTourist.Name_Tourist = _touristName;
            _listUITourist.Add(_newTourist);

            // Reset textbox TouristName
            TouristName = null;

            // Process UI event
            GenerateOrder();
            CheckDataModified();
        }
        #endregion

        #region Execute delete tourist 
        private void ExecuteDeleteTouristCommand(object obj)
        {
            // Remove tourist from UI
            _listUITourist.Remove(_selectedTourist.Tourist);

            // Process UI event
            GenerateOrder();
            CheckDataModified();
        }
        #endregion

        #region Perform save tourists
        public void ExecuteSaveTouristCommand(object obj)
        {
            _listCurrentTourist.Clear();
            foreach(TOURIST tourist in _listUITourist)
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
