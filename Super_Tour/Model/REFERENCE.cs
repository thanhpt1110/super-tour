using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class REFERENCE
    {
        [Key]
        public int Id_Reference { get; set; }
        public bool Update_ACCOUNT { get; set; }
        public bool Update_TYPEPACKAGE { get; set; }
        public bool Update_PACKAGE { get; set; }
        public bool Update_TOUR { get; set; }
        public bool Update_TOUR_DETAIL { get; set; }
        public bool Update_TRAVEL { get; set; }
        public bool Update_CUSTOMER { get; set; }
        public bool Update_BOOKING { get; set; }
        public bool Update_TOURIST { get; set; }
        public bool Update_TICKET { get; set; }
    }
}
