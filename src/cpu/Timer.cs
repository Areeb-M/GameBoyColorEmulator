using System;

namespace Emulator
{
	class Timer
	{
		DataBus<byte> dividerRegister;
		DataBus<byte> timerCounter;
		DataBus<byte> timerModulo;
		DataBus<byte> timerControl;
		
		public DataBus<byte> DIV
		{
			get { return dividerRegister; }
		}
		
		public DataBus<byte> TIMA
		{
			get { return timerCounter; }
		}
		
		public DataBus<byte> TMA
		{
			get { return timerModulo; }
		}
		
		public DataBus<byte> TAC
		{
			get { return timerControl; }
		}
		
		public Timer()
		{
			dividerRegister = new DataBus<byte>((byte)0);
			timerCounter = new DataBus<byte>((byte)0);
			timerModulo = new DataBus<byte>((byte)0);
			timerControl = new DataBus<byte>((byte)0);
		}
	}
}