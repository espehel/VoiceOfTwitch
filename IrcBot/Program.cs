using System;
using System.Collections.Generic;
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

        public void init()
        {
            bot = new Bot("irc.freenode.net", 6667, "#espensChannel");
            statements = new List<Statement>();
        }

        public void Run()
        {
            
            bot.addListener(this);
            bot.Start();

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);

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

            }
            if(!exists)
                statements.Add(new Statement(new Random().Next(),message,DateTime.Now));
        }
        static void Main(string[] args)
        {
            Program program = new Program();
            program.init();
            program.Run();
        }
    }


}
