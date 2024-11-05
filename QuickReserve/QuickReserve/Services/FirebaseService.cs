using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;

namespace QuickReserve.Services
{
    public class FirebaseService
    {
        private static FirebaseClient firebaseClient;

        static FirebaseService()
        {
            firebaseClient = new FirebaseClient("https://quickreserve-9b03a-default-rtdb.firebaseio.com/");
        }

        // Nyilvános elérési pont a FirebaseClient-hez
        public static FirebaseClient Client => firebaseClient;
    }
}
