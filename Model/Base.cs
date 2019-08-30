using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class Base
    {
        //atributos a serem herdados pelas tres entidades
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        //Este método genérico pagina as listas das minhas entidades e insere o usuário na paginda requerida 
        public List<T> GetPage<T>(List<T> list, int page, int pageSize)
        {
            if (page <= 0)
                return new List<T>();
            //aqui eu pulo a quantidade de paginas por qnts registros devem ser vistos e listo o resultado
            return list.Skip(page - 1 * pageSize).Take(pageSize).ToList();
        }
    }
}
