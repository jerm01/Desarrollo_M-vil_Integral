using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;

namespace ODSQuizApp.Services
{
    public class UserService
    {
        private readonly HttpClient _client = new();

        public async Task<bool> CreateUser(User user, string idToken)
        {
            var url = $"{FirebaseConfig.FirestoreUrl}users/{user.Uid}";

            var data = new
            {
                fields = new
                {
                    name = new { stringValue = user.Name },
                    lastName = new { stringValue = user.LastName },
                    role = new { stringValue = user.Role },
                    phoneNumber = new { stringValue = user.PhoneNumber },
                    birthDate = new { stringValue = user.BirthDate },
                    createdAt = new { timestampValue = user.CreatedAt.ToString("o") }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUser(User user, string idToken)
        {
            var url = $"{FirebaseConfig.FirestoreUrl}users/{user.Uid}";

            var data = new
            {
                fields = new
                {
                    name = new { stringValue = user.Name ?? "" },
                    lastName = new { stringValue = user.LastName ?? "" },
                    role = new { stringValue = user.Role ?? "" },
                    phoneNumber = new { stringValue = user.PhoneNumber ?? "" },
                    birthDate = new { stringValue = user.BirthDate ?? "" },
                    createdAt = new { timestampValue = user.CreatedAt.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"🔥 Firestore PATCH response: {responseContent}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<User>> GetAllUsersAsync(string idToken)
        {
            var url = $"{FirebaseConfig.FirestoreUrl}users";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<User>();

            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);

            var users = new List<User>();
            if (json.documents == null) return users;

            foreach (var doc in json.documents)
            {
                var fields = doc.fields;

                DateTime created;
                DateTime.TryParse(fields?.createdAt?.timestampValue?.ToString(), out created);

                var nameParts = doc.name.ToString().Split('/');
                var uid = nameParts[nameParts.Length - 1];

                var user = new User
                {
                    Uid = uid,
                    Name = fields?.name?.stringValue,
                    LastName = fields?.lastName?.stringValue,
                    Role = fields?.role?.stringValue,
                    PhoneNumber = fields?.phoneNumber?.stringValue,
                    BirthDate = fields?.birthDate?.stringValue,
                    CreatedAt = created
                };

                users.Add(user);
            }

            return users;
        }

        public async Task<bool> DeleteUser(string uid, string idToken)
        {
            var url = $"{FirebaseConfig.FirestoreUrl}users/{uid}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
