using System;
using System.Collections.Generic;
using System.Linq;

namespace ODSQuizApp.Models
{
    public static class OdsCatalog
    {
        private static readonly Dictionary<int, string> _ods = new()
        {
            {1, "Fin de la pobreza"},
            {2, "Hambre cero"},
            {3, "Salud y bienestar"},
            {4, "Educación de calidad"},
            {5, "Igualdad de género"},
            {6, "Agua limpia y saneamiento"},
            {7, "Energía asequible y no contaminante"},
            {8, "Trabajo decente y crecimiento económico"},
            {9, "Industria, innovación e infraestructura"},
            {10, "Reducción de las desigualdades"},
            {11, "Ciudades y comunidades sostenibles"},
            {12, "Producción y consumo responsables"},
            {13, "Acción por el clima"},
            {14, "Vida submarina"},
            {15, "Vida de ecosistemas terrestres"},
            {16, "Paz, justicia e instituciones sólidas"},
            {17, "Alianzas para lograr los objetivos"},
        };

        public static bool TryGetName(int number, out string name) => _ods.TryGetValue(number, out name!);

        /// Devuelve “N - Nombre” a partir de “N” o del propio formato ya combinado.
        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var trimmed = value.Trim();

            // Ya está normalizado
            if (trimmed.Contains(" - ")) return trimmed;

            // Vino sólo el número
            if (int.TryParse(trimmed, out var n) && TryGetName(n, out var name))
                return $"{n} - {name}";

            // Intento extraer número inicial si vino con ruido
            var digits = new string(trimmed.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var n2) && TryGetName(n2, out var name2))
                return $"{n2} - {name2}";

            // Devolver tal cual si no se puede mapear
            return trimmed;
        }

        /// Extrae el número (int) a partir de “N - Nombre” o “N”.
        public static int? TryParseNumber(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var numPart = value.Split('-')[0].Trim();
            return int.TryParse(numPart, out var n) ? n : null;
        }
    }
}
