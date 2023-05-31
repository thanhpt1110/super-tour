using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Super_Tour.Ultis
{
    internal class ValidateDataFormat
    { 
        public static bool CheckGmailFormat(string email)
        {
            string pattern = @"@gmail\.com$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}
