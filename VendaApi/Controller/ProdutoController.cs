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
        //post frombody
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
        //busca por id fromquery
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
        //busca por data fromquery
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
        //busca paginada
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
        //deleção por id
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
        //atualizaçoa frombody e escolhendo o que sera ataulizado por Id
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
