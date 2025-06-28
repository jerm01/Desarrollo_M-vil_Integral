using ODSQuizApp.Models;
using ODSQuizApp.Services;

namespace ODSQuizApp.Views;

public partial class QuestionFormPage : ContentPage
{
    private readonly QuestionService _questionService;
    private readonly string _tema;
    private readonly string _quizId;

    public QuestionFormPage(string tema, string quizId, QuestionService questionService)
    {
        InitializeComponent();
        _questionService = questionService;
        _tema = tema;
        _quizId = quizId;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(QuestionEditor.Text) ||
            string.IsNullOrWhiteSpace(Option1.Text) ||
            string.IsNullOrWhiteSpace(Option2.Text) ||
            string.IsNullOrWhiteSpace(Option3.Text) ||
            string.IsNullOrWhiteSpace(Option4.Text) ||
            !int.TryParse(CorrectIndex.Text, out int correctIndex) ||
            !int.TryParse(Points.Text, out int points))
        {
            await DisplayAlert("Error", "Completa todos los campos correctamente.", "OK");
            return;
        }

        var question = new Question
        {
            QuestionText = QuestionEditor.Text,
            Options = new List<string> { Option1.Text, Option2.Text, Option3.Text, Option4.Text },
            CorrectAnswerIndex = correctIndex,
            ImageUrl = string.IsNullOrWhiteSpace(ImageUrl.Text) ? null : ImageUrl.Text,
            Points = points
        };

        bool success = await _questionService.CreateQuestion(_tema, _quizId, question);

        if (success)
        {
            await DisplayAlert("Éxito", "Pregunta guardada", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo guardar la pregunta.", "OK");
        }
    }
}