/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Internationalization.Handlers
{
    public class StaticHandler : RequestHandler
    {
        static public Dictionary<string,string> Mime = new Dictionary<string, string> {
            { ".css",  "text/css" },
            { ".html", "text/html" },
            { ".js",   "text/javascript" },
            { ".json", "application/json" },
            { ".png",  "application/png" },
        };

        readonly string root;
        public StaticHandler(string root) { 
            this.root = root;
        }

        public override bool Get(Request r) {
            if (r.path.Length != 1) return r.status(HttpStatusCode.Forbidden);

            var file = Path.Combine(root, r.path[0]);
            try {
                // Check ETag
                var etag = File.GetLastWriteTimeUtc(file).GetHashCode().ToString();
                if (r.req.Headers["If-None-Match"] == etag) return r.status(HttpStatusCode. NotModified);

                // Read file
                var data = File.ReadAllBytes(file);

                // Send data
                r.res.AddHeader("Etag", etag);
                var ext = Path.GetExtension(r.path[0]);
                if (Mime.TryGetValue(ext, out var mime)) r.content(mime);
                if (data.Length > 0) {
                    return r.write_buffer(HttpStatusCode.OK, data);
                }
            } catch {
            }
            return r.status(HttpStatusCode.NotFound);
        }

    }
}
