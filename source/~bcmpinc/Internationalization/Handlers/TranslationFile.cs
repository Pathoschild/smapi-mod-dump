/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.IO;
using System.Net;

namespace Internationalization.Handlers
{
    public class TranslationFile : RequestHandler
    {
        public TranslationFile() {}

        // Requesting translation files.
        public override bool Get(Request r) {
            if (r.path.Length != 2) return r.status(HttpStatusCode.BadRequest);
            var file = TranslationRegistry.TranslationPath(r.path[0], r.path[1]);
            if (file==null) return r.status(HttpStatusCode.NotFound);
            try {
                var data = File.ReadAllBytes(file);
                r.content_javascript(); // These files often contain comments, which is not valid json.
                return r.write_buffer(HttpStatusCode.OK, data);
            } catch {
                return r.status(HttpStatusCode.NotFound);
            }
        }

        // Saving translation files.
        public override bool Put(Request r) {
            if (r.path.Length != 2) return r.status(HttpStatusCode.BadRequest);
            if (!r.req.HasEntityBody) return r.status(HttpStatusCode.BadRequest);
            var file = TranslationRegistry.TranslationPath(r.path[0], r.path[1]);
            if (file==null) return r.status(HttpStatusCode.NotFound);
            try {
                ModEntry.Log($"Writing new translation file to '{file}'.", StardewModdingAPI.LogLevel.Info);
                var stream = r.req.InputStream;
                // Make backup of old file.
                try { File.Move(file, file+"~"); } catch {};
                var output = File.OpenWrite(file);
                stream.CopyTo(output);
                output.Close();
                return r.status(HttpStatusCode.NoContent);
            } catch {
                return r.status(HttpStatusCode.NotFound);
            }
        }
    }
}
