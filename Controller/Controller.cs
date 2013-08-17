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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

using Wrappers;


namespace Controller
{
	public partial class Controller : Form
	{
		enum Modes { solve, verify, analyze };
		Modes Mode = Modes.solve;

		//Note that Application.StartupPath is  NOT  the working directory!
		string ExeToStart = Application.StartupPath + "\\..\\..\\..\\x64\\Release\\malom_megoldas"; //path and name of solver exe
		//string ExeToStart = Application.StartupPath + "\\..\\..\\..\\x64\\Opt_Debug\\malom_megoldas";
		//string ExeToStart = Application.StartupPath + "\\..\\..\\..\\x64\\Debug\\malom_megoldas";
		//string ExeToStart = Application.StartupPath + "\\malom_megoldas"; //path and name of solver exe

		public Controller(string[] args)
		{
			if (args.Length == 1)
			{
				if (args[0] == "-verify")
					Mode = Modes.verify;
				if (args[0] == "-analyze")
					Mode = Modes.analyze;
			}

			InitializeComponent();
		}

		abstract class WuSec //vagy wu vagy sector
		{
			public id id;

			public override string ToString() { return id.ToString(); }

			public abstract long size();
		}

		class Wu : WuSec
		{
			public List<Wu> AdjT;
			public int ChdCnt, DoneChdCnt;
			public bool twine;
			public bool Done;
			public bool InProgress;

			public Wu(id id, bool twine)
			{
				this.id = id;
				this.AdjT = new List<Wu>();
				this.twine = twine;
			}

			public bool PreReqOk() { return DoneChdCnt == ChdCnt; }

			public override long size()
			{
				int r = id.size();
				if(twine){
					r += (-id).size();
				}
				return r;
			}
		}

		class Sector : WuSec
		{
			public Wu Wu;
			public bool VerifDone, AnalyzeDone;
			public bool VerifInProgress, AnalyzeInProgress;

			public Sector(id id, Wu Wu)
			{
				this.id = id;
				this.Wu = Wu;
			}

			public override long size()
			{
				return id.size();
			}
		}

		private void InitWuGraph()
		{
			int tmp = 0; //ez csinal valamit?
			foreach (var id in Nwu.WuIds)
			{
				var w = new Wu(id, Nwu.Twine(id));
				WuList.Add(w);
				Wus.Add(id, w);
				tmp++;
			}

			foreach (var u in Nwu.WuIds)
				foreach (var v in Nwu.WuGraphT(u))
				{
					Wus[u].AdjT.Add(Wus[v]);
					Wus[v].ChdCnt++;
				}
		}

		private void InitSectorList()
		{
			foreach (var w in WuList)
			{
				SecList.Add(new Sector(w.id, w));
				if (w.twine)
				{
					id nid = w.id;
					nid.negate();
					SecList.Add(new Sector(nid, w));
				}
			}
		}

		List<Wu> WuList = new List<Wu>();
		List<Sector> SecList = new List<Sector>();
		Dictionary<id, Wu> Wus = new Dictionary<id, Wu>();


		String DataDir = Directory.GetCurrentDirectory();
		String LockFileDir = Directory.GetCurrentDirectory() + "\\lockfiles";

		private void Main_Load(object sender, EventArgs e)
		{
			//MessageBox.Show(System.IO.Directory.GetCurrentDirectory().ToString());

			UDNumThreads.MouseWheel += new MouseEventHandler(UDNumThreads_MouseWheel);

			//StartTimer.Interval = Mode == Modes.analyze ? 400 : 2000;
		}

		void UDNumThreads_MouseWheel(object sender, MouseEventArgs e)
		{
			((HandledMouseEventArgs)e).Handled = true;
		}

		void Init()
		{
			//a sorrend fontos
			Nwu.InitWuGraph();
			InitWuGraph();
			InitSectorList();
		}

