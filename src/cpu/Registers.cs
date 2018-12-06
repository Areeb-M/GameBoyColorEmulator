using System;

namespace Emulator
{
	class Registers
	{	
	
		public override string ToString()
		{
			string representation = "PC:[{0:X4}] SP:[{1:X4}] LY:[{10, 3}] ";
			representation += "reg[A:{2:X2} B:{3:X2} C:{4:X2} D:{5:X2} E:{6:X2} F:{7:X2} H:{8:X2} L:{9:X2}]";
			representation = String.Format(representation, PC, SP, A, B, C, D, E, F, H, L, LY);
			return representation;
		}
	
		public Registers(DataBus<byte>[] timerRegisters, DataBus<byte>[] displayRegisters)
		{
			// Timer Registers 4
			// Display Registers 12
			TIMER = timerRegisters;
			DISPLAY = displayRegisters;
		}
	
		private DataBus<byte>[] TIMER;
		private DataBus<byte>[] DISPLAY;
	
		#region PC & SP
		int programCounter = 0x100;
		int stackPointer = 0xFFFE;
		
		public int PC
		{
			get { return programCounter; }
			set { programCounter = value;}
		}
		public int SP
		{
			get { return stackPointer; }
			set { stackPointer = value;}
		}
		
		public byte S
		{
			get { return (byte)(stackPointer & 0xFF00); }
			set
			{
				stackPointer &= 255;
				stackPointer |= value << 8;
			}
		}
		
		public byte P
		{
			get { return (byte)(stackPointer & 0x00FF); }
			set
			{
				stackPointer &= 255 << 8;
				stackPointer |= value;
			}
		}
		#endregion

		#region Registers
		byte[] reg = new byte[8];
		
		public byte A
		{
			get {return reg[0]; }
			set {reg[0] = value;}
		}
		public byte B
		{
			get {return reg[1]; }
			set {reg[1] = value;}
		}
		public byte C
		{
			get {return reg[2]; }
			set {reg[2] = value;}
		}
		public byte D
		{
			get {return reg[3]; }
			set {reg[3] = value;}
		}
		public byte E
		{
			get {return reg[4]; }
			set {reg[4] = value;}
		}
		public byte F
		{
			get {return reg[5]; }
			set {reg[5] = value;}
		}
		public byte H
		{
			get {return reg[6]; }
			set {reg[6] = value;}
		}
		public byte L
		{
			get {return reg[7]; }
			set {reg[7] = value;}
		}
		public int AF
		{
			get 
			{
				return (A << 8) | F;
			}
			set 
			{
				value &= 0xFFFF;
				A = (byte)(value >> 8);
				F = (byte)(value & 0xF0);
			}
		}		
		public int BC
		{
			get 
			{
				return (B << 8) | C;
			}
			set 
			{
				value &= 0xFFFF;
				B = (byte)(value >> 8);
				C = (byte)(value & 0xFF);
			}
		}
		public int DE
		{
			get 
			{
				return (D << 8) | E;
			}
			set 
			{
				value &= 0xFFFF;
				D = (byte)(value >> 8);
				E = (byte)(value & 0xFF);
			}
		}
		public int HL
		{
			get 
			{
				return (H << 8) | L;
			}
			set 
			{
				value &= 0xFFFF;
				H = (byte)(value >> 8);
				L = (byte)(value & 0xFF);
			}
		}
		
		public bool fZ
		{
			get { return (F & 0x80) == 0x80; }
			set 
			{
				if (value)
					F |= 0x80;
				else
					F &= 0x7F;
			}
		}
		public bool fN
		{
			get { return (F & 0x40) == 0x40; }
			set 
			{
				if (value)
					F |= 0x40;
				else
					F &= 0xBF;
			}
		}
		public bool fH
		{
			get { return (F & 0x20) == 0x20; }
			set 
			{
				if (value)
					F |= 0x20;
				else
					F &= 0xDF;
			}
		}
		public bool fC
		{
			get { return (F & 0x10) == 0x10; }
			set 
			{
				if (value)
					F |= 0x10;
				else
					F &= 0xEF;
			}
		}
		#endregion
		
		#region Special IO Registers
		// Register Descriptions are from the Gameboy CPU Manual 
		byte[] io = new byte[43];
		
		public byte P1
		{
			// Register for reading joy pad info and determining system type.
			// Read/Write
			get { return io[0]; }
			set { io[0] = value;}
		}
		
		public byte SB
		{
			// Serial transfer data - 8 bits of data to be read/written
			// Read/Write
			get { return io[1]; }
			set { io[1] = value;}
		}
		
		public byte SC
		{
			// SIO control
			// Read/Write
			get { return io[2]; }
			set { io[2] = value;}
		}
		
		public byte DIV
		{
			// Divider Register
			// Read/Write
			get { return TIMER[0].Data; }
			set { TIMER[0].Data = 0; }
			// Special Case: writing any value sets this to 0
		}
		
		public byte TIMA
		{
			// Timer Counter
			// Read/Write
			get { return TIMER[1].Data; }
			set { TIMER[1].Data = value;}
		}
		
		public byte TMA
		{
			// Timer Modulo
			// Read/Write
			get { return TIMER[2].Data; }
			set { TIMER[2].Data = value;}
		}
		
		public byte TAC
		{
			// Timer Control
			// Read/Write
			get { return TIMER[3].Data; }
			set { TIMER[3].Data = value;}
		}
		
		public byte IF
		{
			// Interrupt Flag
			// Read/Write
			get { return io[7]; }
			set { io[7] = value;}
		}
		
		#region Sound Registers		
		public byte NR10
		{
			// Sound Mode 1 register, Sweep register
			// Read/Write
			get { return io[8]; }
			set { io[8] = value;}
		}
		
