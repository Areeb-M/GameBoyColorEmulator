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
		protected Registers reg;
		
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
		
		public void AttachRegisters(Registers reg)
		{
			this.reg = reg;
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
								switch((index & 0x00F0) >> 4)
								{
									case 0:
									case 1:
									case 2:
									case 3:
									case 4:
										return ReadRegister(index&0xFF);
									default: // this should never be reached
										return (byte)0;
								}
							default: // this should never be reached
								return (byte)0;
						}
					default: // this should never be reached
						return (byte)0; 
				}
			}
		}
		
		protected byte ReadRegister(int index)
		{
			switch(index)
			{
				case 0x00:
					return reg.P1;
				case 0x01:
					return reg.SB;
				case 0x02:
					return reg.SC;
				case 0x04:
					return reg.DIV;
				case 0x05:
					return reg.TIMA;
				case 0x06:
					return reg.TMA;
				case 0x07:
					return reg.TAC;
				case 0x0F:
					return reg.IF;
				case 0x10:
					return reg.NR10;
				case 0x11:
					return reg.NR11;
				case 0x12:
					return reg.NR12;
				case 0x13:
					return reg.NR13;
				case 0x14:
					return reg.NR14;
				case 0x16:
					return reg.NR21;
				case 0x17:
					return reg.NR22;
				case 0x18:
					return reg.NR23;
				case 0x19:
					return reg.NR14;
				case 0x1A:
					return reg.NR30;
				case 0x1B:
					return reg.NR31;
				case 0x1C:
					return reg.NR32;
				case 0x1D:
					return reg.NR33;
				case 0x1E:
					return reg.NR34;
				case 0x20:
					return reg.NR41;
				case 0x21:
					return reg.NR42;
				case 0x22:
					return reg.NR43;
				case 0x23:
					return reg.NR44;
				case 0x24:
					return reg.NR50;
				case 0x25:
					return reg.NR51;
				case 0x26:
					return reg.NR52;
				case 0x40:
					return reg.LCDC;
				case 0x41:
					return reg.STAT;
				case 0x42:
					return reg.SCY;
				case 0x43:
					return reg.SCX;
				case 0x44:
					return reg.LY;
				case 0x45:
					return reg.LYC;
				case 0x46:
					return reg.DMA;
				case 0x47:
					return reg.BGP;
				case 0x48:
					return reg.OBP0;
				case 0x49:
					return reg.OBP1;
				case 0x4A:
					return reg.WY;
				case 0x4B:
					return reg.WX;
				case 0xFF:
					return reg.IE:
			}
		}
	}
}