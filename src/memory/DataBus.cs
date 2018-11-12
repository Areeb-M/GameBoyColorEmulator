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
		
		public virtual T Data
		{
			get { return data; }
			set { data = value;}
		}
	}
}