using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class TOURIST
    {
        [Key]
        public int Id_Tourist { get; set; } 
        public string Name_Tourist { get; set; }
        public int Id_Booking { get; set; }
    }
}
