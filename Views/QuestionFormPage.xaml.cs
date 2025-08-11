using Microsoft.Maui.Controls;
using ODSQuizApp.Models;
using ODSQuizApp.Services;
using System.Collections.Generic;

namespace ODSQuizApp.Views
{
    [QueryProperty(nameof(QuizId), "quizId")]
    [QueryProperty(nameof(Tema), "tema")]
    [QueryProperty(nameof(QuestionId), "questionId")]
    public partial class QuestionFormPage : ContentPage
    {
        private readonly QuestionService _questionService = new();

        public string QuizId { get; set; }
        public string Tema { get; set; }
        public string QuestionId { get; set; }

        private Question currentQuestion;

        public QuestionFormPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(QuestionId))
            {
                currentQuestion = await _questionService.GetQuestionByIdAsync(Tema, QuizId, QuestionId, SessionService.IdToken);

                if (currentQuestion != null)
                {
                    QuestionEntry.Text = currentQuestion.QuestionText;
                    PointsEntry.Text = currentQuestion.Points.ToString();
                    CorrectIndexEntry.Text = currentQuestion.CorrectAnswerIndex.ToString();

                    if (currentQuestion.Options.Count >= 4)
                    {
                        Option1Entry.Text = currentQuestion.Options[0];
                        Option2Entry.Text = currentQuestion.Options[1];
                        Option3Entry.Text = currentQuestion.Options[2];
                        Option4Entry.Text = currentQuestion.Options[3];
                    }
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(QuestionEntry.Text) ||
                    string.IsNullOrWhiteSpace(Option1Entry.Text) ||
                    string.IsNullOrWhiteSpace(Option2Entry.Text) ||
                    string.IsNullOrWhiteSpace(Option3Entry.Text) ||
                    string.IsNullOrWhiteSpace(Option4Entry.Text) ||
                    string.IsNullOrWhiteSpace(CorrectIndexEntry.Text) ||
                    string.IsNullOrWhiteSpace(PointsEntry.Text))
                {
                    await DisplayAlert("Error", "Por favor llena todos los campos obligatorios.", "OK");
                    return;
                }

                var question = new Question
                {
                    Id = QuestionId, // importante para actualizar
                    QuizId = QuizId,
                    QuestionText = QuestionEntry.Text,
                    Options = new List<string>
                    {
                        Option1Entry.Text,
                        Option2Entry.Text,
                        Option3Entry.Text,
                        Option4Entry.Text
                    },
                    CorrectAnswerIndex = int.Parse(CorrectIndexEntry.Text),
                    Points = int.Parse(PointsEntry.Text),
                };

                if (!string.IsNullOrEmpty(QuestionId))
                {
                    await _questionService.UpdateQuestionAsync(Tema, QuizId, question, SessionService.IdToken);
                }
                else
                {
                    await _questionService.AddQuestionAsync(Tema, QuizId, question, SessionService.IdToken);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error inesperado", ex.Message, "OK");
            }
        }
    }
}
