using System;
using System.IO;

namespace Emulator{

	class Memory{
		
		byte[] rom;
		byte[] ram;
		byte[] vram;  // Video RAM
		byte[] io;    // Input/Output Memory
		byte[] oam;   // Object Attribute Memory: Stores information about sprites
		int romBankOffset;
		int ramBankOffset;
		enum GameType {Color, Mono}; // Color = Gameboy Color; Mono = Gameboy (Mono being short for Monochrome)
		enum CartridgeType {ROM =                       0x0,
							ROM_MBC1 =                  0x1, 
							ROM_MBC1_RAM =              0x2, 
							ROM_MBC1_RAM_BATT =         0x3,
							ROM_MBC2 =                  0x5,
							ROM_MBC2_BATT    =          0x6,
							ROM_RAM =                   0x8,
							ROM_RAM_BATT =              0x9,
							ROM_MMM01 =                 0xB,
							ROM_MMM01_SRAM =            0xC,
							ROM_MMM01_SRAM_BATT =       0xD,
							ROM_MBC3_TIMER_BATT =       0xF,
							ROM_MBC3_TIMER_RAM_BATT =   0x10,
							ROM_MBC3 =                  0x11,
							ROM_MBC3_RAM =              0x12,
							ROM_MBC3_RAM_BATT =         0x13,
							ROM_MBC5 =                  0x19,
							ROM_MBC5_RAM =              0x1A,
							ROM_MBC5_RAM_BATT =         0x1B,
							ROM_MBC5_RUMBLE =           0x1C,
							ROM_MBC5_RUMBLE_SRAM =      0x1D,
							ROM_MBC5_RUMBLE_SRAM_BATT = 0x1E,
							Pocket_Camera =             0x1F,
							Bandai_TAMA_5 =             0xFD,
							Hudson_HuC3 =               0xFE,
							Hudson_HuC1 =               0xFF							
							};
		enum DestinationCode {Japanese, Non_Japanese};

		
		string ROM_TITLE;
		int romBanks;
		int ramBanks;
		GameType gameType;
		CartridgeType cartridgeType;
		DestinationCode destinationCode;
		
		
		public Memory(string romPath){
			rom = File.ReadAllBytes(romPath);
			
			string tempName = "";
			for (int i = 0x0134; i < 0x0143; i++){
				char letter = (char)rom[i];
				if (letter != (char)0x00 && letter != ' ')
					tempName += (char)rom[i];
				else
					tempName += '_';
			}			
			ROM_TITLE = tempName;
			
			if (rom[0x0143] == 0x80)
				gameType = GameType.Color;
			else
				gameType = GameType.Mono;
			
			cartridgeType = (CartridgeType)rom[0x0147];
						
			switch (rom[0x0148]){
				case 0x52:
					romBanks = 72;
					break;
				case 0x53:
					romBanks = 80;
					break;
				case 0x54:
					romBanks = 96;
					break;
				default:
					romBanks = (int)Math.Pow(2, rom[0x0148]+1);
					break;
			}
			
			switch (rom[0x0149]){
				case 0:
					ramBanks = 0;
					break;
				case 1: // 2kB 
				case 2: // 8kB These are different sizes, but use the same number of ram banks
					ramBanks = 1;
					break;
				case 3:
					ramBanks = 4;
					break;
				case 4:
					ramBanks = 16;
					break;
			}
			
			destinationCode = (DestinationCode)rom[0x014A];			
			
			ram = new byte[0x2000 * (ramBanks + 1) + 0x007F]; // Plus 1 accounts for the internal RAM; Plus 0x007F accounts for the internal ram at the end of the memory range
			vram = new byte[0x2000];
			io = new byte[0x4C];
			oam = new byte[0x4 * 40]; // 40 4-byte attribute memory slots
			
			romBankOffset = 0;
			ramBankOffset = 0x2000;
			
			Console.WriteLine(tempName);
			Console.WriteLine(gameType);
			Console.WriteLine(cartridgeType);
			Console.WriteLine("ROM Banks: {0}", romBanks);
			Console.WriteLine("RAM Banks: {0}", ramBanks);
			Console.WriteLine(destinationCode);
		}
	
		public byte this[int index]{
			get{
				switch ((index & 0xF000) >> 6*4){ // Use bitwise AND to get topmost nibble, bitshift right 24 bits to move down
					case 0x0:
					case 0x1:
					case 0x2:
					case 0x3:
						return rom[index];
					case 0x4:
					case 0x5:
					case 0x6:
					case 0x7:
						return rom[index + romBankOffset];
					case 0x8:
					case 0x9:
						return vram[index - 0x8000];
					case 0xA:
					case 0xB:
						return ram[index + ramBankOffset - 0x8000]; // Offset by 0x8000 to set at the start of ram banks. 
					case 0xC:
					case 0xD:
						return ram[index - 0xA000];
					case 0xE:
						return ram[index - 0xE000];
					case 0xF:
						switch ((index & 0x0F00) >> 4*4){ // Use bitwise AND to get 3rd from right nibble, bitshift right 4 to move down
							case 0xE:
								switch ((index & 0x00F0) >> 2*4){
									case 0xA:
									case 0xB:
									case 0xC:
									case 0xD:
									case 0xE:
									case 0xF:
										return (byte)0; 
									default:
										return oam[index - 0xFE00];
								}
							case 0xF:
								switch ((index & 0x00F0) >> 2*4){
									case 0x0:
									case 0x1:
									case 0x2:
									case 0x3:
										return io[index - 0xFF00];
									case 0x4:
									case 0x5:
									case 0x6:
									case 0x7:
										return (byte)0;
									default:
										return ram[index - 0xFF00 + 0x2000 * (ramBanks + 1)];					
								}
							default:                                // 0x0 - 0xD are echoes of internal ram 0xC000 - 0xDFFF
								return ram[index - 0xE000];		
						}
					default:
						return (byte)0;
				}
			}
			
			set{/*
				switch(cartridgeType){
					case CartridgeType.ROM_MBC5_RAM_BATT:
						writeMBC5(index, value);
						break;
				}*/
			}
		}	
		
		private void writeMBC5(int index, byte val){
			
		}
		
	}

}