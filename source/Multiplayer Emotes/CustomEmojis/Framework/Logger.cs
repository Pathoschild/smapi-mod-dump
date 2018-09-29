
using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace CustomEmojis.Framework {

    public class Logger : IDisposable {

        private string FilePath { get; set; }
        private IMonitor ModMonitor { get; set; }
        private StreamWriter Stream { get; set; }

        public bool LogToMonitor { get; set; }

        public Logger(string filePath, bool append = true, IMonitor monitor = null) {
            SetFilePath(filePath, append);
            if(monitor != null) {
                SetMonitor(monitor);
                LogToMonitor = true;
            }
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

            WriteLine($"{time} {sb.ToString()}");

            if(LogToMonitor) {
                ModMonitor?.Log(sb.ToString());
            }

        }

        public void Log(params string[] messages) {

            string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
            int timeLength = time.Length + 1;

            WriteLine($"{time} {messages[0]}");

            if(LogToMonitor) {
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

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {

            if(disposing) {
                // free managed resources  
                if(Stream != null) {
                    Stream.Dispose();
                    Stream = null;
                }
            }

        }

    }

}
