using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODSQuizApp.Models;
using Newtonsoft.Json;

namespace ODSQuizApp.Services
{
    public class UserService
    {
        private readonly HttpClient _client = new();

        public async Task<User> GetUserByEmail(string email)
        {
            var url = FirebaseConfig.FirestoreUrl + "users";
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            foreach (var doc in json.documents)
            {
                var f = doc.fields;
                if (f.email.stringValue == email)
                {
                    return new User
                    {
                        Name = f.name.stringValue,
                        Email = f.email.stringValue,
                        Role = f.role.stringValue,
                        CreatedAt = DateTime.Parse(f.createdAt.timestampValue.ToString())
                    };
                }
            }
            return null;
        }

        public async Task<bool> CreateUser(User user)
        {
            var url = FirebaseConfig.FirestoreUrl + "users";
            var data = new
            {
                fields = new
                {
                    name = new { stringValue = user.Name },
                    email = new { stringValue = user.Email },
                    role = new { stringValue = user.Role },
                    createdAt = new { timestampValue = user.CreatedAt.ToString("o") }
                }
            };
            var json = JsonConvert.SerializeObject(data);
            var response = await _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
