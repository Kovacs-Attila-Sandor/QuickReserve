using Firebase.Database;
using QuickReserve.Config;
namespace QuickReserve.Services
{
    public class FirebaseService
    {
        private static FirebaseClient firebaseClient;

        static FirebaseService()
        {
            var apiKey = QuickReserve.Config.Config.FIREBASE_DATABASE_API_KEY;
            firebaseClient = new FirebaseClient(apiKey);
        }

        // Nyilvános elérési pont a FirebaseClient-hez
        public static FirebaseClient Client => firebaseClient;
    }
}
