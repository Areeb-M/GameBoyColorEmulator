using System;
using System.Collections;
using System.Collections.Generic;


namespace Emulator{

	class CPU{
	
		#region Registers
		byte[] reg = new byte[8];
		int PC = 0x100;
		int SP = 0xFFFE;
		
		const int A = 0;
		const int B = 1;
		const int C = 2;
		const int D = 3;
		const int E = 4;
		const int F = 5;
		const int H = 6;
		const int L = 7;
		#endregion
	
		Memory memory;
		
		delegate void OpcodeFunction();
		Dictionary<int, OpcodeFunction> opcodeTable;
	
		public CPU(string romPath){
			memory = new Memory(romPath);
			OpcodeFunction nop = NOP;
			OpcodeFunction jump = JUMP;
			OpcodeFunction restart38 = RESTART38;
			OpcodeFunction compare = COMPARE;
			opcodeTable = new Dictionary<int, OpcodeFunction>(){
				{0x00, nop},
				{0xC3, jump},
				{0xFF, restart38},
				{0xFE, compare},
			};
		}
		
		public bool tick(){
			int opcode = memory[PC];
			if(opcodeTable.ContainsKey(opcode)){
				Console.Write("[{0:X2}]{1:X2}", PC, opcode);
				opcodeTable[opcode]();
				Console.ReadLine();
				return true;
			} else {
				Console.WriteLine("Encountered unknown opcode {0:X2} while executing.", opcode);
				return false;
			}
		}
	
		
		private void NOP(){
			// NOP: Doesn't do anything
			PC += 1; // OPCODE length
		}
	
		private void JUMP(){ 
			// JUMP: Jumps to a location in memory
			PC = memory[PC+1] + (memory[PC+2] << 8);
			Console.Write(": Jump to {0:X4}", PC);
		}
		
		private void RESTART38(){
			// Restart38: Restarts Gameboy from memory location 0x38
			memory[--SP] = (byte)(PC & 0x0000FFFF);
			memory[--SP] = (byte)((PC & 0xFFFF0000) >> 8);
			PC = 0x0038 + 1;
		}
		
		private void COMPARE(){
			int a = reg[A];
			int b;
			switch(memory[PC]){
				case 0xFE:
					b = memory[++PC];
					break;
				default:
					Console.WriteLine("[Error]Unimplemented compary opcode detected!");
					return;
			}
			int result = a - b;
			int f = 0x0000;
			f += ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f += 1; f <<= 2;                   // N Flag
			f += ((a&0xF) + ((-b)&0xF))&0x10; // H Flag
			f += ((result < 0) ? 1 : 0); f <<= 4;
			reg[F] = (byte)f;
			PC += 1;
		}	
	}

}