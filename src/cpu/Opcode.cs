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
			// Does nothing - length 1
			reg.PC += 1;
			yield break;
		}
		
		public static void LOAD_N_D16(Memory mem, Registers reg)
		{
			// 16 bit load
			int high = mem[reg.PC + 2] << 8;
			int low = mem[reg.PC + 1];
			switch(mem[reg.PC])
			{
				case 0x21:
					reg.HL = high | low;
					Debug.Log("LD HL, d16");
					break;
				case 0x31:
					reg.SP = high | low;
					Debug.Log("LD SP, d16");
					break;
			}
			reg.PC += 3;
		}
		
		public static void XOR(Memory mem, Registers reg)
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
					return;
			}
			reg.A = (byte)(a ^ b);
			reg.fZ = reg.A == 0;
			reg.fN = false;
			reg.fH = false;
			reg.fC = false;
			reg.PC += 1;
		}
		
		public static void LDD_HL_A(Memory mem, Registers reg) // LDD (HL), A
		{
			mem[reg.HL] = reg.A;
			Debug.Log("LDD (HL), A");
			reg.PC += 1;
			reg.HL -= 1;
		}
		
		public static void PREFIX_CB(Memory mem, Registers reg)
		{
			PrefixCB.HandleCB(mem, reg);
			reg.PC += 2;
		}
		
		public static void JR_CC_N(Memory mem, Registers reg)
		{
			int n = (sbyte)mem[reg.PC + 1];
			switch(mem[reg.PC])
			{
				case 0x20:
					Debug.Log("JR NZ, r8");
					if (!reg.fZ)
						reg.PC += n;
					break;
			}
			reg.PC += 2;
		}
		
		public static void LOAD_N_D8(Memory mem, Registers reg)
		{
			byte n = mem[reg.PC + 1];
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
		}
		
		public static void LOAD_0xFFCC_A(Memory mem, Registers reg)
		{
			Debug.Log("LD ($FF00+C), A");
			mem[0xFF00 + reg.C] = reg.A;
			reg.PC += 1;
		}
	}
	
	static class PrefixCB
	{
		public static void HandleCB(Memory mem, Registers reg)
		{
			Debug.Log("CB-{0:X2}: ", mem[reg.PC+1]);
			switch(mem[reg.PC+1])
			{
				case 0x7C:
					BIT(mem, reg);
					break;
			}
		}
		
		private static void BIT(Memory mem, Registers reg)
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
		}
	}
}