/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Net;

namespace Internationalization.Handlers
{
    public class Images : RequestHandler
    {
        readonly IGameContentHelper content;
        public Images(IGameContentHelper content) { 
            this.content = content;
        }

        public override bool Get(Request r) {
            try {
                var assetName = string.Join("\\", r.path);
                var res = content.Load<Texture2D>(assetName);
                r.content("image/png");
                r.status(HttpStatusCode.OK);
                res.SaveAsPng(r.res.OutputStream, res.Width, res.Height);
                return true;
            } catch {
                return r.status(HttpStatusCode.NotFound);
            }
        }
    }
}
