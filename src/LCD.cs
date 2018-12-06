using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Emulator
{

	class LCD
	{
		private Thread display;
		private Form form;
		
		private PictureBox bg;
		int frame = 0;
		
		public LCD(int x, int y)
		{
			form = new Form();
			
			form.Bounds = new Rectangle(0, 0, x, y);
			form.FormBorderStyle = FormBorderStyle.FixedSingle;
			form.MaximizeBox = false;
			form.MinimizeBox = false;
			
			bg = new PictureBox();
			bg.SizeMode = PictureBoxSizeMode.Zoom;
			bg.Bounds = form.Bounds;
			bg.Image = new Bitmap(160, 144);
			form.Controls.Add(bg);
			
			display = new Thread(new ThreadStart(Run));
		}
		
		public void Start()
		{
			display.Start();
		}
		
		private void Run()
		{
			form.ShowDialog();
			while(true)
			{
				Thread.Sleep(1);
			}
		}
		
		public void Refresh(Bitmap image)
		{
			lock (this)
			{
				//form.Text = "Frame: " + frame++;
				bg.Image = image;
			}
		}
	}

}