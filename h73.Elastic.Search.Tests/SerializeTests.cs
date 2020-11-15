using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using h73.Elastic.Core.Enums;
using h73.Elastic.Search.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void Serialize_Deserialize_Query()
        {
            var qSub123 = ObjectToByteArray(new Query<EarthFaultEvent>().SetSize((int) (44))
                .Exists(efe=>efe.Asset.SubstationId, BooleanQueryType.MustNot)
                .Terms(new []
                {
                    EventType.EarthFaultRen1.ToString(),
                    EventType.EarthFaultRen2.ToString(),
                    EventType.EarthFaultRen3.ToString()
                }, efe => efe.Type));

            var sObj = ObjectToByteArray(qSub123);
            var qObj = ByteArrayToObject(sObj);

        }


        // Convert an object to a byte array
        private byte[] ObjectToByteArray(Object obj)
        {
            if(obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object) binForm.Deserialize(memStream);

            return obj;
        }
    }
}