using System;

namespace Emulator
{
	class DataBus<T>
	{
		T data;
		
		public DataBus(T val)
		{
			data = val;
		}
		
		public virtual T Read()
		{
			return data;
		}
		
		public virtual void Write(T val)
		{
			data = val;
		}
	}
}