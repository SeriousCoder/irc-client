using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC_bot
{
	class ConsoleUI : IConsole
	{
		public string ConsoleTitle
		{
			get { return Console.Title; }
			set { Console.Title = value; }
		}

		private const string InputIdentifier = "--> ";
		private const int NumInputLines = 1;

		public ConsoleUI()
		{
			Console.Clear();
			ConsoleTitle = string.Empty;
			Console.SetCursorPosition(Console.WindowLeft, Console.WindowHeight);
			Console.Write(InputIdentifier);
		}

		public void OutputMessage(string mess)
		{
			var numLines = 1 + (mess.Length / Console.WindowWidth);

			var targetTop = (Console.WindowTop - numLines + NumInputLines);
			targetTop = targetTop >= 1 ? targetTop : 1;

			Console.MoveBufferArea(Console.WindowLeft, Console.WindowTop + numLines, Console.WindowWidth, Console.WindowHeight - numLines - NumInputLines,
									Console.WindowLeft, targetTop);

			var cursorX = Console.CursorLeft;
			var cursorY = Console.CursorTop;

			Console.SetCursorPosition(Console.WindowLeft, cursorY - numLines);
			Console.WriteLine(mess);
			Console.SetCursorPosition(cursorX, cursorY);
		}

		public void InputUserMessage(string mess, bool magic = false)
		{
			ModifyConsole(mess);

			if (!magic) Console.Write(InputIdentifier);
		}

		public string GetUserInterface(string nickname)
		{
			throw new NotImplementedException();
		}

		private void ModifyConsole(string mess)
		{
			var cursorX = Console.CursorLeft;
			var cursorY = Console.CursorTop;

			var numLines = 1 + (mess.Length / Console.WindowWidth);

			for (var i = 1; i <= numLines; i++)
			{
				Console.SetCursorPosition(Console.WindowLeft, cursorY - i);
				Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
			}
			Console.Write(mess);

			Console.SetCursorPosition(cursorX, cursorY);
		}
	}
}
