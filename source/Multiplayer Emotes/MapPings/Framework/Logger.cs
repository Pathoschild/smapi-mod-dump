
using StardewModdingAPI;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MapPings.Framework {

	public enum LogOutput {
		None,
		File,
		Console,
		Both
	}

	public class Logger : IDisposable {

		private string FilePath { get; set; }
		private IMonitor ModMonitor { get; set; }
		private StreamWriter Stream { get; set; }

		public LogOutput LogToOutput { get; set; }

		public Logger(IMonitor monitor) {
			LogToOutput = LogOutput.Console;
			SetMonitor(monitor);
		}

		public Logger(string filePath, bool append = true) {
			LogToOutput = LogOutput.File;
			SetFilePath(filePath, append);
		}

		public Logger(IMonitor monitor, string filePath, bool append = true) {
			LogToOutput = LogOutput.Both;
			SetFilePath(filePath, append);
			SetMonitor(monitor);
		}

		public void SetMonitor(IMonitor monitor) {
			ModMonitor = monitor;
		}

		public void SetFilePath(string filePath, bool append = true) {

			FilePath = filePath;

			string folderPath = Path.GetDirectoryName(FilePath);
			if(folderPath == null) {
				throw new ArgumentException($"Log path '{FilePath}' not valid.");
			}
			Directory.CreateDirectory(folderPath);

			Stream = new StreamWriter(FilePath, append) {
				AutoFlush = true
			};

		}

		public void LogTrace(string message = "Trace Message:",
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0) {
			LogTrace(LogToOutput, message, memberName, sourceFilePath, sourceLineNumber);
		}

		public void LogTrace(LogOutput logOutput, string message = "Trace Message:",
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
			sb.AppendLine(callerMemberName.PadLeft(callerMemberName.Length + timeLength));
			sb.AppendLine(callerFilePath.PadLeft(callerFilePath.Length + timeLength));
			sb.Append(callerLineNumber.PadLeft(callerLineNumber.Length + timeLength));

			if(logOutput == LogOutput.File || logOutput == LogOutput.Both) {
				WriteLine($"{time} {sb.ToString()}");
			}

			if(logOutput == LogOutput.Console || logOutput == LogOutput.Both) {
				ModMonitor?.Log(sb.ToString());
			}

		}

		public void Log(params string[] messages) {
			Log(LogToOutput, messages);
		}

		public void Log(LogOutput logOutput, params string[] messages) {

			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			int timeLength = time.Length + 1;

			if(logOutput == LogOutput.File || logOutput == LogOutput.Both) {
				WriteLine($"{time} {messages[0]}");
			}

			if(logOutput == LogOutput.Console || logOutput == LogOutput.Both) {
				ModMonitor?.Log(messages[0]);
				for(int i = 1; i < messages.Length; i++) {
					WriteLine($"{messages[i].PadLeft(messages[i].Length + timeLength)}");
					ModMonitor?.Log(messages[i]);
				}
			}

		}

		public void WriteLine(string message) {
			Write(message + "\r\n");
		}

		public void Write(string message) {
			Stream.Write(message);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					// Dispose managed state (managed objects).
					Stream.Dispose();
					Stream = null;
				}

				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion
	}

}
