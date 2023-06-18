using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class CUSTOMER
    {
        [Key]
        public int Id_Customer { get; set; }
        public string Name_Customer { get; set; }
        public string IdNumber { get; set; }
        public string Gender { get; set; }
        [Column(name: "Province")]
        public string Id_Province { get; set; }
        [Column(name: "District")]
        public string Id_District { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public virtual ICollection<BOOKING> BOOKINGs
        {
            get; set;
        }
    }
}
