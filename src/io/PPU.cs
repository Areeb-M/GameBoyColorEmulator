using System;
using System.Collections.Generic;

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
			
			// Assign Default Values
			lcdControl = new DataBus<byte>(0);
			lcdStatus = new DataBus<byte>(0);
			scrollY = new DataBus<byte>(0);
			scrollX = new DataBus<byte>(0);
			scanLine = new DataBus<byte>(0);
			scanLineCompare = new DataBus<byte>(0);
			dmaTransferAddress = new DataBus<byte>(0);
			bgPalette = new DataBus<byte>(0);
			obj0Palette = new DataBus<byte>(0);
			obj1Palette = new DataBus<byte>(0);
			windowY = new DataBus<byte>(0);
			windowX = new DataBus<byte>(0);
			
			ppuState = 0;			
			ppuClock = 0;
			
		}
		
		public void Tick()
		{			
			ppuClock = (ppuClock + 1) % 114;
			
			if (ppuClock == 0)
			{
				scanLine.Data = (byte)((scanLine.Data + 1) % 154);
			}
			else if (ppuClock == 20)
			{
				ppuState = 1; // Pixel Transfer
			}
			
			if (scanLine.Data >= 143) // V-Blank
			{
				ppuState = 3;
			}
		}
		
		int bgMapAddress = 0x1800;
		Queue<byte> pixelQueue = new Queue<byte>();
		
		
		public void ResetFIFO()
		{
			bgMapAddress = 0x1800;
			pixelQueue = new byte[16];
		}
		
		public void FIFO()
		{
			if (ppuState == 3)
				return;
			else if (ppuState == 1)
			{
				if (pixelQueue.Count < 8)
					return
				else
					Console.WriteLine();
			}
		}
		
		public void Fetch()
		{
			if (ppuState == 3)
				return;
			else if (ppuState == 1)
			{
				
			}
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