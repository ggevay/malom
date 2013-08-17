/*
Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright(C) 2007-2016  Gabor E. Gevay, Gabor Danner

See our webpage (and the paper linked from there):
http://compalg.inf.elte.hu/~ggevay/mills/index.php


This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Controller
{
	public partial class ShowOutputControl : UserControl
	{
		public ShowOutputControl()
		{
			InitializeComponent();
		}

		System.Timers.Timer CloseTimer;
		public void DelayedClose()
		{
			CloseTimer = new System.Timers.Timer(5000);
			CloseTimer.AutoReset = false;
			CloseTimer.Elapsed += new System.Timers.ElapsedEventHandler(CloseTimer_Elapsed);
			CloseTimer.Start();
		}

		void CloseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.Invoke(new Action(() => this.Parent = null));
			this.Dispose();
		}


		Thread OutputReaderThread;
		StringBuilder ss = new StringBuilder();
		object sslock = new object();
		public void StartReadOutput(StreamReader stream)
		{
			OutputReaderThread = new Thread(() =>
			{
				while (!stream.EndOfStream)
				{
					String s = new String(new char[] { (char)stream.Read() });
					lock (sslock) ss.Append(s);
				}
			});
			OutputReaderThread.IsBackground = true;
			OutputReaderThread.Start();
			OutputTimer.Enabled = true;
		}

		private void OutputTimer_Tick(object sender, EventArgs e)
		{
			lock (sslock)
			{
				try
				{
					this.Invoke(new Action(() =>
					{
						String s = ss.ToString();
						if (s != "")
						{
							LogsTextbox.Text += s;
							LogsTextbox.Select(LogsTextbox.Text.Length, 0);
							LogsTextbox.ScrollToCaret();
						}
					}));
					ss = new StringBuilder();
				}
				catch (ObjectDisposedException) { }
				catch (InvalidOperationException) { }
			}
		}
	}
}
