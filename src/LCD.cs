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
			display = new Thread(new ThreadStart(Run));
		}
		
		public void Start()
		{
			display.Start();
		}
		
		private void Run()
		{
			this.Show();
			while(true)
			{
				Thread.Sleep(100);
			}
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