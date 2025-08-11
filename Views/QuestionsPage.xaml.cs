using Microsoft.Maui.Controls;
using ODSQuizApp.Models;
using ODSQuizApp.Services;
using System.Collections.Generic;

namespace ODSQuizApp.Views
{
    [QueryProperty(nameof(QuizId), "quizId")]
    [QueryProperty(nameof(Tema), "tema")]
    public partial class QuestionsPage : ContentPage
    {
        private readonly QuestionService _questionService = new();
        private List<Question> _questions;

        public string QuizId { get; set; }
        public string Tema { get; set; }

        public QuestionsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrEmpty(QuizId) || string.IsNullOrEmpty(Tema))
                return;

            try
            {
                _questions = await _questionService.GetQuestionsAsync(Tema, QuizId, SessionService.IdToken);
                QuestionsCollection.ItemsSource = _questions;
            }
            catch
            {
                // Silencioso. Opcional: mostrar mensaje si se desea.
            }
        }

        private async void OnAddQuestionClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"QuestionFormPage?quizId={QuizId}&tema={Tema}");
        }

        private async void OnEditQuestionClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Question question)
            {
                await Shell.Current.GoToAsync($"QuestionFormPage?quizId={QuizId}&tema={Tema}&questionId={question.Id}");
            }
        }

        private async void OnDeleteQuestionClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Question question)
            {
                bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar la pregunta '{question.QuestionText}'?", "Sí", "No");
                if (!confirm) return;

                try
                {
                    await _questionService.DeleteQuestionAsync(Tema, QuizId, question.Id, SessionService.IdToken);
                    _questions.Remove(question);
                    QuestionsCollection.ItemsSource = null;
                    QuestionsCollection.ItemsSource = _questions;
                }
                catch
                {
                    await DisplayAlert("Error", "No se pudo eliminar la pregunta.", "OK");
                }
            }
        }
    }
}
