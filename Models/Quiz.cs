using System;

namespace ODSQuizApp.Models
{
    public class Quiz
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ods { get; set; }  // ← corregido
        public bool IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
