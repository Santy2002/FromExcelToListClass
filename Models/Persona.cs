using System.ComponentModel.DataAnnotations;

namespace FromExcelToListClass.Models
{
    public class Persona
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public int Edad { get; set; }
        public string ComidaFavorita { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Color { get; set; }
        public string Hijos { get; set; }
        //public string Mouse { get; set; }
        //public string Teclado { get; set; }
        //public string Compu { get; set; }
        //public string Play {  get; set; }

        public override string ToString()
        {
            return string.Format("Persona:\nNombre: {0}\nApellido: {1}\nEdad: {2}\nComida Favorita: {3}\nFecha Creacion: {4}\nColor: {5}\nHijos: {6}", Nombre, Apellido, Edad, ComidaFavorita, FechaCreacion, Color, Hijos);
        }
    }
}
