using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class BOOKING_DETAILS
    {
        [Key]
        public int Id_Booking_Details { get; set; }
        public int Id_Booking { get; set; }
        public int Id_Customer { get; set; }
        
       // public virtual BOOKING BOOKING { get; set; }
        
       // public virtual CUSTOMER CUSTOMER { get; set; }

       // public virtual ICollection<TICKET> TICKETs { get; set; }

    }
}
