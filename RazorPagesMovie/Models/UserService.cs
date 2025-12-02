using System.Text.Json;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Services
{
    public class UserService
    {
        private readonly string _filePath = "Data/users.json";

        public List<AppUser> GetUsers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<AppUser>();
            }

            string json = File.ReadAllText(_filePath);

            return JsonSerializer.Deserialize<List<AppUser>>(json)
                   ?? new List<AppUser>();
        }

        public void SaveUsers(List<AppUser> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }
    }
}
