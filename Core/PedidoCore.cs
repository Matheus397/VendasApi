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
            //regras para realização do pedido com RuleFor
            _pedido = pedido;
            RuleFor(e => e.Id)
                .NotNull()
                .WithMessage("Pauta Id inválido");

        }
        //construtor vazio
        public PedidoCore() { }

        public Retorno CadastroPedido()
        {//aqui eu tento realizar o cadastro do pedido
            try
            {
                var results = Validate(_pedido);
                if (!results.IsValid)
                    return new Retorno { Status = false, Resultado = results.Errors };
                var db = file.ManipulacaoDeArquivos(true, null);
                //tdsPedidos recebe a lista de todos os pedidos na base de dados
                var tdsPedidos = db.sistema.Pedidos;
                //clienteDoPedido é o cliente inserido neste pedido e que efetuará o mesmo
                var clienteDoPedido = _pedido.cliente_pedido;
                //var que recebe a lista de produtos daquele especifico pedido
                var produtosDoPedido = _pedido.produtosPedido;
                //todos os produtos da base de dados em tdsProdutos
                var tdsProdutos = db.sistema.Produtos;
                //todos os clientes
                var tdsClientes = db.sistema.Clientes;
                if (db.sistema == null)
                    db.sistema = new Sistema();
                //validando com lamda se já existe na base de dados um pedido com o Id od que está sendo registrado
                if (tdsPedidos.Exists(x => x.Id == _pedido.Id))           
                    return new Retorno() { Status = false, Resultado = "Já existe um pedido com esse Id" };
                //validando se o cli que esta fazendo pedido existe assim como o produto
                if (!tdsClientes.Exists(q => q.Id.Equals(clienteDoPedido.Id) && !tdsProdutos.Exists(c => c.Id.Equals(produtosDoPedido.Select(w => w.Id).ToString()))))
                    return new Retorno() { Status = false, Resultado = "Produto/Cliente inválidos" };
                //para cada produto na lista de produtos daquele pedido farei o acumulo de valores dos produtos em total_pedido
                foreach (var prod in produtosDoPedido)
                    _pedido.total_Pedido += prod.valor_Produto;
                //caso o valor ultrapasse 99 reais há um desconto de 5% e caso ultrasse 299 o desconto é de 10% 
                if (_pedido.total_Pedido > 99 && _pedido.total_Pedido < 299)
                    _pedido.total_Pedido = _pedido.total_Pedido * 0.95;
                else if (_pedido.total_Pedido > 299)
                    _pedido.total_Pedido = _pedido.total_Pedido * 0.90;
                //adicionando o pedido na base de dados
                tdsPedidos.Add(_pedido);
                file.ManipulacaoDeArquivos(false, db.sistema);
                return new Retorno { Status = true, Resultado = _pedido };
            }
            catch
            {
                //por segurança alerto um erro no pedido caso algo dê errado
                return new Retorno { Status = false, Resultado = "Erro no Pedido" };
            }
        }

        public Retorno ExibirTodosPedidos(int page, int sizePage)
        {
            try
            {
                //exibindo todos os pedidos de maneira paginada
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                if (arquivo.sistema == null)
                    arquivo.sistema = new Sistema();
                //instancia da classe base
                Base classeBase = new Base();
                //usando o metodo generico de base para paginar pedidos
                List<Pedido> thirdPage = classeBase.GetPage(arquivo.sistema.Pedidos, page, sizePage);
                return new Retorno { Status = true, Resultado = thirdPage };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na exibição Pedido" }; }
        }

        public Retorno ExibirPedidoId(int id)
        {
            try
            {
                //exibição de pedidos por Id
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                if (arquivo.sistema == null)
                    arquivo.sistema = new Sistema();
                return new Retorno { Status = true, Resultado = arquivo.sistema.Pedidos.Where(x => x.Id == id) };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro no Pedido" }; }
        }

        public Retorno ExibirPedidoDataCadastro(string dataCadastro)
        {
            try
            {
                //exibição de pedidos por data de cadastro
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                if (arquivo.sistema == null)
                    arquivo.sistema = new Sistema();
                return new Retorno { Status = true, Resultado = arquivo.sistema.Pedidos.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro)) };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro no Pedido" }; }
        }

        public Retorno DeletarPedidoId(int id)
        {
            try
            {
                //deleção de pedidos por id 
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                if (arquivo.sistema == null)
                    arquivo.sistema = new Sistema();
                //filtrando o pedido que ser deletado com lambda
                arquivo.sistema.Pedidos.Remove(arquivo.sistema.Pedidos.Find(s => s.Id == id));
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = null };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na deleção do Pedido" }; }
        }
    }
}
