using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WCFServis.Data
{
    [DataContract]
    [Serializable]
    public class Fajlovi
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Byte[] Content { get; set; }
        [DataMember]
        public string Hash_value { get; set; }
        [DataMember]
        public int Encription_method { get; set; }
        [DataMember]
        public int User_id { get; set; }

        public Fajlovi()
        {

        }
    }
}
