using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using IrcBot.Models;

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
    private Thread ThreadMessageEventThread;
    private List<IMessageListener> listeners;
    private Pinger pinger;
  
        public Bot(string server, int port, string channel)
        {
            this.Server = server;
            this.Port = port;
            this.Channel = channel;
            listeners = new List<IMessageListener>(); 
      
        }
        public void Start()
        {
            irc = new TcpClient(Server, Port);
            stream = irc.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            pinger = new Pinger(writer, Server);
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
            pinger.Stop();
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
                        Console.WriteLine("BOT: " + inputLine);
//                        string[] splitted = inputLine.Split(new char[] { ':' });

//                        for (int i = 0; i < splitted.Length; i++)
//                        {
//                            Console.WriteLine("i = " + i + ": " + splitted[i]);
//                        }
//                        Statement statement = new Statement(0, splitted[splitted.Length - 1], DateTime.Now);
//                        FireNewMessageEvent(splitted[splitted.Length - 1]);

                        string trail = extractTrail(inputLine);
                        if (trail != null)
                        {
//                            new Thread(new ThreadStart(FireNewMessageEvent));
                        new Thread(() => FireNewMessageEvent(trail)).Start();
                    }
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

        public void AddListener(IMessageListener listener)
        {
            listeners.Add(listener);
        }

        private void FireNewMessageEvent(string message)
        {
            foreach (var messageListener in listeners)
            {
                messageListener.NewMessage(message);
            }
        }

        public string extractTrail(string message)
        {
            // http://calebdelnay.com/blog/2010/11/parsing-the-irc-message-format-as-a-client

            int prefixEnd = message.IndexOf(" ");

            string trailing = null;
            int trailingStart = message.IndexOf(" :");
            if (trailingStart >= 0)
                trailing = message.Substring(trailingStart + 2);
            else
                trailingStart = message.Length;

            string[] commandAndParameters = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');

            string command = commandAndParameters[0];

            if (command.Equals("PRIVMSG"))
                return trailing;
            else
            {
                return null;
            }


        }

        public void ParseIrcMessage(string message, out string prefix, out string command, out string[] parameters)
        {
            // http://calebdelnay.com/blog/2010/11/parsing-the-irc-message-format-as-a-client
            int prefixEnd = -1, trailingStart = message.Length;
            string trailing = null;
            prefix = command = String.Empty;
            parameters = new string[] { };

            // Grab the prefix if it is present. If a message begins
            // with a colon, the characters following the colon until
            // the first space are the prefix.
            if (message.StartsWith(":"))
            {
                prefixEnd = message.IndexOf(" ");
                prefix = message.Substring(1, prefixEnd - 1);
            }

            // Grab the trailing if it is present. If a message contains
            // a space immediately following a colon, all characters after
            // the colon are the trailing part.
            trailingStart = message.IndexOf(" :");
            if (trailingStart >= 0)
                trailing = message.Substring(trailingStart + 2);
            else
                trailingStart = message.Length;

            // Use the prefix end position and trailing part start
            // position to extract the command and parameters.
            var commandAndParameters = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');

            // The command will always be the first element of the array.
            command = commandAndParameters.First();

            // The rest of the elements are the parameters, if they exist.
            // Skip the first element because that is the command.
            if (commandAndParameters.Length > 1)
                parameters = commandAndParameters.Skip(1).ToArray();

            // If the trailing part is valid add the trailing part to the
            // end of the parameters.
            if (!String.IsNullOrEmpty(trailing))
                parameters = parameters.Concat(new string[] { trailing }).ToArray();
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
