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
			opcodeTable = new Dictionary<int, OpcodeFunction>(){
				{0x00, nop},
				{0xC3, jump}
			};
		}
		
		public bool tick(){
			int opcode = memory[PC];
			if(opcodeTable.ContainsKey(opcode)){
				opcodeTable[opcode]();
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
			PC = memory[PC+1]<<8 + memory[PC+2];
		}
	
	
	}

}