using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class ACCOUNT
    {
        [Key]
        public int Id_Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Account_Name { get; set; }
        public string Service { get; set; }
        public int Priority { get; set; }
        public string Email { get; set; }

    }
}
