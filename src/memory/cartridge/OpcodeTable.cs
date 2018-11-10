using System;
using System.Collections;
using System.Collections.Generic;

namespace Emulator
{
	delegate void OpcodeFunction(Memory mem, Registers reg);
	static class OpcodeTable
	{
		private static Dictionary<byte, OpcodeFunction> table = new Dictionary<byte, OpcodeFunction>() 
		{
			{0x00, (OpcodeFunction)Opcode.NOP},
			{0x0E, (OpcodeFunction)Opcode.LOAD_N_D8},
			{0x20, (OpcodeFunction)Opcode.JR_CC_N},
			{0x21, (OpcodeFunction)Opcode.LOAD_N_D16},
			{0x31, (OpcodeFunction)Opcode.LOAD_N_D16},
			{0x32, (OpcodeFunction)Opcode.LDD_HL_A},
			{0x3E, (OpcodeFunction)Opcode.LOAD_N_D8},
			{0xAF, (OpcodeFunction)Opcode.XOR},
			{0xCB, (OpcodeFunction)Opcode.PREFIX_CB},
		};
		
		public static bool ContainsKey(byte key)
		{
			return table.ContainsKey(key);
		}
		
		public static void Call(byte opcode, Memory mem, Registers reg)
		{
			table[opcode](mem, reg);
		}
	}
}