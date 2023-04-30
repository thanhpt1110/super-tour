using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View.TravelView;

namespace Super_Tour.ViewModel
{
    internal class MainTravelViewModel: ObservableObject
    {
        public ICommand OpenCreateTravelViewCommand { get; }
        public MainTravelViewModel() 
        {
            OpenCreateTravelViewCommand = new RelayCommand(ExecuteOpenCreateTravelViewCommand);
        }
        private void ExecuteOpenCreateTravelViewCommand(object obj)
        {
            CreateTravelView createTravelView = new CreateTravelView();
            createTravelView.ShowDialog();
        }
    }
}
