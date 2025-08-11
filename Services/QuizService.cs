using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;

namespace ODSQuizApp.Services
{
    public class QuizService
    {
        private readonly HttpClient _client = new();

        private static string Base(string tema) =>
            $"{FirebaseConfig.FirestoreUrl}temas/{tema}/quizzes";

        #region Helpers (Mapeo)

        private static string GetString(dynamic f, string key) =>
            (string)(f?[key]?["stringValue"] ?? "");

        private static bool GetBool(dynamic f, string key) =>
            (bool)(f?[key]?["booleanValue"] ?? false);

        private static DateTime GetTime(dynamic f, string key)
        {
            var v = (string)(f?[key]?["timestampValue"] ?? "");
            return DateTime.TryParse(v, out var dt) ? dt : DateTime.UtcNow;
        }

        private static Quiz Map(dynamic doc)
        {
            var fields = doc.fields;
            var name = (string)doc.name;
            var id = name.Split('/').Last();

            return new Quiz
            {
                Id = id,
                Title = GetString(fields, "title"),
                Description = GetString(fields, "description"),
                // Normalizar por si en la BD existen registros viejos solo con número
                Ods = OdsCatalog.Normalize(GetString(fields, "ods")),
                IsPublic = GetBool(fields, "isPublic"),
                CreatedBy = GetString(fields, "createdBy"),
                CreatedAt = GetTime(fields, "createdAt")
            };
        }

        private static object Serialize(Quiz q)
        {
            // Garantizar “N - Nombre” antes de mandar a Firestore
            var ods = OdsCatalog.Normalize(q.Ods);

            return new
            {
                fields = new
                {
                    title = new { stringValue = q.Title ?? "" },
                    description = new { stringValue = q.Description ?? "" },
                    ods = new { stringValue = ods },
                    isPublic = new { booleanValue = q.IsPublic },
                    createdBy = new { stringValue = q.CreatedBy ?? "" },
                    createdAt = new { timestampValue = q.CreatedAt.ToUniversalTime().ToString("o") }
                }
            };
        }

        #endregion

        public async Task<List<Quiz>> GetQuizzes(string tema, string idToken)
        {
            var url = Base(tema);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var res = await _client.SendAsync(req);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json)!;

            var list = new List<Quiz>();
            if (data.documents != null)
            {
                foreach (var d in data.documents)
                    list.Add(Map(d));
            }
            return list.OrderBy(q => OdsCatalog.TryParseNumber(q.Ods) ?? 99).ToList();
        }

        public async Task<Quiz?> GetQuizById(string tema, string quizId, string idToken)
        {
            var url = $"{Base(tema)}/{quizId}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            dynamic doc = JsonConvert.DeserializeObject(json)!;
            return Map(doc);
        }

        public async Task<string?> CreateQuiz(string tema, Quiz quiz, string idToken)
        {
            var url = Base(tema);
            quiz.CreatedAt = quiz.CreatedAt == default ? DateTime.UtcNow : quiz.CreatedAt;

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            req.Content = new StringContent(JsonConvert.SerializeObject(Serialize(quiz)), Encoding.UTF8, "application/json");

            var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            dynamic created = JsonConvert.DeserializeObject(json)!;
            var name = (string)created.name;
            return name.Split('/').Last();
        }

        public async Task<bool> UpdateQuiz(string tema, Quiz quiz, string idToken)
        {
            if (string.IsNullOrEmpty(quiz.Id)) return false;

            // Actualizamos campos clave; si desea incluir createdAt/createdBy, añádalos al updateMask
            var url = $"{Base(tema)}/{quiz.Id}" +
                      "?updateMask.fieldPaths=title" +
                      "&updateMask.fieldPaths=description" +
                      "&updateMask.fieldPaths=ods" +
                      "&updateMask.fieldPaths=isPublic";

            var req = new HttpRequestMessage(HttpMethod.Patch, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            req.Content = new StringContent(JsonConvert.SerializeObject(Serialize(quiz)), Encoding.UTF8, "application/json");

            var res = await _client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuizAsync(string tema, string quizId, string idToken)
        {
            var url = $"{Base(tema)}/{quizId}";
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var res = await _client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
    }
}
