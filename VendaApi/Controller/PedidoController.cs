using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;
using Vendas;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        [HttpPost]
        //post usando frombody e passadno o objeto
        [ProducesResponseType(201, Type = typeof(Pedido))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            var cadastro = new PedidoCore(pedido).CadastroPedido();
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("buscaPorId")]
        //busca por id fromquery
        [ProducesResponseType(200, Type = typeof(Pedido))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            var exibe = new PedidoCore().ExibirPedidoId(id);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPorData")]
        //busca por data fromquery
        [ProducesResponseType(200, Type = typeof(Pedido))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDate([FromQuery] string dataCadastro)
        {
            var exibe = new PedidoCore().ExibirPedidoDataCadastro(dataCadastro);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPaginada")]
        //buscapaginada passando os inteiros que definiraro a config das paginas
        [ProducesResponseType(200, Type = typeof(List<Pedido>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int sizePage)
        {
            var exibe = new PedidoCore().ExibirTodosPedidos(page, sizePage);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpDelete("deletePorId")]
        //delete fromquery
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var deleta = new PedidoCore().DeletarPedidoId(id);

            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }
    }
}