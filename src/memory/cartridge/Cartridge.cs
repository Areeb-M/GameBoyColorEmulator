using System;

namespace Emulator
{
	class Cartridge
	{
		// Universal Cartridge Data
		protected int romBanks;
		protected int ramBanks;
		protected byte[] rom;
		protected byte[] ram;
		protected byte[] hram;
		protected byte[] io;		
		
		// State Information
		protected int romBankSelect;
		protected int ramBankSelect;
		protected int romOffset;
		protected int ramOffset;
		protected bool[] ramBankEnable;
		
		protected Registers reg;		
		protected PPU ppu;
		
		public const int ROM_BANK_SIZE = 0x4000;
		public const int RAM_BANK_SIZE = 0x2000;
		
		public Cartridge(PPU ppu, int ramBanks, int romBanks, byte[] ROM)
		{
			this.ppu = ppu;
			
			this.ramBanks = ramBanks;
			this.romBanks = romBanks;
			
			rom = ROM;
			io = new byte[0x4C];
			ram = new byte[RAM_BANK_SIZE * (ramBanks + 1)];
			hram = new byte[0x80];
			// Plus 1 accounts for the internal RAM
			// internal ram 0x2000
			// ram banks 0x2000 * ram banks
			// HRAM
			// internal ram 0x80
			// Plus 0x0080 accounts for the internal ram at the end of the memory range
			
			ramBankSelect = 0;
			romBankSelect = 0;
			ramOffset = 0x2000;
			romOffset = 0;			
			if (ramBanks > 0)
				ramBankEnable = new bool[ramBanks];
		}
		
		public void AttachRegisters(Registers reg)
		{
			this.reg = reg;
		}
		
