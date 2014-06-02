using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IrcBot.Models;

namespace IrcBot
{
    class Program : IMessageListener
    {
        private List<Statement> statements;
        private Bot bot;
        private DatabaseConnection dbCon;
        private DataSet ds;
        private string conString;
        private Timer timer;

        public void Init()
        {
            bot = new Bot("irc.freenode.net", 6667, "#espensChannel");
            dbCon = new DatabaseConnection();
//            conString = Properties.Settings.Default.StatementsDatabaseConnectionString;
            conString = @"Data Source=E:\Git\VoiceOfTwitch\IrcBot\StatementsDatabase.sdf";
            dbCon.connection_string = conString;
            statements = dbCon.FetchAllStatements();
            timer = new Timer(TimerCallback,null,15000,15000);
//            ds = dbCon.GetConnection;

        }

        public void Run()
        {
            
            bot.AddListener(this);
            bot.Start();

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
//                Console.WriteLine("Program");
//                foreach (var statement in statements)
//                {
//                    ds.Tables[0].Rows.Add(setAsRow(statement));
//                }
//                try{
//                    dbCon.UpdateDatabase(ds);
//                }
//                    catch (Exception err)
//                {
//                    Console.WriteLine(err.Message);
//                }
                switch (c.Key)
                {
                    case ConsoleKey.Q: 
                        bot.Stop();
                        
                        Console.WriteLine("PROGRAM: q");
                        timer.Dispose();
                        dbCon.UpdateStatements(statements);
                        break;
                    case ConsoleKey.P:
                        PrintStatements();
                        Console.WriteLine("PROGRAM: p");
                        break;
                }


//                if (c.Key == ConsoleKey.Q)
//                {
//                    bot.Stop();
//                    return;
//                }

            }
        }
        private void TimerCallback(object state)
        {
            dbCon.UpdateStatements(statements);
        }

        private void PrintStatements()
        {
            foreach (var statement in statements)
            {
                Console.WriteLine("PROGRAM: " + statement);
            }
        }

        public void NewMessage(string message)
        {
            bool exists = false;
//            foreach (var statement in statements)
//            {
//                if (statement.Equals(message))
//                {
//                    statement.IncrementScore(1);
//                    exists = true;
//                }
//                else if (statement.SimilarTo(message))
//                    statement.IncrementScore(0.5);
//
//            }
            Parallel.For(0, statements.Count, (i) =>
            {
                Statement statement = statements[i];
                if (statement.Equals(message))
                {
                    statement.IncrementScore(1);
                    exists = true;
                }
                else if (statement.SimilarTo(message))
                    statement.IncrementScore(0.5);
            });
            if (!exists)
            {
                Statement statement = new Statement(message, DateTime.Now);
                statement.Id = dbCon.insertStatement(statement);
                statements.Add(statement);
            }
        }

        private DataRow setAsRow(Statement statement)
        {
            DataRow row = ds.Tables[0].NewRow();
            row[1] = statement.Text;
            row[2] = statement.CreatedAt;
            row[3] = statement.LastUpdated;
            return row;
        }
        static void Main(string[] args)
        {
            var program = new Program();
            program.Init();
            program.Run();
        }
    }


}
