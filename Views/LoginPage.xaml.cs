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
            var idToken = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);

            if (!string.IsNullOrEmpty(idToken))
            {
                SessionService.IdToken = idToken;
                // Aquí puedes asignar el UID si lo obtienes de otra fuente o Firestore

                await Shell.Current.GoToAsync("MainPage");
            }
            else
            {
                await DisplayAlert("Error", "Credenciales incorrectas", "OK");
            }
        }

        private void OnRegisterClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterPage());
        }
    }
}
