using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Logging
{
	/// <summary>
	/// Logs the outputs to the console using <see cref="Console.WriteLine"/> using a fancy method from https://stackoverflow.com/a/850587/5010271
	/// </summary>
	public class FancyConsoleLogger : ILogger
	{
		public LogLevel Level { get; set; }
		public int OutputHeight { get { return outHeight; } set { outHeight = value; } }

		private int outCol, outRow, outHeight = 10;
		
		public void Info(string message, params object[] args)
		{
			if (Level != LogLevel.Info) return;
			WriteLine("INFO: " + message, args);
		}

		public void Warning(string message, params object[] args)
		{
			if (Level != LogLevel.Info && Level != LogLevel.Warning) return;
			WriteLine("WARN: " + message, args);
		}

		public void Error(string message, params object[] args)
		{
			if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;
			WriteLine("ERR : " + message, args);
		}

		private void WriteLine(string msg, params object[] args)
		{
			string format = string.Format(msg, args);
			Write(format, true);
		}

		#region Fancy Logging

		//A logging system that will allow for nice inputs. 
		// Provided by https://stackoverflow.com/a/850587/5010271
		// Author: BlueMonkMN

		private void Write(string msg, bool appendNewLine)
		{
			int inCol, inRow;
			inCol = Console.CursorLeft;
			inRow = Console.CursorTop;

			int outLines = getMsgRowCount(outCol, msg) + (appendNewLine ? 1 : 0);
			int outBottom = outRow + outLines;
			if (outBottom > outHeight)
				outBottom = outHeight;
			if (inRow <= outBottom)
			{
				int scrollCount = outBottom - inRow + 1;
				Console.MoveBufferArea(0, inRow, Console.BufferWidth, 1, 0, inRow + scrollCount);
				inRow += scrollCount;
			}
			if (outRow + outLines > outHeight)
			{
				int scrollCount = outRow + outLines - outHeight;
				Console.MoveBufferArea(0, scrollCount, Console.BufferWidth, outHeight - scrollCount, 0, 0);
				outRow -= scrollCount;
				Console.SetCursorPosition(outCol, outRow);
			}
			Console.SetCursorPosition(outCol, outRow);
			if (appendNewLine)
				Console.WriteLine(msg);
			else
				Console.Write(msg);
			outCol = Console.CursorLeft;
			outRow = Console.CursorTop;
			Console.SetCursorPosition(inCol, inRow);
		}

		private int getMsgRowCount(int startCol, string msg)
		{
			string[] lines = msg.Split('\n');
			int result = 0;
			foreach (string line in lines)
			{
				result += (startCol + line.Length) / Console.BufferWidth;
				startCol = 0;
			}
			return result + lines.Length - 1;
		}
		#endregion
	}
}
