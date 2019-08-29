using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Produto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("buscaPorId")]
        [ProducesResponseType(200, Type = typeof(Produto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            var exibe = new ProdutoCore().ExibirProdutoId(id);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPorData")]
        [ProducesResponseType(200, Type = typeof(Produto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDate([FromQuery] string dataCadastro)
        {
            var exibe = new ProdutoCore().ExibirProdutoDataCadastro(dataCadastro);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPaginada")]
        [ProducesResponseType(200, Type = typeof(List<Produto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int sizePage)
        {
            var exibe = new ProdutoCore().ExibirTodosProdutos(page, sizePage);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpDelete("deletePorId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var deleta = new ProdutoCore().DeletarProdutoId(id);
            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }

        [HttpPut("atualizaPorId")]
        [ProducesResponseType(200, Type = typeof(Produto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put([FromQuery] int id, [FromBody] Produto pauta)
        {
            var atualiza = new ProdutoCore().AtualizarProdutoId(pauta, id);
            if (atualiza.Status)
                return Ok(atualiza.Resultado);
            return BadRequest(atualiza.Resultado);
        }
    }
}
