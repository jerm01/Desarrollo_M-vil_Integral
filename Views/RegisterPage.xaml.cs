namespace ODSQuizApp.Views;
using ODSQuizApp.Models;
using ODSQuizApp.Services;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService = new();
    private readonly UserService _userService = new();

    public RegisterPage()
    {
        InitializeComponent();
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var token = await _authService.Register(EmailEntry.Text, PasswordEntry.Text);
        if (!string.IsNullOrEmpty(token.idToken) && !string.IsNullOrEmpty(token.uid))
        {
            var user = new User
            {
                Id = token.uid,
                Name = NameEntry.Text,
                Email = EmailEntry.Text,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };
            await _userService.CreateUser(user);
            await DisplayAlert("Éxito", "Cuenta creada, ahora puedes iniciar sesión", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            ErrorLabel.Text = "No se pudo registrar.";
            ErrorLabel.IsVisible = true;
        }
    }

}