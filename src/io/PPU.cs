using System;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		
		int ppuClock;
		int line;
		
		int PPUState;
		// 0 - OAM Search
		// 1 - Pixel Transfer
		// 2 - H-Blank
		// 3 - V-Blank
		
		public PPU()
		{
			vram = new byte[0x2000];
			oam = new byte[40 * 4];
			// 4 bytes of data for each of 40 sprites
			ppuClock = 0;
			line = 0;
			
			PPUState = 0;
		}
		
		
		public byte ReadVRAM(int index)
		{
			switch(PPUState)
			{
				case 0:
				case 2:
				case 3:
					return vram[index];
				case 1:
				default:
					return 0xFF;
			}
		}	
		
		public byte ReadOAM(int index)
		{
			switch(PPUState)
			{
				case 2:
				case 3:
					return oam[index];
				case 0:
				case 1:
				default:
					return 0xFF;
			}
		}
		
		public void WriteVRAM(int index, byte val)
		{
			switch(PPUState)
			{
				case 0:
				case 2:
				case 3:
					vram[index] = val;
					break;
				case 1:
				default:
					break;
			}
		}
		
		public void WriteOAM(int index, byte val)
		{
			switch(PPUState)
			{
				case 2:
				case 3:
					oam[index] = val;
					break;
				case 0:
				case 1:
				default:
					break;
			}
		}
	}
}