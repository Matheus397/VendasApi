using Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Core.util
{

    public static class file
    {
        public static string arquivoDb = AppDomain.CurrentDomain.BaseDirectory + "db.json";
        public static (bool gravacao, Sistema sistema) ManipulacaoDeArquivos(bool read, Sistema _sistema)
        {
            try
            {
                if (!File.Exists(arquivoDb)) { File.Create(arquivoDb).Close(); }

                if (read)
                    return (false, JsonConvert.DeserializeObject<Sistema>(File.ReadAllText(arquivoDb)));

                File.WriteAllText(arquivoDb, JsonConvert.SerializeObject(_sistema));

                return (true, null);

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
