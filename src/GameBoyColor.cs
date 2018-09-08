using System;

namespace Emulator{

	class GameBoyColor{
	
		public GameBoyColor(string romPath){
			CPU cpu = new CPU(romPath);		
			Console.WriteLine("\n========Beginning Emulation========\n\n");
			while(cpu.tick()){};
		}	
		
		
	}
}