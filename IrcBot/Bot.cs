using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace IrcBot
{
    class Bot
    {
    // Irc server to connect 
    public string Server{get; set;}
    // Irc server's port (6667 is default port)
    private int Port { get; set; }
    // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server 
    private string USER = "USER CSharpBot 8 * :I'm a C# irc bot";
    // Bot's nickname
    private string nick = "EspenBot";
    // Channel to join
    private string Channel { get; set; }
    private NetworkStream stream;
    private TcpClient irc;
    private string inputLine;
    private StreamReader reader;
    private StreamWriter writer;
    private Thread ircThread;
  
        public Bot(string server, int port, string channel)
        {
            this.Server = server;
            this.Port = port;
            this.Channel = channel;
         
        }
        public void Start()
        {
            irc = new TcpClient(Server, Port);
            stream = irc.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            Pinger pinger = new Pinger(writer, Server);
            pinger.Start();
            writer.WriteLine(USER);
            writer.Flush();
            writer.WriteLine("NICK " + nick);
            writer.Flush();
            writer.WriteLine("JOIN " + Channel);
            writer.Flush();
            ircThread = new Thread(new ThreadStart(Listener));
            ircThread.Start();
        }
        public void Stop()
        {
            ircThread.Abort();
            // Close all streams
            writer.Close();
            reader.Close();
            irc.Close();
        }
        private void Listener()
        {
            while (true)
            {
                try
                {
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(inputLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Thread.Sleep(5000);
                    Stop();
                    Start();
                }
            }
        }
        /*
        public int Port { get; set; }
        public TcpListener listener;
        private Thread listenerThread;
        public Server(int port)
        {
            this.Port = port;
        }

        public void start()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            listener.Start();
            listenerThread = new Thread(new ThreadStart(Listener));
            listenerThread.Start();
        }
        public void Stop()
        {
            listenerThread.Abort();
        }
        private void Listener()
        {
            while (true)
            {
                try
                {
                    TcpClient c = listener.AcceptTcpClient();
                   // new Thread(new ThreadStart(()=>HandleClient(c))).Start();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private void HandleClient(TcpClient client)
        {
            byte[] packetIDData = new byte[2];
            client.GetStream().Read(packetIDData, 0, 2);
            switch(packetID)
            {
                case 0:
                    byte[] data = new byte[4];
                    client.GetStream().Read(data, 0, 4);
                    int lengt = BitConverter.ToInt32(data,0);
                    data=new byte[length];
                    client.GetStream().Read(data, 0 ,length);

            }
        }
        */
    }
}
