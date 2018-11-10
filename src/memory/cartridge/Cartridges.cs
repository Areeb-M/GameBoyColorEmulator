using System;

namespace Emulator
{
	class BootRom : Cartridge
	{
		private Memory memory;
		private Cartridge cartridge;
		public BootRom (byte[] rom, Memory memory, Cartridge cartridge) : base(0, 0, rom)
		{
			this.memory = memory;
			this.cartridge = cartridge;
		}
		
		public override byte this[int index]
		{
			get
			{
				if (index < 0x100)
					return base[index];
				else
					return cartridge[index];
			}
			set
			{
				if (index < 0x100 || index == 0xFF50)
					base[index] = val;
				else
					cartridge[index] = val;
			}
		}
		
		protected override void WriteEmptyButUnusable(int index, byte val)
		{
			if ((index == 0xFF50) && (val == 0x01))
				memory.DetachBootROM();
		}
	}
}