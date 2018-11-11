using System;

namespace Emulator
{
	class Clock
	{
		PPU ppu;
		int machineCycle;
		public int M_Cycle
		{
			get { return machineCycle; }
		}
		
		public Clock(PPU ppu)
		{
			this.ppu = ppu;
			machineCycle = 0;
		}
		
		public void Tick()
		{
			ppu.Tick();
			machineCycle += 1;
		}
		
	}
}