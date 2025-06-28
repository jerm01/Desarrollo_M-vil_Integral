using ODSQuizApp.Services;

namespace ODSQuizApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService = new();

        public LoginPage()
        {
            InitializeComponent();
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var token = await _authService.SignIn(EmailEntry.Text, PasswordEntry.Text);
            if (!string.IsNullOrEmpty(token.idToken) && !string.IsNullOrEmpty(token.uid))
            {
                var quizService = new QuizService(); 
                var sessionService = new SessionService
                {
                    IdToken = token.idToken,
                    Uid = token.uid
                }; 
                Application.Current.MainPage = new NavigationPage(new MainPage(quizService, sessionService));
            }
            else
            {
                ErrorLabel.Text = "Correo o contraseña incorrectos.";
                ErrorLabel.IsVisible = true;
            }
        }


        private void OnRegisterClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterPage());
        }
    }
}