		public byte NR11
		{
			// Sound Mode 1 register, Sound Length/Wave pattern duty
			// Read/Write
			get { return io[9]; }
			set { io[9] = value;}
		}
		
		public byte NR12
		{
			// Sound Mode 1 register, Envelope
			// Read/Write
			get { return io[10]; }
			set { io[10] = value;}
		}
		
		public byte NR13
		{
			// Sound Mode 1 register, Frequency lo
			// Read/Write
			get { return io[11]; }
			set { io[11] = value;}
		}
		
		public byte NR14
		{
			// Sound Mode 1 register, Frequency hi
			// Read/Write
			get { return io[12]; }
			set { io[12] = value;}
		}
		
		public byte NR21
		{
			// Sound Mode 2 register, Sound Length; Wave Pattern Duty
			// Read/Write
			get { return io[13]; }
			set { io[13] = value;}
		}
		
		public byte NR22
		{
			// Sound Mode 2 register, envelope
			// Read/Write
			get { return io[14]; }
			set { io[14] = value;}
		}
		
		public byte NR23
		{
			// Sound Mode 2 register, frequency lo
			// Read/Write
			get { return io[15]; }
			set { io[15] = value;}
		}		
		
		public byte NR24
		{
			// Sound Mode 2 register, frequency hi
			// Read/Write
			get { return io[16]; }
			set { io[16] = value;}
		}
		
		public byte NR30
		{
			// Sound Mode 3 register, Sound on/off
			// Read/Write
			get { return io[17]; }
			set { io[17] = value;}
		}		
		
		public byte NR31
		{
			// Sound Mode 3 register, Sound length
			// Read/Write
			get { return io[18]; }
			set { io[18] = value;}
		}
		
		public byte NR32
		{
			// Sound Mode 3 register, Select output
			// Read/Write
			get { return io[19]; }
			set { io[19] = value;}
		}
		
		public byte NR33
		{
			// Sound Mode 3 register, Frequency lo
			// Read/Write
			get { return io[20]; }
			set { io[20] = value;}
		}	
		
		public byte NR34
		{
			// Sound Mode 3 register, Frequency hi
			// Read/Write
			get { return io[21]; }
			set { io[21] = value;}
		}	
		
		public byte NR41
		{
			// Sound Mode 4 register, sound length
			// Read/Write
			get { return io[21]; }
			set { io[21] = value;}
		}
			
		public byte NR42
		{
			// Sound Mode 4 register, envelope
			// Read/Write
			get { return io[22]; }
			set { io[22] = value;}
		}
			
		public byte NR43
		{
			// Sound Mode 4 register, polynomial counter
			// Read/Write
			get { return io[23]; }
			set { io[23] = value;}
		}
			
		public byte NR44
		{
			// Sound Mode 4 register, counter/consecutive; initial
			// Read/Write
			get { return io[24]; }
			set { io[24] = value;}
		}
			
		public byte NR50
		{
			// Channel control / ON-OFF / Volume 
			// Read/Write
			get { return io[25]; }
			set { io[25] = value;}
		}
		
		public byte NR51
		{
			// Selection of Sound output terminal
			// Read/Write
			get { return io[26]; }
			set { io[26] = value;}			
		}
		
		public byte NR52
		{
			// Sound on/off
			// Read/Write
			get { return io[27]; }
			set { io[27] = value;}						
		}
		#endregion // Sound
		
		public byte LCDC
		{
			// LCD Control
			// Read/Write
			get { return DISPLAY[0].Data; }
			set { DISPLAY[0].Data = value;}				
		}
		
		public byte STAT
		{
			// LCDC status
			// Read/Write
			get { return DISPLAY[1].Data; }
			set { DISPLAY[1].Data = value;}				
		}		
		
		public byte SCY
		{
			// Scroll Y
			// Read/Write
			get { return DISPLAY[2].Data; }
			set { DISPLAY[2].Data = value;}				
		}
		
		public byte SCX
		{
			// Scroll X
			// Read/Write
			get { return DISPLAY[3].Data; }
			set { DISPLAY[3].Data = value;}				
		}
		
		public byte LY
		{
			// LCDC Y-Coordinate
			// Read/Write
			get { return DISPLAY[4].Data; }
			set { DISPLAY[4].Data = value;}				
		}
		
		public byte LYC
		{
			// LY Compare
			// Read/Write
			get { return DISPLAY[5].Data; }
			set { DISPLAY[5].Data = value;}				
		}
		
		public byte DMA
		{
			// DMA transfer and start address
			// Read/Write
			get { return DISPLAY[6].Data; }
			set { DISPLAY[6].Data = value;}				
		}
		
		public byte BGP
		{
			// BG and Window Palette Data
			// Read/Write
			get { return DISPLAY[7].Data; }
			set { DISPLAY[7].Data = value;}					
		}
		
		public byte OBP0
		{
			// Object Palette 0 Data
			// Read/Write
			get { return DISPLAY[8].Data; }
			set { DISPLAY[8].Data = value;}					
		}
		
		public byte OBP1
		{
			// Object Palette 1 data
			// Read/Write
			get { return DISPLAY[9].Data; }
			set { DISPLAY[9].Data = value;}					
		}
		
		public byte WY
		{
			// Window Y Position
			// Read/Write
			get { return DISPLAY[10].Data; }
			set { DISPLAY[10].Data = value;}					
		}
		
		public byte WX
		{
			// Window X Position
			// Read/Write
			get { return DISPLAY[11].Data; }
			set { DISPLAY[11].Data = value;}		
		}
		
		public byte IE
		{
			// Interrupt Enable 
			// Read/Write
			get { return io[40]; }
			set { io[40] = value;}				
		}		
		#endregion // Registers
		
	}
	
}