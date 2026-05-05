using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSegundocorte.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [MaxLength(250, ErrorMessage = "La descripcion no puede superar los 250 caracteres")]
        public string? Descripction { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La categoria es obligatoria")]
        public int CategoryId { get; set; }

        public Categoria? Category { get; set; }
    }
}