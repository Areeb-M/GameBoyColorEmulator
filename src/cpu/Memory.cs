using System;
using System.IO;

namespace Emulator{

	class Memory{
		
		byte[] rom;
		
		string ROM_TITLE;
		
		public Memory(string romPath){
			rom = File.ReadAllBytes(romPath);
			
			string tempName = "";
			for(int i = 0x0134; i < 0x0143; i++){
				char letter = (char)rom[i];
				if(letter != (char)0x00 && letter != ' ')
					tempName += (char)rom[i];
				else
					tempName += '_';
			}
			Console.WriteLine(tempName);
		}
	
	}

}