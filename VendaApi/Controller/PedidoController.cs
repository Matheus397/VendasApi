//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Core;
//using Microsoft.AspNetCore.Mvc;
//using Model;

//namespace ApiProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VotoController : ControllerBase
//    {
//        [HttpPost]
//        [ProducesResponseType(201, Type = typeof(Voto))]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        public async Task<IActionResult> Post([FromBody] Voto voto)
//        {
//            var cadastro = new VotoCore(voto).CadastroVoto();
//            if (cadastro.Status)
//                return Created("https://localhost", cadastro.Resultado);
//            return BadRequest(cadastro.Resultado);
//        }

//        [HttpGet]
//        [ProducesResponseType(200, Type = typeof(List<Voto>))]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        public async Task<IActionResult> GetAll()
//        {
//            var exibe = new VotoCore().ExibirTodosVotos();
//            if (exibe.Status)
//                return Ok(exibe.Resultado);
//            return BadRequest(exibe.Resultado);
//        }

//        [HttpGet("{id}")]
//        [ProducesResponseType(200, Type = typeof(Voto))]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        public async Task<IActionResult> Get(string id)
//        {
//            var exibe = new VotoCore().ExibirVotoId(id);
//            if (exibe.Status)
//                return Ok(exibe.Resultado);
//            return BadRequest(exibe.Resultado);
//        }
//    }
//}