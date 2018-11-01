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
			
			Debug.Log("\n========Beginning Emulation========\n\n");
			while(cpu.tick()){};
			Debug.Log("\n========Ending    Emulation========\n\n");
			
			Debug.Log("Check if State Dumps directory exists...");
			if (!Directory.Exists("c:/dumps"))
			{
				Debug.Log("\nCreating directory...");
				Directory.CreateDirectory("c:/dumps");
			}
			Debug.Log("\nSaving CPU state...");
			cpu.SaveState(savePath);
			
		}	
		
		
	}
}