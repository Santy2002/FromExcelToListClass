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
        public Dictionary<string, string> Opcionales { get; set; }

        public override string ToString()
        {
            return string.Format("Persona:\nNombre: {0}\nApellido: {1}\nEdad: {2}\nOpcionales: {3}", Nombre, Apellido, Edad, Opcionales.ToDebugString());
        }
    }

    public static class Extendidos
    {
        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary != null)
            {
                return "{" + string.Join(",\n", dictionary.Select(kv => kv.Key + " = " + kv.Value).ToArray()) + "}";
            }

            return string.Empty;
        }
    }
}
