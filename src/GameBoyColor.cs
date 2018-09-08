using System;

namespace Emulator{

	class GameBoyColor{
	
		public GameBoyColor(string romPath){
			CPU cpu = new CPU(romPath);		
			
			while(cpu.tick()){};
		}	
		
		
	}
}