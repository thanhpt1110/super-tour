using FireSharp.Config;
using FireSharp.Response;
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
        private static FirebaseResponse _res = null;
        private static UPDATE_CHECK _tracker = null;

        public static FirebaseConfig Firebaseconfig = new FirebaseConfig()
        {
            AuthSecret = "zV6tTqw9fzIYfyeqMpOGdBkpY1Cf9OMaoeRCP5nv",
                BasePath = "https://supertour-30e53-default-rtdb.firebaseio.com/"
        };

        public static FireSharp.FirebaseClient Client = new FireSharp.FirebaseClient(Firebaseconfig);

        public static UPDATE_CHECK getTracker(string table)
        {
            _res = Client.Get(@"Update/" + table);
            return _res.ResultAs<UPDATE_CHECK>();
        }

        public static void NotifyChange(string table, DateTime timeUpdate)
        {
            _tracker = getTracker(table);
            _tracker.DateTimeUpdate = timeUpdate.ToString();
        }
    }
}
