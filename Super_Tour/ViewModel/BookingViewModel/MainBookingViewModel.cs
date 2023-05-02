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
    internal class MainBookingViewModel: ObservableObject
    {
        public ICommand OpenCreateBookingViewCommand { get; }
        public MainBookingViewModel() 
        {
            OpenCreateBookingViewCommand = new RelayCommand(ExecuteOpenCreateBookingViewCommand);
        }

        private void ExecuteOpenCreateBookingViewCommand(object obj)
        {
            CreateBookingView createBookingView = new CreateBookingView();
            createBookingView.ShowDialog();
        }
    }
}
