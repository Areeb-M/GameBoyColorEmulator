using System;

namespace Emulator{

	class Debug{
		
		public static bool DEBUG = true;
		
		public static void Opcode(int PC, int opcode){
			if (DEBUG)
				Console.Write("[{0:X4}]{1:X2}", PC, opcode);
		}
		
		public static void UnknownOpcode(int PC, int opcode){			
			Console.WriteLine("Encountered unknown opcode {0:X2} at [{1:X4}] while executing.", opcode, PC);
		}
		
		public static void JUMP(int PC){
			if (DEBUG)
				Console.Write(": Jump to [{0:X4}]", PC);
		}
		
		public static void XOR(int a, int b, byte regA){
			if (DEBUG)
				Console.Write(": XOR reg[A]={0} with b={1} to get {2}. Store in reg[A].", a, b, regA);				
		}
		
		public static void LOAD(string reg, int val){
			if (DEBUG)
				Console.Write(": Load 0x{0:X2} into {1}", val, reg);
		}
		
		public static void LOADH(int address, int val){
			if (DEBUG)
				Console.Write(": Load 0x{0:X2} into memory at [{1:X4}]", val, address);
		}
	}
}