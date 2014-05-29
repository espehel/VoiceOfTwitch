using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot("irc.freenode.net",6667,"#espensChannel");

            bot.Start();

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Q)
                {
                    bot.Stop();
                    return;
                }
                
            }
        }
    }
}
