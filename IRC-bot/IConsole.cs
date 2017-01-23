using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC_bot
{
	interface IConsole
	{
		string ConsoleTitle { get; set; }

		void OutputMessage(string mess);

		string GetUserInterface(string nickname);
	}
}
