using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
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
        private int counter = 0;
        private Channel channel;

        public void Init(StartConfig config)
        {

            string channelName = config.ChannesList[0];
            //bot = new Bot("irc.freenode.net", 6667, "#espensChannel");
            bot = new Bot("irc.twitch.tv", 6667, "#"+channelName);
            dbCon = new DatabaseConnection();
            conString = Properties.Settings.Default.StatementsDatabaseConnectionString;
//            conString = @"Data Source=E:\Git\VoiceOfTwitch\IrcBot\StatementsDatabase.sdf";
            //conString = @"Data Source=C:\Users\Espen\Documents\StatementsDatabase.sdf";
            dbCon.connection_string = conString;
            Console.WriteLine("PROGRAM: Deleted " + dbCon.ClearStatements() + " rows.");
            channel = new Channel(channelName,DateTime.Now,DateTime.Now);
            var id = dbCon.InsertChannel(channel);
            if (id == -1)
                dbCon.UpdateChannel(channel);
            else
                channel.Id = id;
            statements = dbCon.FetchAllStatements();
            timer = new Timer(TimerCallback,null,30000,30000);
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
                        dbCon.UpdateStatements(statements,counter);
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
            dbCon.UpdateStatements(statements,counter);
            dbCon.ClearStatements(2);
            statements = dbCon.FetchAllStatements();
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

            Statement newStatement = new Statement(message);
            Parallel.For(0, statements.Count, (i) =>
            {
                Statement oldStatement = statements[i];
                if (oldStatement.Equals(message))
                {
                    oldStatement.IncrementScore(1);
                    oldStatement.Occurrences++;
                    exists = true;
                }
                else
                    oldStatement.SimilarTo(newStatement);
            });
            if (!exists)
            {
                newStatement.ChannelId = channel.Id;
                newStatement.Id = dbCon.insertStatement(newStatement);
                statements.Add(newStatement);
            }
            counter++;
        }

        //private DataRow setAsRow(Statement statement)
        //{
        //    DataRow row = ds.Tables[0].NewRow();
        //    row[1] = statement.Text;
        //    row[2] = statement.CreatedAt;
        //    row[3] = statement.LastUpdated;
        //    return row;
        //}
        static void Main(string[] args)
        {
            var argIndex = 0;
            if(args == null)
            {
                args = new[] { "-d", "#beyondthesummit,#D2L" };
                Console.WriteLine("No channel specified");
                //Console.WriteLine("closing  application...");
                //Environment.Exit(0);
            }

            var config = new StartConfig();
            if (args[argIndex].StartsWith("-"))
            {
                Array.ForEach(args[argIndex].ToString(CultureInfo.InvariantCulture).ToCharArray(),x => ExecCommand(config, x));
                argIndex++;
            }

            config.ChannesList = args[argIndex].Split(new char[]{',','#'},StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var channelName in config.ChannesList)
            {
                foreach (var symbol in channelName.ToCharArray())
                {
                    if (!Char.IsLetterOrDigit(symbol))
                    {
                        Console.WriteLine("Invalid channelname (" + channelName +")");
                        Console.WriteLine("closing  application...");
                        Environment.Exit(0);
                    }
                }
            }

            var program = new Program();
            program.Init(config);
            //program.Run();
            Console.WriteLine(args.Length);
        }

        static void ExecCommand(StartConfig config, char command)
        {
            switch (command)
            {
                case 'd':
                    config.Delete = true;
                    break;
            }

        }
    }


}
