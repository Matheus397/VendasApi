using System;
using System.Collections.Generic;
using Vendas;

namespace Model
{
    public class Pedido
    {
        public int id_Pedido { get; set; }
        public DateTime data_Pedido { get; set; }
        public double total_Pedido { get; set; }       
        public Cliente cliente_pedido { get; set; }     
    }
}
