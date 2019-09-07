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
        //post usando frombody e passadno o objeto cliente
        [ProducesResponseType(201, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            var cadastro = new ClienteCore(cliente).CadastroCliente();
            //aqui verifico o status do cadastro e retorno o meu Created com a Url de localhost com o resultado do cadastro
            if (cadastro.Status)
                return Created("https://localhost", cadastro.Resultado);
            //caso não tenha sucesso retorno o badrequest status
            return BadRequest(cadastro.Resultado);
        }
        //busca por id from Path
        [HttpGet("BuscaPorId/{id}")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(int id)
        {
            //uso minha variavel para armazenar o clientecore e puxo meu método de exibição passando o Id
            var exibe = new ClienteCore().ExibirClienteId(id);         
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }
        //busca por data fromquery
        [HttpGet("BuscaPorData")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDate([FromQuery] string dataCadastro)
        {
            
            var exibe = new ClienteCore().ExibirClienteDataCadastro(dataCadastro);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }
        //buscapaginada passando os inteiros que definiraro a config das paginas
        [HttpGet("BuscaPaginada")]
        [ProducesResponseType(200, Type = typeof(List<Cliente>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int sizePage)
        {
            var exibe = new ClienteCore().ExibirTodos(page, sizePage);
            if (exibe.Status)
                return Ok(exibe.Resultado);
            return BadRequest(exibe.Resultado);
        }
        //delete from Path
        [HttpDelete("DeleçãoPorId/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleta = new ClienteCore().DeletarClienteId(id);
            if (deleta.Status)
                return Ok(deleta.Resultado);
            return BadRequest(deleta.Resultado);
        }
        //atualizando via id fromquery e os dados da atualização frombody
        [HttpPut("AtualizaPorId/{id}")]
        [ProducesResponseType(200, Type = typeof(Cliente))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int id, [FromBody] Cliente cliente)
        {
            var atualiza = new ClienteCore().AtualizarId(cliente, id);
            if (atualiza.Status)
                return Ok(atualiza.Resultado);
            return BadRequest(atualiza.Resultado);
        }
    }
}