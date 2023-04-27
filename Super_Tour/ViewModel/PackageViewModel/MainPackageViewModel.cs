using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainPackageViewModel: ObservableObject
    {
        public ICommand OpenCreatePackageViewCommand { get; }
        public MainPackageViewModel() 
        {
            OpenCreatePackageViewCommand = new RelayCommand(ExecuteOpenCreatePackageViewCommand);
        }

        private void ExecuteOpenCreatePackageViewCommand(object obj)
        {
            CreatePackageView createPackageView = new CreatePackageView();
            createPackageView.ShowDialog();
        }
            
    }
}
