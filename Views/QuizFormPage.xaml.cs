using ODSQuizApp.Models;
using ODSQuizApp.Services;

namespace ODSQuizApp.Views;

public partial class QuizFormPage : ContentPage
{
    private readonly QuizService _quizService;
    private readonly SessionService _session;
    private const string Tema = "educacion";

    public QuizFormPage(SessionService session, QuizService quizService)
    {
        InitializeComponent();
        _session = session;
        _quizService = quizService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var quiz = new Quiz
        {
            Title = TitleEntry.Text,
            Description = DescriptionEditor.Text,
            Ods = int.Parse(OdsEntry.Text),
            IsPublic = IsPublicSwitch.IsToggled,
            CreatedBy = _session.Uid,
            CreatedAt = DateTime.UtcNow
        };

        string result = await _quizService.CreateQuiz(Tema, quiz, _session.IdToken);

        if (!string.IsNullOrEmpty(result))
        {
            await DisplayAlert("Éxito", "Quiz creado correctamente", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Error al guardar el quiz", "OK");
        }
    }
}