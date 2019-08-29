using System.Collections.Generic;
using Vendas;

namespace Model
{
    public class Sistema
    {
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public List<Produto> Produtos { get; set; }
    }
}
