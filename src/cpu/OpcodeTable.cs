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
			{0x04, (OF)Opcode.INCREMENT_REG},
			{0x05, (OF)Opcode.DECREMENT_REG},
			{0x06, (OF)Opcode.LOAD_N_D8},
			{0x0C, (OF)Opcode.INCREMENT_REG},
			{0x0D, (OF)Opcode.DECREMENT_REG},
			{0x0E, (OF)Opcode.LOAD_N_D8},
			{0x11, (OF)Opcode.LOAD_N_D16},
			{0x13, (OF)Opcode.INCREMENT_16_REG},
			{0x15, (OF)Opcode.DECREMENT_REG},
			{0x17, (OF)Opcode.RLA},
			{0x18, (OF)Opcode.JR_R8},
			{0x1A, (OF)Opcode.LOAD_A_N},
			{0x1D, (OF)Opcode.DECREMENT_REG},
			{0x1E, (OF)Opcode.LOAD_N_D8},
			{0x20, (OF)Opcode.JR_CC_R8},
			{0x21, (OF)Opcode.LOAD_N_D16},
			{0x22, (OF)Opcode.LDI_HL_A},
			{0x23, (OF)Opcode.INCREMENT_16_REG},
			{0x24, (OF)Opcode.INCREMENT_REG},
			{0x28, (OF)Opcode.JR_CC_R8},
			{0x2E, (OF)Opcode.LOAD_N_D8},
			{0x31, (OF)Opcode.LOAD_N_D16},
			{0x32, (OF)Opcode.LDD_HL_A},
			{0x3D, (OF)Opcode.DECREMENT_REG},
			{0x3E, (OF)Opcode.LOAD_N_D8},
			{0x4F, (OF)Opcode.LOAD_N_A},
			{0x57, (OF)Opcode.LOAD_N_A},
			{0x67, (OF)Opcode.LOAD_N_A},
			{0x77, (OF)Opcode.LOAD_N_A},
			{0x7B, (OF)Opcode.LOAD_A_N},
			{0x7C, (OF)Opcode.LOAD_A_N},
			{0x90, (OF)Opcode.SUB},
			{0xAF, (OF)Opcode.XOR},
			{0xC1, (OF)Opcode.POP},
			{0xC5, (OF)Opcode.PUSH},
			{0xC9, (OF)Opcode.RETURN},
			{0xCB, (OF)Opcode.PREFIX_CB},
			{0xCD, (OF)Opcode.CALL_NN},
			{0xE0, (OF)Opcode.LD_FFNN_A},
			{0xE2, (OF)Opcode.LOAD_0xFFCC_A},
			{0xEA, (OF)Opcode.LOAD_N_A},	
			{0xF0, (OF)Opcode.LOAD_A_FFNN},			
			{0xFE, (OF)Opcode.COMPARE},
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