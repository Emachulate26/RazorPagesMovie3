using System.Collections.Generic;
using RazorPagesMovie.Models;
using RazorPagesMovie.Services;

namespace RazorPagesMovie.Services
{
    public class UserService
    {
        private static List<AppUser> users = new List<AppUser>();

        public List<AppUser> GetUsers()
        {
            return users;
        }

        public void SaveUsers(List<AppUser> updatedUsers)
        {
            users = updatedUsers;
        }
    }
}
