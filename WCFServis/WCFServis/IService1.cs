using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace WCFServis
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        Int32 LogIn(string username, string password);

        [OperationContract]
        Data.Korisnik GetKorisnik(string username);

        [OperationContract]
        void InsertFiles(byte[] file, string name, string hash, int method, int clientId, string pass);

        [OperationContract]
        void DeleteFiles(string name, int id);

        [OperationContract]
        Table GetFiles(int id);

        [OperationContract]
        byte[] DownloadFiles(string name, int id);

        [OperationContract]
        byte[] GetBytes(string path);

        [OperationContract]
        void SaveBytes(byte[] file, string newName);

        [OperationContract]
        int GetMethod(string name, int clientId);

        [OperationContract]
        string GetHash(string name, int clientId);

        [OperationContract]
        string GetPass(string name, int clientId);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WCFServis.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    public class Table
    {
        public DataTable imena;

        public Table()
        {
            imena = new DataTable("Fajlovi");
        }
    }
}
