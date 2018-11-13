using System;
using System.Collections;
using System.Collections.Generic;

namespace Emulator
{
	class CPU
	{
		Memory memory;
		Registers reg;
		InterruptController ic;
		
		private IEnumerator opcode;
		private bool alive;
		
		public bool Alive
		{
			get { return alive; }
		}

		public Registers registers
		{
			get { return reg; }
		}
		
		public CPU(InterruptController interruptController, Memory mem, Registers registers)
		{			
			memory = mem;
			ic = interruptController;
			reg	= registers;
			
			opcode = OpcodeTable.Call(memory[reg.PC], memory, reg);			
			alive = true;
		}
		
		public void Tick()
		{
			if (!opcode.MoveNext())
			{
				opcode = OpcodeTable.Call(memory[reg.PC], memory, reg);
				// Fetch takes 1 cycle
			}
		}
	}	
}