using System;
using System.Diagnostics;

namespace Emulator
{	
	public class GameBoyColor
	{
		private InterruptController interruptController;
		private Timer timer;
		private PPU ppu;
		private Registers registers;
		private Memory memory;
		private CPU cpu;
		private Clock clock;
		
		private Stopwatch stopwatch;
		
		public GameBoyColor(string romPath)
		{
			// Base Derivation
			Setup();
			memory = new Memory(romPath, registers, ppu);
			cpu = new CPU(interruptController, memory, registers);
			clock = new Clock(timer, cpu, ppu);
		}
		
		public GameBoyColor(string romPath, string bootROMPath)
		{
			Setup();
			memory = new Memory(romPath, bootROMPath, registers, ppu);
			cpu = new CPU(interruptController, memory, registers);
			clock = new Clock(timer, cpu, ppu);
		}
		
		private void Setup()
		{
			interruptController = new InterruptController();
			timer = new Timer(interruptController);
			ppu = new PPU(interruptController);
			registers = new Registers(timer.TimerRegisters, ppu.DisplayRegisters);
			
			stopwatch = new Stopwatch();
		}
		
		public void Run()
		{
			Debug.Log("\n=====Beginning Emulation=====\n\n");
			stopwatch.Start();
			while(cpu.Alive)
			{
				clock.Tick();
			}
			stopwatch.Stop();
			ppu.DisplayFullBackground();
			//for(int i = 0; i < 255; i++)
				//ppu.PrintTile(i);
			//ppu.PrintBGMap();
			ppu.DumpVRAM();
			Console.WriteLine("\nClock Cycles: {0}\nEmulation Speed: {1:F3}Mhz", clock.C_Cycle, (clock.C_Cycle / stopwatch.ElapsedMilliseconds / 1000.0));
		}
		
	}
	
}