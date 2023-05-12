using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Province { get; set; }
        public string District { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
