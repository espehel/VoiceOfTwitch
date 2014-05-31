using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public void Init()
        {
            bot = new Bot("irc.freenode.net", 6667, "#espensChannel");
            statements = new List<Statement>();
            dbCon = new DatabaseConnection();
            conString = Properties.Settings.Default.StatementsDatabaseConnectionString;
            dbCon.connection_string = conString;
            ds = dbCon.GetConnection;

        }

        public void Run()
        {
            
            bot.addListener(this);
            bot.Start();

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                Console.WriteLine("Program");
                foreach (var statement in statements)
                {
                    ds.Tables[0].Rows.Add(setAsRow(statement));
                }
                try{
                    dbCon.UpdateDatabase(ds);
                }
                    catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                switch (c.Key)
                {
                    case ConsoleKey.Q: 
                        bot.Stop(); 
                        Console.WriteLine("PROGRAM: q");
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
            foreach (var statement in statements)
            {
                if (statement.Equals(message))
                {
                    statement.Score++;
                    exists = true;
                }
                else if (statement.SimilarTo(message))
                    statement.Score += 0.5;

            }
            if(!exists)
                statements.Add(new Statement(new Random().Next(),message,DateTime.Now));
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
