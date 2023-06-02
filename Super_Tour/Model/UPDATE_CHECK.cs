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
            AuthSecret = "35egc5rk4B2jnq73OVN9mDxt9v9tl51N6uTMKkJ0",
            BasePath = "https://supertour-75495-default-rtdb.firebaseio.com/"
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
            if(_tracker == null)
            {
                _tracker = new UPDATE_CHECK();
                _tracker.DateTimeUpdate = timeUpdate.ToString();
                _tracker.Type_Update = table;
                Client.Set(@"Update/" + table, _tracker);
            }
            else
            {
                _tracker.DateTimeUpdate = timeUpdate.ToString();
                Client.Update(@"Update/" + table, _tracker);
            }
        }
    }
}
