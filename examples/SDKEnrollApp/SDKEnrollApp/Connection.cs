using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SDKEnrollApp
{
    internal class Connection
    {
        private SqlConnection _conn;

        public void Connect()
        {
            _conn = new SqlConnection("Data Source=(localdb)\\v11.0;Initial Catalog=HuellaDigital;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            _conn.Open();
        }

        public void Disconnect()
        {
            _conn.Close();
        }

        public bool ExecQuery(string query)
        {
            Connect();
            var con = new SqlCommand(query, _conn);
            var modifiedRows = con.ExecuteNonQuery();
            Disconnect();
            return modifiedRows > 0;
        }

        public void UpdateTableUser(DataGridView dg)
        {
            const string query = "Select * from users";
            Connect();
            var ds = new DataSet();
            var da = new SqlDataAdapter(query, _conn);
            da.Fill(ds, "users");
            dg.DataSource = ds;
            dg.DataMember = "users";
            Disconnect();
        }

        public bool SaveUser(string documentType, string document, string name, string lastName, string birthdate)
        {
            var query = string.Format("Insert into users (document_type,document,name,last_name,birthdate) values ('{0}', '{1}', '{2}', '{3}', '{4}')", documentType, document, name, lastName, birthdate);
            return ExecQuery(query);
        }

        public bool UpdateUser(string documentType, string document, string name, string lastName, string birthdate, int id)
        {
            var query = string.Format("Update users set document_type = '{0}', document = '{1}', name = '{2}', last_name = '{3}', birthdate = '{4}' where Id = {5}", documentType, document, name, lastName, birthdate, id);
            return ExecQuery(query);
        }

        public bool DeleteUser(int id)
        {
            var query = string.Format("Delete from users where Id = {0}", id);
            return ExecQuery(query);
        }

        public string[] FindByDocument(string document)
        {
            var query = string.Format("Select * from users where document = {0}", document);
            var data = new string[6];
            Connect();
            var con = new SqlCommand(query, _conn);
            var dr = con.ExecuteReader();
            if (dr.Read())
            {
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = dr[i].ToString();
                }
            }
            Disconnect();

            return data; 
        }
    }
}
