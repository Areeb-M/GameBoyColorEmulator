using System;

namespace Emulator
{
	class CPU
	{
		Memory memory;
		Registers reg;
		PPU ppu;

		public Registers registers
		{
			get { return reg; }
		}
		
		public CPU(PPU ppu)
		{
			reg	= new Registers();
			this.ppu = ppu;
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