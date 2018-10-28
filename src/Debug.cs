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
		
		public static void Log(string log)
		{
			if (DEBUG)
				Console.WriteLine(log);
		}
	}
}