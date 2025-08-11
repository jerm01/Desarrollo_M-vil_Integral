using ODSQuizApp.Models;
using ODSQuizApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace ODSQuizApp.Views
{
    public partial class UserListPage : ContentPage
    {
        private readonly UserService _userService = new();
        private ObservableCollection<User> _users = new();
        private bool _needsRefresh = true;

        public UserListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_needsRefresh)
            {
                _needsRefresh = false;
                await ReloadUsers();
            }
        }

        private async Task ReloadUsers()
        {
            try
            {
                var idToken = await SessionService.GetIdTokenAsync();
                if (string.IsNullOrEmpty(idToken))
                {
                    await DisplayAlert("Error", "Sesión inválida o token no disponible.", "OK");
                    return;
                }

                var list = await _userService.GetAllUsersAsync(idToken);
                _users = new ObservableCollection<User>(list);
                UserListView.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar los usuarios:\n{ex.Message}", "OK");
            }
        }

        private async void OnCreateUserClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserFormPage());
            _needsRefresh = true;
        }

        private async void OnEditUserClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is User user)
            {
                var idToken = await SessionService.GetIdTokenAsync();
                await Navigation.PushAsync(new UserFormPage(user, idToken));
                _needsRefresh = true;
            }
        }

        private async void OnDeleteUserClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is User user)
            {
                var confirm = await DisplayAlert("Confirmar", $"¿Eliminar a {user.Name} {user.LastName}?", "Sí", "No");
                if (!confirm) return;

                var idToken = await SessionService.GetIdTokenAsync();
                var success = await _userService.DeleteUser(user.Uid, idToken);

                if (success)
                {
                    _users.Remove(user);
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar el usuario.", "OK");
                }
            }
        }
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MainPage");
        }
    }
}
