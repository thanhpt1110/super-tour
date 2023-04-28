using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TYPE_PACKAGE
    {
        [Key]
        public int Id_Type_Package { get; set; }
        public string Name_Type { get; set; }
        public string Description { get; set; }
        public virtual ICollection<PACKAGE> PACKAGEs
        {
            get; set;
        }
    }
}
