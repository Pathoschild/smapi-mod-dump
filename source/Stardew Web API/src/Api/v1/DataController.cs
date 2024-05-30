/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewWebApi.Game.Players;
using StardewWebApi.Game.RawData;
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.v1;

[Route("/api/v1/data")]
public class DataController : ApiControllerBase
{
    [Route("/achievements")]
    public void GetAchievements()
    {
        Response.Ok(DataLoader.Achievements(Game1.content).Select(a => new Achievement(a.Key)));
    }

    [Route("/locations")]
    public void GetLocations()
    {
        Response.Ok(DataLoader.Locations(Game1.content)
            .Select(l => new LocationStub(l.Key, l.Value))
        );
    }

    [Route("/locations/{name}")]
    public void GetLocationByName(string name)
    {
        if (DataLoader.Locations(Game1.content).ContainsKey(name))
        {
            Response.Ok(new Location(name, DataLoader.Locations(Game1.content)[name]));
        }
        else
        {
            Response.NotFound($"No location found with name {name}");
        }
    }
}