namespace ODSQuizApp
{
    using ODSQuizApp.Views;
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
