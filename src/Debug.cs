using System;

namespace Emulator
{

	class Debug
	{
		
		public static bool DEBUG = true;
		
		public static void ToggleDebug()
		{
			DEBUG = !DEBUG;
		}
		
		public static void SetDebug(bool state)
		{
			DEBUG = state;
		}
		
		public static void Opcode(int PC, int opcode)
		{
			if (DEBUG)
				Console.Write("[{0:X4}]{1:X2}", PC, opcode);
		}
		
		public static void UnknownOpcode(int PC, int opcode)
		{			
			if (DEBUG)
				Console.WriteLine("Encountered unknown opcode {0:X2} at [{1:X4}] while executing.", opcode, PC);
		}
		
		public static void Log(string log, params object[] args)
		{
			if (DEBUG)
				Console.Write(log, args);
		}
		
		public static void PrintBinary(byte num)
		{
			if (DEBUG)
				Console.Write(Convert.ToString(num, 2).PadLeft(8, '0'));
		}
		
		public static void ERROR(string log, params object[] args)
		{
			Console.Write("\n[ERROR]" + log, args);
			Console.ReadKey();
		}
	}
}