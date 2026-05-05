using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSegundocorte.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Name { get; set; }

        [MaxLength(250, ErrorMessage = "La descripcion no puede superar los 250 caracteres")]
        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<Producto> Products { get; set; } = new List<Producto>();
    }
}