		private void Main_Shown(object sender, EventArgs e)
		{
			try
			{
				this.Text = "Initializing...";
				Init();
				foreach (var w in WuList)
				{
					WuListBox.Items.Add(w);
				}
				this.Text = "Controller";

				DeleteAbandonedLockfiles();
				UpdateWuSecStatuses();
				CreateMovegenLookuptables();

				if ((Mode == Modes.verify || Mode == Modes.analyze) && WuList.Any(x => !x.Done)) MessageBox.Show("Not all sectors are done.");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace.ToString() + "\n\n" + new System.Diagnostics.StackTrace().ToString());
			}
		}

		private void RepeatStartNext()
		{
			Application.DoEvents();
			while (StartNext()) ;
		}

		private void StartTimer_Tick(object sender, EventArgs e)
		{
			RepeatStartNext();
		}

		private void CreateMovegenLookuptables()
		{
			if (!File.Exists(Constants.MovegenFname))
			{
				this.Text = "Generating movegen lookuptables";
				Process p = new Process();
				p.StartInfo = new ProcessStartInfo(ExeToStart, "-writemovegenlookups");
				p.StartInfo.WorkingDirectory = DataDir;
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.CreateNoWindow = true;
				p.Start();
				try
				{
					p.WaitForExit();
				}
				catch (Win32Exception) { } //ez dobodik itt, ha mar kilepett
				if (p.ExitCode != 0)
					MessageBox.Show("Nonzero exit code when running writemovegenlookups (code: " + p.ExitCode.ToString() + ")");
				this.Text = "Controller";
			}
		}

		private void Main_Click(object sender, EventArgs e)
		{

		}

		HashSet<id> OwnInProgressIDs = new HashSet<id>();
		Thread RunnerThread;

		List<WuSec> GetStartables()
		{
			UpdateWuSecStatuses();

			var r = new List<WuSec>();

			switch (Mode)
			{
				case Modes.solve:
					foreach (var w in WuList)
						if (w.PreReqOk() && !w.Done && !w.InProgress)
							r.Add(w);
					break;
				case Modes.verify:
					foreach (var s in SecList)
						if (s.Wu.Done && !s.VerifDone && !s.VerifInProgress)
							r.Add(s);
					break;
				case Modes.analyze:
					foreach (var s in SecList)
						if (s.Wu.Done && !s.AnalyzeDone && !s.AnalyzeInProgress)
							r.Add(s);
					break;
				default:
					break;
			}

			return r;
		}

		int CompareTuple(Tuple<int, int> a, Tuple<int, int> b)
		{
			if (a.Item1 != b.Item1)
				return a.Item1.CompareTo(b.Item1);
			else
				return a.Item2.CompareTo(b.Item2);
		}

		private void EmptyStartables()
		{
			switch (Mode)
			{
				case Modes.solve:
					if (!WuList.Any(x => x.InProgress))
					{
						Trace.Assert(!WuList.Any(x => !x.Done), "No more startables, but haven't done all sectors.");
						//StartTimer.Enabled = false;
						//MessageBox.Show("Kesz az egesz! (" + DateTime.Now.ToString() + ")");

						Mode = Modes.verify;
					}
					break;
				case Modes.verify:
					if (!SecList.Any(x => x.VerifInProgress))
					{
						if (SecList.Any(x => !x.VerifDone))
						{
							StartTimer.Enabled = false;
							MessageBox.Show("All done sectors are verified, but not all sectors are done."); //"Minden kesz szektor verifikalva, de nincs kesz minden szektor."
						}
						else
						{
							//StartTimer.Enabled = false;
							//MessageBox.Show("Kesz az egesz verification is.");

							Mode = Modes.analyze;
						}
					}
					break;
				case Modes.analyze:
					if (!SecList.Any(x => x.AnalyzeInProgress))
					{
						if (SecList.Any(x => !x.AnalyzeDone))
						{
							StartTimer.Enabled = false;
							MessageBox.Show("All done sectors are analyzed, but not all sectors are done.");
						}
						else
						{
							StartTimer.Enabled = false;
							MessageBox.Show("Everything done."); //"Kesz az egesz elemzes is."
						}
					}
					break;
				default:
					break;
			}
		}

