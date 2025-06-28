using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;

namespace ODSQuizApp.Services
{
    public class QuestionService
    {
        private readonly HttpClient _client = new();

        public async Task<List<Question>> GetQuestions(string tema, string quizId)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}/questions";
            var response = await _client.GetAsync(url);
            var result = new List<Question>();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                foreach (var doc in json.documents)
                {
                    var f = doc.fields;
                    result.Add(new Question
                    {
                        QuestionText = f.question.stringValue,
                        Options = f.options.arrayValue.values.ToObject<List<string>>(),
                        CorrectAnswerIndex = int.Parse(f.correctAnswerIndex.integerValue.ToString()),
                        ImageUrl = f.imageUrl?.stringValue,
                        Points = int.Parse(f.points.integerValue.ToString())
                    });
                }
            }
            return result;
        }

        public async Task<bool> CreateQuestion(string tema, string quizId, Question question)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}/questions";
            var data = new
            {
                fields = new
                {
                    question = new { stringValue = question.QuestionText },
                    options = new { arrayValue = new { values = question.Options.Select(o => new { stringValue = o }).ToArray() } },
                    correctAnswerIndex = new { integerValue = question.CorrectAnswerIndex },
                    imageUrl = new { stringValue = question.ImageUrl ?? "" },
                    points = new { integerValue = question.Points }
                }
            };
            var json = JsonConvert.SerializeObject(data);
            var response = await _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateQuestion(string tema, string quizId, string questionId, Question question)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}/questions/{questionId}";
            var data = new
            {
                fields = new
                {
                    question = new { stringValue = question.QuestionText },
                    options = new { arrayValue = new { values = question.Options.Select(o => new { stringValue = o }).ToArray() } },
                    correctAnswerIndex = new { integerValue = question.CorrectAnswerIndex },
                    imageUrl = new { stringValue = question.ImageUrl ?? "" },
                    points = new { integerValue = question.Points }
                }
            };
            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuestion(string tema, string quizId, string questionId)
        {
            var url = FirebaseConfig.FirestoreUrl + $"temas/{tema}/quizzes/{quizId}/questions/{questionId}";
            var response = await _client.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
    }
}
