using Microsoft.Maui.Controls;

namespace ODSQuizApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GoToQuizzesCRUD(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///QuizListPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void GoToUsersCRUD(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///UserListPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
