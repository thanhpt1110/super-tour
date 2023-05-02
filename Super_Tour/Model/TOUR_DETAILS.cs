using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TOUR_DETAILS
    {
        [Key]
        public int Id_TourDetails { get; set; }
        public int Id_Tour { get; set; }
        public int Id_Package { get; set; }
        public string Session { get; set; }
        public int Date_Order_Package { get; set; }
        public TimeSpan Start_Time_Package { get; set; }
/*        [ForeignKey("tour_details_ibfk_1")]
        public virtual TOUR TOUR { get; set; }
        [ForeignKey("tour_details_ibfk_2")]
        public virtual PACKAGE PACKAGE { get; set; }*/
    }
}
