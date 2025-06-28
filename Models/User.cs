using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODSQuizApp.Models
{
    public class User
    {
        public string Id { get; set; }  // ← NUEVO (Firebase uid)
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
