using System;

namespace Emulator
{
	class CPU
	{
		private Memory memory;
		private Registers reg;		
		
		public CPU(Memory mem)
		{
			reg	= new Registers();
			memory = mem;
			memory.AttachRegisters(reg);
		}		
		
		public bool Tick()
		{
			byte opcode = mem[reg.PC];
		}
	}	
}