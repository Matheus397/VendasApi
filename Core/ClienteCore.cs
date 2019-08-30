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
            //validando o cliente sendo inserido com fluent validation
            var results = Validate(_cliente);
            //caso o cliente não seja válido retorno o resultado dos erros
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();
            //caso na minha base de dados já tenha um cliente com aquele mesmo Id eu retorno uma mensagem
            if (db.sistema.Clientes.Exists(x => x.Id == _cliente.Id))
                return new Retorno() { Status = false, Resultado = "Cliente já cadastrado" };
            //add o cliente na lista
            db.sistema.Clientes.Add(_cliente);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _cliente };
        }

        public Retorno ExibirClienteId(int id)
        {
            
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            //caso o arquivo do sistema esteja vazio, crio uma nova instancia para ele
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //resultado filtra o cliente pelo Id e armazena ana var resultado
            var resultado = arquivo.sistema.Clientes.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirClienteDataCadastro(string dataCadastro)
        {//var arquivo serve para receber o arquivo manipulado usando os métodos da biblioteca newtonsoft json
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //filtrando o resultado através da data de cadastro 
            var resultado = arquivo.sistema.Clientes.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirTodos(int page, int sizePage)
        {
            
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //instancia da classe base que armazena o metodo genérico de paginação das minhas listas
            Base classeBase = new Base();
            //passando minha lista de cliente para ser paginada em base
            List<Cliente> thirdPage = classeBase.GetPage(arquivo.sistema.Clientes, page, sizePage);

            return new Retorno() { Status = true, Resultado = thirdPage };
        }

        public Retorno DeletarClienteId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //removendo meu cliente usnado expressao lambda
            var resultado = arquivo.sistema.Clientes.Remove(arquivo.sistema.Clientes.Find(s => s.Id == id));
            //salvando a deleção feita
            file.ManipulacaoDeArquivos(false, arquivo.sistema);
            return new Retorno() { Status = true, Resultado = null };
        }

        public Retorno AtualizarId(Cliente novo, int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //armazenando o cliente com dados desatualizados em uma var velho 
            var velho = arquivo.sistema.Clientes.Find(s => s.Id == id);
            //troca recebe o resultado do metodo TrocaDados que por sua vez retorna o cliente atualizado
            var troca = TrocaDados(novo, velho);
            //retirando da lista o cli com dados desatualizados e trocando pelo novo
            arquivo.sistema.Clientes.Add(troca);
            arquivo.sistema.Clientes.Remove(velho);
            //salvando as alterações
            file.ManipulacaoDeArquivos(false, arquivo.sistema);

            return new Retorno() { Status = true, Resultado = troca };
        }
        //método de atualização dos dados do cliente
        public Cliente TrocaDados(Cliente novo, Cliente velho)
        {//aqui pego os dados passados do usuario e os atribuo no cli antigo como atualização
            velho.Nome = novo.Nome;
            novo.DataCadastro = velho.DataCadastro;
            novo.Id = velho.Id;
            return novo;
        }
    }
}