using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Super_Tour.ViewModel
{
    internal class UpdatePackageTypeViewModel: ObservableObject
    {
        private SUPER_TOUR db = new SUPER_TOUR();
        private string _description;
        private string _typePackageName;
       
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string TypePackageName
        {
            get => _typePackageName;
            set
            {
                _typePackageName = value;
                OnPropertyChanged(nameof(TypePackageName));
            }
        }
        public RelayCommand UpdatePackageCommand;
        public UpdatePackageTypeViewModel()
        {
           // UpdatePackageCommand = new RelayCommand();
        }
        private void ExecuteUpdateNewCommand(object obj)
        {

        }
    }
}
