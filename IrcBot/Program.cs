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
using DataAccess.Adapters;
using IrcBot.Models;
using Shared.Models;

namespace IrcBot
{
    internal class Program : IMessageListener
    {
        //private List<Statement> statements;
        private List<StatementModel> _statements;
        private Bot bot;
        private DatabaseConnection dbCon;
        private string conString;
        private Timer timer;
        private int counter = 0;
        private ChannelModel channel;
        private VoiceAdapter adapter;
        private ServerInfo[] servers;
        private int serverIndex = 0;
        private string channelName;

        public void Init(StartConfig config)
        {
            InitServers();
            channelName = "#"+config.ChannesList[0];
            //bot = new Bot("irc.freenode.net", 6667, "#espensChannel");
            if(!config.IsEvent)
                bot = new Bot("irc.twitch.tv", 6667, channelName);
            else
                bot = new Bot("199.9.250.117", 80, channelName);
            
            dbCon = new DatabaseConnection();
            conString = Properties.Settings.Default.StatementsDatabaseConnectionString;
            dbCon.connection_string = conString;
            adapter = new VoiceAdapter();
            if (config.Delete)
            {
                //Console.WriteLine("PROGRAM: Deleted " + dbCon.ClearStatements() + " rows.");
                adapter.DeleteAllStatements();
                Console.WriteLine("PROGRAM: Deleted all rows.");
            }
            //channel = new Channel(channelName,DateTime.Now,DateTime.Now);
            channel = new ChannelModel()
            {
                Name = channelName.Substring(1),
                Live = true
            };
            adapter.AddOrUpdateChannel(channel);
            //var id = dbCon.InsertChannel(channel);
            //if (id == -1)
            //    dbCon.UpdateChannel(channel);
            //else
            //    channel.Id = id;
            _statements = adapter.GetStatements();
            //statements = dbCon.FetchAllStatements();
            timer = new Timer(TimerCallback, null, 30000, 30000);
//            ds = dbCon.GetConnection;
        }

        private void InitServers()
        {
            servers = new[]
            {
                new ServerInfo()
                {
                    Adress = "199.9.250.117",
                    Port = 80
                },
                new ServerInfo()
                {
                    Adress = "199.9.251.213",
                    Port = 80
                },
                new ServerInfo()
                {
                    Adress = "199.9.252.26",
                    Port = 80
                },
                new ServerInfo()
                {
                    Adress = "199.9.250.117",
                    Port = 443
                },
                new ServerInfo()
                {
                    Adress = "199.9.251.213",
                    Port = 443
                },
                new ServerInfo()
                {
                    Adress = "199.9.252.26",
                    Port = 443
                }
            };
        }

        public void Run()
        {
            bot.AddListener(this);
            bot.Start();

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                switch (c.Key)
                {
                    case ConsoleKey.Q:
                        bot.Stop();

                        Console.WriteLine("PROGRAM: q");
                        timer.Dispose();
                        //dbCon.UpdateStatements(statements, counter);
                        adapter.UpdateStatements(_statements, counter);
                        break;
                    case ConsoleKey.P:
                        PrintStatements();
                        Console.WriteLine("PROGRAM: p");
                        break;
                    case ConsoleKey.N:
                        bot.Stop();
                        Console.Clear();
                        Console.WriteLine("Switching server...");
                        bot = new Bot(servers[serverIndex % 6].Adress,servers[serverIndex % 6].Port,channelName);
                        serverIndex++;
                        bot.AddListener(this);
                        bot.Start();
                        break;
                }

            }
        }

        private void TimerCallback(object state)
        {
            //dbCon.UpdateStatements(statements, counter);
            adapter.UpdateStatements(_statements, counter);
            //dbCon.ClearStatements(2);
            //adapter.DeleteRareStatements(2);
            //statements = dbCon.FetchAllStatements();
            _statements = adapter.GetStatements();
        }

        private void PrintStatements()
        {
            foreach (var statement in _statements)
            {
                Console.WriteLine("PROGRAM: " + statement.Text + "[" + statement.Score + "]");
            }
        }

        public void NewMessage(string message)
        {
            bool exists = false;

            var newStatement = new StatementWrapper(new StatementModel()
            {
                Text = message,
                Score = 1,
                Occurrences = 1
            });
            //Parallel.For(0, statements.Count, (i) =>
            Parallel.For(0, _statements.Count, (i) =>
            {
                var oldStatement = new StatementWrapper(_statements[i]);
                if (oldStatement.Equals(message))
                {
                    oldStatement.IncrementScore(1);
                    oldStatement.Statement.Occurrences++;
                    exists = true;
                }
                else
                    oldStatement.SimilarTo(newStatement);
            });
            if (!exists)
            {
                newStatement.Statement.ChannelId = channel.Id;
                //newStatement.Statement.Id = dbCon.insertStatement(newStatement);
                //newStatement.Statement.Id = dbCon.insertStatement(newStatement.Statement);
                newStatement.Statement.Id = adapter.InsertStatement(newStatement.Statement);
                _statements.Add(newStatement.Statement);
            }
            counter++;
        }

        private static void Main(string[] args)
        {
            var argIndex = 0;
            if (args == null || args.Length == 0)
            {
                args = new[] {"-d","dota2ti,#beyondthesummit,#D2L"};
                Console.WriteLine("No channel specified");
                //Console.WriteLine("closing  application...");
                //Environment.Exit(0);
            }

            var config = new StartConfig();
            if (args[argIndex].StartsWith("-"))
            {
                Array.ForEach(args[argIndex].ToString(CultureInfo.InvariantCulture).ToCharArray(),
                    x => ExecCommand(config, x));
                argIndex++;
            }

            config.ChannesList =
                args[argIndex].Split(new char[] {',', '#'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var channelName in config.ChannesList)
            {
                foreach (var symbol in channelName.ToCharArray())
                {
                    if (!(Char.IsLetterOrDigit(symbol) || symbol == '_'))
                    {
                        Console.WriteLine("Invalid channelname (" + channelName + ")");
                        Console.WriteLine("closing  application...");
                        Environment.Exit(0);
                    }
                }
            }

            config.IsEvent = checkIfEventChannel(config.ChannesList[1]);
            config.IsEvent = true;
            var program = new Program();
            program.Init(config);
            program.Run();
        }

        private static void ExecCommand(StartConfig config, char command)
        {
            switch (command)
            {
                case 'd':
                    config.Delete = true;
                    break;
            }
        }

        private static bool checkIfEventChannel(string channelName)
        {
            var eventChannels = new string[] { "dota2ti", "dota2ti_1", "dota2ti_2", "dota2ti_3", "dota2ti_4" };
            return eventChannels.Contains(channelName);
        }
    }
}