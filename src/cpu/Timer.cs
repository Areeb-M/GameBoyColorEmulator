using System;

namespace Emulator
{
	class Timer
	{
		#region Timer Registers
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
		
		public DataBus<byte>[] TimerRegisters
		{
			get { return new DataBus<byte>[]
					{
						DIV,
						TIMA, 
						TMA,
						TAC
					};
				}
		}
		
		#endregion
		
		InterruptController ic;
		
		public Timer(InterruptController interruptController)
		{
			dividerRegister = new DataBus<byte>((byte)0);
			timerCounter = new DataBus<byte>((byte)0);
			timerModulo = new DataBus<byte>((byte)0);
			timerControl = new DataBus<byte>((byte)0);
			
			ic = interruptController;
		}
		
		public void Tick(int clockCycle)
		{
			if (clockCycle % 256 == 0)
				DIV.Data += 1;
			
			if ((TAC.Data & 4) == 4)
			{
				switch(TAC.Data & 3)
				{
					case 0: // 4.096 KHz
						if (clockCycle % 1024 == 0)
							TIMA.Data += 1;
						break;
					case 1: // 262.144 KHz
						if (clockCycle % 16 == 0)
							TIMA.Data += 1;
						break;
					case 2: // 65.536 KHz
						if (clockCycle % 64 == 0)
							TIMA.Data += 1;
						break;
					case 3: // 16.384 KHz
						if (clockCycle % 256 == 0)
							TIMA.Data += 1;
						break;
				}
				
				if (TIMA.Data == 0)
				{
					TIMA.Data = TMA.Data;
					ic.GenerateTimerOverflowInterrupt();
				}
			}
		}
	}
}