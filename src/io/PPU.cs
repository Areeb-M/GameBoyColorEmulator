using System;
using System.Linq;
using System.Collections.Generic;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		byte[] output;
		
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
			output = new byte[256*256];
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
			if (((lcdControl.Data >> 7)&1) == 0)
			{
				return;
			}
			ppuClock = (ppuClock + 1) % 114;
			
			if (ppuClock == 0)
			{
				ppuState = 0; // OAM Search
				scanLine.Data = (byte)((scanLine.Data + 1) % 154);
			}
			else if (ppuClock == 20)
			{
				ppuState = 1; // Pixel Transfer
			} else if (ppuClock == 62)
			{
				ppuState = 2; // HBlank
			}
			
			if (scanLine.Data == 144) // V-Blank
			{
				ppuState = 3;
				//RenderFullBackground();
				//OutputScreen();
			}
		}
		
		int bgMapAddress = 0x1800;
		Queue<byte> pixelQueue = new Queue<byte>();
		string[] shades = new string[]{" ", "|", "=", "O"};
		
		
		public void ResetFIFO()
		{
			bgMapAddress = 0x1800;
			pixelQueue = new Queue<byte>();
		}
		
		public void OutputScreen()
		{
			int startX = scrollX.Data;
			int startY = scrollY.Data;
			
			Console.Write("\n---------------------");
			
			for(int y = startY; y < startY +  144; y++)
			{
				string line = "";
				for(int x = startX; x < startX + 160; x++)
				{
					line += shades[GetPixel(x%256, y%256)];
				}
				Console.WriteLine(line);
			}
		}
		
		public byte GetPixel(int x, int y)
		{
			return output[x + 256*y];
		}
		
		public void RenderFullBackground()
		{
			//Console.WriteLine("[{0:X}]", string.Join(", ", vram));
			
			int tileAddress = 0;
			for(int y = 0; y < 32; y++)
			{
				for(int ly = 0; ly < 8; ly++)
				{
					for(int x = 0; x < 32; x++)
					{
						tileAddress = 16 * vram[0x1800 + x + 32*y] + 2*ly;
						//Console.WriteLine("{0:X4}: {1}", 0x8000 + 0x1800 + x + y*32, vram[0x1800 + x + 32*y]);
						
						for (int shift = 7; shift >= 0; shift--)
						{
							int low = vram[tileAddress] >> shift;
							low &= 0x1;
							int high = vram[tileAddress+1] >> shift;
							high &= 0x1;
							output[256*(8*y+ly) + 8*x + (7-shift)] = (byte)((high << 1) | low);
							//Console.Write(shades[(high << 1)|low]);							
						}
					}					
					//}
					//Console.WriteLine();
				}
			}
		}
		
		public void PrintTile(int i)
		{
			int index = i;
			Console.WriteLine("----------------");
			for(int y = 0; y < 8; y++)
			{
				int tileAddress = 16 * i + 2*y;
				for (int shift = 7; shift > 0; shift--)
				{
					int low = vram[tileAddress] >> shift;
					low &= 0x1;
					int high = vram[tileAddress+1] >> shift;
					high &= 0x1;
					Console.Write(shades[(high << 1)|low]);
				}
				Console.WriteLine();
			}
		}
		
		public void PrintBGMap()
		{
			for(int y = 0; y < 32; y++)
			{
				for(int x = 0; x < 32; x++)
				{
					Console.Write("{0:X2},", vram[x+32*y]);
				}
				Console.WriteLine();
			}
		}
		
		public void FIFO()
		{
			if (ppuState == 3)
				return;
			else if (ppuState == 1)
			{
				if (pixelQueue.Count < 8)
					return;
				else
					Console.WriteLine();
			}
		}
		
		public void DumpVRAM()
		{
			System.IO.File.WriteAllBytes("vram.dmp", vram);
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