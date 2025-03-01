using Firebase.Auth;
using Firebase.Auth.Providers;

namespace QuickReserve.Services
{
    public class FirebaseAuthService
    {
        public FirebaseAuthClient AuthClient { get; private set; }
        private readonly string _webApiKey = "AIzaSyBrYyBcrfzwaWYVU1XbUG7CZ660XTwSmyU";  // API key

        public FirebaseAuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = _webApiKey,
                AuthDomain = "quickreserve-9b03a.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            };

            AuthClient = new FirebaseAuthClient(config);
        }
    }
}
