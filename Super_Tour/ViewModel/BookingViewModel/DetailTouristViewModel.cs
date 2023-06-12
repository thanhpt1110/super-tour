using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Super_Tour.ViewModel.AddTouristViewModel;
using System.Windows.Input;

namespace Super_Tour.ViewModel.BookingViewModel
{
    internal class DetailTouristViewModel : ObservableObject
    {
        #region Declare variable 
        private string _touristName = null;
        private int _order = 0;
        private List<TOURIST> _listUITourist = null;
        private ObservableCollection<DatagridTourist> _listTourists;
        private DatagridTourist _selectedTourist = null;
        #endregion

        #region Declare binding
        public string TouristName
        {
            get { return _touristName; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _touristName = value;
                    OnPropertyChanged(nameof(TouristName));
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

        #region Constructor
        public DetailTouristViewModel(List<TOURIST> listTourist)
        {
            // Create objects
            _listUITourist = new List<TOURIST>(listTourist);
            ListTourists = new ObservableCollection<DatagridTourist>();

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
    }
}
