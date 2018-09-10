using System;

namespace Emulator{

	class Debug{
		
		public static bool DEBUG = true;
		
		public static void Opcode(int PC, int opcode){
			if (DEBUG)
				Console.Write("[{0:X2}]{1:X2}", PC, opcode);
		}
		
		public static void UnknownOpcode(int PC, int opcode){			
			Console.WriteLine("Encountered unknown opcode {0:X2} at [{1:X2}] while executing.", opcode, PC);
		}
		
		public static void JUMP(int PC){
			Console.Write(": Jump to [{0:X4}]", PC);
		}
		
		public static void XOR(){
			
		}
	}
}