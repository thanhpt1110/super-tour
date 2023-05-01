using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateTravelViewModel: ObservableObject
    {
        public ICommand OpenSelectTourForTravelViewCommand { get; }
        public CreateTravelViewModel()
        {
            OpenSelectTourForTravelViewCommand = new RelayCommand(ExecuteOpenSelectTourForTravelViewCommand);
        }
        private void ExecuteOpenSelectTourForTravelViewCommand(object obj)
        {
            SelectTourForTravelView selectTourForTravelView = new SelectTourForTravelView();
            selectTourForTravelView.ShowDialog();
        }
    }
}
