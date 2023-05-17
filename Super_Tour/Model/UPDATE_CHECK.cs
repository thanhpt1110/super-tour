using FireSharp.Config;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    internal class UPDATE_CHECK
    {
        public string Type_Update { get; set; }
        public string DateTimeUpdate { get; set; }
        public static FirebaseConfig Firebaseconfig = new FirebaseConfig()
        {
            AuthSecret = "zV6tTqw9fzIYfyeqMpOGdBkpY1Cf9OMaoeRCP5nv",
                BasePath = "https://supertour-30e53-default-rtdb.firebaseio.com/"
            };
        public static FireSharp.FirebaseClient Client = new FireSharp.FirebaseClient(Firebaseconfig);
        public static void NotifyChange(string table)
        {
            var res = Client.Get(@"Update/"+table);
            UPDATE_CHECK check = res.ResultAs<UPDATE_CHECK>();
            check.DateTimeUpdate = DateTime.Now.ToString();
            var set = Client.Update(@"Update/"+table, check);
        }
    }
}
