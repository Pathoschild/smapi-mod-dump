/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Net;

namespace StardewWebApi.Server;

public abstract class ApiControllerBase
{
    public HttpListenerContext? HttpContext { get; set; }
    public HttpListenerRequest Request => HttpContext?.Request!;
    public HttpListenerResponse Response => HttpContext?.Response!;
}