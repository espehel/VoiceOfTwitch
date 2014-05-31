using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot
{
    class DatabaseConnection
    {
        private string sql_string;
        private string strCon;
        SqlDataAdapter da_1;

        public string Sql
        {
            set { sql_string = value; }
        }
        public string connection_string
        {
            set { strCon = value; }
        }
        public DataSet GetConnection
        {

            get { return MyDataSet(); }

        }
        private DataSet MyDataSet()
        {
            SqlConnection con = new SqlConnection(strCon);
            con.Open();
            da_1 = new SqlDataAdapter(sql_string, con);
            DataSet dat_set = new DataSet();
            da_1.Fill(dat_set, "Table_Data_1");

            con.Close();

            return dat_set;

        }

        public void UpdateDatabase(DataSet ds)
        {
            SqlCommandBuilder cb = new SqlCommandBuilder(da_1);
            cb.DataAdapter.Update(ds.Tables[0]);
        }
    }
}
