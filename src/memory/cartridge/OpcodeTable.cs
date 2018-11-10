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
			{0x21, (OpcodeFunction)Opcode.LOAD_N_NN},
			{0x31, (OpcodeFunction)Opcode.LOAD_N_NN},
			{0x32, (OpcodeFunction)Opcode.LDD_HL_A},
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