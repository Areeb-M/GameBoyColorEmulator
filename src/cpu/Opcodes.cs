using System;
using System.Collections;
using System.Collections.Generic;

namespace Emulator
{
	class OpcodeTable
	{	
		public delegate void OpcodeFunction(CPU cpu, Memory mem);
		public static Dictionary<byte, OpcodeFunction> OPCODE_TABLE;
		
		public static void GenerateOpcodeTable()
		{
			OpcodeFunction nop = NOP;
			OpcodeFunction jump = JUMP;
			OpcodeFunction restart38 = RESTART38;
			OpcodeFunction compare = COMPARE;
			OpcodeFunction jumpForward = JUMP_FORWARD;
			OpcodeFunction jumpForwardIf = JUMP_FORWARD_IF;
			OpcodeFunction xor = XOR;
			OpcodeFunction loadNNintoN = LOAD_NN_INTO_N;
			OpcodeFunction disableInterrupts = DISABLE_INTERRUPTS;
			OpcodeFunction loadAintoMemN = LOAD_A_INTO_MEM_N;
			OpcodeFunction addRegToA = ADD_REG_TO_A;
			OpcodeFunction callNN = CALL_NN;
			OpcodeFunction loadMemNintoA = LOAD_MEM_N_INTO_A;
			OpcodeFunction pushRegPair = PUSH_REG_PAIR;
			OpcodeFunction loadAintoN = LOAD_A_INTO_N;
			OpcodeFunction loadNintoA = LOAD_N_INTO_A;
			OpcodeFunction and = AND;
			OpcodeFunction flagConditionalJump = FLAG_CONDITIONAL_JUMP;
			OpcodeFunction conditionalReturn = CONDITIONAL_RETURN;
			OpcodeFunction decrementRegister = DECREMENT_REGISTER;
			OpcodeFunction putR2inR1 = PUT_R2_IN_R1;
			OpcodeFunction prefixCB = PREFIX_CB;
			OPCODE_TABLE = new Dictionary<byte, OpcodeFunction>()
			{
				{0x00, nop},
				{0x01, loadNNintoN},
				{0x10, nop}, // this instruction is actually supposed to be STOP, but I don't have buttons implemented yet, so no can do
				{0x18, jumpForward},
				{0x1D, decrementRegister},
				{0x20, flagConditionalJump},
				{0x28, jumpForwardIf},
				{0x30, flagConditionalJump},
				{0x3D, decrementRegister},
				{0x3E, loadNintoA}, 
				{0x47, loadAintoN},
				{0x4F, loadAintoN},
				{0x7E, putR2inR1},
				{0x80, addRegToA},
				{0xA7, and},
				{0xA8, xor},
				{0xA9, xor},
				{0xAA, xor},
				{0xAB, xor},
				{0xAC, xor},
				{0xAD, xor},
				{0xAE, xor},
				{0xAF, xor},
				{0xB8, compare},
				{0xBA, compare},
				{0xC3, jump},
				{0xC5, pushRegPair},
				{0xC8, conditionalReturn},
				{0xCB, prefixCB},
				{0xCD, callNN},
				{0xD0, conditionalReturn},
				{0xD5, pushRegPair},
				{0xE0, loadAintoMemN},
				{0xE5, pushRegPair},
				{0xEA, loadAintoN},
				{0xEE, xor},
				{0xF0, loadMemNintoA},
				{0xF3, disableInterrupts},
				{0xF5, pushRegPair},
				{0xFA, loadNintoA},
				{0xFE, compare},
				{0xFF, restart38},	
			};
		}
			
	/*	Template Function
		public static void name(CPU cpu, Memory mem)
		{
			
		}
	*/
		public static void NOP(CPU cpu, Memory mem)
		{
			// NOP: does nothing
			cpu.PC += 1; // opcode length
			Debug.Log(": NOP");
		}
		
		public static void JUMP(CPU cpu, Memory mem)
		{
			// JUMP: Jumps to a location in memory
			int low = mem[++cpu.PC];
			int high = mem[++cpu.PC] << 8;
			cpu.PC = high | low;
			Debug.Log(": Jump to {0:X4}", cpu.PC);
		}
		
