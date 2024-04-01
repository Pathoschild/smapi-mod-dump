/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.Net;
using System.Text;

namespace Internationalization
{
    public abstract class RequestHandler
    {
        public bool Handle(Request r) {
            try {
                switch (r.req.HttpMethod) {
                    case "GET": return Get(r);
                    case "PUT": return Put(r);
                    default: return r.status(HttpStatusCode.MethodNotAllowed);
                }
            } catch (System.Exception ex) {
                return r.write_text(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        public virtual bool Get(Request r) => r.status(HttpStatusCode.MethodNotAllowed);
        public virtual bool Put(Request r) => r.status(HttpStatusCode.MethodNotAllowed);
    }

    public class Request
    {
        public string[] path;
        public readonly HttpListenerRequest req;
        public readonly HttpListenerResponse res;

        internal Request(HttpListenerContext ctx)
        {
            path = ctx.Request.Url.LocalPath.Split("/", System.StringSplitOptions.RemoveEmptyEntries); ;
            req = ctx.Request;
            res = ctx.Response;
            ModEntry.Log($"{req.HttpMethod} {req.Url}");
        }

        public bool status(HttpStatusCode code) {
            res.StatusCode = (int)code;
            res.StatusDescription = code.ToString();
            return true;
        }
        public bool write_buffer(HttpStatusCode status, byte[] data) {
            this.status(status);
            res.ContentLength64 = data.Length;
            res.OutputStream.Write(data, 0, data.Length);
            return true;
        }

        public bool write_text(HttpStatusCode status, string data) {
            res.ContentEncoding = Encoding.UTF8;
            return write_buffer(status, Encoding.UTF8.GetBytes(data));
        }

        public void content(string type) => res.Headers.Set("Content-Type", type);
        public void content_text() => content("text/plain");
        public void content_json() => content("application/json");
        public void content_javascript() => content("text/javascript");
    }
}
