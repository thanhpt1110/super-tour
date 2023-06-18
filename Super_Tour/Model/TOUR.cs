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
        public decimal PriceTour { get; set; }
        public string Name_Tour { get; set; }
        public int TotalDay { get; set; }
        public int TotalNight { get; set; }
        public string PlaceOfTour { get; set; }
        public string Status_Tour { get; set; }
        public virtual ICollection<TOUR_DETAILS> TOUR_DETAILs
        {
            get; set;
        }
        public virtual ICollection<TRAVEL> TRAVELs
        {
            get; set;
        }
    }
}
