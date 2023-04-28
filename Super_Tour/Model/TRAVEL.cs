using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TRAVEL
    {
        [Key]
        public int Id_Travel { get; set; }
        public int Id_Tour { get; set; }
        public string StartLocation { get; set; }
        public DateTime StartDateTimeTravel { get; set; }
        public int RemainingTicket { get; set; }
        public int Discount { get; set; }
/*        [ForeignKey("travel_ibfk_1  ")]
        public virtual TOUR TOUR { get; set; }*/
    }
}
