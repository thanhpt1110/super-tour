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
        public TRAVEL()
        { }
        public TRAVEL(TRAVEL travel)
        {
            Id_Travel = travel.Id_Travel;
            Id_Tour = travel.Id_Tour;
            StartLocation = travel.StartLocation;
            MaxTicket = travel.MaxTicket;
            StartDateTimeTravel = travel.StartDateTimeTravel;
            RemainingTicket = travel.RemainingTicket;
            Discount = travel.Discount;
        }

        [Key]
        public int Id_Travel { get; set; }
        public int Id_Tour { get; set; }
        public string StartLocation { get; set; }
        public int MaxTicket { get; set; }
        public DateTime StartDateTimeTravel { get; set; }
        public int RemainingTicket { get; set; }
        public int Discount { get; set; }
        [ForeignKey("Id_Tour")]
        public virtual TOUR TOUR { get; set; }
    }
}
