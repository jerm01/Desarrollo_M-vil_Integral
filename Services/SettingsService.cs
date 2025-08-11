using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODSQuizApp.Models; 
using Newtonsoft.Json;

namespace ODSQuizApp.Services
{
    public class SettingsService
    {
        private readonly HttpClient _client = new();

        public async Task<AppSettings> GetSettings()
        {
            var url = FirebaseConfig.FirestoreUrl + "settings/general";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var f = json.fields;
                return new AppSettings
                {
                    Version = f.version.stringValue,
                    MaintenanceMode = f.maintenanceMode.booleanValue
                };
            }

            return null;
        }
    }
}
