using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainTicketViewModel: ObservableObject
    {
        public ICommand OpenTicket { get; }

        public MainTicketViewModel() {
            OpenTicket = new RelayCommand(ExecuteOpenTicket);
        }

        private void ExecuteOpenTicket(object obj)
        {
            ExportedTicketView exportedTicketView = new ExportedTicketView();
            exportedTicketView.ShowDialog();
        }
    }
}
