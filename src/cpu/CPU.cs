using System;
using System.IO;
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
			set { stackPointer = value & 0xFFFF;}
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
			memory[0xFF44] = 0x91;
		}
		
		public CPU(string romPath, string savePath)
		{
			memory = new Memory(romPath);
			OpcodeTable.GenerateOpcodeTable();
			opcodeTable = OpcodeTable.OPCODE_TABLE;
			
			byte[] stateDump = File.ReadAllBytes(savePath);
			int lenCpuState = 2 + 2 + 1 + reg.Length;
			
			PC = stateDump[0] | (stateDump[1] << 8);
			SP = stateDump[2] | (stateDump[3] << 8);
			interrupts = (stateDump[4] & 0x1) == 0x1;
			toggleInterrupts = (stateDump[4] & 0x2) == 0x2;
			
			byte[] ramDump = new byte[stateDump.Length - lenCpuState];
			Array.Copy(stateDump, lenCpuState, ramDump, 0, ramDump.Length);
			memory.loadRAM(ramDump);
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
				Console.ReadKey();
				Debug.Log("\n");
				return true;
			} else {
				Debug.UnknownOpcode(PC, opcode);
				return false;
			}
				
		}
		
		public void SaveState(string path)
		{
			byte[] ramDump = memory.dumpRAM();
			byte[] cpuState = new byte[2 + 2 + 1 + reg.Length];
			cpuState[0] = (byte)(PC & 0xFF); // lower byte of PC
			cpuState[1] = (byte)((PC & 0xFF00) >> 8); // upper byte of PC
			cpuState[2] = (byte)(SP & 0xFF); // lower byte of SP
			cpuState[3] = (byte)((SP & 0XFF00) >> 8); // upper byte of SP
			cpuState[4] = (byte)((interrupts ? 1 : 0) + (toggleInterrupts ? 2 : 0));
			// bit 1 = interrupts, bit 2 = toggleInterrupts
			Array.Copy(reg, 0, cpuState, 5, reg.Length);
			
			FileStream fs = File.Create(path);
			
			fs.Write(cpuState, 0, cpuState.Length);
			fs.Write(ramDump, 0, ramDump.Length);
			fs.Flush();
			fs.Dispose();			
		}
	
	}

}