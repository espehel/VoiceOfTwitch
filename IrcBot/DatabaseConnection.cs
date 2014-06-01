using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrcBot.Models;

namespace IrcBot
{
    class DatabaseConnection
    {
        private string sql_string;
        private string strCon;
//        SqlDataAdapter da_1;

        public string Sql
        {
            set { sql_string = value; }
        }
        public string connection_string
        {
            set { strCon = value; }
        }
//        public DataSet GetConnection
//        {
//
//            get { return MyDataSet(); }
//
//        }
//        private DataSet MyDataSet()
//        {
//            SqlConnection con = new SqlConnection(strCon);
//            con.Open();
//            da_1 = new SqlDataAdapter(sql_string, con);
//            DataSet dat_set = new DataSet();
//            da_1.Fill(dat_set, "Table_Data_1");
//
//            con.Close();
//
//            return dat_set;
//
//        }
//
//        public void UpdateDatabase(DataSet ds)
//        {
//            SqlCommandBuilder cb = new SqlCommandBuilder(da_1);
//            cb.DataAdapter.Update(ds.Tables[0]);
//        }

        public int insertStatement(Statement statement)
        {
            string insertSql = @"INSERT INTO Statement(text,createdAt,lastUpdated,score) VALUES(@text, @createdAt, @lastUpdated,@score)";
//            string identity = @"SELECT id FROM Statement WHERE id = @@IDENTITY";
            string identitySql = @"SELECT @@IDENTITY AS ID";
            Object o;

            using (SqlCeConnection myConnection = new SqlCeConnection(strCon))
            {
                Console.WriteLine("DATABASE: " + myConnection.ConnectionString);
                myConnection.Open();
                Console.WriteLine("DATABASE: Open");
                SqlCeCommand myCommand = new SqlCeCommand(insertSql, myConnection);
                myCommand.Parameters.AddWithValue("@text", statement.Text);
                myCommand.Parameters.AddWithValue("@createdAt", statement.CreatedAt);
                myCommand.Parameters.AddWithValue("@lastUpdated", statement.LastUpdated);
                myCommand.Parameters.AddWithValue("@score", statement.Score);
                myCommand.ExecuteNonQuery();

                myCommand = new SqlCeCommand(identitySql,myConnection);
                o = myCommand.ExecuteScalar();
                myConnection.Close();
            }
            return Convert.ToInt32(o);
        }
    }
}
