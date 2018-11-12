using System;

namespace Emulator
{
	class Clock
	{
		Timer timer;
		CPU cpu;
		PPU ppu;
		
		int machineCycle;
		int clockCycle;
		public int M_Cycle
		{
			get { return machineCycle; }
		}
		public int C_Cycle
		{
			get { return clockCycle; }
		}
		
		public Clock(Timer timer, CPU cpu, PPU ppu)
		{
			this.timer = timer;
			this.cpu = cpu;
			this.ppu = ppu;
			clockCycle = 0;
			machineCycle = 0;
		}
		
		public void Tick()
		{
			ppu.FIFO.tick();
			
			if (clockCycle % 2 == 0)
				ppu.Fetch.tick();
			
			if (clockCycle % 4 == 0)
			{
				cpu.Tick();
				ppu.Tick();
				machineCycle += 1;
			}
			
			if (clockCycle % 256 == 0)
				timer.Tick();
			
			clockCycle += 1;
		}
		
	}
}