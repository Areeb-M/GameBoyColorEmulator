using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Emulator
{

	class LCD : Form
	{
		private Thread display;
		
		public LCD(int x, int y) : base()
		{
			this.Bounds = new Rectangle(0, 0, x, y);
			display = new Thread(new ThreadStart(this.Show));
		}
		
		public void Start()
		{
			display.Start();
		}
		
		public void Refresh(Bitmap image)
		{
			lock (this)
			{
				BackgroundImage = image;
			}
		}
	}

}