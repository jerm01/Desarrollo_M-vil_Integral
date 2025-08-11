using Microsoft.Maui.Controls;
using ODSQuizApp.Models;
using ODSQuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODSQuizApp.Views
{
    public partial class QuizFormPage : ContentPage
    {
        private readonly QuizService _quizService = new();

        // Mantengo OdsItem para poblar el Picker
        public static readonly List<OdsItem> OdsList = new()
        {
            new() { Numero = "1",  Nombre = "Fin de la pobreza" },
            new() { Numero = "2",  Nombre = "Hambre cero" },
            new() { Numero = "3",  Nombre = "Salud y bienestar" },
            new() { Numero = "4",  Nombre = "Educación de calidad" },
            new() { Numero = "5",  Nombre = "Igualdad de género" },
            new() { Numero = "6",  Nombre = "Agua limpia y saneamiento" },
            new() { Numero = "7",  Nombre = "Energía asequible y no contaminante" },
            new() { Numero = "8",  Nombre = "Trabajo decente y crecimiento económico" },
            new() { Numero = "9",  Nombre = "Industria, innovación e infraestructura" },
            new() { Numero = "10", Nombre = "Reducción de las desigualdades" },
            new() { Numero = "11", Nombre = "Ciudades y comunidades sostenibles" },
            new() { Numero = "12", Nombre = "Producción y consumo responsables" },
            new() { Numero = "13", Nombre = "Acción por el clima" },
            new() { Numero = "14", Nombre = "Vida submarina" },
            new() { Numero = "15", Nombre = "Vida de ecosistemas terrestres" },
            new() { Numero = "16", Nombre = "Paz, justicia e instituciones sólidas" },
            new() { Numero = "17", Nombre = "Alianzas para lograr los objetivos" }
        };

        private OdsItem? _selectedOds;

        public QuizFormPage()
        {
            InitializeComponent();
            OdsPicker.ItemsSource = OdsList;
        }

        private void OnOdsChanged(object? sender, EventArgs e)
        {
            _selectedOds = OdsPicker.SelectedItem as OdsItem;
            OdsDescripcionLabel.Text = _selectedOds is null ? "" : _selectedOds.Nombre;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionService.IdToken))
                {
                    await DisplayAlert("Sesión expirada", "Debes iniciar sesión para continuar.", "OK");
                    await Shell.Current.GoToAsync("///LoginPage");
                    return;
                }

                if (_selectedOds is null)
                {
                    await DisplayAlert("Error", "Debes seleccionar un ODS.", "OK");
                    return;
                }

                var title = TitleEntry.Text?.Trim() ?? "";
                var description = DescriptionEntry.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(title))
                {
                    await DisplayAlert("Error", "El título es obligatorio.", "OK");
                    return;
                }

                // *** CLAVE: Guardar N - Nombre en un solo campo ***
                var odsCombined = $"{_selectedOds.Numero} - {_selectedOds.Nombre}";

                var quiz = new Quiz
                {
                    Title = title,
                    Description = description,
                    Ods = odsCombined,
                    IsPublic = true,
                    CreatedBy = "", // opcional: correo del usuario
                    CreatedAt = DateTime.UtcNow
                };

                var id = await _quizService.CreateQuiz("educacion", quiz, SessionService.IdToken);

                if (id != null)
                {
                    await DisplayAlert("Éxito", "Quiz creado correctamente.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo crear el quiz.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Excepción", ex.Message, "OK");
            }
        }
    }
}