		public static void RESTART38(CPU cpu, Memory mem)
		{
			// Restart38: Restarts Gameboy from memory location 0x38
			mem[--cpu.SP] = (byte)(cpu.PC & 0x00FF);
			mem[--cpu.SP] = (byte)((cpu.PC & 0xFF00) >> 8);
			cpu.PC = 0x0038 + 1;
			Debug.Log(": restart from 0x0038");
		}
		
		public static void COMPARE(CPU cpu, Memory mem)
		{
			int a = cpu.A;
			int b;
			Debug.Log(": Compare regA({0:X4}) - ", a);
			switch(mem[cpu.PC]){
				case 0xB8:
					b = cpu.B;
					Debug.Log("regB({0:X4})", b);
					break;
				case 0xBA:
					b = cpu.D;
					Debug.Log("regD({0:X4})", b);
					break;
				case 0xFE:
					b = mem[++cpu.PC];
					Debug.Log("{0:X4}", b);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented compare opcode detected!");
					return;
			}
			int result = a - b;
			int f = 0x0000;
			f += ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f += 1; f <<= 2;                   // N Flag
			f += (((a&0xF) + ((-b)&0xF))&0x10)>>3; // H Flag
			f += ((result < 0) ? 1 : 0); f <<= 4;
			cpu.F = (byte)f;
			cpu.PC += 1;
			Debug.Log(" - reg[F] = {0}", Convert.ToString(cpu.F, 2).PadLeft(8, '0'), a, b);
		}
		
		public static void JUMP_FORWARD_IF(CPU cpu, Memory mem)
		{
			switch(mem[cpu.PC]){
				case 0x28:
					if ((cpu.F & 0x80) == 0x80){
						JUMP_FORWARD(cpu, mem);
					} else {
						cpu.PC += 2;
						Debug.Log(": Jump forward failed since Z flag was not set");
					}
					break;
				default:
					Debug.Log("\n[Error]Unimplemented jumpForwardIf opcode detected!");
					break;
			}
		}
		
		public static void JUMP_FORWARD(CPU cpu, Memory mem)
		{
			int n = 2 + (sbyte)mem[cpu.PC+1];
			cpu.PC += n;
			Debug.Log(": Jump forward by {1} to {0:X4}", cpu.PC, n);
		}
		
		public static void XOR(CPU cpu, Memory mem)
		{
			int a = cpu.A;
			int b;
			Debug.Log(": XOR regA({0:X4}) with ", a);
			switch (mem[cpu.PC]){
				case 0xAE:
					b = cpu.HL;
					Debug.Log("regHL({0:X4})", b);
					break;
				case 0xAF:
					b = cpu.A;
					Debug.Log("regA");
					break;
				default:
					Debug.Log("\n[Error]Unimplemented XOR opcode detected!");
					return;
			}
			cpu.A = (byte)(a ^ b);
			cpu.F = (cpu.A == 0) ? (byte)0x80 : (byte)0;
			cpu.PC += 1;
			Debug.Log(" to get {0}. Store in reg[A]. reg[F] = ", cpu.A);
			Debug.PrintBinary(cpu.F);
		}
		
		public static void LOAD_NN_INTO_N(CPU cpu, Memory mem)
		{
			int nn = mem[cpu.PC+2] << 8 + mem[cpu.PC+1];
			switch(mem[cpu.PC])
			{
				case 0x01:
					cpu.BC = nn;
					Debug.Log(": Store {0:X4} into regBC", nn);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented LOAD_NN_INTO_N opcode detected!");
					break;					
			}
			cpu.PC += 3;
		}
		
		public static void DISABLE_INTERRUPTS(CPU cpu, Memory mem)
		{			
			if (cpu.Interrupts)
				cpu.ToggleInterrupts = true;
			cpu.PC += 1;
			Debug.Log(": Disable interrupts opcode");
		}
				
		public static void LOAD_A_INTO_MEM_N(CPU cpu, Memory mem)
		{
			int address = 0xFF00 + mem[++cpu.PC];
			mem[address] = cpu.A;
			
			cpu.PC += 1;
			Debug.Log(": Loaded regA({1:X2}) into {0:X4}", address, cpu.A);
		}
		
