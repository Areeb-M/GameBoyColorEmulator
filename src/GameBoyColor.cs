using System;


namespace Emulator
{
	
	public class GameBoyColor
	{
		private InterruptController interruptController;
		private Timer timer;
		private CPU cpu;
		private PPU ppu;
		private Memory memory;
		
		public GameBoyColor(string romPath)
		{
			// Base Derivation
			Setup();
			cpu = new CPU(ppu);
			memory = new Memory(romPath, cpu.registers, ppu);
			cpu.AttachMemory(memory);
		}
		
		public GameBoyColor(string romPath, string bootROMPath)
		{
			Setup();
			cpu = new CPU(ppu);
			memory = new Memory(romPath, bootROMPath, cpu.registers, ppu);
			cpu.AttachMemory(memory);
		}
		
		private void Setup()
		{
			interruptController = new InterruptController();
			timer = new Timer(interruptController);
			ppu = new PPU(interruptController);			
		}
		
		public void Run()
		{
			Debug.Log("\n=====Beginning Emulation=====\n\n");
			while(cpu.Tick())
			{
				Debug.Log(" | {0}\n", cpu.registers);
			}
			Debug.Log(" | {0}\n", cpu.registers);
			Console.ReadLine();
		}
		
	}
	
}