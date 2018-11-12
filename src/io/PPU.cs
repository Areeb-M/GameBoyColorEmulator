using System;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		Color[] output;
		
		DataBus<byte[]> VRAM;
		DataBus<byte[]> OAM;
		DataBus<Color[]> OUTPUT;
		
		int ppuClock;
		
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
			
			VRAM = new DataBus<byte[]>(vram);
			OAM = new DataBus<byte[]>(oam);
			OUTPUT = new DataBus<Color[]>(output);
			
			PPUState = 0;
			
			ppuClock = 0;
			
		}
		
		public void Tick()
		{
			switch(PPUState)
			{
				case 0:  // OAM Search
					break;
				case 1:  // Push Pixels to Screen
					break;
				case 2:  // H-Blank
				case 3:  // V-Blank
					break;
			}
			// 0 - OAM Search, find and select Sprites that should be visible in this line. oam.x != 0, LY + 16 >=oam.Y, LY + 16 < oam.y + h
			// 1 - Push Pixels to Screen
			// 2 - Remainder is H-Blank
			
			ppuClock = (ppuClock + 1) % 114;
			
			if (ppuClock == 0)
				reg.LY = (byte)((reg.LY + 1) % 154);
			else if (ppuClock == 20)
				PPUState = 1;
			
			if (reg.LY >= 143) // V-Blank
				PPUState = 3;
		}		
		
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