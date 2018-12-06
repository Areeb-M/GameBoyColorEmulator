using System;
using System.Collections;
using System.Collections.Generic;


namespace Emulator
{	
	static class Opcode
	{
		private class ZMath // Short for Z80 Math
		{
			public static bool CheckHalfCarry(int a, int b) // (a) + (b)
			{
				return (((a&0xF) + (b&0xF)) & 0x10) == 0x10;
			}		
			
			public static bool CheckHalfBorrow(int a, int b) // (a) - (b)
			{
				return (a & 0xF) > (b & 0xF);
			}
			
		}		
	
		public static IEnumerable<bool> NOP(Memory mem, Registers reg) // 0x00
		{
			Debug.Log("NOP");
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_N_D16(Memory mem, Registers reg)
		{						
			// 16 bit load
			byte high = mem[reg.PC + 2];
			byte low = mem[reg.PC + 1];	
			
			switch(mem[reg.PC])
			{
				case 0x11:
					reg.E = low;
					yield return true;
					reg.D = high;
					yield return true;
					Debug.Log("LD DE, d16");
					break;	
				case 0x21:
					reg.L = low;
					yield return true;
					reg.H = high;
					yield return true;
					Debug.Log("LD HL, d16");
					break;					
				case 0x31:
					reg.P = low;
					yield return true;
					reg.S = high;
					yield return true;
					Debug.Log("LD SP, d16");
					break;
			}			
			reg.PC += 3;
			
			yield break;
		}
		
		public static IEnumerable<bool> XOR(Memory mem, Registers reg)
		{
			int a = reg.A;
			int b;
			switch (mem[reg.PC]){
				case 0xAE:
					b = mem[reg.HL];
					break;
				case 0xAF:
					b = reg.A;
					Debug.Log("XOR A");
					break;
				default:
					Debug.Log("\n[Error]Unimplemented XOR opcode detected!");
					yield break;
			}
			reg.A = (byte)(a ^ b);
			reg.fZ = reg.A == 0;
			reg.fN = false;
			reg.fH = false;
			reg.fC = false;
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> LDD_HL_A(Memory mem, Registers reg) // LDD (HL), A
		{
			mem[reg.HL] = reg.A;
			yield return true;
			
			Debug.Log("LDD (HL), A");
			//Console.WriteLine("{0:X4}: {1:X2}", reg.HL, reg.A);
			reg.PC += 1;
			reg.HL -= 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> PREFIX_CB(Memory mem, Registers reg)
		{
			yield return true;
			foreach(bool b in PrefixCB.HandleCB(mem, reg))
			{
				yield return b;
			}
			
			reg.PC += 2;
			
			yield break;
		}
		
		public static IEnumerable<bool> JR_CC_R8(Memory mem, Registers reg)
		{
			int n = (sbyte)mem[reg.PC + 1];
			yield return true;
			
			switch(mem[reg.PC])
			{
				case 0x20:
					Debug.Log("JR NZ, r8");
					if (!reg.fZ)
					{
						reg.PC += n;
						yield return true;
					}
					break;
				case 0x28:
					Debug.Log("JR Z, r8");
					if (reg.fZ)
					{
						reg.PC += n;
						yield return true;
					}
					break;
			}
			reg.PC += 2;
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_N_D8(Memory mem, Registers reg)
		{
			byte n = mem[reg.PC + 1];
			yield return true;
			
			switch(mem[reg.PC])
			{
				case 0x06:
					Debug.Log("LD B, d8");
					reg.B = n;
					break;
				case 0x0E:
					Debug.Log("LD C, d8");
					reg.C = n;
					break;
				case 0x1E:
					Debug.Log("LD E, d8");
					reg.E = n;
					break;
				case 0x2E:
					Debug.Log("LD L, d8");
					reg.L = n;
					break;
				case 0x3E:
					Debug.Log("LD A, d8");
					reg.A = n;
					break;
			}
			reg.PC += 2;
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_0xFFCC_A(Memory mem, Registers reg)
		{
			Debug.Log("LD ($FF00+C), A");
			mem[0xFF00 + reg.C] = reg.A;
			yield return true;
			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> INCREMENT_REG(Memory mem, Registers reg)
		{
			byte val;
			switch(mem[reg.PC])
			{
				case 0x04:
					val = reg.B;
					reg.B += 1;
					Debug.Log("INC B");
					break;
				case 0x0C:
					val = reg.C;
					reg.C += 1;
					Debug.Log("INC C");
					break;
				case 0x24:
					val = reg.H;
					reg.H += 1;
					Debug.Log("INC H");
					break;
				default:
					throw new InvalidOperationException("Increment instruction has not been implemented yet!");
					
			}
			reg.fZ = (reg.C == 0) ? true : false;
			reg.fN = false;
			reg.fH = ZMath.CheckHalfCarry(val, 1);
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_N_A(Memory mem, Registers reg)
		{
			switch(mem[reg.PC])
			{
				case 0x4F:
					reg.C = reg.A;
					Debug.Log("LD C, A");
					break;
				case 0x57:
					reg.D = reg.A;
					Debug.Log("LD D, A");
					break;
				case 0x67:
					reg.H = reg.A;
					Debug.Log("LD H, A");
					break;
				case 0x77:
					mem[reg.HL] = reg.A;
					Debug.Log("LD HL, A");
					yield return true;
					break;
				case 0xEA:
					byte lower = mem[reg.PC + 1];
					yield return true;
					byte upper = mem[reg.PC + 2];
					yield return true;
					mem[(upper << 8) | lower] = reg.A;
					Debug.Log("LD (d16), A");
					reg.PC += 2;
					break;
			}
			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> LD_FFNN_A(Memory mem, Registers reg)
		{
			byte n = mem[reg.PC + 1];
			yield return true;
			
			mem[0xFF00 + n] = reg.A;
			yield return true;
			
			Debug.Log("LD (&FF00 + n), A");
			reg.PC += 2;
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_A_N(Memory mem, Registers reg)
		{
			switch(mem[reg.PC])
			{
				case 0x1A:
					reg.A = mem[reg.DE];
					Debug.Log("LD A, (DE)");
					yield return true;
					break;
				case 0x7B:
					reg.A = reg.E;
					Debug.Log("LD A, E");
					break;
				case 0x7C:
					reg.A = reg.H;
					Debug.Log("LD A, H");
					break;
			}
			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> CALL_NN(Memory mem, Registers reg)
		{
			byte low = mem[reg.PC + 1];
			yield return true;
			
			byte high = mem[reg.PC + 2];
			yield return true;
			
			reg.SP -= 1;
			yield return true; // There is no documentation on how this instruction works internally
							   // So this is mostly the product of guesswork 	
			int address = reg.PC + 3;		
			mem[reg.SP] = (byte)((address & 0xFF00) >> 8);
			yield return true;
			
			mem[--reg.SP] = (byte)(address & 0xFF);
			yield return true;
			
			Debug.Log("CALL d16");
			reg.PC = (high << 8) | low;		
			yield break;			
		}
		
		public static IEnumerable<bool> PUSH(Memory mem, Registers reg)
		{
			yield return true;
			switch(mem[reg.PC])
			{
				case 0xC5:
					mem[--reg.SP] = reg.B;
					yield return true;					
					mem[--reg.SP] = reg.C;
					yield return true;
					Debug.Log("PUSH BC");
					break;
			}			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> POP(Memory mem, Registers reg)
		{
			switch(mem[reg.PC])
			{
				case 0xC1:
					reg.C = mem[reg.SP++];
					yield return true;
					reg.B = mem[reg.SP++];
					yield return true;
					Debug.Log("POP BC");
					break;
			}			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> RLA(Memory mem, Registers reg)
		{
			Debug.Log("RLA");
			int val = (reg.A << 1) + ((reg.fC) ? 1 : 0);
			
			reg.fZ = false;
			reg.fN = false;
			reg.fH = false;
			reg.fC = val > 0xFF;
			
			reg.A = (byte)val;
			
			//Console.WriteLine("{0} {1}", val, reg.A);
			
			reg.PC += 1;
			
			yield break;			
		}
		
		public static IEnumerable<bool> DECREMENT_REG(Memory mem, Registers reg)
		{
			byte val = 0;
			switch(mem[reg.PC])
			{
				case 0x05:
					val = reg.B;
					reg.B -= 1;
					Debug.Log("DEC B");
					break;
				case 0x15:
					val = reg.D;
					reg.D -= 1;
					Debug.Log("DEC D");
					break;
				case 0x0D:
					val = reg.C;
					reg.C -= 1;
					Debug.Log("DEC C");
					break;
				case 0x1D:
					val = reg.E;
					reg.E -= 1;
					Debug.Log("DEC E");
					break;
				case 0x3D:
					val = reg.A;
					reg.A -= 1;
					Debug.Log("DEC A");
					break;
			}
			reg.fZ = (val - 1) == 0;
			reg.fN = true;
			reg.fH = ZMath.CheckHalfBorrow(val, 1);
			
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> LDI_HL_A(Memory mem, Registers reg)
		{
			mem[reg.HL++] = reg.A;
			yield return true;
		
			Debug.Log("LDI (HL++), A");
			reg.PC += 1;
		
			yield break;
		}
		
		public static IEnumerable<bool> INCREMENT_16_REG(Memory mem, Registers reg)
		{			
			switch(mem[reg.PC])
			{
				case 0x13:
					reg.DE += 1;
					Debug.Log("INC DE");
					break;
				case 0x23:
					reg.HL += 1;
					Debug.Log("INC HL");
					break;
			}
			yield return true;
			reg.PC += 1;
			
			yield break;
		}
		
		public static IEnumerable<bool> RETURN(Memory mem, Registers reg)
		{
			byte lower = mem[reg.SP++];
			yield return true;
			byte upper = mem[reg.SP++];
			yield return true;			
			reg.PC = (upper << 8) | lower;
			yield return true;
			
			Debug.Log("RET");
			
			yield break;
		}
		
		public static IEnumerable<bool> COMPARE(Memory mem, Registers reg)
		{
			byte val = 0;
			switch(mem[reg.PC])
			{
				case 0xFE:
					val = mem[reg.PC + 1];
					Debug.Log("CP d8");
					yield return true;
					break;
			}
			reg.fZ = reg.A == val;
			reg.fN = true;
			reg.fH = !ZMath.CheckHalfBorrow(reg.A, val);
			reg.fC = reg.A < val;
			
			reg.PC += 2;
			
			yield break;
		}
		
		public static IEnumerable<bool> JR_R8(Memory mem, Registers reg)
		{
			int n = (sbyte)mem[reg.PC + 1];
			yield return true;
			
			reg.PC += 2 + n; // Account for length of JR instruction
			yield return true;
			
			Debug.Log("JR r8");
			
			yield break;
		}
		
		public static IEnumerable<bool> LOAD_A_FFNN(Memory mem, Registers reg)
		{
			byte n = mem[reg.PC + 1];
			yield return true;
			reg.A = mem[0xFF00 + n];
			yield return true;
			
			Debug.Log("LD A, (0xFF00 + d8)");
			reg.PC += 2;
			
			yield break;			
		}
		
		public static IEnumerable<bool> SUB(Memory mem, Registers reg)
		{
			byte val = reg.A;
			switch(mem[reg.PC])
			{
				case 0x90:
					val = reg.B;
					Debug.Log("SUB B");
					break;
				default:
					throw new InvalidOperationException("Subtraction instruction has not been implemented yet!");
			}
			reg.fZ = reg.A - val == 0;
			reg.fN = true;
			reg.fH = !ZMath.CheckHalfBorrow(reg.A, val);
			reg.fC = reg.A > val;
			reg.A -= val;
			
			reg.PC += 1;
			yield break;
		}
		
		/*
		public static IEnumerable<bool> LOAD_R2_R1(Memory mem, Registers reg)
		{
			Implement Later
		}*/
	}
	
	static class PrefixCB
	{
		public static IEnumerable<bool> HandleCB(Memory mem, Registers reg)
		{
			Debug.Log("CB-{0:X2}: ", mem[reg.PC+1]);
			switch(mem[reg.PC+1])
			{
				case 0x11:
					return RL(mem, reg);
				case 0x7C:
					return BIT(mem, reg);
				default:
					throw new InvalidOperationException("CB prefix instruction has not been implemented yet!");
			}
		}
		
		private static IEnumerable<bool> BIT(Memory mem, Registers reg)
		{
			Debug.Log("BIT ");
			int t = 0;
			switch(mem[reg.PC+1])
			{
				case 0x7C:
					Debug.Log("7, H");
					t = reg.H & (1 << 7);
					break;
			}
			reg.fZ = (t&0xFF) == 0;
			reg.fN = false;
			reg.fH = true;
			
			yield break;
		}
		
		private static IEnumerable<bool> RL (Memory mem, Registers reg) // Rotate Left, store bit 7 in fC
		{
			Debug.Log("RL ");
			int val = 0;
			switch(mem[reg.PC+1])
			{
				case 0x11:
					val = (reg.C << 1) + ((reg.fC) ? 1 : 0);
					reg.C = (byte)val;
					Debug.Log("C");
					break;
			}
			reg.fZ = (val&0xFF) == 0;
			reg.fN = false;
			reg.fH = false;
			reg.fC = val > 0xFF;			
			
			yield break;
		}
	}
}