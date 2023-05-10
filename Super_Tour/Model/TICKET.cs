using System;
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
        public int Id_Tourist { get; set; }
        public string Status { get; set; }
        [ForeignKey("Id_Tourist")]
        public virtual TOURIST TOURIST { get; set; }
    }
}
