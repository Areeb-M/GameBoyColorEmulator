using System;


namespace Emulator
{
	
	public class GameBoyColor
	{
		private CPU cpu;
		private Memory memory;
		
		public GameBoyColor(string romPath)
		{
			// Base Derivation
			cpu = new CPU();
			memory = new Memory(romPath, cpu.registers);
			cpu.AttachMemory(memory);
		}
		
		public GameBoyColor(string romPath, string bootROMPath)
		{
			// Base Derivation
			cpu = new CPU();
			memory = new Memory(romPath, bootROMPath, cpu.registers);
			cpu.AttachMemory(memory);
		}
		
		public void Run()
		{
			
		}
		
	}
	
}