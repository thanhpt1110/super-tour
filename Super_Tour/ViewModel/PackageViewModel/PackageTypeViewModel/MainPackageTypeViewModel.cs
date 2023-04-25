using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainPackageTypeViewModel : ObservableObject
    {
        //Test DataGrid
        public class PackageTypeTest{
            private string _packageTypeID;
            private string _packageTypeName;
            private string _description;

            public string PackageTypeID { get => _packageTypeID; set => _packageTypeID = value; }
            public string PackageTypeName { get => _packageTypeName; set => _packageTypeName = value; }
            public string Description { get => _description; set => _description = value; }
            public PackageTypeTest() { 
            }
            public PackageTypeTest(string packageTypeID, string packageTypeName, string description)
            {
                this.PackageTypeID = packageTypeID;
                this.PackageTypeName = packageTypeName;
                this.Description = description;
            }
        }
        private ObservableCollection<PackageTypeTest> _packageTypeTests;

        public ObservableCollection<PackageTypeTest> PackageTypeTests 
        { 
            get => _packageTypeTests;
            set
            {
                _packageTypeTests = value;
                OnPropertyChanged(nameof(PackageTypeTests));
            } 
        }
        // End Test
        public ICommand OpenCreatePackageTypeViewCommand { get; }
        public MainPackageTypeViewModel() {
            PackageTypeTests = new ObservableCollection<PackageTypeTest>();
            PackageTypeTests.Add(new PackageTypeTest("1", "Ăn uống", "Nội dung của gồm tên quán ăn, địa điểm"));
            PackageTypeTests.Add(new PackageTypeTest("2", "Tắm biển", "Nội dung của gồm tên bãi, thời gian"));
            PackageTypeTests.Add(new PackageTypeTest("3", "Tham quan", "Nội dung gồm địa điểm tham quan"));
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
        }

        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
            createPackageTypeView.ShowDialog();
        }
    }
}
