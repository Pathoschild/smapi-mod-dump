/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using Microsoft.Owin.Hosting;
using Owin;
using System.Threading.Tasks;
using System.IO;

namespace StarStats.Web
{
    public class StatServer : Mod
    {

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            if(config.ContentDir == null)
            {
                config.ContentDir = Path.Combine(Helper.DirectoryPath,"content");
            }
            Task.Run(() =>
            {
                using (WebApp.Start(config.BindAddress, Configuration))
                {
                    Monitor.Log($"Server running on {config.BindAddress}");
                    Console.ReadKey();
                }
            });
        }

        public void Configuration(IAppBuilder app)
        {
            app.Use(async (context, nextMiddleWare) =>
            {
                if(context.Request.Path.Value == "/")
                {
                    var resp = context.Response;
                    resp.StatusCode = 200;
                    resp.Headers["Content-Type"] = "text/html";
                    using(var f = File.OpenRead(Path.Combine(config.ContentDir, "index.html")))
                    {
                        await f.CopyToAsync(resp.Body);
                    }
                    return;
                }
                await nextMiddleWare();
  
            });
            app.MapSignalR();
            
        }
    }
}