		//returns true if it actually started something
		private bool StartNext()
		{
			if (OwnInProgressIDs.Count >= (int)(UDNumThreads.Value)) return false;

			var Startables = GetStartables();
			if (Startables.Count == 0)
			{
				EmptyStartables();
				return false;
			}
			//Startables.Sort((a, b) => CompareTuple( new Tuple<int, int>(a.id.WF + a.id.BF, -a.id.W - a.id.B), new Tuple<int, int>(b.id.WF + b.id.BF, -b.id.W - b.id.B)));
			//WuSec w = Startables[0];
			WuSec w = ChooseRandom(Startables);


			FileStream lockfile = null;
			try
			{
				lockfile = File.Create(LockFileDir + '\\' + w.id.ToString() + "." + Mode.ToString() + "lock");
			}
			catch (Exception) //majd meg kell nezni, hogy ez pontosan milyen exception
			{
				return false;
			}


			lock (this)
			{
				OwnInProgressIDs.Add(w.id);
			}

			Process p = new Process();
			p.StartInfo = new ProcessStartInfo(
				//Application.StartupPath + "\\..\\..\\..\\x64\\Release\\malom_megoldas",
				ExeToStart,
				w.id.W.ToString() + " " + w.id.B.ToString() + " " + w.id.WF.ToString() + " " + w.id.BF.ToString() + " -" + Mode.ToString());
			p.StartInfo.WorkingDirectory = DataDir;
			p.StartInfo.UseShellExecute = false;
			//p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //ez valahogy nem mukodik
			//p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; //ez se
			p.StartInfo.CreateNoWindow = true;

			p.StartInfo.RedirectStandardOutput = true;
			//p.StartInfo.RedirectStandardError = true;

			RunnerThread = new Thread(() =>
			{
				p.Start();

				ShowOutputControl OutputControl = null;
				this.Invoke(new Action(() =>
				{
					//OutputForm = new ShowOutput();
					//OutputForm.Text = w.id.ToString();
					////OutputForm.WindowState = FormWindowState.Minimized;
					//OutputForm.Show();

					//OutputForm.StartReadOutput(p.StandardOutput);

					OutputControl = new ShowOutputControl();
					OutputControl.Parent = flowLayoutPanel;
					OutputControl.StartReadOutput(p.StandardOutput);
				}));


				try
				{
					p.WaitForExit();
				}
				catch (Win32Exception) { } //if it already exited

				try
				{
					this.Invoke(new Action(() =>
					{
						OutputControl.DelayedClose();
					}));
				}
				catch (InvalidOperationException) { } //form was closed

				if (p.ExitCode != 0)
				{
					this.Invoke(new Action(() => this.Activate()));
					MessageBox.Show("Nonzero exit code when running " + w.id.ToString() + " (code: " + p.ExitCode.ToString() + ")");
				}

				lock (this)
				{
					OwnInProgressIDs.Remove(w.id);
				}
				lockfile.Close();

				this.BeginInvoke(new Action(UpdateWuSecStatuses));
				this.BeginInvoke(new Action(RepeatStartNext));
			});
			RunnerThread.IsBackground = true;

			//this.Text = (VerificationMode ? "Verifying " : "Solving ") + w.ToString();
			RunnerThread.Start();
			return true;
		}


		//it assumes that if one sector file of a wu is present, then the other is also there
		void UpdateWuSecStatuses()
		{
			var files = new HashSet<string>(Directory.GetFiles(DataDir));
			if (!Directory.Exists(LockFileDir)) Directory.CreateDirectory(LockFileDir);
			var lockFiles = new HashSet<string>(Directory.GetFiles(LockFileDir));

			foreach (var w in WuList)
				w.DoneChdCnt = 0;
			foreach (var w in WuList)
			{
				w.Done = files.Contains(DataDir + "\\" + w.id.ToString() + ".sec" + Wrappers.Constants.Fname_suffix);
				if (w.Done)
					foreach (var v in w.AdjT)
						v.DoneChdCnt++;

				w.InProgress = lockFiles.Contains(LockFileDir + "\\" + w.id.ToString() + ".solvelock");
			}

			foreach (Sector s in SecList)
			{
				s.VerifDone = files.Contains(DataDir + "\\" + s.id.ToString() + ".verificationlog" + Wrappers.Constants.Fname_suffix);
				s.VerifInProgress = lockFiles.Contains(LockFileDir + "\\" + s.id.ToString() + ".verifylock");
				s.AnalyzeDone = files.Contains(DataDir + "\\" + s.id.ToString() + ".analyzelog" + Wrappers.Constants.Fname_suffix);
				s.AnalyzeInProgress = lockFiles.Contains(LockFileDir + "\\" + s.id.ToString() + ".analyzelock");
			}

			UpdateProgressbars();
		}

