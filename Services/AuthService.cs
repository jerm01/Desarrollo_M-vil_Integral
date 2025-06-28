using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using ODSQuizApp.Services;

namespace ODSQuizApp.Services
{
    public class AuthService
    {
        private static readonly HttpClient client = new();

        public async Task<(string idToken, string uid)> SignIn(string email, string password)
        {
            var data = new { email, password, returnSecureToken = true };
            var json = JsonConvert.SerializeObject(data);

            var response = await client.PostAsync(
                FirebaseConfig.AuthUrl + "signInWithPassword?key=" + FirebaseConfig.ApiKey,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return (result.idToken.ToString(), result.localId.ToString());
            }

            return (null, null);
        }

        public async Task<(string idToken, string uid)> Register(string email, string password)
        {
            var data = new { email, password, returnSecureToken = true };
            var json = JsonConvert.SerializeObject(data);

            var response = await client.PostAsync(
                FirebaseConfig.AuthUrl + "signUp?key=" + FirebaseConfig.ApiKey,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return (result.idToken.ToString(), result.localId.ToString());
            }
            else
            {
                Console.WriteLine("Firebase Error: " + content);
                return (null, null);
            }
        }
    }
}
