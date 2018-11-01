using System;
using System.IO;

namespace Emulator{

	class GameBoyColor{
	
		public GameBoyColor(string romPath){
			string savePath = @"c:/dumps/save.dump";
			CPU cpu;
			
			if (!File.Exists(savePath))			
				cpu = new CPU(romPath);	
			else
				cpu = new CPU(romPath, savePath);
			
			Console.WriteLine("\n========Beginning Emulation========\n\n");
			while(cpu.tick()){};
			
			if (!Directory.Exists("c:/dumps"))
			{
				Directory.CreateDirectory("c:/\dumps");
			}
			
			cpu.SaveState(savePath);
			
		}	
		
		
	}
}