using System.Runtime.Serialization.Formatters.Binary;

namespace api.gestaopessoal.Core
{
    public static class ObjectExtension
    {
        public static TObjeto Clonar<TObjeto>(this TObjeto objeto) where TObjeto : class
        {
            if (objeto == null)
                return null;

            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, objeto);
                ms.Seek(0, SeekOrigin.Begin);
                return (TObjeto)bf.Deserialize(ms);
                ms.Close();
            }
        }
    }
}
