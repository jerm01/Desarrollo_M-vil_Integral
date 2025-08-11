using ODSQuizApp.Models;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ODSQuizApp.Services
{
    public class QuestionService
    {
        private readonly HttpClient _client = new();

        private string GetBaseUrl(string tema, string quizId) =>
            $"{FirebaseConfig.FirestoreUrl}temas/{tema}/quizzes/{quizId}/questions";

        public async Task<List<Question>> GetQuestionsAsync(string tema, string quizId, string idToken)
        {
            var url = GetBaseUrl(tema, quizId);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);
            var result = new List<Question>();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (json?.documents != null)
                {
                    foreach (var doc in json.documents)
                    {
                        var f = doc.fields;
                        var parts = doc.name.ToString().Split('/');

                        var question = new Question
                        {
                            Id = parts[parts.Length - 1],
                            QuizId = quizId,
                            QuestionText = f.QuestionText?.stringValue,
                            ImageUrl = f.ImageUrl?.stringValue,
                            Points = f.Points != null ? Convert.ToInt32(f.Points.integerValue) : 0,
                            CorrectAnswerIndex = f.CorrectAnswerIndex != null ? Convert.ToInt32(f.CorrectAnswerIndex.integerValue) : 0,
                            Options = new List<string>()
                        };

                        foreach (var val in f.Options?.arrayValue?.values ?? new List<dynamic>())
                        {
                            question.Options.Add((string)val.stringValue);
                        }

                        result.Add(question);
                    }
                }
            }

            return result;
        }

        public async Task<Question> GetQuestionByIdAsync(string tema, string quizId, string questionId, string idToken)
        {
            var url = $"{GetBaseUrl(tema, quizId)}/{questionId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var f = json.fields;

                var question = new Question
                {
                    Id = questionId,
                    QuizId = quizId,
                    QuestionText = f.QuestionText?.stringValue,
                    ImageUrl = f.ImageUrl?.stringValue,
                    Points = f.Points != null ? Convert.ToInt32(f.Points.integerValue) : 0,
                    CorrectAnswerIndex = f.CorrectAnswerIndex != null ? Convert.ToInt32(f.CorrectAnswerIndex.integerValue) : 0,
                    Options = new List<string>()
                };

                foreach (var val in f.Options?.arrayValue?.values ?? new List<dynamic>())
                {
                    question.Options.Add((string)val.stringValue);
                }

                return question;
            }

            return null;
        }

        public async Task AddQuestionAsync(string tema, string quizId, Question question, string idToken)
        {
            var url = GetBaseUrl(tema, quizId);

            var optionsList = new List<object>();
            foreach (var opt in question.Options)
            {
                optionsList.Add(new { stringValue = opt });
            }

            var data = new
            {
                fields = new
                {
                    QuestionText = new { stringValue = question.QuestionText },
                    ImageUrl = new { stringValue = question.ImageUrl ?? "" },
                    Points = new { integerValue = question.Points },
                    CorrectAnswerIndex = new { integerValue = question.CorrectAnswerIndex },
                    Options = new
                    {
                        arrayValue = new
                        {
                            values = optionsList.ToArray()
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await _client.SendAsync(request);
        }

        public async Task UpdateQuestionAsync(string tema, string quizId, Question question, string idToken)
        {
            var url = $"{GetBaseUrl(tema, quizId)}/{question.Id}";

            var optionsList = new List<object>();
            foreach (var opt in question.Options)
            {
                optionsList.Add(new { stringValue = opt });
            }

            var data = new
            {
                fields = new
                {
                    QuestionText = new { stringValue = question.QuestionText },
                    ImageUrl = new { stringValue = question.ImageUrl ?? "" },
                    Points = new { integerValue = question.Points },
                    CorrectAnswerIndex = new { integerValue = question.CorrectAnswerIndex },
                    Options = new
                    {
                        arrayValue = new
                        {
                            values = optionsList.ToArray()
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await _client.SendAsync(request);
        }

        public async Task DeleteQuestionAsync(string tema, string quizId, string questionId, string idToken)
        {
            var url = $"{GetBaseUrl(tema, quizId)}/{questionId}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            await _client.SendAsync(request);
        }
    }
}
