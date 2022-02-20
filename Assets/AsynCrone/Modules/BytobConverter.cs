using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace AsCrone.Module
{
    public static class BytobConverter<T> 
    {
        public static T ByteArrayToObject(byte[] arrBytes)
        {
            T data;
            using (var memStream = new MemoryStream(arrBytes))
            {
                var binForm = new BinaryFormatter();
                data = (T)binForm.Deserialize(memStream);
                return data;
            }
        }

        public static byte[] ObjectToByteArray(object type)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, type);
                return ms.ToArray();
            }
        }
    }
}


