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
    internal class MainTourViewModel: ObservableObject
    {
        public ICommand OpenCreateTourViewCommand { get; }
        public MainTourViewModel() 
        {
            OpenCreateTourViewCommand = new RelayCommand(ExecuteOpenCreateTourViewCommand);
        }

        private void ExecuteOpenCreateTourViewCommand(object obj)
        {
            CreateTourView createTourView = new CreateTourView();
            createTourView.ShowDialog();
        }
    }
}
