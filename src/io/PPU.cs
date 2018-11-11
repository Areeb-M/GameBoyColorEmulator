using System;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		Color[] output;
		
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
			output = new Color[160*144];
			// one color for each pixel
			
			ppuClock = 0;
			line = 0;
			
			PPUState = 0;
		}
		
		public void Tick()
		{
			ppuClock = (ppuClock + 1) % 114;
			
			if (ppuClock == 0)
				line = (line + 1) % 154;
			else if (ppuClock == 20)
				PPUState = 1;
			
			if (line >= 143) // V-Blank
				PPUState = 3;
			
			// 1 - OAM Search, find and select Sprites that should be visible in this line. oam.x != 0, LY + 16 >=oam.Y, LY + 16 < oam.y + h
			// 2 - Push Pixels to Screen
			// 3 - Remainder is H-Blank
		}		
		
		
		#region Data Access
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
		#endregion
		
		struct Color
		{
			public int r, g, b;
			public Color(int r, int g, int b)
			{
				this.r = r;
				this.g = g;
				this.b = b;
			}
		}
	}
}