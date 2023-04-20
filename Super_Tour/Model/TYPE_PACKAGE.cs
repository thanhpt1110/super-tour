using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    internal class TYPE_PACKAGE
    {
        [Key]
        public int Id_Type_Package { get; set; }
        public string Name_Type { get; set; }
        public virtual ICollection<PACKAGE> PACKAGEs
        {
            get; set;
        }
    }
}
