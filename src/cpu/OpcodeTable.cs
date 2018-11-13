using System;
using System.Collections;
using System.Collections.Generic;

namespace Emulator
{
	delegate IEnumerable<bool> OF(Memory mem, Registers reg);
	// OF = OpcodeFunction
	static class OpcodeTable
	{
		private static Dictionary<byte, OF> table = new Dictionary<byte, OF>() 
		{
			{0x00, (OF)Opcode.NOP},
			{0x0E, (OF)Opcode.LOAD_N_D8},
			{0x20, (OF)Opcode.JR_CC_N},
			{0x21, (OF)Opcode.LOAD_N_D16},
			{0x31, (OF)Opcode.LOAD_N_D16},
			{0x32, (OF)Opcode.LDD_HL_A},
			{0x3E, (OF)Opcode.LOAD_N_D8},
			{0xAF, (OF)Opcode.XOR},
			{0xCB, (OF)Opcode.PREFIX_CB},
			{0xE2, (OF)Opcode.LOAD_0xFFCC_A}, 
		};
		
		public static bool ContainsKey(byte key)
		{
			return table.ContainsKey(key);
		}
		
		public static IEnumerator<bool> Call(byte opcode, Memory mem, Registers reg)
		{
			return table[opcode](mem, reg).GetEnumerator();
		}
	}
}