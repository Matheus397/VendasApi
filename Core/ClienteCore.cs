using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System;
using Vendas;

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

            // Se o modelo é inválido, retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            // Caso o modelo seja válido, escreve no arquivo db
            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();

            if (db.sistema.Clientes.Exists(x => x.Id == _cliente.Id))
            {

                return new Retorno() { Status = true, Resultado = "CPF já cadastrado" };
            }
            db.sistema.Clientes.Add(_cliente);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _cliente };
        }

        public Retorno ExibirClienteId(int id)
        {

            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Clientes.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = p };

        }

        public Retorno ExibirTodos()
        {
            var y = file.ManipulacaoDeArquivos(true, null);

            if (y.sistema == null)
                y.sistema = new Sistema();

            var q = y.sistema.Clientes;
            return new Retorno() { Status = true, Resultado = q };
        }

        public Retorno DeletarClienteId(int id)
        {
            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Clientes.Remove(t.sistema.Clientes.Find(s => s.Id == id));

            file.ManipulacaoDeArquivos(false, t.sistema);

            return new Retorno() { Status = true, Resultado = null };
        }

        public Retorno AtualizarId(Cliente novo, int id)
        {
            var f = file.ManipulacaoDeArquivos(true, null);

            if (f.sistema == null)
                f.sistema = new Sistema();

            var velho = f.sistema.Clientes.Find(s => s.Id == id);
            var troca = TrocaDados(novo, velho);

            f.sistema.Clientes.Add(troca);
            f.sistema.Clientes.Remove(velho);

            file.ManipulacaoDeArquivos(false, f.sistema);

            return new Retorno() { Status = true, Resultado = troca };
        }

        public Cliente TrocaDados(Cliente novo, Cliente velho)
        {
            if (velho.Nome == null) novo.Nome = velho.Nome;
            if (velho.Id == 0) novo.Id = velho.Id;                   
            return novo;
        }
    }
}
