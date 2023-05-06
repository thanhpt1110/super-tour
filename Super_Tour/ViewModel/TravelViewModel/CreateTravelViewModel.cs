using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
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
        private TOUR _tour;
        public TOUR Tour
        {
            get { return _tour; }
            set
            {
                _tour=value;
                OnPropertyChanged(nameof(Tour));
            }
        }
        public ICommand OpenSelectTourForTravelViewCommand { get; }
        public CreateTravelViewModel()
        {
            _tour = new TOUR();
            OpenSelectTourForTravelViewCommand = new RelayCommand(ExecuteOpenSelectTourForTravelViewCommand);
        }
        private void ExecuteOpenSelectTourForTravelViewCommand(object obj)
        {
            SelectTourForTravelView selectTourForTravelView = new SelectTourForTravelView();
            selectTourForTravelView.DataContext = new SelectTourForTravelViewModel(_tour);
            selectTourForTravelView.ShowDialog();
            Console.WriteLine("Hello");
        }
    }
}
