﻿using Model;
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
            try
            {
                //validando o produto sendo inserido com fluent validation
                var results = Validate(_produto);
                //caso o produto não seja válido retorno o resultado dos erros
                if (!results.IsValid)
                    return new Retorno { Status = false, Resultado = results.Errors };
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se sistema esta nulo, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();            
                //filtragem para ver se já há um produto na base de dados com o Id fornecido
                if (arquivo.sistema.Produtos.Exists(x => x.Id == _produto.Id))
                    return new Retorno { Status = false, Resultado = "Produto já cadastrado" };
                //adicionando enfim o produto ja validado à lista 
                arquivo.sistema.Produtos.Add(_produto);
                //salvando alterações
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = _produto };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro no cadastramento do Produto" }; }
        }

        public Retorno ExibirProdutoId(int id)
        {
            try
            {
                //exibindo produto via Id 
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando por Id o resultado a ser exibido
                var resultado = arquivo.sistema.Produtos.Where(x => x.Id == id);
                return new Retorno { Status = true, Resultado = resultado };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na exibição Produto" }; }
        }

        public Retorno ExibirProdutoDataCadastro(string dataCadastro)
        {
            try
            {
                //fazendo o mesmo via data cadastro 
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se sistema esta null, caso esteja crio uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando por data de cadastro com Lambda
                var resultado = arquivo.sistema.Produtos.Where(x => x.DataCadastro.ToString("ddMMyyyy").Equals(dataCadastro));
                return new Retorno { Status = true, Resultado = resultado };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na exibição Produto" }; }
        }

        public Retorno ExibirTodosProdutos(int page, int sizePage)
        {

            var arquivo = file.ManipulacaoDeArquivos(true, null);
            try
            {
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //instancia de base pois usarei seu método genérico para paginar minha lista de produtos
                Base classeBase = new Base();
                //verificando o formato das paginas passados pelo usuário, caso a pagina seja menor que 1 ou a quantidade de registros seja menor que 1 retorno uma msg
                if (sizePage < 1 || page < 1)
                    return new Retorno { Status = false, Resultado = "Paginação Inválida!" };
                if (page > arquivo.sistema.Produtos.Count)
                    return new Retorno { Status = false, Resultado = "Não há Produtos nesta página até então" };
                //passando minha lista de produtos para a paginação em base
                List<Produto> thirdPage = classeBase.GetPage(arquivo.sistema.Produtos, page, sizePage);
                return new Retorno { Status = true, Resultado = thirdPage };
            }
            catch { return new Retorno { Status = true, Resultado = "Erro na exibição" }; }
        }

        public Retorno DeletarProdutoId(int id)
        {
            try
            {
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se o sistema esta null
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando o produto que será removido
                var resultado = arquivo.sistema.Produtos.Remove(arquivo.sistema.Produtos.Find(s => s.Id == id));
                //salvando remoção do produto no arquivo
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = "Produto Deletada!" };
            }
            catch { return new Retorno { Status = false, Resultado = "Erro na deleção do Produto" }; }
        }

        public Retorno AtualizarProdutoId(Produto nova, int id)
        {
            try
            {
                //método para atualizar o produto via Id
                var arquivo = file.ManipulacaoDeArquivos(true, null);
                //verificando se o sistema esta null, caso esteja retorno uma nova instancia dele
                var sistemaNull = arquivo.sistema ?? new Sistema();
                //filtrando o produto a ser atualizado
                var velha = arquivo.sistema.Produtos.Find(s => s.Id == id);
                //abastecendo o método de TrocaDados para que ele possa atualizar meu produto
                var troca = TrocaProduto(nova, velha);
                //adicionando o novo produto atualizado e retirando o antigo
                arquivo.sistema.Produtos.Add(troca);
                arquivo.sistema.Produtos.Remove(velha);
                //salvando alterações
                file.ManipulacaoDeArquivos(false, arquivo.sistema);
                return new Retorno { Status = true, Resultado = $"{troca}\n\nProduto Atualizado!" };
            }
            catch
            {
                return new Retorno { Status = false, Resultado = "Erro na atualização do Produto" };
            }
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
