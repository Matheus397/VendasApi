using System;
using System.Collections.Generic;
using Vendas;

namespace Model
{
    public class Pedido : Base
    {
        public double total_Pedido { get; set; }
        public List<Produto> produtosPedido { get; set; }           
        public Cliente cliente_pedido { get; set; }     
    }
}
