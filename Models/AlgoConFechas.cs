using NPOI.OpenXmlFormats.Dml;
using System.ComponentModel.DataAnnotations;

namespace FromExcelToListClass.Models
{
    public class AlgoConFechas
    {
        [Required]
        public DateOnly CumpleDateOnly { get; set; }
        [Required]
        public DateTime CumpleDateTime { get; set; }
        [Required]
        public int Edad {  get; set; }
        //esta propiedad es ignorada. Contemplar Funcionalidad con propiedades sin anotation?
        public string Algo { get; set; }
        public Dictionary<string, string> Opcionales { get; set; }

        public override string ToString()
        {
            return $"CumpleDateOnly: {CumpleDateOnly}\nCumpleDateTime: {CumpleDateTime}\nEdad: {Edad}\nAlgo: {Algo}\nOpcionales:{Opcionales.ToDebugString()}";
        }
    }
}
