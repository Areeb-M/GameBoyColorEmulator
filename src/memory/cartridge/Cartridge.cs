using System;

namespace Emulator
{
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
			ram = new byte[RAM_BANK_SIZE * (ramBanks + 1) + 0x0080]; 
			// Plus 1 accounts for the internal RAM
			// internal ram 0x2000
			// ram banks 0x2000 * ram banks
			// internal ram 0x80
			// Plus 0x0080 accounts for the internal ram at the end of the memory range
			
			ramBankSelect = 0;
			romBankSelect = 0;
			ramOffset = 0x2000;
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
				{ // get topmost nibble of index
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
						return ram[index - 0xA000 + ramOffset];
					case 0xC:
					case 0xD:
						return ram[index - 0xC000];
					case 0xE:
						return ram[index - 0xE000];
					case 0xF:
						switch ((index & 0x0F00) >> 8)
						{ // get third nibble of index
							case 0x0: // Continuation
							case 0x1: // of
							case 0x2: // the
							case 0x3: // echo
							case 0x4: // of
							case 0x5: // internal
							case 0x6: // ram
							case 0x7: // from
							case 0x8: // 0xE000
							case 0x9: // to 
							case 0xA: // 0xFE00
							case 0xB:
							case 0xC:
							case 0xD:
								return ram[index - 0xE000];
							case 0xE:
								switch ((index & 0x00F0) >> 4)
								{
									case 0x0:
									case 0x1:
									case 0x2:
									case 0x3:
									case 0x4:
									case 0x5:
									case 0x6:
									case 0x7:
									case 0x8:
									case 0x9:
										return oam[index - 0xFE00];
									case 0xA: // A-F
									case 0xB: // are
									case 0xC: // empty
									case 0xD: // but
									case 0xE: // unusable
									case 0xF:
										return (byte)0;
									default: // this should never be reached
										return (byte)0;
								}
							case 0xF:
								
							default: // this should never be reached
								return (byte)0;
						}
					default: // this should never be reached
						return (byte)0; 
				}
			}
		}
	}
}