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
			set { programCounter = value;}
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
		}
		
		public bool tick(){
			byte opcode = memory[PC];
			if (opcodeTable.ContainsKey(opcode)){
				Debug.Opcode(PC, opcode);			// DEBUG
				opcodeTable[opcode](this, memory);
				
				if (toggleInterrupts && opcode != 0xFE && opcode != 0xFB){
					toggleInterrupts = false;
					interrupts = !interrupts;
				}
				
				Console.ReadLine();
				return true;
			} else {
				Debug.UnknownOpcode(PC, opcode);
				return false;
			}
				
		}
	
	}

}