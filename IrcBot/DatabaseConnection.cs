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
        SqlCeDataAdapter dataAdapter;

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
//            dataAdapter = new SqlDataAdapter(sql_string, con);
//            DataSet dat_set = new DataSet();
//            dataAdapter.Fill(dat_set, "Table_Data_1");
//
//            con.Close();
//
//            return dat_set;
//
//        }
//
//        public void UpdateDatabase(DataSet ds)
//        {
//            SqlCommandBuilder cb = new SqlCommandBuilder(dataAdapter);
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

        public List<Statement> FetchAllStatements()
        {
            List<Statement> statements = new List<Statement>();
            SqlCeConnection con = new SqlCeConnection(strCon);
                        con.Open();
                        dataAdapter = new SqlCeDataAdapter("SELECT * FROM Statement", con);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet, "Statement");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Statement statement = new Statement(row[0],row[1],row[2],row[3],row[4]);
                statements.Add(statement);
            }            
                        con.Close();
            return statements;
        }

        public void UpdateStatements(List<Statement> statements )
        {
            Console.WriteLine("DBCON: updates statements");
            using (SqlCeConnection conn = new SqlCeConnection(strCon))
            {
            try
            {
                conn.Open();
                foreach (Statement statement in statements)
                {
                    SqlCeCommand UpdateCmd = new SqlCeCommand("UPDATE Statement SET score = @Score WHERE (id=@Id)", conn);
                    UpdateCmd.Parameters.AddWithValue("@Score", statement.Score);
                    UpdateCmd.Parameters.AddWithValue("@Id", statement.Id);
                    UpdateCmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            }
        }
    }
}
