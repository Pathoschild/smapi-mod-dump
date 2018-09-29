
using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MultiplayerEmotes.Framework {

	public class Logger : IDisposable {

		private string FilePath { get; set; } = Directory.GetCurrentDirectory() + "\\Mods";
		private IMonitor ModMonitor { get; set; }
		private StreamWriter Stream { get; set; }

		public Logger(string filePath, bool append = true, IMonitor monitor = null) {
			SetFilePath(filePath, append);
			if(monitor != null) {
				SetMonitor(monitor);
			}
		}

		public void SetMonitor(IMonitor monitor) {
			ModMonitor = monitor;
		}

		public void SetFilePath(string filePath, bool append = true) {
			string folderPath = Path.GetDirectoryName(FilePath);
			if(folderPath == null) {
				throw new ArgumentException($"Log path '{FilePath}' not valid.");
			}
			Directory.CreateDirectory(folderPath);
			FilePath = filePath;
			try {
				Stream = new StreamWriter(FilePath, append) {
					AutoFlush = true
				};
			} catch(IOException) {
				string dir = Path.GetDirectoryName(filePath);
				string fileName = Path.GetFileNameWithoutExtension(filePath);
				string fileExt = Path.GetExtension(filePath);
				FilePath = Path.Combine(dir, fileName + "2", fileExt);
			}
		}

		public void LogTrace(string message = "Trace Message:",
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0) {

			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			int timeLength = time.Length + 1;
			string callerMemberName = "CallerMemberName: " + memberName;
			string callerFilePath = "CallerFilePath: " + sourceFilePath;
			string callerLineNumber = "CallerLineNumber: " + sourceLineNumber;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(message);
			sb.AppendLine(callerMemberName.PadLeft(callerMemberName.Length));
			sb.AppendLine(callerFilePath.PadLeft(callerFilePath.Length));
			sb.Append(callerLineNumber.PadLeft(callerLineNumber.Length));

			ModMonitor?.Log(sb.ToString());

			sb.Clear();

			sb.AppendLine(message);
			sb.AppendLine(callerMemberName.PadLeft(callerMemberName.Length + timeLength));
			sb.AppendLine(callerFilePath.PadLeft(callerFilePath.Length + timeLength));
			sb.Append(callerLineNumber.PadLeft(callerLineNumber.Length + timeLength));

			WriteLine($"{time} {sb.ToString()}");
		}

		public void Log(params string[] messages) {

			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			int timeLength = time.Length + 1;

			WriteLine($"{time} {messages[0]}");
			ModMonitor?.Log(messages[0]);
			for(int i = 1; i < messages.Length; i++) {
				WriteLine($"{messages[i].PadLeft(messages[i].Length + timeLength)}");
				ModMonitor?.Log(messages[i]);
			}

		}

		public void WriteLine(string message) {
			Write(message + "\r\n");
		}

		public void Write(string message) {
			Stream.Write(message);
		}

		public void Dispose() {
			Stream.Dispose();
		}
	}

}
