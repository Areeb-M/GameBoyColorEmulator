using System;

namespace Emulator
{
	class CPU
	{
		Memory memory;
		Registers reg;
		InterruptController ic;
		
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
			
			alive = true;
		}
		
		public void Tick()
		{
			
		}
	}	
}