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
    internal class MainTicketViewModel: ObservableObject
    {
        #region Declare variable
        private DispatcherTimer _timer = null;
        #endregion

        #region Declare binding

        #endregion

        #region Command
        public ICommand OpenTicket { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion


        public MainTicketViewModel() {
            OpenTicket = new RelayCommand(ExecuteOpenTicket);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            // Write function
        }

        private void ExecuteOpenTicket(object obj)
        {
            ExportedTicketView exportedTicketView = new ExportedTicketView();
            exportedTicketView.ShowDialog();
        }
    }
}
