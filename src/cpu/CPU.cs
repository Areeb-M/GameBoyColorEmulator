using System;
using System.Collections;
using System.Collections.Generic;


namespace Emulator
{
	
	class CPU
	{
	
		#region Registers
		byte[] reg = new byte[8];
		int programCounter = 0x100;
		int stackPointer = 0xFFFE;
		
		public int PC
		{
			get { return programCounter; }
			set { programCounter = value & 0xFFFF;}
		}
		public int SP
		{
			get { return stackPointer; }
			set { stackPointer = value;}
		}
		
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
				return A << 8 + F;
			}
			set 
			{
				A = (byte)(value >> 8);
				F = (byte)(value & 0xFF);
			}
		}		
		public int BC
		{
			get 
			{
				return B << 8 + C;
			}
			set 
			{
				B = (byte)(value >> 8);
				C = (byte)(value & 0xFF);
			}
		}
		public int DE
		{
			get 
			{
				return D << 8 + E;
			}
			set 
			{
				D = (byte)(value >> 8);
				E = (byte)(value & 0xFF);
			}
		}
		public int HL
		{
			get 
			{
				return H << 8 + L;
			}
			set 
			{
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
	
		Memory memory;
		Dictionary<byte, OpcodeTable.OpcodeFunction> opcodeTable;
	
		bool interrupts = true;
		bool toggleInterrupts = false;
		public bool ToggleInterrupts
		{
			get { return toggleInterrupts; }
			set { toggleInterrupts = value;}
		}
	
		public bool Interrupts
		{
			get { return interrupts; }
		}
	
		public CPU(string romPath)
		{
			memory = new Memory(romPath);
			OpcodeTable.GenerateOpcodeTable();
			opcodeTable = OpcodeTable.OPCODE_TABLE;
			
			// Test
			//A = 0x11;
		}
		
		public bool tick(){
			byte opcode = memory[PC];
			if (opcodeTable.ContainsKey(opcode)){
				Debug.Opcode(PC, opcode);			// DEBUG
				opcodeTable[opcode](this, memory);
				
				if (toggleInterrupts && opcode != 0xF3 && opcode != 0xFB){
					toggleInterrupts = false;
					interrupts = !interrupts;
					Debug.Log(" - Interrupts are now {0}", interrupts);
				}
				//Console.ReadKey();
				Debug.Log("\n");
				return true;
			} else {
				Debug.UnknownOpcode(PC, opcode);
				return false;
			}
				
		}
	
	}

}