		void UpdateProgressbars()
		{
			int done, max;
			long doneNodes = 0, allNodes = SecList.Sum(s => s.size());
			switch (Mode)
			{
				case Modes.solve:
					done = WuList.Count(w => w.Done);
					max = WuList.Count;
					doneNodes += WuList.Where(w => w.Done).Sum(s => s.size());
					break;
				case Modes.verify:
					done = SecList.Count(s => s.VerifDone);
					max = SecList.Count;
					doneNodes += SecList.Where(w => w.VerifDone).Sum(s => s.size());
					break;
				case Modes.analyze:
					done = SecList.Count(s => s.AnalyzeDone);
					max = SecList.Count;
					doneNodes += SecList.Where(w => w.AnalyzeDone).Sum(s => s.size());
					break;
				default:
					max = 0; done = 0;
					break;
			}
			OverallProgressBar.Maximum = max;
			OverallProgressBar.Value = done;
			OverallProgressLabel.Text = done.ToString() + "/" + max.ToString() + " (" + Math.Round(((double)100 * done / max), 2).ToString() + "%)";

			int ncmax = 1000000000;
			int doneNodesInt = (int)Math.Round((double)ncmax / allNodes * doneNodes);
			NodecountProgressbar.Maximum = ncmax;
			NodecountProgressbar.Value = doneNodesInt;
			NodecountProgressLabel.Text = doneNodes.ToString() + "/" + allNodes.ToString() + " (" + Math.Round(((double)100 * doneNodes / allNodes), 2).ToString() + "%)";
		}

		Random Rnd = new Random();
		T ChooseRandom<T>(List<T> l)
		{
			return l[Rnd.Next(l.Count)];
		}

		private void LockDeleter_Tick(object sender, EventArgs e)
		{
			DeleteAbandonedLockfiles();
		}

		public void DeleteAbandonedLockfiles()
		{
			foreach (var f in Directory.GetFiles(LockFileDir))
			{
				try
				{
					//We try to open the logfile (if it exists), then delete the lockfile. If both have succeeded, then it was really abandoned.
					string loggingfile = Path.Combine(DataDir, Path.GetFileName(f.Remove(f.Length - Path.GetExtension(f).Length) + ".logging" + Wrappers.Constants.Fname_suffix));
					if (File.Exists(loggingfile)) //If it doesn't exist, then it might have been deleted, so we don't want an exception, but we want to delete the lockfile
						File.OpenWrite(loggingfile).Close();

					File.Delete(f); //If it is running, then it keeps the file open, so this won't succeed
				}
				catch (IOException)
				{
					//it is being used
				}
			}
		}

		private void Controller_DragDrop(object sender, DragEventArgs e)
		{
			string[] L = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (L.Length != 1) { MessageBox.Show("Drop one exe file."); return; }
			ExeToStart = L[0];
			this.Text = ExeToStart;
		}

		private void Controller_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}


		PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", String.Empty);
		private void MemoryTimer_Tick(object sender, EventArgs e)
		{
			if (ramCounter.NextValue() > (int)MemInc.Value && (int)UDNumThreads.Value < (int)MaxThreads.Value) //6000 //8000
				UDNumThreads.Value += 1;
			if (ramCounter.NextValue() < (int)MemDec.Value && (int)UDNumThreads.Value > 0) //4000 //5000
				UDNumThreads.Value -= 1;
		}
		private void AutoNumThreads_CheckedChanged(object sender, EventArgs e)
		{
			if (MemoryTimer.Enabled = AutoNumThreads.Checked)
				MemoryTimer_Tick(null, null);
		}

		private void UDNumThreads_ValueChanged(object sender, EventArgs e)
		{
			RepeatStartNext();
		}

	}
}
