using System;

namespace Emulator
{
	class InterruptController
	{
		DataBus<byte> interruptEnableFlag;
		bool interruptsEnabled;
		bool hasInterrupt;
		int currentInterrupt;
		
		public DataBus<byte> IE
		{
			get { return interruptEnableFlag; }
		}
		
		public InterruptController()
		{
			interruptEnableFlag = new DataBus<byte>((byte)0);
			interruptsEnabled = false;
			hasInterrupt = false;
			currentInterrupt = -1;
		}
		
		public void EnableInterrupts()
		{
			interruptsEnabled = true;
			currentInterrupt = -1;
		}
		
		public void DisableInterrupts()
		{
			interruptsEnabled = false;
		}
		
		public bool HasInterrupt()
		{
			if (hasInterrupt)
			{
				hasInterrupt = false;
				return true;
			}
			return false;
		}
		
		public int GetInterrupt()
		{
			return currentInterrupt;
		}
		
		public void GenerateVBlankInterrupt()
		{
			if (interruptsEnabled && (IE.Data & (1 << 0)) == 1)
			{
				currentInterrupt = 0;
				interruptsEnabled = false;
			}			
		}	
		
		public void GenerateLCDCInterrupt()
		{
			if (interruptsEnabled && (IE.Data & (1 << 1)) == 2)
			{
				currentInterrupt = 1;
				interruptsEnabled = false;
			}			
		}	
		
		public void GenerateTimerOverflowInterrupt()
		{
			if (interruptsEnabled && (IE.Data & (1 << 2)) == 4)
			{
				currentInterrupt = 2;
				interruptsEnabled = false;
			}			
		}
			
		public void GenerateSerialIOTransferCompleteInterrupt()
		{
			if (interruptsEnabled && (IE.Data & (1 << 3)) == 8)
			{
				currentInterrupt = 3;
				interruptsEnabled = false;
			}			
		}
			
		public void GenerateHiToLowInterrupt()
		{
			if (interruptsEnabled && (IE.Data & (1 << 4)) == 16)
			{
				currentInterrupt = 4;
				interruptsEnabled = false;
			}			
		}
	}
}