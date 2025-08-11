namespace ODSQuizApp.Models
{
    public class User
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
