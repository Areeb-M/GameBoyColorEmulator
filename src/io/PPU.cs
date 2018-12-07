using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Emulator
{
	class PPU
	{
		byte[] vram;
		byte[] oam;
		byte[] output;
		
		int[] spriteList;
		bool[] spritePriority;
		
		LCD lcd;
		DirectBitmap finished;
		
		int ppuClock;
		int ppuState;
		bool rendered;

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
		
		public PPU(InterruptController interruptController, LCD display)
		{
			vram = new byte[0x2000];
			oam = new byte[40 * 4];
			// 4 bytes of data for each of 40 sprites
			output = new byte[256*256];
			// one color for each pixel
			
			lcd = display;
			finished = new DirectBitmap(160, 144);
			
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
			rendered = false;
		}
		
		public void Tick()
		{					
			if (((lcdControl.Data >> 7)&1) == 0)
			{
				return;
			} else 
			{
				if (!rendered)			
				{
					RenderFullBackground();
					rendered = true;
				}
			}
			ppuClock = (ppuClock + 1) % 114;
			
			if (scanLine.Data >= 144)
			{
				if (ppuClock == 0)
					scanLine.Data = (byte)((scanLine.Data + 1) % 154);
			}
			else if (ppuClock == 0)
			{
				ppuState = 0; // OAM Search
				scanLine.Data = (byte)((scanLine.Data + 1) % 154);		
				if (scanLine.Data == 144)
				{
					RefreshLCD();
					ppuState = 3;
				} else 
				{
					OAMSearch();
				}
			}
			else if (ppuClock == 20)
			{
				ppuState = 1; // Pixel Transfer
			} 
			else if (ppuClock == 62)
			{
				ppuState = 2; // HBlank
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
			int startY = 0;//scrollY.Data;
			
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
		
		public void RefreshLCD()
		{			
			int startX = scrollX.Data + 256; // Allows for backscroll without running into the negatives
			int startY = scrollY.Data + 256;
			
			Color c = Color.White;
			
			for(int y = 0; y < 144; y++)
			{
				for(int x = 0; x < 160; x++)
				{
					switch(GetPixel((x+startX)%256, (y+startY)%256))
					{
						case 0:
							c = Color.White;
							break;
						case 1:
							c = Color.Black;
							break;
						case 2:
							c = Color.Gray;
							break;
						case 3:
							c = Color.DarkGray;
							break;
					}
					
					finished.SetPixel(x, y, c);
					//image.SetPixel(2*(x-startX)+1, 2*(y-startY), c);
					//image.SetPixel(2*(x-startX), 2*(y-startY)+1, c);
					//image.SetPixel(2*(x-startX)+1, 2*(y-startY)+1, c);
				}
			}
			
			lcd.Refresh(finished.Bitmap);
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
		
		
		
		public void OAMSearch()
		{
			bool finished = false;
			spriteList = new int[]{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
			int index = 0;
			int pointer = 0;
			while (!finished)
			{
				if (pointer == 40 || index == 0)
				{
					finished = true;
					continue;
				}
			}
		}
		
		/*
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
		
		public void Fetch()
		{
			if (ppuState == 3)
				return;
			else if (ppuState == 1)
			{
				
			}
		}*/
		
		public void DumpVRAM()
		{
			System.IO.File.WriteAllBytes("vram.dmp", vram);
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
	}
	
	public class DirectBitmap : IDisposable
	{
		public Bitmap Bitmap { get; private set; }
		public Int32[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		public DirectBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new Int32[width * height];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
		}

		public void SetPixel(int x, int y, Color colour)
		{
			int index = x + (y * Width);
			int col = colour.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Bitmap.Dispose();
			BitsHandle.Free();
		}
	}
}