using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Super_Tour.Model;
namespace Super_Tour.ViewModel
{
    internal class CreatePackageTypeViewModel
    {
        SUPER_TOUR db = new SUPER_TOUR();
        private string _packageTypeName;
        private string _description;
        public string PackageTypeName { get => _packageTypeName; set => _packageTypeName = value; }
        public string Description { get => _description; set => _description = value; }
        
        public CreatePackageTypeViewModel()
        {
            _packageTypeName = "Hello";
            _description = "tôi là Phúc Bình";
            execute_CreateNewType_Package(null);
        }
        public void execute_CreateNewType_Package(object obj)
        {
            TYPE_PACKAGE type_package = new TYPE_PACKAGE();
            type_package.Id_Type_Package = 0;
            type_package.Name_Type= _packageTypeName;
            type_package.Description = _description;
            db.TYPE_PACKAGEs.Add(type_package);
            db.SaveChangesAsync();
        }
    }
}
