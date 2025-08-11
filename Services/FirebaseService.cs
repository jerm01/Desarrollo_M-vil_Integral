using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;

namespace ODSQuizApp.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _client = new();
        private readonly string baseUrl = $"{FirebaseConfig.FirestoreUrl}usuarios";

        public async Task SaveUserToFirestoreAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PutAsync($"{baseUrl}/{user.Uid}.json", content);
        }

        public async Task<List<User>> GetUsersFromFirestoreAsync()
        {
            var response = await _client.GetAsync($"{baseUrl}.json");
            if (!response.IsSuccessStatusCode)
                return new List<User>();

            var json = await response.Content.ReadAsStringAsync();
            var dict = JsonConvert.DeserializeObject<Dictionary<string, User>>(json);
            return dict?.Values.ToList() ?? new List<User>();
        }
    }
}
