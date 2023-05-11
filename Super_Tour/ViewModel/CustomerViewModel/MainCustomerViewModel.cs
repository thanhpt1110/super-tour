using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainCustomerViewModel: ObservableObject
    {
        #region Declare variable 
        private DispatcherTimer _timer;
        #endregion

        #region Declare binding

        #endregion

        #region Command
        public ICommand OpenCreateCustomerViewCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainCustomerViewModel() 
        {
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
