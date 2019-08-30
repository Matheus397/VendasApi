using Model;
using FluentValidation;
using Core.util;
using System.Linq;
using System.Collections.Generic;

namespace Core
{
    public class ProdutoCore : AbstractValidator<Produto>
    {
        private Produto _produto { get; set; }

        public ProdutoCore(Produto produto)
        {
            _produto = produto;

            RuleFor(e => e.Id)               
                .NotNull()
                .WithMessage("Id inválida");
            RuleFor(e => e.nome_Produto)
               .Length(3, 25)
               .NotNull()
               .WithMessage("Nome inválida");
            RuleFor(e => e.valor_Produto)              
               .NotNull()
               .WithMessage("Preço inválido");
        }

        public ProdutoCore() { }

        public Retorno CadastroProduto()
        {

            var results = Validate(_produto);
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();

            if (db.sistema.Produtos.Exists(x => x.Id == _produto.Id))
            {
                return new Retorno() { Status = true, Resultado = "Produto já cadastrado" };
            }
            db.sistema.Produtos.Add(_produto);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _produto };
        }

        public Retorno ExibirProdutoId(int id)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            var resultado = arquivo.sistema.Produtos.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = resultado };

        }

        public Retorno ExibirProdutoDataCadastro(string dataCadastro)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            var resultado = arquivo.sistema.Produtos.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirTodosProdutos(int page, int sizePage)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            Base classeBase = new Base();

            List<Produto> thirdPage = classeBase.GetPage(arquivo.sistema.Produtos, page, sizePage);

            return new Retorno() { Status = true, Resultado = thirdPage };
        }

        public Retorno DeletarProdutoId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            var resultado = arquivo.sistema.Produtos.Remove(arquivo.sistema.Produtos.Find(s => s.Id == id));

            file.ManipulacaoDeArquivos(false, arquivo.sistema);
            return new Retorno() { Status = true, Resultado = "Produto Deletada!" };
        }

        public Retorno AtualizarProdutoId(Produto nova, int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();

            var velha = arquivo.sistema.Produtos.Find(s => s.Id == id);
            var troca = TrocaProduto(nova, velha);

            arquivo.sistema.Produtos.Add(troca);
            arquivo.sistema.Produtos.Remove(velha);

            file.ManipulacaoDeArquivos(false, arquivo.sistema);

            return new Retorno() { Status = true, Resultado = $"{troca}\n\nProduto Atualizado!" };
        }

        public Produto TrocaProduto(Produto nova, Produto velha)
        {
            velha.nome_Produto = nova.nome_Produto;       
            velha.valor_Produto = nova.valor_Produto;
            nova.DataCadastro = velha.DataCadastro;
            nova.Id = velha.Id;
            return nova;
        }
    }
}
