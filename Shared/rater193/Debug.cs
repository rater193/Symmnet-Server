using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.shared.rater193
{
	public class Debug
	{
		public ConsoleColor debugColorTime = ConsoleColor.Yellow;
		public ConsoleColor debugColorTitle = ConsoleColor.White;
		public ConsoleColor debugColorInfo = ConsoleColor.Green;
		public ConsoleColor debugColorError = ConsoleColor.Red;

		private string name = "";

		public Debug(string name)
		{
			this.name = name;
		}

		public string log(ConsoleColor textColor = ConsoleColor.Green, params object[] messages)
		{
			//"[ " + (DateTime.Now.ToString()) + " ] [ " + name + " ] "
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("[ ");
			Console.ForegroundColor = debugColorTime;
			Console.Write((DateTime.Now.ToString()));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] [ ");
			Console.ForegroundColor = debugColorTitle;
			Console.Write(name);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] ");
			Console.ForegroundColor = textColor;

			string ret = "";
			foreach(object msg in messages)
			{
				ret += " " + msg;
			}
			Console.WriteLine(ret);
			Console.ForegroundColor = ConsoleColor.White;
			return ret;
		}


		public static string Log(ConsoleColor textColor = ConsoleColor.Green, params object[] messages)
		{
			//"[ " + (DateTime.Now.ToString()) + " ] [ " + name + " ] "
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("[ ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write((DateTime.Now.ToString()));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] ");
			Console.ForegroundColor = textColor;

			string ret = "";
			foreach(object msg in messages)
			{
				ret += " " + msg;
			}
			Console.WriteLine(ret);
			Console.ForegroundColor = ConsoleColor.White;
			return ret;
		}


		public static string Log(params object[] messages)
		{
			//"[ " + (DateTime.Now.ToString()) + " ] [ " + name + " ] "
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("[ ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write((DateTime.Now.ToString()));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] ");
			Console.ForegroundColor = ConsoleColor.Green;

			string ret = "";
			foreach(object msg in messages)
			{
				ret += " " + msg;
			}
			Console.WriteLine(ret);
			Console.ForegroundColor = ConsoleColor.White;
			return ret;
		}

		public string log(params object[] messages)
		{
			//"[ " + (DateTime.Now.ToString()) + " ] [ " + name + " ] "
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("[ ");
			Console.ForegroundColor = debugColorTime;
			Console.Write((DateTime.Now.ToString()));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] [ ");
			Console.ForegroundColor = debugColorTitle;
			Console.Write(name);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" ] ");
			Console.ForegroundColor = debugColorInfo;

			string ret = "";
			foreach(object msg in messages)
			{
				ret += " " + msg;
			}
			Console.WriteLine(ret);
			Console.ForegroundColor = ConsoleColor.White;
			return ret;
		}

		internal static void LogError(Exception e)
		{
			var errdbg = new Debug("ERROR");
			errdbg.debugColorTime = ConsoleColor.Red;
			errdbg.debugColorInfo = ConsoleColor.Red;
			errdbg.debugColorError = ConsoleColor.Red;
			errdbg.debugColorTitle = ConsoleColor.Red;
			errdbg.log("\n" + "Exception: " + e.GetType() + "\n" + e.Source + "\n" + e.StackTrace + "\n");
		}
	}
}
