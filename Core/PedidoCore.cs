using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Core
{
    public class PedidoCore : AbstractValidator<Pedido>
    {

        //getters setters privados
        private Pedido _pedido { get; set; }
        public PedidoCore(Pedido pedido)
        {
            _pedido = pedido;
            RuleFor(e => e.Id)
                .NotNull()
                .WithMessage("Pauta Id inválido");

        }
        //construtor vazio
        public PedidoCore() { }

        public Retorno CadastroPedido()
        {
            try
            {
                var results = Validate(_pedido);
                if (!results.IsValid)
                    return new Retorno { Status = false, Resultado = results.Errors };

                var db = file.ManipulacaoDeArquivos(true, null);

                var pedidos = db.sistema.Pedidos;

                if (db.sistema == null)
                    db.sistema = new Sistema();

                if (pedidos.Exists(x => x.Id == _pedido.Id))           
                    return new Retorno() { Status = true, Resultado = "Já existe um pedido com esse Id" };
                
                var pedidoCli = db.sistema.Pedidos.Find(s => s.Id == _pedido.Id);

                var produtosDoPedido = pedidoCli.produtosPedido;               

                foreach (var prod in produtosDoPedido)
                    pedidoCli.total_Pedido += prod.valor_Produto;

                db.sistema.Pedidos.Add(pedidoCli);

                file.ManipulacaoDeArquivos(false, db.sistema);
                return new Retorno() { Status = true, Resultado = _pedido };
            }
            catch
            {
                return new Retorno() { Status = true, Resultado = "Erro no Pedido" };
            }
        }

        public Retorno ExibirTodosPedidos(int page, int sizePage)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            Base classeBase = new Base();
            List<Pedido> thirdPage = classeBase.GetPage(arquivo.sistema.Pedidos, page, sizePage);
            return new Retorno() { Status = true, Resultado = thirdPage };
        }

        public Retorno ExibirPedidoId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            var pedidos = arquivo.sistema.Pedidos.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = pedidos };
        }

        public Retorno ExibirPedidoDataCadastro(string dataCadastro)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var resultado = arquivo.sistema.Pedidos.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno DeletarPedidoId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var resultado = arquivo.sistema.Pedidos.Remove(arquivo.sistema.Pedidos.Find(s => s.Id == id));
            file.ManipulacaoDeArquivos(false, arquivo.sistema);
            return new Retorno() { Status = true, Resultado = null };
        }
    }
}
