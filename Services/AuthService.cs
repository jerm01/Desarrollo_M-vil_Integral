using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ODSQuizApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _client = new();

        public async Task<string?> SignUpAsync(string email, string password)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                email,
                password,
                returnSecureToken = true
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseConfig.ApiKey}", content);
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent)!;
            return result.localId;
        }

        public async Task<string?> SignInAsync(string email, string password)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                email,
                password,
                returnSecureToken = true
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseConfig.ApiKey}", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent)!;
            return result.idToken;
        }

        public async Task<bool> UpdateEmailAndPasswordAsync(string idToken, string newEmail, string newPassword)
        {
            var payload = new
            {
                idToken,
                email = newEmail,
                password = newPassword,
                returnSecureToken = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:update?key={FirebaseConfig.ApiKey}", content);

            return response.IsSuccessStatusCode;
        }
    }
}
