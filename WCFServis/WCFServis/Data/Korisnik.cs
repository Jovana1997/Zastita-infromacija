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
    public class Korisnik
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }

        public Korisnik()
        {

        }
    }
}
