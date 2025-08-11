using ODSQuizApp.Views;

namespace ODSQuizApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Solo registramos rutas que necesitan recibir parámetros
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("UserListPage", typeof(UserListPage));
            Routing.RegisterRoute("QuizFormPage", typeof(QuizFormPage));

            // ✅ Agrega estos para que funcionen con parámetros
            Routing.RegisterRoute("QuestionsPage", typeof(QuestionsPage));
            Routing.RegisterRoute("QuestionFormPage", typeof(QuestionFormPage));
        }
    }
}
