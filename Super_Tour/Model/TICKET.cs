﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TICKET
    {
        [Key]
        public int Id_Ticket { get; set; }
        public int Id_Booking_Details { get; set; }
        public string Status { get; set; }
 /*       [ForeignKey("ticket_ibfk_1")]
        public virtual BOOKING_DETAILS BOOKING_DETAILS { get; set; }*/
    }
}
