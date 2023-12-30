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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using StardewSpeak.Pathfinder;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StardewValley.Menus;
using System.Reflection;
using System.Collections.Concurrent;

namespace StardewSpeak
{
    public class SpeechEngine
    {
        Process Proc;
        public readonly object RequestQueueLock;
        public readonly Action<dynamic> OnMessage;
        public HashSet<string> UnvalidatedModeAllowableMessageTypes = new()
        { 
            "HEARTBEAT", "REQUEST_BATCH", "NEW_STREAM", "STOP_STREAM", "GET_ACTIVE_MENU", "GET_MOUSE_POSITION",
            "SET_MOUSE_POSITION", "SET_MOUSE_POSITION_RELATIVE", "MOUSE_CLICK", "UPDATE_HELD_BUTTONS", "RELEASE_ALL_KEYS",
            "PRESS_KEY"
        };
        public SpeechProcessNamedPipe NamedPipe;


        public SpeechEngine(Action<dynamic> onMessage)
        {
            this.OnMessage = onMessage;
            this.RequestQueueLock = new object();
            this.NamedPipe = new SpeechProcessNamedPipe(this.OnNamedPipeInput);
        }

        public void Restart() 
        {
            this.NamedPipe.StartShutdown();
            this.NamedPipe = new SpeechProcessNamedPipe(this.OnNamedPipeInput);
            this.LaunchProcess();
        }

        public void LaunchProcess()
        {
            string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string namedPipe = this.NamedPipe.FileName;
            #if DEBUG
                ModEntry.Log("Running Python client in debug mode", LogLevel.Trace);
                string pythonRoot = Path.Combine(rootDir, @"StardewSpeak\lib\speech-client");
                string executable = Path.Combine(pythonRoot, @".venv\Scripts\python.exe");
                string main = Path.Combine(pythonRoot, @"speech-client\main.py");
                string arguments = $"\"{main}\" --python_root \"{pythonRoot}\" --named_pipe \"{namedPipe}\"";
#else
                ModEntry.Log("Running Python client in release mode", LogLevel.Trace);
                string pythonRoot = Path.Combine(rootDir, @"lib\speech-client\dist");
                string executable = Path.Combine(pythonRoot, @"speech-client.exe");
                string arguments = $"--python_root \"{pythonRoot}\" --named_pipe \"{namedPipe}\"";
#endif
            if (!File.Exists(executable))
            {
                ModEntry.Log($"Missing executable {executable}", LogLevel.Error);
            }
            else
            {
                ModEntry.Log($"Launching Python client '{executable} {arguments}'", LogLevel.Debug);
                Task.Factory.StartNew(() => RunProcessAsync("\"" + executable + "\"", arguments));
            }
        }

        public async Task<int> RunProcessAsync(string fileName, string args)
        {
            using (this.Proc = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,

                },
                EnableRaisingEvents = true
            })
            {
                var proc = await RunProcessAsync(this.Proc).ConfigureAwait(false);
                return proc;
            }
        }

        public void Exit() 
        {
            this.NamedPipe.StartShutdown();
        }

        private void HandleExited(Process process, TaskCompletionSource<int> tcs)
        {
            tcs.SetResult(process.ExitCode);
        }
        private Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => HandleExited(process, tcs);

            bool started = process.Start();
            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }
            return tcs.Task;
        }

        public void RespondToMessage(dynamic msg, string gameLoopContext) 
        {   
            dynamic resp;
            bool unvalidatedGameContext = gameLoopContext == "UnvalidatedUpdateTicked";
            try
            {
                string msgType = msg.type;
                if (unvalidatedGameContext && !UnvalidatedModeAllowableMessageTypes.Contains(msgType))
                {
                    throw new InvalidOperationException($"Unsafe message type during unvalidated game context: {msgType}");
                }
                dynamic msgData = msg.data;
                resp = Requests.HandleRequest(msg);
            }
            catch (Exception e)
            {
                string body = e.ToString();
                string error = "STACK_TRACE";
                resp = new { body, error };
            }
            string msgId = msg.id;
            this.SendResponse(msgId, resp.body, resp.error);
        }

        void OnNamedPipeInput(string messageText) 
        {
            dynamic msg;
            try
            {
                msg = JsonConvert.DeserializeObject(messageText);
            }
            catch
            {
                return;
            }
            this.OnMessage(msg);
        }

        void SendResponse(string id, object value = null, object error = null) 
        {
            var respData = new { id, value, error };
            this.SendMessage("RESPONSE", respData);
        }

        public void SendMessage(string msgType, object data = null)     
        {
            var message = new MessageToEngine(msgType, data);
            var settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            settings.Error = (serializer, err) => err.ErrorContext.Handled = true;
            string msgStr = JsonConvert.SerializeObject(message, Formatting.None, settings);
            this.NamedPipe.SendQueue.Add(msgStr);
        }

        public void SendEvent(string eventType, object data = null) {
            var msg = new { eventType, data };
            this.SendMessage("EVENT", msg);
        }
    }

    public record MessageToEngine(string type, object data);

    public record ResponseData(string id, object value);

}
