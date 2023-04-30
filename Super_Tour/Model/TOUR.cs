using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TOUR
    {
        [Key]
        public int Id_Tour { get; set; }
        public byte TotalDay { get; set; }
        public byte TotalNight { get; set; }
        public int MaxTicket { get; set; }
        public string PlaceOfTour { get; set; }
        public string Status_Tour { get; set; }
        public string Name_Tour { get; set; }
 /*       public virtual ICollection<TOUR_DETAILS> TOUR_DETAILs
        {
            get; set;
        }
        public virtual ICollection<TRAVEL> TRAVELs
        {
            get; set;
        }*/
    }
}
