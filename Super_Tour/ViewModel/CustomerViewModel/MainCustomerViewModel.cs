using System;
using System.Windows.Input;
using System.Windows.Threading;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainCustomerViewModel: ObservableObject
    {
        #region Declare variable 
        private DispatcherTimer _timer = null;
        private SUPER_TOUR db = null;
        #endregion

        #region Declare binding

        #endregion

        #region Command
        public ICommand OpenCreateCustomerViewCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainCustomerViewModel() 
        {
            db = SUPER_TOUR.db; 
            OpenCreateCustomerViewCommand = new RelayCommand(ExecuteOpenCreateCustomerViewCommand);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            // Write function
        }

        private void ExecuteOpenCreateCustomerViewCommand(object obj)
        {
            CreateCustomerView createCustomerView = new CreateCustomerView();
            createCustomerView.ShowDialog();    
        }
    }
}
