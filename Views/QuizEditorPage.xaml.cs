using Microsoft.Maui.Controls;
using ODSQuizApp.Models;
using ODSQuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODSQuizApp.Views
{
    public partial class QuizEditorPage : ContentPage, IQueryAttributable
    {
        private readonly QuizService _quizService = new();
        private string? _quizId;
        private OdsItem? _selectedOds;
        private Quiz? _quiz;

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

        public QuizEditorPage()
        {
            InitializeComponent();
            OdsPicker.ItemsSource = OdsList;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _quizId = query.TryGetValue("quizId", out var idObj) ? idObj?.ToString() : null;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SessionService.IdToken) || string.IsNullOrEmpty(_quizId))
                {
                    await DisplayAlert("Error", "Datos insuficientes para cargar el quiz.", "OK");
                    await Shell.Current.GoToAsync("///QuizListPage");
                    return;
                }

                _quiz = await _quizService.GetQuizById("educacion", _quizId!, SessionService.IdToken);

                if (_quiz is null)
                {
                    await DisplayAlert("Error", "No se encontró el quiz.", "OK");
                    await Shell.Current.GoToAsync("///QuizListPage");
                    return;
                }

                TitleEntry.Text = _quiz.Title;
                DescriptionEntry.Text = _quiz.Description;

                // Selección del ODS a partir del valor combinado “N - Nombre”
                var number = OdsCatalog.TryParseNumber(_quiz.Ods);
                if (number is not null)
                {
                    var item = OdsList.FirstOrDefault(x => x.Numero == number.Value.ToString());
                    if (item != null)
                    {
                        OdsPicker.SelectedItem = item;
                        _selectedOds = item;
                        OdsDescripcionLabel.Text = item.Nombre;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Excepción", ex.Message, "OK");
            }
        }

        private void OnOdsChanged(object? sender, EventArgs e)
        {
            _selectedOds = OdsPicker.SelectedItem as OdsItem;
            OdsDescripcionLabel.Text = _selectedOds?.Nombre ?? "";
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (_quiz is null || _selectedOds is null)
                {
                    await DisplayAlert("Error", "Faltan datos para guardar.", "OK");
                    return;
                }

                _quiz.Title = TitleEntry.Text?.Trim() ?? "";
                _quiz.Description = DescriptionEntry.Text?.Trim() ?? "";
                _quiz.Ods = $"{_selectedOds.Numero} - {_selectedOds.Nombre}"; // *** CLAVE ***

                var ok = await _quizService.UpdateQuiz("educacion", _quiz, SessionService.IdToken);

                if (ok)
                {
                    await DisplayAlert("Éxito", "Quiz actualizado correctamente.", "OK");
                    await Shell.Current.GoToAsync("///QuizListPage");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar el quiz.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Excepción", ex.Message, "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///QuizListPage");
        }
    }
}
