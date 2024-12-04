using EmprestimosAPI.Data;
using EmprestimosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EmprestimosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprestimosController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public EmprestimosController(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEmprestimos([FromQuery] int usuarioId)
        {
            var emprestimos = _context.Emprestimos
                                      .Where(e => e.UsuarioId == usuarioId)
                                      .ToList();

            if (!emprestimos.Any())
            {
                return NotFound("Nenhum empréstimo encontrado para o usuário.");
            }

            return Ok(emprestimos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEmprestimo([FromBody] Emprestimo emprestimo)
        {
            // Verifica se o livro está disponível
            var livroUrl = $"https://localhost:7206/api/livros/{emprestimo.LivroId}";
            var response = await _httpClient.GetAsync(livroUrl);
           
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("O livro não foi encontrado.");
            }

            var livroJson = await response.Content.ReadAsStringAsync();
            var livro = JsonSerializer.Deserialize<Livro>(livroJson);
            if (livro.Emprestado)
            {
                return BadRequest("O livro já está emprestado.");
            }

            // Verifica se o usuário existe
            var usuarioUrl = $"https://localhost:7131/api/usuarios/{emprestimo.UsuarioId}";
            var responseUsuario = await _httpClient.GetAsync(usuarioUrl);
            if (!responseUsuario.IsSuccessStatusCode)
            {
                return BadRequest("O usuário não foi encontrado.");
            }

            // Atualiza o status do livro para "emprestado"
            var livroUpdateUrl = $"https://localhost:7206/api/livros/{emprestimo.LivroId}";


            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return BadRequest($"Erro ao buscar o livro: {response.StatusCode} - {errorDetails}");
            }

            var payload = JsonSerializer.Serialize(new { emprestado = true });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var responseLivroUpdate = await _httpClient.PatchAsync(livroUpdateUrl, content);
            if (!responseLivroUpdate.IsSuccessStatusCode)
            {
                return BadRequest("Falha ao atualizar o status do livro.");
            }

            // Registra o empréstimo
            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();
            return Ok("Empréstimo registrado com sucesso!");
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarEmprestimo(int id)
        {
            var emprestimo = _context.Emprestimos.FirstOrDefault(e => e.Id == id);
            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            _context.Emprestimos.Remove(emprestimo);
            _context.SaveChanges();
            return NoContent(); // Retorna 204
        }

        public class Livro
        {
            public int Id { get; set; }
            public string Titulo { get; set; }

            public string Autor {  get; set; }
            public bool Emprestado { get; set; }
        }
    }
}
