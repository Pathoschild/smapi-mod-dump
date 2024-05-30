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
using StardewWebApi.Game.NPCs;
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.V1;

[RequireLoadedGame]
[Route("/api/v1/npcs")]
public class NPCsController : ApiControllerBase
{
    [Route("/")]
    public void GetAllNPCs()
    {
        Response.Ok(NPCUtilities.GetAllNPCs()
            .Select(n => NPCInfo.FromNPC(n)
        ));
    }

    [Route("/name/{name}")]
    public void GetNPCByName(string name)
    {
        var npc = NPCInfo.FromNPCName(name);

        if (npc is not null)
        {
            Response.Ok(npc);
        }
        else
        {
            Response.NotFound($"No NPC found with name '{name}'");
        }
    }

    [Route("/birthday/{season}/{day}")]
    public void GetNPCByBirthday(string season, int day)
    {
        var npcs = NPCUtilities.GetNPCsByBirthday(season, day)
            .Select(n => NPCInfo.FromNPC(n));

        Response.Ok(npcs);
    }

    [Route("/pets")]
    public void GetAllPets()
    {
        var npcs = NPCUtilities.GetAllNPCsOfType(NPCType.Pet)
            .Select(n => NPCInfo.FromNPC(n));

        Response.Ok(npcs);
    }
}