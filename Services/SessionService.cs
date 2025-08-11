using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ODSQuizApp.Services
{
    public class SessionService
    {
        public static string IdToken { get; set; }
        public static string Uid { get; set; }

        public static async Task<string> GetIdTokenAsync()
        {
            if (!string.IsNullOrEmpty(IdToken))
                return IdToken;

            var token = await SecureStorage.Default.GetAsync("IdToken");
            IdToken = token;
            return token;
        }

        public static async Task SaveIdTokenAsync(string token)
        {
            IdToken = token;
            await SecureStorage.Default.SetAsync("IdToken", token);
        }

        public static async Task ClearSessionAsync()
        {
            IdToken = null;
            Uid = null;
            SecureStorage.Default.Remove("IdToken");
        }
    }
}
