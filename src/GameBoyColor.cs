using System;


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
		}
		
		public void Run()
		{
			Debug.Log("\n=====Beginning Emulation=====\n\n");
			while(cpu.Alive)
			{
				clock.Tick();
				Debug.Log(" | {0}\n", cpu.registers);
			}
			Console.ReadLine();
		}
		
	}
	
}