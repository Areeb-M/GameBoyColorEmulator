using System;

namespace Emulator
{
	class Timer
	{
		#region Timer Registers
		DataBus<byte> dividerRegister;
		DataBus<int> timerCounter;
		DataBus<byte> timerModulo;
		DataBus<byte> timerControl;
		
		public DataBus<byte> DIV
		{
			get { return dividerRegister; }
		}
		
		public DataBus<int> TIMA
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
		#endregion
		
		DataBus<byte> interruptFlag;
		
		public Timer(DataBus<byte> IF)
		{
			dividerRegister = new DataBus<byte>((byte)0);
			timerCounter = new DataBus<int>(0);
			timerModulo = new DataBus<byte>((byte)0);
			timerControl = new DataBus<byte>((byte)0);
			
			interruptFlag = IF;
		}
		
		public void Tick()
		{
			DIV.Data += 1;
			
			if ((TAC.Data & 4) == 4)
			{
				switch(TAC.Data & 3)
				{
					case 0: // 4.096 KHz
						if (DIV.Data % 4 == 0)
							TIMA.Data += 1;
						break;
					case 1: // 262.144 KHz
						TIMA.Data += 16;
						break;
					case 2: // 65.536 KHz
						TIMA.Data += 4;
						break;
					case 3: // 16.384 KHz
						TIMA.Data += 1;
						break;
				}
				
				if (TIMA.Data % 256 < TIMA.Data)
				{
					TIMA.Data = TMA.Data;
					interruptFlag.Data |= (byte)(1 << 2);
				}
			}
		}
	}
}