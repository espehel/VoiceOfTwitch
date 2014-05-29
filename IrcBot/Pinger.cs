using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace IrcBot
{
    class Pinger
    {
    static string PING = "PING :";
    private Thread pingSender;
    private StreamWriter writer;
    public string Server { get; set; }
    // Empty constructor makes instance of Thread
    public Pinger(StreamWriter writer, string server)
    {
        this.writer = writer;
        this.Server = server;
        pingSender = new Thread(new ThreadStart(this.Run));
    }
    // Starts the thread
    public void Start()
    {
        pingSender.Start();
    }
    // Send PING to irc server every 15 seconds
    public void Run()
    {
        while (true)
        {
            writer.WriteLine(PING + Server);
            writer.Flush();
            Thread.Sleep(15000);
        }
    }
    }
}
