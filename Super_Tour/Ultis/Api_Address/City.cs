using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Ultis.Api_Address
{
    internal class City
    {
        public string name { get; set; }
        public int code { get; set; }
        public string codename { get; set; }
        public string division_type { get; set; }
        public int phone_code { get; set; }
        public List<District> districts { get; set; }
    }
}
