using System;
using System.IO;

namespace Emulator
{
	public enum OutputTarget {Console = 1, File = 2};
	public static class Debug
	{
		private static byte target = OutputTarget.Console;
		// By default, route Debug messages to Console
		
		private static StreamWriter file;
		
		private static void Write(string message, params object[] args)
		{
			if (target & OutputTarget.Console == OutputTarget.Console)
			{
				Console.Write(message, args);
			}
			if (target & OutputTarget.File == OutputTarget.File)
			{
				string processed_message = String.Format(message, args);
				file.Write(processed_message);
			}
		}
		
		public static void TargetLogFile(string path)
		{
			if (File.Exists(path))
			{
				LogWarning("Attempting to log to pre-existing file({0}).", path);
			}
			
			try
			{
				file = new File.AppendText(path);
			} catch (DirectoryNotFoundException e)
			{
				LogError("Invalid log file path provided {0}", path);
				throw;
			}
			target |= OutputTarget.File;
		}
		
		public static void DetachTarget(OutputTarget t)
		{
			LogWarning("Attempting to detach {0} as an output target", t);
			byte mask = 255 ^ t;
			target &= mask;
		}
		
		public static void LogWarning(string message, params object[] args)
		{
			Write("[Warning!]"+message, args);
		}
		
		public static void LogError(string message, params object[] args)
		{
			Write("[ERROR!]"+message, args);
		}
		
		public static void Log(string message, params object[] args)
		{
			Write(message, args);
		}		
	}
	
}
