using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using Vendas;
using System.Collections.Generic;

namespace Core
{
    public class ClienteCore : AbstractValidator<Cliente>
    {

        private Cliente _cliente { get; set; }

        public ClienteCore(Cliente cliente)
        {
            //Regras para se cadastrar o Cliente
            _cliente = cliente;

            RuleFor(q => q.Id)
                .NotNull()
                .WithMessage("Id inválido");
            RuleFor(e => e.Nome)
                .MinimumLength(3)
                .NotNull()
                .WithMessage("O nome deve ser preenchido e deve ter o mínimo de 3 caracteres");
        }

        //Construtor vazio
        public ClienteCore() { }

        //método para cadastrar cliente
        public Retorno CadastroCliente()
        {
            try
            {
                //validando o cliente sendo inserido com fluent validation
                var results = Validate(_cliente);
                //caso o cliente não seja válido retorno o resultado dos erros
                if (!results.IsValid)
                    return new Retorno { Status = false, Resultado = results.Errors };
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //caso na minha base de dados já tenha um cliente com aquele mesmo Id eu retorno uma mensagem
                if (arquivo.sistema.Clientes.Exists(x => x.Id == _cliente.Id))
                    return new Retorno { Status = false, Resultado = "Cliente já cadastrado" };
                //adicionando enfim o cliente ja validado à lista 
                arquivo.sistema.Clientes.Add(_cliente);
                //salvando alterações na base de dados 
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = _cliente };
            }
            catch { return new Retorno { Status = false, Resultado = "Ocorreu um Erro ao cadastrar o Cliente" }; }
        }

        public Retorno ExibirClienteId(int id)
        {
            try
            {
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //caso o arquivo do sistema esteja vazio, crio uma nova instancia para ele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //resultado filtra o cliente pelo Id e armazena ana var resultado  
                return new Retorno { Status = true, Resultado = arquivo.sistema.Clientes.Where(x => x.Id == id) };
            }
            catch { return new Retorno { Status = false, Resultado = "Ocorreu um Erro ao exibir o Cliente" }; }
        }

        public Retorno ExibirClienteDataCadastro(string dataCadastro)
        {
            try
            {
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando o resultado através da data de cadastro 
                return new Retorno { Status = true, Resultado = arquivo.sistema.Clientes.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro)) };
            }
            catch { return new Retorno { Status = false, Resultado = "Ocorreu um Erro ao exibir o Cliente" }; }
        }

        public Retorno ExibirTodos(int page, int sizePage)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            try
            {
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //instancia da classe base que armazena o metodo genérico de paginação das minhas listas
                Base classeBase = new Base();
               //verificando se o usuário passou páginas ou registros menores que um a serem vistos
                if (sizePage < 1 || page < 1)
                    return new Retorno { Status = false, Resultado = "Paginação Inválida!" };
                if (page > arquivo.sistema.Clientes.Count)
                    return new Retorno { Status = false, Resultado = "Não há Clientes nesta página até então" };
                //passando minha lista de clientes e seu formato de páginas para o meu método de paginação em base
                List<Cliente> thirdPage = classeBase.GetPage(arquivo.sistema.Clientes, page, sizePage);
                return new Retorno { Status = true, Resultado = thirdPage };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na Exibição" }; }
        }

        public Retorno DeletarClienteId(int id)
        {
            try
            {
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //removendo meu cliente usnado expressao lambda
                arquivo.sistema.Clientes.Remove(arquivo.sistema.Clientes.Find(s => s.Id == id));
                //salvando a deleção feita
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = null };
            }
            catch { return new Retorno { Status = false, Resultado = "Ocorreu um erro ao deletar o Cliente" }; }
        }

        public Retorno AtualizarId(Cliente novoCli, int id)
        {
            try
            {
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //armazenando o cliente com dados desatualizados em uma var velho 
                var velhoCli = arquivo.sistema.Clientes.Find(s => s.Id == id);
                //troca recebe o resultado do metodo TrocaDados que por sua vez retorna o cliente atualizado
                var troca = TrocaDados(novoCli, velhoCli);
                var tdsClientes = arquivo.sistema.Clientes;
                //retirando da lista o cli com dados desatualizados e trocando pelo novo cliente 
                tdsClientes.Add(troca);
                tdsClientes.Remove(velhoCli);
                //salvando as alterações
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = troca };
            }
            catch { return new Retorno { Status = false, Resultado = "Ocorreu um Erro ao atualizar o Cliente" }; }
        }
        //método de atualização dos dados do cliente
        public Cliente TrocaDados(Cliente novo, Cliente velho)
        {
            //aqui pego os dados passados do usuario e os atribuo no cli antigo como atualização
            velho.Nome = novo.Nome;
            novo.DataCadastro = velho.DataCadastro;
            novo.Id = velho.Id;
            return novo;
        }
    }
}