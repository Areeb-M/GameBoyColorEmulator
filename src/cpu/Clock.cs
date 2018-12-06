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
			timer.Tick(clockCycle);
			//ppu.FIFO();
			
			//if (clockCycle % 2 == 0)
			//	ppu.Fetch();
			
			if (clockCycle % 4 == 0)
			{
				cpu.Tick();
				ppu.Tick();
				machineCycle += 1;
			}
			
			clockCycle += 1;
		}
		
	}
}