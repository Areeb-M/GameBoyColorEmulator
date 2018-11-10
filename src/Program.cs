using System;

namespace Emulator
{
	
	class Program
	{
		
		public static void Main(String[] args)
		{
			// Possible Arguments
			// {0} Game ROM path
			// {1] Boot ROM path
			GameBoyColor gbc = new GameBoyColor(args[0], args[1]);
			
		}
		
	}
	
}