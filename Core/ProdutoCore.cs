using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System;
using Vendas;
namespace Core
{
    public class ProdutoCore : AbstractValidator<Produto>
    {
        private Produto _produto { get; set; }
        public ProdutoCore(Produto produto)
        {
            _produto = produto;

            RuleFor(e => e.id_Produto)
                .NotNull()
                .WithMessage("Id de produto inválido");

            RuleFor(e => e.valor_Produto)
                .NotNull()
                .WithMessage("O valor do produto não pode ser nulo!");
        }

        public ProdutoCore() { }

        public Retorno CadastroProduto()
        {

            var results = Validate(_produto);

            // Se o modelo é inválido, retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            // Caso o modelo seja válido, escreve no arquivo db
            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();

            if (db.sistema.Produtos.Exists(x => x.id_Produto == _produto.id_Produto))
            {

                return new Retorno() { Status = true, Resultado = "Id de produto já cadastrado" };
            }

            db.sistema.Produtos.Add(_produto);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _produto };
        }

        public Retorno ExibirProdutoId(int id)
        {

            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Produtos.Where(x => x.id_Produto == id);
            return new Retorno() { Status = true, Resultado = p };

        }

        public Retorno ExibirTodosProdutos()
        {
            var y = file.ManipulacaoDeArquivos(true, null);

            if (y.sistema == null)
                y.sistema = new Sistema();

            var q = y.sistema.Produtos;
            return new Retorno() { Status = true, Resultado = q };
        }

        public Retorno DeletarProdutoId(int id)
        {
            var t = file.ManipulacaoDeArquivos(true, null);

            if (t.sistema == null)
                t.sistema = new Sistema();

            var p = t.sistema.Produtos.Remove(t.sistema.Produtos.Find(s => s.id_Produto == id));

            file.ManipulacaoDeArquivos(false, t.sistema);

            return new Retorno() { Status = true, Resultado = null };
        }

        public Retorno AtualizarProdutoId(Produto novo, int id)
        {
            var f = file.ManipulacaoDeArquivos(true, null);

            if (f.sistema == null)
                f.sistema = new Sistema();

            var velho = f.sistema.Produtos.Find(s => s.id_Produto == id);
            var troca = TrocaDadosProduto(novo, velho);

            f.sistema.Produtos.Add(troca);
            f.sistema.Produtos.Remove(velho);

            file.ManipulacaoDeArquivos(false, f.sistema);

            return new Retorno() { Status = true, Resultado = troca };
        }

        public Produto TrocaDadosProduto(Produto novo, Produto velho)
        {
            if (velho.id_Produto == 0) novo.id_Produto = velho.id_Produto;
            if (velho.valor_Produto == 0) novo.valor_Produto = velho.valor_Produto;
            if (velho.nome_Produto == null) novo.nome_Produto = velho.nome_Produto;
            return novo;
        }
    }
}
