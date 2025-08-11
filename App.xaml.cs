using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using ODSQuizApp.Views;

namespace ODSQuizApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Establece AppShell como el contenedor principal
            MainPage = new AppShell();
        }
    }
}
