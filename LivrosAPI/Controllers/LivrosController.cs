using LivrosAPI.Data;
using LivrosAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LivrosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LivrosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLivros()
        {
            var livros = _context.Livros.ToList();
            return Ok(livros);
        }

        [HttpPost]
        public IActionResult AddLivro([FromBody] Livro livro)
        {
            _context.Livros.Add(livro);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetLivros), new { id = livro.Id }, livro);
        }

        [HttpPatch("{id}")]
        public IActionResult AtualizarStatus(int id, [FromBody] AtualizarStatusPayload payload)
        {
            var livro = _context.Livros.Find(id);
            if (livro == null)
            {
                return NotFound();
            }

            livro.Emprestado = payload.Emprestado;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetLivroPorId(int id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
            if (livro == null)
            {
                return NotFound();
            }
            return Ok(livro);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarLivro(int id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
            if (livro == null)
            {
                return NotFound("Livro não encontrado.");
            }

            _context.Livros.Remove(livro);
            _context.SaveChanges();
            return NoContent(); // Retorna 204
        }

        public class AtualizarStatusPayload
        {
            public bool Emprestado { get; set; }
        }
    }
}