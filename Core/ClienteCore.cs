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
            _cliente = cliente;

            RuleFor(q => q.Id)               
                .NotNull()
                .WithMessage("Id inválido");
            RuleFor(e => e.Nome)
                .MinimumLength(3)
                .NotNull()
                .WithMessage("O nome deve ser preenchido e deve ter o mínimo de 3 caracteres");
        }
        
        public ClienteCore() { }

        public Retorno CadastroCliente()
        {
            var results = Validate(_cliente);
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();

            if (db.sistema.Clientes.Exists(x => x.Id == _cliente.Id))
                return new Retorno() { Status = true, Resultado = "Id já cadastrado" };

            db.sistema.Clientes.Add(_cliente);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _cliente };
        }

        public Retorno ExibirClienteId(int id)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var resultado = arquivo.sistema.Clientes.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirClienteDataCadastro(string dataCadastro)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var resultado = arquivo.sistema.Clientes.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirTodos(int page, int sizePage)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            Base classeBase = new Base();

            List<Cliente> thirdPage = classeBase.GetPage(arquivo.sistema.Clientes, page, sizePage);

            return new Retorno() { Status = true, Resultado = thirdPage };
        }

        public Retorno DeletarClienteId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var resultado = arquivo.sistema.Clientes.Remove(arquivo.sistema.Clientes.Find(s => s.Id == id));

            file.ManipulacaoDeArquivos(false, arquivo.sistema);
            return new Retorno() { Status = true, Resultado = null };
        }

        public Retorno AtualizarId(Cliente novo, int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var velho = arquivo.sistema.Clientes.Find(s => s.Id == id);
            var troca = TrocaDados(novo, velho);

            arquivo.sistema.Clientes.Add(troca);
            arquivo.sistema.Clientes.Remove(velho);

            file.ManipulacaoDeArquivos(false, arquivo.sistema);

            return new Retorno() { Status = true, Resultado = troca };
        }

        public Cliente TrocaDados(Cliente novo, Cliente velho)
        {
            velho.Nome = novo.Nome;
            novo.DataCadastro = velho.DataCadastro;
            novo.Id = velho.Id;
            return novo;
        }
    }
}