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
            //regras para cadastrar produto com RuleFor
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
            //validadno formato do produto passado pelo usuário
            var results = Validate(_produto);
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            var db = file.ManipulacaoDeArquivos(true, null);

            if (db.sistema == null)
                db.sistema = new Sistema();
            //filtragem para ver se já há um produto na base de dados com o Id fornecido
            if (db.sistema.Produtos.Exists(x => x.Id == _produto.Id))           
               return new Retorno() { Status = false, Resultado = "Produto já cadastrado" };
            
            db.sistema.Produtos.Add(_produto);

            file.ManipulacaoDeArquivos(false, db.sistema);

            return new Retorno() { Status = true, Resultado = _produto };
        }

        public Retorno ExibirProdutoId(int id)
        {
            //exibindo produto via Id 
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //filtrando por Id o resultado a ser exibido
            var resultado = arquivo.sistema.Produtos.Where(x => x.Id == id);
            return new Retorno() { Status = true, Resultado = resultado };

        }

        public Retorno ExibirProdutoDataCadastro(string dataCadastro)
        {
            //fazendo o mesmo via data cadastro 
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //filtrando por data de cadastro com Lambda
            var resultado = arquivo.sistema.Produtos.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
            return new Retorno() { Status = true, Resultado = resultado };
        }

        public Retorno ExibirTodosProdutos(int page, int sizePage)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //instancia de base pois usarei seu método genérico para paginar minha lista de produtos
            Base classeBase = new Base();
            //passando a lista de produtos para o metodo de base
            List<Produto> thirdPage = classeBase.GetPage(arquivo.sistema.Produtos, page, sizePage);

            return new Retorno() { Status = true, Resultado = thirdPage };
        }

        public Retorno DeletarProdutoId(int id)
        {
            var arquivo = file.ManipulacaoDeArquivos(true, null);
            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //filtrando o produto que será removido
            var resultado = arquivo.sistema.Produtos.Remove(arquivo.sistema.Produtos.Find(s => s.Id == id));
            //salvando remoção do produto no arquivo
            file.ManipulacaoDeArquivos(false, arquivo.sistema);
            return new Retorno() { Status = true, Resultado = "Produto Deletada!" };
        }

        public Retorno AtualizarProdutoId(Produto nova, int id)
        {
            //método para atualizar o produto via Id
            var arquivo = file.ManipulacaoDeArquivos(true, null);

            if (arquivo.sistema == null)
                arquivo.sistema = new Sistema();
            //filtrando o produto a ser atualizado
            var velha = arquivo.sistema.Produtos.Find(s => s.Id == id);
            //abastecendo o método de TrocaDados para que ele possa atualizar meu produto
            var troca = TrocaProduto(nova, velha);
            //adicionando o novo produto atualizado e retirando o antigo
            arquivo.sistema.Produtos.Add(troca);
            arquivo.sistema.Produtos.Remove(velha);
            //salvando alterações
            file.ManipulacaoDeArquivos(false, arquivo.sistema);

            return new Retorno() { Status = true, Resultado = $"{troca}\n\nProduto Atualizado!" };
        }

        public Produto TrocaProduto(Produto nova, Produto velha)
        {
            //método que recebo os novos atributos do objeto e os substitui para atualizar
            velha.nome_Produto = nova.nome_Produto;       
            velha.valor_Produto = nova.valor_Produto;
            nova.DataCadastro = velha.DataCadastro;
            nova.Id = velha.Id;
            return nova;
        }
    }
}
