using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class Base
    {
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public List<T> GetPage<T>(List<T> list, int page, int pageSize)
        {
            if (page <= 0)
                return new List<T>();
            return list.Skip(page - 1 * pageSize).Take(pageSize).ToList();
        }
    }
}
