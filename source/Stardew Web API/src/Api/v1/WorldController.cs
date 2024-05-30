/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewWebApi.Game;
using StardewWebApi.Game.World;
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.V1;

[Route("/api/v1/world")]
public class WorldController : ApiControllerBase
{
    [Route("/"), RequireLoadedGame]
    public void GetWorldInfo()
    {
        Response.Ok(World.Current);
    }

    [Route("/actions/playSound/{name}")]
    public void PlaySound(string name, int? pitch = null)
    {
        Response.Ok(WorldActions.PlaySound(name, pitch));
    }

    [Route("/actions/playMusicTrack/{name}")]
    public void PlayMusicTrack(string name)
    {
        Response.Ok(WorldActions.PlayMusicTrack(name));
    }

    [Route("/actions/stopMusicTrack")]
    public void StopMusicTrack()
    {
        Response.Ok(WorldActions.StopMusicTrack());
    }
}