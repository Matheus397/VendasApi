using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Produto : Base
    {
        //atributos do produto
        public string nome_Produto { get; set; }
        public double valor_Produto { get; set; } 
        public int quantidade { get; set; }
    }
}
