using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC_bot
{
	class Program
	{
		static void Main(string[] args)
		{
			string server = "irc.freenode.org";
			int port = 6667;
			string nickname = "JohnnyDau";
			string channel = "#spbnet";


			var irc = new IRC(nickname, channel);

			irc.Connect(server, port);
		}
	}
}
