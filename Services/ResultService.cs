using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODSQuizApp.Models;

namespace ODSQuizApp.Services
{
    public class ResultService
    {
        private readonly HttpClient _client = new();

        public async Task<bool> CreateResult(Result result)
        {
            var url = FirebaseConfig.FirestoreUrl + "results";
            var data = new
            {
                fields = new
                {
                    userId = new { stringValue = result.UserId },
                    tema = new { stringValue = result.Tema },
                    quizId = new { stringValue = result.QuizId },
                    score = new { integerValue = result.Score },
                    totalQuestions = new { integerValue = result.TotalQuestions },
                    correctAnswers = new { integerValue = result.CorrectAnswers },
                    finishedAt = new { timestampValue = result.FinishedAt.ToString("o") }
                }
            };
            var json = JsonConvert.SerializeObject(data);
            var response = await _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