		public static void ADD_REG_TO_A(CPU cpu, Memory mem)
		{
			byte a = cpu.A;
			byte b;
			Debug.Log(": Add regA({0}) and reg", a);
			switch(mem[cpu.PC])
			{
				case 0x80:
					b = cpu.B;
					Debug.Log("B({0})", b);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented addREGtoA opcode detected!");
					return;				
			}
			byte result = (byte)(a + b);
			cpu.A = result;
			int f = 0x00;
			f += ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f <<= 1;                   // Reset N Flag
			f += (((a&0xF) + (b&0xF)) & 0x10) >> 4; f <<= 1; // Half Carry flag
			f += (((a&0xF0) + (b&0xF0)) & 0x100) >> 8; // Full Carry flag
			f <<= 4;
			cpu.F = (byte)f;
			
			cpu.PC += 1;
			
			Debug.Log(" to get {0} and store it in regA - regF is ", result);
			Debug.PrintBinary(cpu.F);
		}
		
		public static void CALL_NN(CPU cpu, Memory mem)
		{
			//cpu.PC += 3;
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)(((cpu.PC+3) & 0xFF00) >> 8);
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)((cpu.PC+3) & 0xFF);
			
			Debug.Log(": Push {0:X4} onto the stack ", cpu.PC);
			
