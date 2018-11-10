using System;

namespace Emulator
{
	class CPU
	{
		private Memory memory;
		private Registers reg;	

		public Registers registers
		{
			get { return reg; }
		}
		
		public CPU()
		{
			reg	= new Registers();
		}		
		
		public void AttachMemory(Memory mem)
		{
			memory = mem;
		}
		
		public bool Tick()
		{
			byte opcode = memory[reg.PC];
			Debug.LogOpcode(reg.PC, opcode);
			if (OpcodeTable.ContainsKey(opcode)){
				OpcodeTable.Call(opcode, memory, reg);
				return true;
			} else {
				Debug.Log("Unknown Opcode");
				return false;
			}
		}
	}	
}