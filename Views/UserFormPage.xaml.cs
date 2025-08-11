using ODSQuizApp.Models;
using ODSQuizApp.Services;

namespace ODSQuizApp.Views
{
    public partial class UserFormPage : ContentPage
    {
        private readonly AuthService _authService = new();
        private readonly UserService _userService = new();

        private string? userId;
        private string? idToken;
        private DateTime createdAt = DateTime.UtcNow;
        private bool isEditMode => !string.IsNullOrEmpty(userId);

        public UserFormPage()
        {
            InitializeComponent();
        }

        public UserFormPage(User user, string? idToken) : this()
        {
            this.userId = user.Uid;
            this.idToken = idToken;
            this.createdAt = user.CreatedAt;

            NameEntry.Text = user.Name;
            SurnameEntry.Text = user.LastName;
            PhoneNumberEntry.Text = user.PhoneNumber;
            BirthDatePicker.Date = DateTime.Parse(user.BirthDate);
            RolePicker.SelectedItem = user.Role;

            // ocultar campos que ya no aplican en edición
            PasswordEntry.IsVisible = false;
            EmailEntry.IsVisible = false;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim() ?? "";
            string lastName = SurnameEntry.Text?.Trim() ?? "";
            string phone = PhoneNumberEntry.Text?.Trim() ?? "";
            string role = RolePicker.SelectedItem?.ToString() ?? "";
            string birthDate = BirthDatePicker.Date.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(role))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            if (isEditMode)
            {
                var user = new User
                {
                    Uid = userId!,
                    Name = name,
                    LastName = lastName,
                    Role = role,
                    PhoneNumber = phone,
                    BirthDate = birthDate,
                    CreatedAt = createdAt
                };

                bool saved = await _userService.UpdateUser(user, idToken!);
                if (saved)
                    await Navigation.PopAsync();
                else
                    await DisplayAlert("Error", "No se pudo actualizar en Firestore", "OK");
            }
            else
            {
                string email = EmailEntry.Text?.Trim() ?? "";
                string password = PasswordEntry.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Error", "Correo y contraseña son requeridos al crear", "OK");
                    return;
                }

                var newUid = await _authService.SignUpAsync(email, password);
                if (newUid == null)
                {
                    await DisplayAlert("Error", "No se pudo registrar en Authentication", "OK");
                    return;
                }

                userId = newUid;
                idToken = await _authService.SignInAsync(email, password);
                if (idToken == null)
                {
                    await DisplayAlert("Error", "No se pudo autenticar para Firestore", "OK");
                    return;
                }

                var user = new User
                {
                    Uid = userId!,
                    Name = name,
                    LastName = lastName,
                    Role = role,
                    PhoneNumber = phone,
                    BirthDate = birthDate,
                    CreatedAt = DateTime.UtcNow
                };

                bool saved = await _userService.CreateUser(user, idToken!);
                if (saved)
                    await Navigation.PopAsync();
                else
                    await DisplayAlert("Error", "No se pudo guardar en Firestore", "OK");
            }
        }
    }
}
