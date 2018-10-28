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
	enum MemoryModel {MM16x8, MM4x32}
	
	class Memory
	{		
		string ROM_TITLE;
		GameType gameType;
		CartridgeType cartridgeType;
		DestinationCode destinationCode;
		MemoryModel memoryModel;
		
		Cartridge cartridge;
		
		public Memory(string romPath)
		{
			byte[] rom = File.ReadAllBytes(romPath);
			
			ROM_TITLE = GetROMTitle(rom);			
			gameType = GetGameType(rom);			
			cartridgeType = GetCartridgeType(rom);
			
			int romBanks = GetNumROMBanks(rom);
			int ramBanks = GetNumRAMBanks(rom);
			
			destinationCode = GetDestinationCode(rom);	
			memoryModel = MemoryModel.MM16x8;
			
			cartridge = AssembleCartridge(ramBanks, romBanks, rom);
			
			Console.WriteLine(ROM_TITLE);
			Console.WriteLine(gameType);
			Console.WriteLine(cartridgeType);
			Console.WriteLine("ROM Banks: {0}", romBanks);
			Console.WriteLine("RAM Banks: {0}", ramBanks);
			Console.WriteLine(destinationCode);
		}
		
		private string GetROMTitle(byte[] rom)
		{
			// retrieve the Cartridge Title from memory location [0134] to [0142]			
			string tempName = "";
			for (int i = 0x0134; i < 0x0143; i++){
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
			switch (rom[0x0148]){
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
			switch (rom[0x0149]){
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
		
		private Cartridge AssembleCartridge(int ramBanks, int romBanks, byte[] ROM)
		{
			// TO-DO: implement an actual assembling function, this is just a placeholder
			// while I work on getting Pokemon Yellow to work
			return new MemoryBankController5(ramBanks, romBanks, ROM);
		}
		
		public byte this[int index]
		{
			get { return cartridge[index]; }
			set { cartridge[index] = value;}
		}
		
		public void write(int index, byte val)
		{			
			cartridge.write(index, val);
		}
	}

	
	abstract class Cartridge
	{
		// Universal Cartridge Data
		protected int romBanks;
		protected int ramBanks;
		protected byte[] rom;
		protected byte[] ram;
		protected byte[] vram;
		protected byte[] io;
		protected byte[] oam; // Object Attribute Memory
		
		// State Information
		protected int romBankSelect;
		protected int ramBankSelect;
		protected int romOffset;
		protected int ramOffset;
		
		public const int ROM_BANK_SIZE = 0x4000;
		public const int RAM_BANK_SIZE = 0x2000;
		
		public Cartridge(int ramBanks, int romBanks, byte[] ROM)
		{
			this.ramBanks = ramBanks;
			this.romBanks = romBanks;
			
			rom = ROM;
			vram = new byte[RAM_BANK_SIZE];
			io = new byte[0x4C];
			oam = new byte[0x4 * 40]; // 40 4-byte attribute memory slots
			ram = new byte[RAM_BANK_SIZE * (ramBanks + 1) + 0x007F]; 
			// Plus 1 accounts for the internal RAM
			// Plus 0x007F accounts for the internal ram at the end of the memory range
			
			ramBankSelect = 0;
			romBankSelect = 0;
			ramOffset = 0;
			romOffset = 0;
		}
		
		public abstract void write(int index, byte val);
		
		public byte this[int index]
		{
		/*	Gameboy Memory Map from Game Boy CPU Manual
			Interrupt Enable Register
			--------------------------- FFFF
			Internal RAM
			--------------------------- FF80
			Empty but unusable for I/O
			--------------------------- FF4C
			I/O ports
			--------------------------- FF00
			Empty but unusable for I/O
			--------------------------- FEA0
			Sprite Attrib Memory (OAM)
			--------------------------- FE00
			Echo of 8kB Internal RAM
			--------------------------- E000
			8kB Internal RAM
			--------------------------- C000
			8kB switchable RAM bank
			--------------------------- A000
			8kB Video RAM
			--------------------------- 8000 --
			16kB switchable ROM bank 		 	|
			--------------------------- 4000 	|= 32kB Cartrigbe
			16kB ROM bank #0 				 	|
			--------------------------- 0000 --
		*/
			get
			{
				switch ((index & 0xF000) >> 12)
				{ // Use bitwise AND to get topmost nibble, bitshift right 24 bits to move down
					case 0x0:
					case 0x1:
					case 0x2:
					case 0x3:
						return rom[index];
					case 0x4:
					case 0x5:
					case 0x6:
					case 0x7:
						return rom[index + romOffset];
					case 0x8:
					case 0x9:
						return vram[index - 0x8000];
					case 0xA:
					case 0xB:
						return ram[index + ramOffset - 0xA000 + 0x2000]; // Offset by 0xA000 to set at the start of ram banks.
					case 0xC:											 // Offset by 0x2000 to account for internal ram
					case 0xD:
						return ram[index - 0xC000];
					case 0xE:
						return ram[index - 0xE000];
					case 0xF:
						switch ((index & 0x0F00) >> 8)
						{ // Use bitwise AND to get 3rd from right nibble, bitshift right 4 to move down
							case 0xE:
								switch ((index & 0x00F0) >> 4)
								{
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
								switch ((index & 0x00F0) >> 4)
								{
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
			set { write(index, value); }
		}
	}
	
	class MemoryBankController5: Cartridge
	{
		public MemoryBankController5(int ramBanks, int romBanks, byte[] ROM) : base(ramBanks, romBanks, ROM)
		{
			
		}
		
		public override void write(int index, byte val)
		{
			switch((index & 0xF000) >> 12)
			{
				case 0x2:
					int bitNine = (1<<9) & romBankSelect; // store bit 9 of the romBankSelect
					romBankSelect = bitNine + val; // add it back to the given index
					romOffset = romBankSelect * ROM_BANK_SIZE;
					break;
				case 0x3:
					int otherEight = 0xFF & romBankSelect; // store bits 1-8 of romBankSelect
					romBankSelect = ((val & 0x1) << 8) + otherEight;
					romOffset = romBankSelect * ROM_BANK_SIZE;
					break;
				case 0x4:
				case 0x5:
					if (ramBanks > 0)
					{
						ramBankSelect = val & 0xFF;
						ramOffset = ramBankSelect * RAM_BANK_SIZE;
					}
					break;
			}
		}
		
	}
	
	
}