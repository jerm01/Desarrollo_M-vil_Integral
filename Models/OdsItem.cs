using System;

namespace ODSQuizApp.Models
{
    public class OdsItem
    {
        public string Numero { get; set; } = "";
        public string Nombre { get; set; } = "";

        // Se muestra “N - Nombre” (sin el prefijo “ODS ”)
        public override string ToString() => $"{Numero} - {Nombre}";
    }
}
