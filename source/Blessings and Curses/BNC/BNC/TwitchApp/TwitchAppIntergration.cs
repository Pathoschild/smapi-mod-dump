/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace BNC.TwitchAppIntergration
{
    class AppIntergration
    {
        private readonly string _connection;
        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private CancellationTokenSource _source;
        private Task _task;

        public AppIntergration(string connection)
        {
            _connection = connection;
        }

        public void Start()
        {
            _messages = new ConcurrentQueue<string>();
            _source = new CancellationTokenSource();
            var token = _source.Token;
            _task = Task.Factory.StartNew(() =>
            {
                BNC_Core.Logger.Log("Starting Integration connection", LogLevel.Info);
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        using (var client = new NamedPipeClientStream(".", _connection, PipeDirection.In))
                        {
                            using (var reader = new StreamReader(client))
                            {

                                while (!token.IsCancellationRequested && !client.IsConnected)
                                {
                                    try
                                    {
                                        client.Connect(1000);
                                    }
                                    catch (TimeoutException)
                                    {
                                        // Ignore
                                    }
                                    catch (Win32Exception)
                                    {
                                        Thread.Sleep(500);
                                    }
                                    catch (Exception e)
                                    {
                                        BNC_Core.Logger.Log($"Error in pipe connection: {e}",LogLevel.Debug);
                                        Thread.Sleep(500);
                                    }
                                }
                                BNC_Core.Logger.Log("Connected to Integration", LogLevel.Info);
                                while (!token.IsCancellationRequested && client.IsConnected)
                                {
                                    var readTask = reader.ReadLineAsync();
                                    if (readTask.Wait(5000, token))
                                    {
                                        var line = readTask.Result;
                                        if (line != null)
                                        {
                                           // if (Utils.AnySpawned)
                                            //{
                                                Handle(line);
                                            //}
                                            //else
                                           // {
                                                //_messages.Enqueue(line);
                                           // }
                                        }
                                        else
                                        {
                                            BNC_Core.Logger.Log("Empty data, reconnect", LogLevel.Debug);
                                            break;
                                        }
                                        reader.DiscardBufferedData();
                                    }
                                    else
                                    {
                                        BNC_Core.Logger.Log("Read timeout, reconnect", LogLevel.Debug);
                                        break;
                                    }

                                    Thread.Sleep(50);
                                }
                                BNC_Core.Logger.Log("Disconnected to Integration", LogLevel.Debug);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        BNC_Core.Logger.Log($"Error in socket connection: {e}", LogLevel.Error);
                    }
                }
            }, token);
        }

        public void Close()
        {
            if (_source == null) return;
            _source.Cancel(true);
            _task?.Wait(1000);
            _task = null;
            _messages = null;
        }

        public void Reconnect()
        {
            Close();
            Start();
        }

        public void Update()
        {
            while (_messages.TryDequeue(out var line))
            {
                Handle(line);
            }
        }

        private void Handle(string line)
        {
            if (line.StartsWith("Action: "))
            {
               BNC_Core.Logger.Log(line, LogLevel.Debug);
                var action = line.Substring(8);
                BNC_Core.actionManager.HandleAction(action);
            }
            else if (line.StartsWith("Message: "))
            {
                BNC_Core.Logger.Log(line, LogLevel.Debug);
                var message = line.Substring(9);
                BNC_Core.actionManager.HandleMessage(message);
            }
        }
    }
}

