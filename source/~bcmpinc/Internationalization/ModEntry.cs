/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewModdingAPI;
using System.Net;
using StardewModdingAPI.Events;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Internationalization.Handlers;

namespace Internationalization
{
    public class ModEntry : Mod {
        const string URI = "http://localhost:8018/";
        static ModEntry instance;

        private HttpListener server;
        private Task<HttpListenerContext> task;
        Dictionary<string,RequestHandler> handlers;

        public override void Entry(IModHelper helper) {
            instance = this;

            I18n.Init(helper.Translation);
            TranslationRegistry.Init(Helper.ModRegistry);

            // Start a webserver.
            server = new HttpListener();
            server.Prefixes.Add(URI);
            server.Start();

            // Location from where to serve static stuff.
            handlers = new Dictionary<string, RequestHandler> {
                {"static", new StaticHandler(Path.Combine(Helper.DirectoryPath, "Static"))},
                {"info",   new Info(Helper.Translation)},
                {"file",   new TranslationFile()},
                {"lang",   new Translations()},
                {"images", new Images(Helper.GameContent)},
            };

            Monitor.Log("Translation website available at: " + URI, LogLevel.Alert);

            Helper.Events.GameLoop.UpdateTicking += process;
        }

        private void process(object sender, UpdateTickingEventArgs e) {
            if (task == null) {
                task = server.GetContextAsync();
            }
            for (task ??= server.GetContextAsync(); task.IsCompleted; task = server.GetContextAsync()) {
                var req = new Request(task.Result);

                if (!req.req.IsLocal) {
                    // Do not allow remote connections.
                    req.res.Abort();
                    return;
                }

                // Serve index when '/' is requested.
                if (req.path.Length == 0) req.path = new string[]{"static", "index.html"};

                bool handled = false;
                if (handlers.TryGetValue(req.path[0], out var handler)) {
                    req.path = req.path.Skip(1).ToArray();
                    handled |= handler.Handle(req);
                } 
                if (!handled) {
                    req.write_text(HttpStatusCode.NotFound, "No handler for: " + string.Join("/", req.path));
                    Monitor.Log("No handler for: " + string.Join("/", req.path));
                }
                req.res.Close();
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Trace) => instance.Monitor.Log(message, level);
        public static void LogOnce(string message, LogLevel level = LogLevel.Trace) => instance.Monitor.LogOnce(message, level);
    }
}

