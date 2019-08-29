using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Vendas;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            var cadastro = new ClienteCore(cliente).CadastroCliente();
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("buscaPorId")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            var exibe = new ClienteCore().ExibirClienteId(id);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPorData")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDate([FromQuery] string dataCadastro)
        {
            var exibe = new ClienteCore().ExibirClienteDataCadastro(dataCadastro);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpGet("buscaPaginada")]
        [ProducesResponseType(200, Type = typeof(List<Cliente>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int sizePage)
        {
            var exibe = new ClienteCore().ExibirTodos(page, sizePage);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }

        [HttpDelete("deletePorId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var deleta = new ClienteCore().DeletarClienteId(id);

            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }

        [HttpPut("atualizaPorId")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put([FromQuery] int id, [FromBody] Cliente cliente)
        {
            var atualiza = new ClienteCore().AtualizarId(cliente, id);
            if (atualiza.Status)
                return Ok(atualiza.Resultado);
            return BadRequest(atualiza.Resultado);
        }
    }
}