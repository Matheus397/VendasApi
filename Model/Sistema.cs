using System;
using System.Collections.Generic;
using System.Text;
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
