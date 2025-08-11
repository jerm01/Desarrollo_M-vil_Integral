using ODSQuizApp.Models;
using ODSQuizApp.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ODSQuizApp.Views
{
    public partial class QuizListPage : ContentPage
    {
        private readonly QuizService _quizService = new();
        public ObservableCollection<QuizGroup> GroupedQuizzes { get; set; } = new();

        public QuizListPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadQuizzes();
        }

        private async Task LoadQuizzes()
        {
            try
            {
                if (string.IsNullOrEmpty(SessionService.IdToken))
                {
                    await DisplayAlert("Sesión expirada", "Debes iniciar sesión para continuar.", "OK");
                    await Shell.Current.GoToAsync("///LoginPage");
                    return;
                }

                var list = await _quizService.GetQuizzes("educacion", SessionService.IdToken);

                var grouped = list
                    .GroupBy(q => q.Ods)                // ← clave: usa el valor combinado
                    .OrderBy(g => OdsCatalog.TryParseNumber(g.Key) ?? 99)
                    .Select(g => new QuizGroup(g.Key, g));

                GroupedQuizzes.Clear();
                foreach (var g in grouped)
                    GroupedQuizzes.Add(g);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnCreateQuizClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("QuizFormPage");

        private async void OnEditQuizClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Quiz q)
                await Shell.Current.GoToAsync($"///QuizEditorPage?quizId={q.Id}");
        }

        private async void OnViewQuestionsClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Quiz q)
                await Shell.Current.GoToAsync($"QuestionsPage?quizId={q.Id}&tema=educacion");
        }

        private async void OnDeleteQuizClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not Quiz q) return;
            var confirm = await DisplayAlert("Confirmar", "¿Eliminar este quiz?", "Sí", "No");
            if (!confirm) return;

            var ok = await _quizService.DeleteQuizAsync("educacion", q.Id, SessionService.IdToken);
            if (ok) await LoadQuizzes();
            else await DisplayAlert("Error", "No se pudo eliminar el quiz.", "OK");
        }

        private async void OnBackToMainPageClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("MainPage");
    }

    public class QuizGroup : ObservableCollection<Quiz>
    {
        public string OdsName { get; }
        public QuizGroup(string odsName, IEnumerable<Quiz> quizzes) : base(quizzes) => OdsName = odsName;
    }
}
