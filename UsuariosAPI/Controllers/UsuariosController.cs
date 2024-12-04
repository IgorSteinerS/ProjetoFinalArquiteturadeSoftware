using UsuariosAPI.Data;
using UsuariosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public UsuariosController(AppDbContext context,HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }

        [HttpPost]
        public IActionResult AddUsuario([FromBody] Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioPorId(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // URL da API de Empréstimos
            var emprestimosUrl = $"https://localhost:7280/api/emprestimos?usuarioId={id}";
            var emprestimosResponse = await _httpClient.GetAsync(emprestimosUrl);

            if (!emprestimosResponse.IsSuccessStatusCode)
            {
                return Ok(new { Usuario = usuario, Emprestimo = new EmprestimoStatus { PossuiEmprestimo = false } });
            }

            var emprestimosJson = await emprestimosResponse.Content.ReadAsStringAsync();
            var emprestimos = JsonSerializer.Deserialize<List<Emprestimo>>(emprestimosJson);

            if (emprestimos == null || !emprestimos.Any())
            {
                return Ok(new { Usuario = usuario, Emprestimo = new EmprestimoStatus { PossuiEmprestimo = false } });
            }

            Livro livroEmprestado = null;
            var livroId = emprestimos.First().LivroId;
            var livroUrl = $"https://localhost:7206/api/livros/{livroId}";
            var livroResponse = await _httpClient.GetAsync(livroUrl);

            if (livroResponse.IsSuccessStatusCode)
            {
                var livroJson = await livroResponse.Content.ReadAsStringAsync();
                
                livroEmprestado = JsonSerializer.Deserialize<Livro>(livroJson);
            }

            return Ok(new
            {
                Usuario = usuario,
                Emprestimo = new EmprestimoStatus
                {
                    PossuiEmprestimo = true,
                    Livro = livroEmprestado
                }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarUsuario(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
            return NoContent(); // Retorna 204
        }

        public class Emprestimo
        {
            public int Id { get; set; }
            public int LivroId { get; set; }
            public int UsuarioId { get; set; }
            public DateTime DataEmprestimo { get; set; }
            public DateTime? DataDevolucao { get; set; }
        }

        public class Livro
        {
            public int Id { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public bool Emprestado { get; set; }
        }

        public class EmprestimoStatus
        {
            public bool PossuiEmprestimo { get; set; }
            public Livro Livro { get; set; } // Será `null` caso não haja empréstimo
        }
    }
}

