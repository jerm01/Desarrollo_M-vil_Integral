using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODSQuizApp.Models
{
    public class Question
    {
        public string Id { get; set; }
        public string QuizId { get; set; }  
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string ImageUrl { get; set; }
        public int Points { get; set; }
    }

}
