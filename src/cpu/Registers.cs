using System;

namespace Emulator.CPU
{
	class Register
	{	
		#region PC & SP
		int programCounter = 0x100;
		int stackPointer = 0xFFFE;
		
		public PC
		{
			get { return programCounter; }
			set { programCounter = value;}
		}
		public SP
		{
			get { return stackPointer; }
			set { stackPointer = value;}
		}
		#endregion

		#region Registers
		byte[] registers = new byte[8];
		
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
		byte[] IOregisters = new byte[43];
		
		
		#endregion
		
		
	}
	
}