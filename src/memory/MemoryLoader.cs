using System;

namespace Emulator
{
	static class MemoryLoader
	{
		
		public static Memory ReloadSaveState(Memory memory, CPU cpu, string pathToSave)
		{
			return memory;
		}
		
		public static byte[] DumpSaveState(Memory memory, CPU cpu)
		{
			return null;
		}
		
		public static void DumpSaveState(Memory memory, CPU cpu, string pathToSave)
		{
			
		}
	}
}