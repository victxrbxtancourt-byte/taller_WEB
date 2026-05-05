using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSegundocorte.Data;
using WebSegundocorte.Models;

namespace WebSegundocorte.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriaController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAll()
        {
            var categorias = await _db.Categorias
                .Include(c => c.Products)
                .ToListAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetById(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound(new { message = $"Categoría con ID {id} no encontrada." });

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> Create([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _db.Categorias
                .AnyAsync(c => c.Name.ToLower() == categoria.Name.ToLower());

            if (exists)
                return Conflict(new { message = $"Ya existe una categoría con el nombre '{categoria.Name}'." });

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el del cuerpo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _db.Categorias
                .AnyAsync(c => c.Name.ToLower() == categoria.Name.ToLower() && c.Id != id);

            if (exists)
                return Conflict(new { message = $"Ya existe otra categoría con el nombre '{categoria.Name}'." });

            _db.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Categorias.AnyAsync(c => c.Id == id))
                    return NotFound(new { message = $"Categoría con ID {id} no encontrada." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound(new { message = $"Categoría con ID {id} no encontrada." });

            if (categoria.Products.Any())
                return BadRequest(new { message = $"No se puede eliminar '{categoria.Name}' porque tiene {categoria.Products.Count} producto(s) asociado(s)." });

            _db.Categorias.Remove(categoria);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}