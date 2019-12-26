using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Security.AccessControl;
using System.Management;

namespace WCFServis
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public Service1()
        {
            ConnectToDB();
        }
        SqlConnection conn;
        SqlCommand comm;

        SqlConnectionStringBuilder connStringBuilder;

        void ConnectToDB()
        {
            connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = "DESKTOP-8Q7NCEF\\SQLEXPRESS";
            connStringBuilder.InitialCatalog = "ZastitaInformacija";
            //connStringBuilder.Encrypt = true;
            connStringBuilder.TrustServerCertificate = true;
            connStringBuilder.ConnectTimeout = 30;
            connStringBuilder.AsynchronousProcessing = true;
            connStringBuilder.MultipleActiveResultSets = true;
            connStringBuilder.IntegratedSecurity = true;

            conn = new SqlConnection(connStringBuilder.ToString());
            comm = conn.CreateCommand();
        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        public Int32 LogIn(string username, string password)
        {
            Int32 count;
            try
            {
                comm.CommandText = "SELECT COUNT(*) FROM Korisnik WHERE username='" + username + "' AND password='" + password + "'";
                //comm.Parameters.AddWithValue("Username", k.Username);
                //comm.Parameters.AddWithValue("Password", k.Password);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                count = (Int32)comm.ExecuteScalar();

                return count;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        public Data.Korisnik GetKorisnik(string username)
        {
            Data.Korisnik korisnik = new Data.Korisnik();
            try
            {
                comm.CommandText = "SELECT * FROM Korisnik WHERE username=@Username";
                comm.Parameters.AddWithValue("Username", username);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while(reader.Read())
                {
                    korisnik.Id = Convert.ToInt32(reader[0]);
                    korisnik.Username = reader[1].ToString();
                    korisnik.Password = reader[2].ToString();
                }
                return korisnik;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public void InsertFiles(byte[] file, string name, string hash, int method, int clientId, string pass)
        {
            try
            {
                comm.CommandText = "INSERT INTO Fajlovi(ID, name, content, hash, method, userID, pass) VALUES('" + Guid.NewGuid().ToString() + "', @Name, @File, @Hash, @Method, @ClientID, @Pass)";
                comm.Parameters.AddWithValue("Name", name);
                comm.Parameters.AddWithValue("File", file);
                comm.Parameters.AddWithValue("Hash", hash);
                comm.Parameters.AddWithValue("Method", method);
                comm.Parameters.AddWithValue("ClientID", clientId);
                comm.Parameters.AddWithValue("Pass", pass);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                comm.ExecuteNonQuery();

            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public void DeleteFiles(string name, int id)
        {
            try
            {
                comm.CommandText = "DELETE FROM Fajlovi WHERE name=@Name AND userID=@Id";
                comm.Parameters.AddWithValue("Name", name);
                comm.Parameters.AddWithValue("Id", id);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public Table GetFiles(int id)
        {
            Table t = new Table();
            DataRow r;
            DataColumn c = new DataColumn();
            c.DataType = System.Type.GetType("System.String");
            c.ColumnName = "Your files";
            t.imena.Columns.Add(c);
            try
            {
                comm.CommandText = "SELECT DISTINCT name FROM Fajlovi WHERE userID='" + id.ToString() + "'";
                //comm.Parameters.AddWithValue("Korisnik", id);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while(reader.Read())
                {
                    r = t.imena.NewRow();
                    r["Your files"] = reader[0].ToString();
                    t.imena.Rows.Add(r);
                }
                return t;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public byte[] DownloadFiles(string name, int id)
        {
            DirectoryInfo dInfo = new DirectoryInfo("D:\\Downloads");
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

            
            byte[] file = { };
            try
            {
                comm.CommandText = "SELECT content FROM Fajlovi WHERE name=@Name AND userID=@Id";
                comm.Parameters.AddWithValue("Name", name);
                comm.Parameters.AddWithValue("Id", id);
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader();
                if(reader != null)
                {
                    reader.Read();
                    file = new Byte[(reader.GetBytes(0, 0, null, 0, int.MaxValue))];
                    reader.GetBytes(0, 0, file, 0, file.Length);
                   
                }
                return file;
            }
            catch(System.InvalidOperationException e)
            {
                throw e;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public byte[] GetBytes(string path)
        {
            byte[] file;
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            file = reader.ReadBytes((int)stream.Length);
            return file;
        }
        public void SaveBytes(byte[] file, string newName)
        {
            MemoryStream ms = new MemoryStream();
            string path = "D:\\Downloads\\" + newName + ".txt";
            using (FileStream fs = File.Create(path))
            {
                //FileStream fs = new FileStream("D:\\Downloads\\download.txt", FileMode.Open, FileAccess.Write);
                fs.Write(file, 0, file.Length);
            }
        }
        public int GetMethod(string name, int clientId)
        {
            Int32 method;
            try
            {
                comm.CommandText = "SELECT DISTINCT method FROM Fajlovi WHERE userID=@Client AND name=@Name";
                comm.Parameters.AddWithValue("Client", clientId);
                comm.Parameters.AddWithValue("Name", name);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                method = (Int32)comm.ExecuteScalar();
                return method;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public string GetHash(string name, int clientId)
        {
            string hash = null;
            try
            {
                comm.CommandText = "SELECT DISTINCT hash FROM Fajlovi WHERE userID=@Client AND name=@Name";
                comm.Parameters.AddWithValue("Client", clientId);
                comm.Parameters.AddWithValue("Name", name);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while(reader.Read())
                {
                    hash = reader.GetString(0);
                }
                return hash;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public string GetPass(string name, int clientId)
        {
            string pass = null;
            try
            {
                comm.CommandText = "SELECT DISTINCT pass FROM Fajlovi WHERE userID=@Client AND name=@Name";
                comm.Parameters.AddWithValue("Client", clientId);
                comm.Parameters.AddWithValue("Name", name);
                comm.CommandType = System.Data.CommandType.Text;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    pass = reader.GetString(0);
                }
                return pass;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
    }
}
