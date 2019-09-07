using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System.Collections.Generic;
using Vendas;

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
        public PedidoCore(){}
        //Fu~ção de cadastro do Pedido, Core Principal
        public Retorno CadastroPedido()
        {
            //aqui eu tento realizar o cadastro do pedido
            try
            {
                var results = Validate(_pedido);
                if (!results.IsValid)
                    return new Retorno { Status = false, Resultado = results.Errors };
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //tdsPedidos recebe a lista de todos os pedidos na base de dados
                var tdsPedidos = arquivo.sistema.Pedidos;
                //clienteDoPedido é o cliente inserido neste pedido e que efetuará o mesmo
                var clienteDoPedido = _pedido.cliente_pedido;
                //var que recebe a lista de produtos daquele especifico pedido
                var produtosDoPedido = _pedido.produtosPedido;
                //todos os produtos da base de dados em tdsProdutos
                var tdsProdutos = arquivo.sistema.Produtos;
                //todos os clientes
                var tdsClientes = arquivo.sistema.Clientes;
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //validando com lamda se já existe na base de dados um pedido com o Id od que está sendo registrado
                if (tdsPedidos.Exists(x => x.Id == _pedido.Id))
                    return new Retorno() { Status = false, Resultado = "Já existe um pedido com esse Id" };
                //validando se o cli que esta fazendo pedido existe assim como o produto
                var cliDoPedidoNaBase = tdsClientes.Find(q => q.Id.Equals(clienteDoPedido.Id));
                //caso o cliente que esta fazendo o pedido ou um dos produtos do pedido não existam na base de dados retorno uma mensagem
                if (!tdsClientes.Exists(q => q.Id.Equals(clienteDoPedido.Id) && !tdsProdutos.Exists(c => c.Id.Equals(produtosDoPedido.Select(w => w.Id).ToString()))))
                    return new Retorno { Status = false, Resultado = "Produto/Cliente inválidos" };
                //caso o usuário esteja comprando uma quantidade menor que 1 (de algum produto), ou esteja comprando mais do há no estoque retorno uma mensagem!
                for (int i = 0; i < produtosDoPedido.Count; i++)
                    if (produtosDoPedido[i].quantidade < 1 || produtosDoPedido[i].quantidade > tdsProdutos.Find(w => w.Id == produtosDoPedido[i].Id).quantidade)
                        return new Retorno { Status = false, Resultado = "A quantidade de algum produto do pedido excede o estoque ou é inválida!" };
                //aqui eu puxo meus métodos de preenchimento, e passo para eles as listas de clientes e produtos tanto do pedido quanto da base de dados
                PreencheProdutos(produtosDoPedido, tdsProdutos);
                PreencheClientes(clienteDoPedido, cliDoPedidoNaBase);
                //atualizando o estoque dos produtos comprados no pedido, fazendo o decréscimo na quantidade
                produtosDoPedido.ForEach(d => tdsProdutos.FirstOrDefault(c => c.Id == d.Id).quantidade -= d.quantidade);
                //através do lambda também faço o cálculo do saldo do pedido, multiplicando o preço de cada produto comprado por sua respectiva quantidade inserida
                produtosDoPedido.ForEach(d => _pedido.total_Pedido += d.valor_Produto * d.quantidade);
                //caso o valor ultrapasse 99 reais há um desconto de 5% e caso ultrasse 299 o desconto é de 10% 
                if (_pedido.total_Pedido > 99 && _pedido.total_Pedido < 299)
                    _pedido.total_Pedido = _pedido.total_Pedido * 0.95;
                else if (_pedido.total_Pedido > 299)
                    _pedido.total_Pedido = _pedido.total_Pedido * 0.90;
                //adicionando o pedido na base de dados
                tdsPedidos.Add(_pedido);
                //salvando o pedido adicionado à lista
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = _pedido };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro no Pedido" }; }
        }
        //este método é privado para não ser instanciado fora da classe, os atributos do cliente na base de dados são passados para o cli de mesmo Id que está no pedido
        private object PreencheClientes(Cliente clienteDoPedido, Cliente cli)
        {
            //pegando os atributos do cliente na base que tem o Id igual ao do pedindo e os atribuindo
            clienteDoPedido.Nome = cli.Nome;
            clienteDoPedido.DataCadastro = cli.DataCadastro;
            clienteDoPedido.Id = cli.Id; 
            //retornando o cli apreenchido
            return clienteDoPedido;
        }
        //método privado para não ser instanciado em outra classe, os atributos de cada produto na base de dados que sejam de mesmo Id aos produtosa do pedido sao preenchidos com eles
        private List<Produto> PreencheProdutos(List<Produto> produtosDoPedido, List<Produto> tdsProdutos)
        {
            //aqui eu percorro os produtos do meu pedido e resgato seus atributos na base de dados, fazendo a atribuição
            for (int i = 0; i < produtosDoPedido.Count; i++)
            {
                var produtoDaBaseMesmoId = tdsProdutos.Find(w => w.Id == produtosDoPedido[i].Id);              
                produtosDoPedido[i].nome_Produto = produtoDaBaseMesmoId.nome_Produto;
                produtosDoPedido[i].valor_Produto = produtoDaBaseMesmoId.valor_Produto;        
            }  
            //retornando a lista de produtos já preenchida
            return produtosDoPedido;
        }      
        //função parar exibir pedidos através de páginas
        public Retorno ExibirTodosPedidos(int page, int sizePage)
        {
            //exibindo todos os pedidos de maneira paginada
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            try
            {   
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //instancia da classe base para posteriormente acessar a paginação
                Base classeBase = new Base();
                //verificando se o usuário passou páginas ou registros menores que um a serem vistos
                if (sizePage < 1 || page < 1)
                    return new Retorno { Status = false, Resultado = "Paginação Inválida!" };
                if (page > arquivo.sistema.Pedidos.Count)
                    return new Retorno { Status = false, Resultado = "Não há Pedidos nesta página até então" };
                //passando minha lista de clientes e seu formato de páginas para o meu método de paginação em base
                List<Pedido> thirdPage = classeBase.GetPage(arquivo.sistema.Pedidos, page, sizePage);
                return new Retorno { Status = true, Resultado = thirdPage };
            }
            catch { return new Retorno { Status = true, Resultado = "Erro na Exibição" }; }
        }

        public Retorno ExibirPedidoId(int id)
        {
            try
            {
                //exibição de pedidos por Id
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se o sistema está null, caso esteja criou uma nova instancia para ele
                var sistemaNull = arquivo.sistema ?? new Sistema();
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
                //verificando se o sistema está null, caso esteja criou uma nova instancia para ele
                var sistemaNull = arquivo.sistema ?? new Sistema();
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
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando o pedido que ser deletado com lambda e o removendo da base de dados
                arquivo.sistema.Pedidos.Remove(arquivo.sistema.Pedidos.Find(s => s.Id == id));
                //salvando as alterações na base
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = null };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na deleção do Pedido" }; }
        }
    }
}
