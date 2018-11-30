using System;
using System.Drawing;
using System.Windows.Forms;

namespace Emulator
{

	class LCD : Form
	{
		public LCD(int x, int y) : base()
		{
			this.Bounds = new Rectangle(0, 0, x, y);
			this.Show();
		}
	}

}