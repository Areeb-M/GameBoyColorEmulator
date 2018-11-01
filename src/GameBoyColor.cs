using System;
using System.IO;

namespace Emulator{

	class GameBoyColor{
	
		public GameBoyColor(string romPath){
			string savePath = @"/dumps/save.dump";
			
			if (!File.Exists(savePath))			
				CPU cpu = new CPU(romPath);	
			else
				CPU cpu = new CPU(romPath, savePath);
			Console.WriteLine("\n========Beginning Emulation========\n\n");
			while(cpu.tick()){};
			
			if (!Directory.Exists("/dumps"))
			{
				Directory.CreateDirectory("/dumps");
			}
			
			cpu.SaveState(savePath);
			
		}	
		
		
	}
}