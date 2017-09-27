using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MPS.Common.Serializer
{
    public class SerializeHelper
    {
        public static void XMLSerialize<T>(T result, string path)
        {

            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                xs.Serialize(stream, result);
            }
        }
        public static T XMLDeserialize<T>(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = (T)xs.Deserialize(stream);
                return result;
            }

        }
        public static void BinarySerialize(object result, string path)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fsStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fsStream, result);
            }
        }
        public static T BinaryDeserialize<T>(string path)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = (T)binFormat.Deserialize(stream);
                return result;
            }

        }
    }
}
