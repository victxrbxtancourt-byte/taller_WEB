using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSegundocorte.Data;
using WebSegundocorte.Models;

namespace WebSegundocorte.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductoController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
        {
            var productos = await _db.Productos
                .Include(p => p.Category)
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });

            return Ok(producto);
        }

        [HttpGet("categoria/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetByCategoria(int categoryId)
        {
            bool categoriaExiste = await _db.Categorias.AnyAsync(c => c.Id == categoryId);

            if (!categoriaExiste)
                return NotFound(new { message = $"Categoría con ID {categoryId} no encontrada." });

            var productos = await _db.Productos
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return Ok(productos);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> Create([FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool categoriaExiste = await _db.Categorias.AnyAsync(c => c.Id == producto.CategoryId);

            if (!categoriaExiste)
                return NotFound(new { message = $"La categoría con ID {producto.CategoryId} no existe." });

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            await _db.Entry(producto).Reference(p => p.Category).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el del cuerpo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool categoriaExiste = await _db.Categorias.AnyAsync(c => c.Id == producto.CategoryId);

            if (!categoriaExiste)
                return NotFound(new { message = $"La categoría con ID {producto.CategoryId} no existe." });

            _db.Entry(producto).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Productos.AnyAsync(p => p.Id == id))
                    return NotFound(new { message = $"Producto con ID {id} no encontrado." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _db.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });

            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}