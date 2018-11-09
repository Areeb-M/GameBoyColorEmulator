using System;

namespace Emulator
{
	
	public static void NOP(Memory mem, Registers reg) // 0x00
	{
		// Does nothing - length 1
		reg.PC += 1;
	}
	
}