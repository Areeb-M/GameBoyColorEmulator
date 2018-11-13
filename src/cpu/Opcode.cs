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
				return ((((a&0xF) + ((0-b)&0xF)) & 0x10) >> 4) == 1;
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
		
		public static IEnumerable<bool> JR_CC_N(Memory mem, Registers reg)
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
				case 0x0E:
					Debug.Log("LD C, D8");
					reg.C = n;
					break;
				case 0x3E:
					Debug.Log("LD A, D8");
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
				case 0x0C:
					val = reg.C;
					reg.C += 1;
					Debug.Log("INC C");
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
				case 0x77:
					mem[reg.HL] = reg.A;
					Debug.Log("LD HL, A");
					break;
			}
			yield return true;
			
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
		
		public static IEnumerable<bool> LD_N_A(Memory mem, Registers reg)
		{
			switch(mem[reg.PC])
			{
				case 0x1A:
					break;
			}
		}
	}
	
	static class PrefixCB
	{
		public static IEnumerable<bool> HandleCB(Memory mem, Registers reg)
		{
			Debug.Log("CB-{0:X2}: ", mem[reg.PC+1]);
			switch(mem[reg.PC+1])
			{
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
	}
}