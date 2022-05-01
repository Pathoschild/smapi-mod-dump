/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StardewSpeak
{
    public class SpeechProcessNamedPipe
    {
        public string FileName;
        readonly BinaryReader Reader;
        readonly BinaryWriter Writer;
        readonly NamedPipeServerStream ReaderStream;
        readonly NamedPipeServerStream WriterStream;
        readonly Action<string> OnMessage;
        public bool DoShutdown = false;
        bool ReadClosed = false;
        bool WriteClosed = false;
        public BlockingCollection<string> SendQueue = new();
        readonly CancellationTokenSource WriteCancel = new();
        public SpeechProcessNamedPipe(Action<string> onMessage)
        {
            this.OnMessage = onMessage;
            this.FileName = System.Guid.NewGuid().ToString();
            this.ReaderStream = new NamedPipeServerStream(this.FileName + "Reader");
            this.Reader = new BinaryReader(this.ReaderStream);
            this.ReaderStream.BeginWaitForConnection(this.RunReader, null);
            this.WriterStream = new NamedPipeServerStream(this.FileName + "Writer");
            this.Writer = new BinaryWriter(this.WriterStream);
            this.WriterStream.BeginWaitForConnection(this.RunWriter, null);

        }

        void RunReader(object state)
        {
            if (this.DoShutdown) return;
            while (true)
            {
                try
                {
                    string msg = this.ReadNext();
                    this.OnMessage(msg);
                }
                catch (EndOfStreamException)
                {
                    this.ReaderStream.Close();
                    this.ReaderStream.Dispose();
                    break;
                }
            }
            this.ReadClosed = true;
            this.StartShutdown();
        }
        void RunWriter(object state)
        {
            if (this.DoShutdown) return;
            string next;
            while (true)
            {
                try
                {
                    next = this.SendQueue.Take(this.WriteCancel.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                try
                {
                    this.SendMessage(next);
                }
                catch (Exception ex)
                {
                    if (ex is EndOfStreamException || ex is IOException)
                    {
                        ModEntry.Log("Client disconnected.", LogLevel.Debug);
                        break;
                    }
                    throw;
                }
            }
            this.WriterStream.Close();
            this.WriterStream.Dispose();
            this.WriteClosed = true;
            this.StartShutdown();
        }

        public void StartShutdown() 
        {
            if (this.DoShutdown) return;
            ModEntry.Log("Shutting down StardewSpeak named pipe", LogLevel.Debug);
            this.DoShutdown = true;
            if (!this.ReadClosed) this.ConnectToPipeServer(this.ReaderStream, this.FileName + "Reader");
            if (!this.WriteClosed)
            {
                this.WriteCancel.Cancel();
                this.ConnectToPipeServer(this.WriterStream, this.FileName + "Writer");
            }
        }

        void ConnectToPipeServer(NamedPipeServerStream stream, string name) 
        {
            if (stream.IsConnected) return;
            // unblock stream.WaitForConnection()
            using (NamedPipeClientStream npcs = new(name))
            {
                try
                {
                    npcs.Connect(500);
                }
                catch (TimeoutException) 
                {
                
                }
            }
        }


        string ReadNext() 
        {
            var len = (int)this.Reader.ReadUInt32();            // Read string length
            var str = new string(this.Reader.ReadChars(len));    // Read string
            return str;
        }

        void SendMessage(string message) 
        {
            var buf = Encoding.UTF8.GetBytes(message);     // Get ASCII byte array     
            this.Writer.Write((uint)buf.Length);                // Write string length
            this.Writer.Write(buf);
        }

    }
}
