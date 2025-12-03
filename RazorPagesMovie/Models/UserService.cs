using RazorPagesMovie.Models;

namespace RazorPagesMovie.Services
{
    public class UserService
    {
        private readonly List<AppUser> _users = new();

        public List<AppUser> GetUsers() => _users;

        public void AddUser(string username, string password)
        {
            _users.Add(new AppUser
            {
                Username = username,
                Password = password
            });
        }
    }
}
