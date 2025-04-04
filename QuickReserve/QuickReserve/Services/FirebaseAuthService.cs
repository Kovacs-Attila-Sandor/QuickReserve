using Firebase.Auth;
using Firebase.Auth.Providers;

namespace QuickReserve.Services
{
    public class FirebaseAuthService
    {
        // Az egyetlen példány
        private static FirebaseAuthService _instance;

        // Publikus property az egyetlen példány eléréséhez
        public static FirebaseAuthService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FirebaseAuthService();
                }
                return _instance;
            }
        }

        // Firebase Auth kliens
        public FirebaseAuthClient AuthClient { get; private set; }

        // Privát konstruktor, hogy ne lehessen kívülről példányosítani
        private FirebaseAuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = QuickReserve.Config.Config.FIREBASE_AUTH_API_KEY,
                AuthDomain = QuickReserve.Config.Config.FIREBASE_AUTH_DOMAIN_API_KEY,
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            };

            AuthClient = new FirebaseAuthClient(config);
        }
    }
}