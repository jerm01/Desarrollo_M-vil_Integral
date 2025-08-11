using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODSQuizApp.Models
{
    public class Result
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Tema { get; set; }
        public string QuizId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}
