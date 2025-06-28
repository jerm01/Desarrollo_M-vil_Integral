using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODSQuizApp.Services
{
    public static class FirebaseConfig
    {
        public static string ProjectId = "odsquizapp";
        public static string ApiKey = "AIzaSyDm_UDFXgkmE1n9-QHvA9Blg1v1EOJrFN0\r\n";
        public static string AuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:";
        public static string FirestoreUrl = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/";
    }
}