using Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Core.util
{
    //classe que trabalhrá com os arquivos
    public static class file
    { //string public de caminho de arquivo
        public static string arquivoDb = AppDomain.CurrentDomain.BaseDirectory + "db.json";
        //classe utilizando tuple recebendo um um booleano para ver se gravou o arquvio e o sistema onda há as listas
        public static (bool gravacao, Sistema sistema) ManipulacaoDeArquivos(bool read, Sistema _sistema)
        {
            // tentando fazer a manipulacao de arquivos
            try
            {
                if (!File.Exists(arquivoDb)) { File.Create(arquivoDb).Close(); }

                if (read)
                    return (false, JsonConvert.DeserializeObject<Sistema>(File.ReadAllText(arquivoDb)));
                //file.WriteAllText escrevendo no nosso arquivo e serializando nossas listas de Sistema
                File.WriteAllText(arquivoDb, JsonConvert.SerializeObject(_sistema));

                return (true, null);

            }
            catch (Exception)
            {
                //tratando possivel exceção na manipulação doa rquivo
                throw;
            }
        }
    }  
}
