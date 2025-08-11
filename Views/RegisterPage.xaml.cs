using ODSQuizApp.Models;
using ODSQuizApp.Services;

namespace ODSQuizApp.Views
{
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
            var role = RolePicker.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberEntry.Text) ||
                string.IsNullOrWhiteSpace(role))
            {
                ErrorLabel.Text = "Por favor llena todos los campos.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var uid = await _authService.SignUpAsync(EmailEntry.Text, PasswordEntry.Text);
            if (!string.IsNullOrEmpty(uid))
            {
                var idToken = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
                if (string.IsNullOrEmpty(idToken))
                {
                    await DisplayAlert("Error", "No se pudo autenticar", "OK");
                    return;
                }

                var user = new User
                {
                    Uid = uid,
                    Name = NameEntry.Text.Trim(),
                    LastName = LastNameEntry.Text.Trim(),
                    Role = role,
                    PhoneNumber = PhoneNumberEntry.Text.Trim(),
                    BirthDate = BirthDatePicker.Date.ToString("yyyy-MM-dd"),
                    CreatedAt = DateTime.UtcNow
                };

                bool saved = await _userService.CreateUser(user, idToken);
                if (saved)
                {
                    await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo guardar en Firestore", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo registrar el usuario", "OK");
            }
        }
    }
}
