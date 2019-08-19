using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;
using Vendas;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var exibe = new ProdutoCore().ExibirProdutoId(id);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var exibe = new ProdutoCore().ExibirTodosProdutos();
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleta = new ProdutoCore().DeletarProdutoId(id);
            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Produto produto)
        {
            var atualiza = new ProdutoCore().AtualizarProdutoId(produto, id);
            if (atualiza.Status)
                return Ok(atualiza.Resultado);
            return BadRequest(atualiza.Resultado);
        }
    }
}