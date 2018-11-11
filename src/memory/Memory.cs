using System;
using System.IO;

namespace Emulator
{	
	enum GameType {Color, Mono}; // Color = Gameboy Color; Mono = Gameboy (Mono being short for Monochrome)
	enum CartridgeType 
	{					ROM =                       0x0,
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
	enum MemoryModel {MM16x8, MM4x32};
	
	class Memory
	{
		// Meta Data
		string ROM_TITLE;
		GameType gameType;
		CartridgeType cartridgeType;
		DestinationCode destinationCode;
		MemoryModel memoryModel;		
		int romBanks, ramBanks;
		
		Registers reg;
		PPU ppu;
		Cartridge cartridge;
		Cartridge cartridgeNorm;
		Cartridge bootROM;
		
		public Memory(string romPath, Registers registers, PPU ppu)
		{
			byte[] rom = File.ReadAllBytes(romPath);
			
			ScrapeMetaData(rom);			
			reg = registers;
			this.ppu = ppu;
			
		}
		
		public Memory(string romPath, string bootROMPath, Registers registers, PPU ppu)
		{			
			byte[] rom = File.ReadAllBytes(romPath);
			byte[] boot = File.ReadAllBytes(bootROMPath);
			
			ScrapeMetaData(rom);
			reg = registers;
			reg.PC = 0x0; // Bootrom starts at 0x00
			this.ppu = ppu;
			
			cartridgeNorm = ConstructCartridge(rom);
			bootROM = new BootRom(ppu, boot, this, cartridgeNorm);
			cartridge = bootROM;
			
			cartridge.AttachRegisters(reg);
			cartridgeNorm.AttachRegisters(reg);
		}
		
		private Cartridge ConstructCartridge(byte[] rom)
		{
			switch(cartridgeType)
			{
				case CartridgeType.ROM:
					return new Cartridge(ppu, ramBanks, romBanks, rom);
				default:
					return new Cartridge(ppu, ramBanks, romBanks, rom);
			}
		}
		
		#region Metadata		
		
		private void ScrapeMetaData(byte[] rom)
		{			
			ROM_TITLE = GetROMTitle(rom);			
			gameType = GetGameType(rom);			
			cartridgeType = GetCartridgeType(rom);
			
			romBanks = GetNumROMBanks(rom);
			ramBanks = GetNumRAMBanks(rom);
			
			destinationCode = GetDestinationCode(rom);	
			memoryModel = MemoryModel.MM16x8;		
			
			Debug.Log("ROM Title:{0}\nGame Type:{1}\nCartridge Type:{2}\nROM Banks:{3}\nRAM Banks:{4}\nDestination Code:{5}\n", 
			ROM_TITLE, gameType, cartridgeType, romBanks, ramBanks, destinationCode);		
		}
		
		private string GetROMTitle(byte[] rom)
		{
			// retrieve the Cartridge Title from memory location [0134] to [0142]			
			string tempName = "";
			for (int i = 0x0134; i < 0x0143; i++)
			{
				char letter = (char)rom[i];
				if (letter != (char)0x00 && letter != ' ')
					tempName += (char)rom[i];
				else
					tempName += '_';
			}			
			return tempName;
		}
		
		private GameType GetGameType(byte[] rom)
		{
			// check if cartridge is a Color game or not			
			if (rom[0x0143] == 0x80)
				return GameType.Color;
			else
				return GameType.Mono;
		}
		
		private CartridgeType GetCartridgeType(byte[] rom)
		{
			return (CartridgeType)rom[0x0147];
		}
		
		private int GetNumROMBanks(byte[] rom)
		{
			// retrieve the number of rom banks in the cartridge			
			switch (rom[0x0148])
			{
				case 0x52:
					return 72;
				case 0x53:
					return 80;
				case 0x54:
					return 96;
				default:
					return (int)Math.Pow(2, rom[0x0148]+1);
			}
		}
		
		private int GetNumRAMBanks(byte[] rom)
		{
			// retrieve the number of ram banks in the cartridge
			switch (rom[0x0149])
			{
				case 0:
					return 0;
				case 1: // 2kB 
				case 2: // 8kB These are different sizes, but use the same number of ram banks
					return 1;
				case 3:
					return 4;
				case 4:
					return 16;
				default:
					return 0;
			}
		}
		
		private DestinationCode GetDestinationCode(byte[] rom)
		{
			return (DestinationCode)rom[0x014A];
		}
		#endregion
		
		public void DetachBootROM()
		{
			cartridge = cartridgeNorm;
		}
		
		public Cartridge GetCartridge()
		{
			if (bootROM != null)
				return cartridgeNorm;
			return cartridge;
		}
		
		public byte this[int index]
		{
			get { return cartridge[index]; }
			set { cartridge[index] = value;}
		}
	}	
}