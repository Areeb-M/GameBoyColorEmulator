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
			
			opcode = DefaultFunc().GetEnumerator();		
			alive = true;
		}
		
		private IEnumerable<bool> DefaultFunc()
		{
			// A Placeholder function to act as the default opcode
			yield break;
		}
		
		public void Tick()
		{
			if (!opcode.MoveNext())
			{
				if (OpcodeTable.ContainsKey(memory[reg.PC]))
				{
					opcode = OpcodeTable.Call(memory[reg.PC], memory, reg);
					// Fetch takes 1 cycle
					Debug.Log("\n{0:X2} - ", reg);
				}
				else
				{
					Console.Write("\nUnknown Opcode: {0:X2} at {1:X4}", memory[reg.PC], reg.PC);
					Console.Write(" - {0}", reg);
					alive = false;
				}
				
			}
		}
	}	
}