using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System;

namespace Core
{
    public class PedidoCore : AbstractValidator<Pedido>
    {
        private Pedido _pedido { get; set; }
        public PedidoCore(Pedido pedido)
        {
            _pedido = pedido;

            RuleFor(e => e.id_Pedido)               
                .NotNull()
                .WithMessage("Id de pedido inválido");

            RuleFor(e => e.total_Pedido)            
                .NotNull()
                .WithMessage("O total do pedido não pode ser nulo!");
        }

        public PedidoCore() { }

        public Retorno CadastroPedido()
        {

            var results = Validate(_pedido);

            // Se o modelo é inválido, retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            // Caso o modelo seja válido, escreve no arquivo db
            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();

            if (db.sistema.Pedidos.Exists(x => x.id_Pedido == _pedido.id_Pedido))
            {

                return new Retorno() { Status = true, Resultado = "Id de pedido já cadastrado" };
            }
            db.sistema.Pedidos.Add(_pedido);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _pedido };
        }

        public Retorno ExibirPedidoId(int id)
        {

            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Pedidos.Where(x => x.id_Pedido == id);
            return new Retorno() { Status = true, Resultado = p };

        }

        public Retorno ExibirTodosPedidos()
        {
            var y = file.ManipulacaoDeArquivos(true, null);

            if (y.sistema == null)
                y.sistema = new Sistema();

            var q = y.sistema.Pedidos;
            return new Retorno() { Status = true, Resultado = q };
        }

        public Retorno DeletarPedidoId(int id)
        {
            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Pedidos.Remove(t.sistema.Pedidos.Find(s => s.id_Pedido == id));

            file.ManipulacaoDeArquivos(false, t.sistema);

            return new Retorno() { Status = true, Resultado = null };
        }

        public Retorno AtualizarPedidoId(Pedido novo, int id)
        {
            var f = file.ManipulacaoDeArquivos(true, null);

            if (f.sistema == null)
                f.sistema = new Sistema();

            var velho = f.sistema.Pedidos.Find(s => s.id_Pedido == id);
            var troca = TrocaDadosPedido(novo, velho);

            f.sistema.Pedidos.Add(troca);
            f.sistema.Pedidos.Remove(velho);

            file.ManipulacaoDeArquivos(false, f.sistema);

            return new Retorno() { Status = true, Resultado = troca };
        }

        public Pedido TrocaDadosPedido(Pedido novo, Pedido velho)
        {
            if (velho.id_Pedido == 0) novo.id_Pedido = velho.id_Pedido;
            if (velho.total_Pedido == 0) novo.total_Pedido = velho.total_Pedido;                    
            return novo;
        }
    }
}
