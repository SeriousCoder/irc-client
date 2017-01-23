using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IRC_bot
{
	class IRC
	{


		private string ircServer;
		private int ircPort;
		private string ircNick;
		private string ircRealName;
		private string ircUser;
		private string ircChannel;
		private bool ircIsInvisible;

		private TcpClient ircConnection;
		private NetworkStream ircStream;
		private StreamReader ircReader;
		private StreamWriter ircWriter;
		private ConsoleUI ircConsole;

		public IRC(string nick, string channel)
		{
			ircNick = nick;
			ircUser = Environment.MachineName;
			ircRealName = "Nikita Tarasenko";
			ircChannel = channel;
			ircIsInvisible = false;

			ircConsole = new ConsoleUI();
		}

		public void Connect(string server, int port)
		{
			ircServer = server;
			ircPort = port;

			string isInvisible = ircIsInvisible ? "8" : "0";
			ircConnection = new TcpClient(ircServer, ircPort);
			ircStream = ircConnection.GetStream();
			ircReader = new StreamReader(ircStream);
			ircWriter = new StreamWriter(ircStream);

			ircWriter.WriteLine($"USER {ircUser} {isInvisible} * :{ircRealName}");
			ircWriter.Flush();
			ircWriter.WriteLine($"NICK {ircNick}");
			ircWriter.Flush();
			ircWriter.WriteLine($"JOIN {ircChannel}");
			ircWriter.Flush();

			Thread ListenThread = new Thread(new ThreadStart(Listener));
			ListenThread.Start();

			while (ListenThread.IsAlive)
			{
				string ircCommand;

				
				if (!ListenThread.IsAlive) break;
				ircCommand = Console.ReadLine();
				if (ircCommand != null)
				{
					if ((ircCommand[0] == '/') && (UserCommand(ircCommand)))
						break;
				}
				
				lock (ircWriter)
				{
					ircWriter.WriteLine($"PRIVMSG {ircChannel} :{ircCommand}");
					ircWriter.Flush();
				}

				PrintClientsMessage(ircCommand);
			}

			ListenThread.Abort();

			ircWriter.Close();
			ircReader.Close();
			ircConnection.Close();
		}

		private void Listener()
		{
			try
			{
				while (true)
				{
					string ircCommand;
					/*while ((ircCommand = ircReader.ReadLine()) != null)
					{
						string[] commandParts = ircCommand.Split(' ');
						if (commandParts[0].Substring(0, 1) == ":")
						{
							commandParts[0] = commandParts[0].Remove(0, 1);
						}

						if (commandParts[0] == "PING")
						{

						}
					}*/
					if ((ircCommand = ircReader.ReadLine()) != null)
					{
						//ircConsole.OutputMessage(ircCommand);
						CommandReceived(ircCommand);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR: Unable to connect to server. App will be closed.");
			}
		}

		private void PrintClientsMessage(string msg, bool isMagic = false)
		{
			var time = DateTime.Now.ToString("t");

			ircConsole.InputUserMessage($"[{time}] <{ircNick}> {msg}", isMagic);
		}

		private bool UserCommand(string command)
		{
			var com = command.Split(' ');
			
			switch (com[0].ToLower())
			{
				case "/quit":
					ircConsole.InputUserMessage("Quit...");
					return true;
					break;
			}

			return false;
		}

		private void CommandReceived(string command)
		{
			var time = DateTime.Now.ToString("t");

			var mess = command.Split(':').Where(x => x != "").ToArray();
			var info = mess[0].Split(' ');

			if (info[0] == "PING")
			{
				lock (ircWriter)
				{
					ircWriter.WriteLine($"PONG {mess[1]}");
					ircWriter.Flush();
				}

				return;
			}

			switch (info[1])
			{
				case "NOTICE":
					ircConsole.OutputMessage($"[{time}] {mess[1]}");
					break;
				case "PRIVMSG":
					if (NeedMagic(mess[1])) return;
					var name = info[0].Remove(info[0].IndexOf("!", StringComparison.Ordinal));
					ircConsole.OutputMessage($"[{time}] <{name}> {mess[1]}");
					break;
				case "QUIT":
					ircConsole.OutputMessage($"[{time}] {mess[1]}: {mess[2]}");
					break;
				default:
					ircConsole.OutputMessage($"[{time}] {mess[mess.Length - 1]}");
					break;
			}
		}

		private bool NeedMagic(string str)
		{
			if (!str.ToLower().StartsWith("make some magic " + ircNick.ToLower())) return false;
			var time = DateTime.Now.ToString("t");
			var quote = QuoteAPI.GetQuote();

			lock (ircWriter)
			{
				ircWriter.WriteLine($"PRIVMSG {ircChannel} :{quote}");
				ircWriter.Flush();
			}

			PrintClientsMessage(quote, true);
			return true;
		}
	}
}
