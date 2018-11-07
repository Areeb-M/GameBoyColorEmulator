using System;

namespace Emulator
{
	static class MemoryLoader
	{
		public static Memory ConstructMemoryClass(string pathToRom)
		{
			Memory mem = new Memory(path);
			return mem;
		}
		
		public static Memory ReloadSaveState(Memory memory, CPU cpu, string pathToSave)
		{
			
		}
		
		public static byte[] DumpSaveState(Memory memory, CPU cpu)
		{
			
		}
		
		public static void DumpSaveState(Memory memory, CPU cpu, string pathToSave)
		{
			
		}
	}
}