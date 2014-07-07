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

        public long insertStatement(Statement statement)
        {
            string insertSql = @"INSERT INTO Statement(text,createdAt,lastUpdated,score,occurrences,channelId) VALUES(@text, @createdAt, @lastUpdated, @score, @occurrences, @channelId)";
//            string identity = @"SELECT id FROM Statement WHERE id = @@IDENTITY";
            string identitySql = @"SELECT @@IDENTITY AS ID";
            Object o;

            using (SqlCeConnection myConnection = new SqlCeConnection(strCon))
            {
                myConnection.Open();
                SqlCeCommand myCommand = new SqlCeCommand(insertSql, myConnection);
                myCommand.Parameters.AddWithValue("@text", statement.Text);
                myCommand.Parameters.AddWithValue("@createdAt", statement.CreatedAt);
                myCommand.Parameters.AddWithValue("@lastUpdated", statement.LastUpdated);
                myCommand.Parameters.AddWithValue("@score", statement.Score);
                myCommand.Parameters.AddWithValue("@occurrences", statement.Occurrences);
                myCommand.Parameters.AddWithValue("@channelId", statement.ChannelId);
                myCommand.ExecuteNonQuery();

                myCommand = new SqlCeCommand(identitySql,myConnection);
                o = myCommand.ExecuteScalar();
                myConnection.Close();
            }
            return Convert.ToInt64(o);
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
                //Statement statement = new Statement(row[0],row[1],row[2],row[3],row[4]);
                Statement statement = new Statement(row[0], row[1], row[4], row[3], row[2], row[5]);
                statements.Add(statement);
            }            
            con.Close();
            Console.WriteLine("DBCON: fetched " + statements.Count + " statements");
            return statements;
        }

        public void UpdateStatements(List<Statement> statements, int counter)
        {
            //TODO: clear database, insert top statements, delete rest
            Console.WriteLine("DBCON: updates statements [" + counter+"]");
            using (SqlCeConnection conn = new SqlCeConnection(strCon))
            {
            try
            {
                conn.Open();
                foreach (Statement statement in statements)
                {
                    SqlCeCommand UpdateCmd = new SqlCeCommand("UPDATE Statement SET score = @Score, lastUpdated = @LastUpdated, occurrences = @Occurrences WHERE (id=@Id)", conn);
                    UpdateCmd.Parameters.AddWithValue("@Score", statement.Score);
                    UpdateCmd.Parameters.AddWithValue("@Id", statement.Id);
                    UpdateCmd.Parameters.AddWithValue("@LastUpdated", statement.LastUpdated);
                    UpdateCmd.Parameters.AddWithValue("@Occurrences", statement.Occurrences);
                    UpdateCmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            }
            Console.WriteLine("DBCON: updates statements [" + counter+"]");
        }
        public int ClearStatements()
        {
            string sql = "DELETE Statement";
            SqlCeConnection con = new SqlCeConnection(strCon);
            con.Open();
            SqlCeCommand myCommand = new SqlCeCommand(sql, con);
            int rowsDeleted = myCommand.ExecuteNonQuery();
            con.Close();
            Console.WriteLine("DBCON: deleted " + rowsDeleted + " statements");
            return rowsDeleted;
        }
        public int ClearStatements(int threshold)
        {
            string sql = "DELETE Statement WHERE (score < @Threshold)";
            SqlCeConnection con = new SqlCeConnection(strCon);
            con.Open();
            SqlCeCommand myCommand = new SqlCeCommand(sql, con);
            myCommand.Parameters.AddWithValue("@Threshold",threshold);
            int rowsDeleted = myCommand.ExecuteNonQuery();
            con.Close();
            Console.WriteLine("DBCON: deleted " + rowsDeleted +" statements");
            return rowsDeleted;
        }
        public List<Channel> FetchAllChannels()
        {
            List<Channel> channels = new List<Channel>();
            SqlCeConnection con = new SqlCeConnection(strCon);
            con.Open();
            dataAdapter = new SqlCeDataAdapter("SELECT * FROM Channel", con);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Channel");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //Statement statement = new Statement(row[0],row[1],row[2],row[3],row[4]);
                Channel channel = new Channel(row[0], row[1], row[2], row[3]);
                channels.Add(channel);
            }
            con.Close();
            Console.WriteLine("DBCON: fetched " + channels.Count + " channels");
            return channels;
        }
        public long InsertChannel(Channel channel)
        {
            List<Channel> channels = FetchAllChannels();
            foreach (var chan in channels)
            {
                if (chan.Name.Equals(channel.Name))
                    return -1;
            }

            string insertSql = @"INSERT INTO Channel(name,startedAt,createdAt) VALUES(@name, @startedAt, @createdAt)";
            //            string identity = @"SELECT id FROM Statement WHERE id = @@IDENTITY";
            string identitySql = @"SELECT @@IDENTITY AS ID";
            Object o;

            using (SqlCeConnection myConnection = new SqlCeConnection(strCon))
            {
                myConnection.Open();
                SqlCeCommand myCommand = new SqlCeCommand(insertSql, myConnection);
                myCommand.Parameters.AddWithValue("@name", channel.Name);
                myCommand.Parameters.AddWithValue("@startedAt", channel.StartedAt);
                myCommand.Parameters.AddWithValue("@createdAt", channel.CreatedAt);
                myCommand.ExecuteNonQuery();

                myCommand = new SqlCeCommand(identitySql, myConnection);
                o = myCommand.ExecuteScalar();
                myConnection.Close();
            }
            return Convert.ToInt64(o);
        }

        public void UpdateChannel(Channel channel)
        {
            using (SqlCeConnection conn = new SqlCeConnection(strCon))
            {
                try
                {
                    conn.Open();
                        SqlCeCommand UpdateCmd = new SqlCeCommand("UPDATE Channel SET startedAt = @startedAt WHERE (name=@name)", conn);
                    UpdateCmd.Parameters.AddWithValue("@startedAt", channel.StartedAt);
                    UpdateCmd.Parameters.AddWithValue("@name",channel.Name);
                    UpdateCmd.ExecuteNonQuery();
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
