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
    internal class MainCustomerViewModel: ObservableObject
    {
        public ICommand OpenCreateCustomerViewCommand { get; }
        public MainCustomerViewModel() 
        {
            OpenCreateCustomerViewCommand = new RelayCommand(ExecuteOpenCreateCustomerViewCommand);
        }

        private void ExecuteOpenCreateCustomerViewCommand(object obj)
        {
            CreateCustomerView createCustomerView = new CreateCustomerView();
            createCustomerView.ShowDialog();    
        }
    }
}
