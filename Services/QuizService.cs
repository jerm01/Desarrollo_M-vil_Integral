using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;
using System.Net.Http.Headers;


namespace ODSQuizApp.Services
{
    public class QuizService
    {
        private readonly HttpClient _client = new();

        public async Task<List<Quiz>> GetQuizzes(string tema, string idToken)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            var result = new List<Quiz>();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                foreach (var doc in json.documents)
                {
                    var f = doc.fields;
                    result.Add(new Quiz
                    {
                        Title = f.title.stringValue,
                        Description = f.description.stringValue,
                        Ods = int.Parse(f.ods.integerValue.ToString()),
                        IsPublic = f.isPublic.booleanValue,
                        CreatedBy = f.createdBy.stringValue,
                        CreatedAt = DateTime.Parse(f.createdAt.timestampValue.ToString())
                    });
                }
            }

            return result;
        }

        public async Task<string> CreateQuiz(string tema, Quiz quiz, string idToken)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes";

            var data = new
            {
                fields = new
                {
                    title = new { stringValue = quiz.Title },
                    description = new { stringValue = quiz.Description },
                    ods = new { integerValue = quiz.Ods },
                    isPublic = new { booleanValue = quiz.IsPublic },
                    createdBy = new { stringValue = quiz.CreatedBy },
                    createdAt = new { timestampValue = quiz.CreatedAt.ToString("o") }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(resultJson);
            string name = result.name;
            string quizId = name.Split('/').Last();

            return quizId;
        }

        public async Task<bool> UpdateQuiz(string tema, string quizId, Quiz quiz, string idToken)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}?updateMask.fieldPaths=title&updateMask.fieldPaths=description&updateMask.fieldPaths=ods&updateMask.fieldPaths=isPublic";

            var data = new
            {
                fields = new
                {
                    title = new { stringValue = quiz.Title },
                    description = new { stringValue = quiz.Description },
                    ods = new { integerValue = quiz.Ods },
                    isPublic = new { booleanValue = quiz.IsPublic }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuiz(string tema, string quizId, string idToken)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}