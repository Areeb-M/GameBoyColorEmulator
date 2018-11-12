using System;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		Color[] output;
		
		int ppuClock;
		int ppuState;

		DataBus<byte> lcdControl;
		DataBus<byte> lcdStatus;
		DataBus<byte> scrollY;
		DataBus<byte> scrollX;
		DataBus<byte> scanLine;
		DataBus<byte> scanLineCompare;
		DataBus<byte> dmaTransferAddress;
		DataBus<byte> bgPalette;
		DataBus<byte> obj0Palette;
		DataBus<byte> obj1Palette;
		DataBus<byte> windowY;
		DataBus<byte> windowX;
		
		public DataBus<byte>[] DisplayRegisters
		{
			get 
			{
				return new DataBus<byte>[]{
					lcdControl,
					lcdStatus,
					scrollY,
					scrollX,
					scanLine,
					scanLineCompare,
					dmaTransferAddress,
					bgPalette,
					obj0Palette,
					obj1Palette,
					windowY,
					windowX
				};
			}
		}		
		
		// 0 - OAM Search
		// 1 - Pixel Transfer
		// 2 - H-Blank
		// 3 - V-Blank
		
		public PPU(InterruptController interruptController)
		{
			vram = new byte[0x2000];
			oam = new byte[40 * 4];
			// 4 bytes of data for each of 40 sprites
			output = new Color[160*144];
			// one color for each pixel
			
			OUTPUT = new DataBus<Color[]>(output);
			
			ppuState = 0;			
			ppuClock = 0;
			
		}
		
		public void Tick()
		{
			switch(ppuState)
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
				scanLine = (byte)((scanLine + 1) % 154);
			else if (ppuClock == 20)
				PPUState = 1;
			
			if (scanLine >= 143) // V-Blank
				PPUState = 3;
		}		
		
		#region Data Access
		public byte ReadVRAM(int index)
		{
			switch(ppuState)
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
			switch(ppuState)
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
			switch(ppuState)
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
			switch(ppuState)
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