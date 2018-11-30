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
			LCD lcd = new LCD(400, 400);
			GameBoyColor gbc = new GameBoyColor(args[0], args[1]);
			gbc.Run();
		}
		
	}
	
}