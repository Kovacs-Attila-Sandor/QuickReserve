using Firebase.Auth;
using Firebase.Auth.Providers;

namespace QuickReserve.Services
{
    public class FirebaseAuthService
    {
        public FirebaseAuthClient AuthClient { get; private set; }
        private readonly string _webApiKey = QuickReserve.Config.Config.FIREBASE_AUTH_API_KEY;

        public FirebaseAuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = _webApiKey,
                AuthDomain = QuickReserve.Config.Config.FIREBASE_AUTH_DOMAIN_API_KEY,
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            };

            AuthClient = new FirebaseAuthClient(config);
        }
    }
}
