using System;

namespace Emulator
{
	class CPU
	{
		Memory memory;
		Registers reg;
		InterruptController ic;

		public Registers registers
		{
			get { return reg; }
		}
		
		public CPU(InterruptController interruptController, Memory mem, Registers registers)
		{			
			memory = mem;
			ic = interruptController;
			reg	= registers;
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