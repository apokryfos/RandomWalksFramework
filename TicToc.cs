using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;


namespace RandomWalkFramework {
	public class TicToc {

		public static TicToc Tic() {
			LastReference = new TicToc();
			return LastReference;
		}
		public static TicToc Tic(string LogPath) {
			LastReference = new TicToc(LogPath);
			return LastReference;
		}

		public static TicToc Tic(string Name, bool supressConsole) {
			LastReference = new TicToc(Name, supressConsole);
			return LastReference;
		}

		public static void LToc() { if (LastReference != null) LastReference.Toc(); }

		private static TicToc LastReference = null;

		private string MethodName;
		private bool SupressConsole;

		private string LogPath;


		private TicToc(string LogPath, bool supressConsole) {
			if (!supressConsole) {
				StackTrace stackTrace = new StackTrace();
				MethodName = stackTrace.GetFrame(3).GetMethod().Name;
				Console.WriteLine();
				Console.Write("Tic from {0}...", MethodName);
			}
			Ticks = DateTime.Now.Ticks;
			this.LogPath = LogPath;
			this.SupressConsole = supressConsole;
		}


		private TicToc(string name)
			: this(name, false) {

		}

		private TicToc()
			: this(null, false) {

		}
		public void Toc() {
			if (!SupressConsole) { Console.WriteLine("toc from {1} after {0}", TimeSpan.FromTicks(DateTime.Now.Ticks - Ticks), MethodName); }
			if (LogPath != null) { File.AppendAllText(LogPath, TimeSpan.FromTicks(DateTime.Now.Ticks - Ticks).TotalMilliseconds.ToString() + Environment.NewLine); }
		}

		private long Ticks = 0;
	}

}