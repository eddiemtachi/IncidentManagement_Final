namespace IncidentManagement.Shared.Services
{
    public class AuthState
    {
        public bool IsLoggedIn { get; private set; }
        public string? CurrentUser { get; private set; }
        public string? CurrentUserRole { get; private set; }
        public string? AccessToken { get; private set; }
        public int CurrentUserId { get; private set; }

        public void Login(string user, string role, string? accessToken = null, int userId = 0)
        {
            IsLoggedIn = true;
            CurrentUser = user;
            CurrentUserRole = role;
            AccessToken = accessToken;
            CurrentUserId = userId;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            CurrentUser = null;
            CurrentUserRole = null;
            AccessToken = null;
            CurrentUserId = 0;
        }
    }
}
