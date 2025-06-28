using ODSQuizApp.Services;

namespace ODSQuizApp.Views;

public partial class MainPage : ContentPage
{
    private readonly QuizService _quizService;
    private readonly SessionService _session;
    private const string Tema = "educacion";

    public MainPage(QuizService quizService, SessionService session)
    {
        InitializeComponent();
        _quizService = quizService;
        _session = session;
        LoadQuizzes();
    }

    private async void LoadQuizzes()
    {
        try
        {
            var quizzes = await _quizService.GetQuizzes(Tema, _session.IdToken);
            QuizList.ItemsSource = quizzes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudieron cargar los quizzes.\n" + ex.Message, "OK");
        }
    }
    private async void OnAddQuizClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QuizFormPage(_session, _quizService));
    }
}