		public virtual byte this[int index]
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
						return ppu.ReadVRAM(index - 0x8000);
					case 0xA:
					case 0xB:
						if (ramBanks > 0 && ramBankEnable[ramBankSelect])
							return ram[index - 0xA000 + ramOffset];
						else
							return 0xFF;
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
										return ppu.ReadOAM(index - 0xFE00);
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
								return ReadFBlock(index&0xFF);
							default: // this should never be reached
								return (byte)0;
						}
					default: // this should never be reached
						return (byte)0; 
				}
			}
			set
			{
				// Instead of having each Cartridge type implement 
				// its own writing page table, each section of
				// the memory table is handled by an overrideable
				// function, so only cartridge specific changes
				// have to be made instead of entirely rewriting
				// it for every class. Neat && extendable!
				switch ((index & 0xF000) >> 12)
				{
					case 0x0:
					case 0x1:
					case 0x2:
					case 0x3:
						WriteROMBank0(index, value);
						break;
					case 0x4:
					case 0x5:
					case 0x6:
					case 0x7:
						WriteSwitchableROMBank(index, value);
						break;
					case 0x8:
					case 0x9:
						WriteVRAM(index, value);
						break;
					case 0xA:
					case 0xB:
						if (ramBanks > 0 && ramBankEnable[ramBankSelect])
							WriteSwitchableRAM(index, value);
						break;
					case 0xC:
					case 0xD:
						WriteInternalRAM(index, value);
						break;
					case 0xE:
						WriteInternalRAMEcho(index, value);
						break;
					case 0xF:
						WriteFBlock(index, value);
						break;						
				}
			}
		}
		
		protected virtual void WriteROMBank0(int index, byte val)
		{
			// Does nothing
		}
		
		protected virtual void WriteSwitchableROMBank(int index, byte val)
		{
			// Does nothing
		}
		
		protected virtual void WriteVRAM(int index, byte val)
		{
			ppu.WriteVRAM(index - 0x8000, val);
			//Console.WriteLine("{0:X4}: {1:X2}", index, val);
		}
		
		protected virtual void WriteSwitchableRAM(int index, byte val)
		{
			ram[index - 0xA000 + ramOffset] = val;
		}
		
		protected virtual void WriteInternalRAM(int index, byte val)
		{
			ram[index - 0xC000] = val;
		}
		
		protected virtual void WriteInternalRAMEcho(int index, byte val)
		{
			ram[index - 0xE000] = val;
		}
		
		protected virtual void WriteOAM(int index, byte val)
		{
			ppu.WriteOAM(index - 0xFE00, val);
		}
		
		protected virtual void WriteEmptyButUnusable(int index, byte val)
		{
			// Does nothing
		}
		
		protected virtual void WriteIORegister(int index, byte val)
		{
			switch(index)
			{
				case 0x00:
					reg.P1 = val;
					break;
				case 0x01:
					reg.SB = val;
					break;
				case 0x02:
					reg.SC = val;
					break;
				case 0x04:
					reg.DIV = val;
					break;
				case 0x05:
					reg.TIMA = val;
					break;
				case 0x06:
					reg.TMA = val;
					break;
				case 0x07:
					reg.TAC = val;
					break;
				case 0x0F:
					reg.IF = val;
					break;
				case 0x10:
					reg.NR10 = val;
					break;
				case 0x11:
					reg.NR11 = val;
					break;
				case 0x12:
					reg.NR12 = val;
					break;
				case 0x13:
					reg.NR13 = val;
					break;
				case 0x14:
					reg.NR14 = val;
					break;
				case 0x16:
					reg.NR21 = val;
					break;
				case 0x17:
					reg.NR22 = val;
					break;
				case 0x18:
					reg.NR23 = val;
					break;
				case 0x19:
					reg.NR14 = val;
					break;
				case 0x1A:
					reg.NR30 = val;
					break;
				case 0x1B:
					reg.NR31 = val;
					break;
				case 0x1C:
					reg.NR32 = val;
					break;
				case 0x1D:
					reg.NR33 = val;
					break;
				case 0x1E:
					reg.NR34 = val;
					break;
				case 0x20:
					reg.NR41 = val;
					break;
				case 0x21:
					reg.NR42 = val;
					break;
				case 0x22:
					reg.NR43 = val;
					break;
				case 0x23:
					reg.NR44 = val;
					break;
				case 0x24:
					reg.NR50 = val;
					break;
				case 0x25:
					reg.NR51 = val;
					break;
				case 0x26:
					reg.NR52 = val;
					break;
				case 0x40:
					reg.LCDC = val;
					break;
				case 0x41:
					reg.STAT = val;
					break;
				case 0x42:
					reg.SCY = val;
					break;
				case 0x43:
					reg.SCX = val;
					break;
				case 0x44: // Special Case: According to docs, writing values to LY resets it.
					reg.LY = 0;
					break;
				case 0x45:
					reg.LYC = val;
					break;
				case 0x46:
					reg.DMA = val;
					break;
				case 0x47:
					reg.BGP = val;
					break;
				case 0x48:
					reg.OBP0 = val;
					break;
				case 0x49:
					reg.OBP1 = val;
					break;
				case 0x4A:
					reg.WY = val;
					break;
				case 0x4B:
					reg.WX = val;
					break;
				case 0xFF:
					reg.IE = val;
					break;
				default:
					io[index] = val;
					break;
			}
			
		}
		
		protected virtual void WriteHRAM(int index, byte val)
		{
			hram[index - 0xFF80] = val;
		}
		
		protected virtual void WriteFBlock(int index, byte val)
		{
			// Since this block has so many different purposes
			// and the behavior is mostly consistent across 
			// different cartridges, everything will be contained
			// within one function
			
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
					WriteInternalRAMEcho(index, val);
					break;
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
							WriteOAM(index, val);
							break;
						case 0xA: // A-F
						case 0xB: // are
						case 0xC: // empty
						case 0xD: // but
						case 0xE: // unusable
						case 0xF:
							WriteEmptyButUnusable(index, val);
							break;
					}
					break;
				case 0xF:
					switch((index & 0x00F0) >> 4)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							WriteIORegister(index&0xFF, val);
							break;
						case 4:
							if (index < 0xFF4C)
								WriteIORegister(index&0xFF, val);
							else
								WriteEmptyButUnusable(index, val);
							break;
						default:
							if (index < 0xFFFF)
								WriteHRAM(index, val);
							else
								WriteIORegister(index, val);
							break;						
					}
					break;
			}	
		}
		
		protected byte ReadFBlock(int index)
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
					return reg.IE;
				default:
					if (index < 0x4C)
						return io[index];
					else if (index >= 0x80)
						return hram[index - 0x80];
					else
						return (byte)0;
			}
		}
	}
}