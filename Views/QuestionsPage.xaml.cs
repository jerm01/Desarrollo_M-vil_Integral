namespace ODSQuizApp.Views;
using ODSQuizApp.Models;
using ODSQuizApp.Services;

public partial class QuestionsPage : ContentPage
{
    private readonly QuestionService _questionService = new();
    private readonly string _tema;
    private readonly string _quizId;

    public QuestionsPage(string tema, string quizId)
    {
        InitializeComponent();
        _tema = tema;
        _quizId = quizId;
        LoadQuestions();
    }

    private async void LoadQuestions()
    {
        var questions = await _questionService.GetQuestions(_tema, _quizId);
        QuestionList.ItemsSource = questions;
    }
    private void OnAddQuestionClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new QuestionFormPage(_tema, _quizId, _questionService));
    }
}
