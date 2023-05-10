using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class BOOKING
    {
        [Key]
        public int Id_Booking { get; set; }
        public int Id_Customer_Booking { get; set; }
        public int Id_Travel { get; set; }
        public DateTime Booking_Date { get; set; }
        public string Status { get; set; }
         [ForeignKey("Id_Customer_Booking")]
        public virtual CUSTOMER CUSTOMER { get; set; }
         [ForeignKey("Id_Travel")]
        public virtual TRAVEL TRAVEL { get; set; }
        public virtual ICollection<TOURIST> TOURISTs
        {
            get; set;
        }

    }
}