			//cpu.PC -= 3;
			JUMP(cpu, mem);
			//cpu.PC = mem[++cpu.PC] + mem[++cpu.PC] << 8;
			//Debug.Log("and jump to {0:X4}", cpu.PC);
		}
		
		public static void LOAD_MEM_N_INTO_A(CPU cpu, Memory mem)
		{
			int address = 0xFF00 + mem[++cpu.PC];
			cpu.A = mem[address];
			cpu.PC += 1;
			Debug.Log(": Load [{0:X4}]({1:X4}) into regA", address, cpu.A);
		}
		
		public static void PUSH_REG_PAIR(CPU cpu, Memory mem)
		{
			int nn;
			Debug.Log(": Push reg");
			switch(mem[cpu.PC])
			{
				case 0xF5:
					nn = cpu.AF;
					Debug.Log("AF({0:X4})", nn);
					break;
				case 0xC5:
					nn = cpu.BC;
					Debug.Log("BC({0:X4})", nn);
					break;
				case 0xD5:
					nn = cpu.DE;
					Debug.Log("DE({0:X4})", nn);
					break;
				case 0xE5:
					nn = cpu.HL;
					Debug.Log("HL({0:X4})", nn);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented pushRegPair opcode detected!");
					return;				
			}			
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)((nn & 0xFF00) >> 4);
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)(nn & 0xFF);
			cpu.PC += 1;
			Debug.Log(" onto the stack");
		}
		
		public static void LOAD_A_INTO_N(CPU cpu, Memory mem)
		{	
			Debug.Log(": Load regA({0:X4}) into ", cpu.A);		
			switch(mem[cpu.PC])
			{
				case 0x47:
					cpu.B = cpu.A;
					Debug.Log("regB");
					break;
				case 0x4F:
					cpu.C = cpu.A;
					Debug.Log("regC");
					break;
				case 0xEA:
					int address = mem[++cpu.PC] + (mem[++cpu.PC] << 8);
					mem[address] = cpu.A;
					Debug.Log("mem[{0:X4}]", address);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented loadAintoN opcode detected!");
					return;				
			}
			cpu.PC += 1;
		}
		
		public static void LOAD_N_INTO_A(CPU cpu, Memory mem)
		{
			int n;
			Debug.Log(": Load ");
			switch(mem[cpu.PC])
			{
				case 0x3E:
				case 0xFA:
					n = mem[++cpu.PC];
					Debug.Log(" mem[PC+1]({0:X4})", n);
					break;
				default:
					Debug.Log("[Error]Unimplemented LOAD_N_INTO_A opcode detected!");
					return;
			}
			cpu.A = (byte)n;
			cpu.PC += 1;
			Debug.Log(" into regA");
		}
		
		public static void AND(CPU cpu, Memory mem)
		{
			byte n, f;
			Debug.Log(": AND regA({0:X4}) with ", cpu.A);
			switch(mem[cpu.PC])
			{
				case 0xA7:
					n = cpu.A;
					Debug.Log(" regA ");
					break;
				default:
					Debug.Log("[Error]Unimplemented AND opcode detected!");
					return;
			}
			cpu.A &= n;
			f = (byte)((cpu.A == 0) ? 1 : 0); f <<= 1; // Flag Z
			f <<= 1; // reset Flag N
			f += 1; f <<= 1; // set Flag H
			f <<= 4; // reset Flag C
			cpu.F = f;
			
			cpu.PC += 1;
			
			Debug.Log(" to get {0:X4} - regF = ", cpu.A);
			Debug.PrintBinary(cpu.F);
		}
		
		public static void FLAG_CONDITIONAL_JUMP(CPU cpu, Memory mem)
		{
			int n = (sbyte)mem[cpu.PC + 1];
			Debug.Log(": Flag ");
			switch(mem[cpu.PC])
			{
				case 0x20:
					Debug.Log("NZ conditional jump ");
					if ((cpu.F & 0x80) == 0)
					{
						cpu.PC += n;
						Debug.Log(" passed, PC += " + n);
						return;
					}
					break;
				case 0x30:
					Debug.Log("NC conditional jump ");
					if ((cpu.F & 0x10) == 0)
					{
						cpu.PC += n;
						Debug.Log(" passed, PC += " + n);
						return;
					}
					break;
			}
			cpu.PC += 1;
			Debug.Log("failed");
		}
		
		public static void CONDITIONAL_RETURN(CPU cpu, Memory mem)
		{
			Debug.Log(": Flag  ");
			switch(mem[cpu.PC])
			{
				case 0xC8:
					Debug.Log("Z conditional return ");
					if ((cpu.F & 0x80) == 0x80)
					{
						int nn = mem[cpu.SP] + (mem[++cpu.SP] << 8);
						cpu.SP += 1;
						cpu.PC = nn;
						Debug.Log(" passed, PC = [{0:X4}]", cpu.PC);
						return;
					}
					break;
				case 0xD0:
					Debug.Log("NC conditional return ");
					if ((cpu.F & 0x10) == 0)
					{
						int nn = mem[cpu.SP] + (mem[++cpu.SP] << 8);
						cpu.SP += 1;
						cpu.PC = nn;
						Debug.Log("passed, PC = [{0:X4}]", cpu.PC);
						return;
					}
					break;
			}
			cpu.PC += 1;
			Debug.Log("failed");
		}
		
		public static void DECREMENT_REGISTER(CPU cpu, Memory mem)
		{
			int a, result, f;
			Debug.Log(": Decrement reg");
			switch(mem[cpu.PC])
			{
				case 0x1D:
					a = cpu.E;
					cpu.E -= 1;
					Debug.Log("E to get {0:X4}", cpu.E);
					break;
				case 0x3D:
					a = cpu.A;
					cpu.A -= 1;
					Debug.Log("A to get {0:X4}", cpu.A);
					break;
				default:
					Debug.Log("\n[ERROR]Unimplemented DECREMENT_REGISTER opcode detected");
					return;
			}
			result = a - 1;
			
			f = ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f += 1; f <<= 1;                   // Set N Flag
			f += 1 - ((((a&0xF) + ((-1)&0xF)) & 0x10) >> 4); f <<= 1; // No Half Borrow flag
			f <<= 4;
			f += cpu.F & 0x10; // keep flag C the same
			cpu.F = (byte)f;
			cpu.PC += 1;
			
			Debug.Log(" - regF = ");
			Debug.PrintBinary(cpu.F);
		}
		
		public static void PUT_R2_IN_R1(CPU cpu, Memory mem)
		{
			switch(mem[cpu.PC])
			{
				case 0x7E:
					Debug.Log(": Load regHL({0}) into reg A({1})", cpu.HL, cpu.A);
					cpu.A = (byte)cpu.HL;
					break;
			}
			cpu.PC += 1;
		}
		
		public static void PREFIX_CB(CPU cpu, Memory mem)
		{
			byte instruction = mem[++cpu.PC];
			Debug.Log("-{0:X4}", instruction);
			switch(instruction)
			{
				case 0x87:
					CB.RESET_BIT_B_IN_REG(cpu, mem);
					break;
				default:
					Debug.Log("[ERROR] Unimplemented CB prefix instruction {0:X2}", instruction);
					Console.ReadLine();
					break;
			}
			cpu.PC += 2;
		}
		
	}
	
	class CB
	{
		public static void RESET_BIT_B_IN_REG(CPU cpu, Memory mem)
		{
			byte b = mem[cpu.PC + 1];
			byte mask = (byte)(255 - (1 << b));
			Debug.Log(": reset bit " + b + " in ");
			switch(mem[cpu.PC])
			{
				case 0x87:
					Debug.Log("regA");
					cpu.A &= mask;
					break;
			}
		}
		
	}

}