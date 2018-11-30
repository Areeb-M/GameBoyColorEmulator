using System;
using System.Drawing;
using System.Windows.Forms;

namespace Emulator
{

	class LCD
	{
		public LCD(int x, int y)
		{
			Form test = new Form();
			test.Bounds = new Rectangle(0, 0, x, y);
			test.Show();
		}
	}

}