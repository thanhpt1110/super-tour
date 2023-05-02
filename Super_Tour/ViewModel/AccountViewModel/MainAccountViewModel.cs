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
    internal class MainAccountViewModel: ObservableObject
    {
        public ICommand OpenCreateAccountViewCommand { get; }
        public MainAccountViewModel() 
        {
            OpenCreateAccountViewCommand = new RelayCommand(ExecuteOpenCreateAccountViewCommand);
        }

        private void ExecuteOpenCreateAccountViewCommand(object obj)
        {
            CreateAccountView createAccountView = new CreateAccountView();
            createAccountView.ShowDialog();
        }
    }
}
