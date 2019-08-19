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
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            var cadastro = new PedidoCore(pedido).CadastroPedido();
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var exibe = new PedidoCore().ExibirPedidoId(id);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var exibe = new PedidoCore().ExibirTodosPedidos();
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleta = new PedidoCore().DeletarPedidoId(id);
            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Pedido pedido)
        {
            var atualiza = new PedidoCore().AtualizarPedidoId(pedido, id);
            if (atualiza.Status)
                return Ok(atualiza.Resultado);
            return BadRequest(atualiza.Resultado);
        }
    }
}