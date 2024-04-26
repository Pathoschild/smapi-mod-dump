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
using StardewWebApi.Game.Items;
using StardewWebApi.Game.Players;
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.V1;

[RequireLoadedGame]
[Route("/api/v1/player")]
public class PlayerController : ApiControllerBase
{
    [Route("/")]
    public void GetPlayerInfo()
    {
        Response.Ok(Player.FromMain());
    }

    [Route("/actions/refillEnergy")]
    public void RefillEnergy()
    {
        PlayerActions.RefillEnergy();
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/passOut")]
    public void PassOut()
    {
        PlayerActions.PassOut();
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/fullyHeal")]
    public void FullyHeal()
    {
        PlayerActions.FullyHeal();
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/knockOut")]
    public void KnockOut()
    {
        PlayerActions.KnockOut();
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/giveMoney/{amount}")]
    public void GiveMoney(int amount = 1000)
    {
        PlayerActions.GiveMoney(amount);
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/giveItem/id/{itemId}")]
    public void GiveItemById(string itemId, int amount = 1, int quality = 0)
    {
        var item = ItemUtilities.GetItemByFullyQualifiedId(itemId, amount, quality);
        if (item is null)
        {
            Response.BadRequest($"Item with ID '{itemId}' does not exist");
            return;
        }

        PlayerActions.GiveItem(item);
        Response.Ok(new ActionResult(true, BasicItem.FromItem(item)));
    }

    [Route("/actions/giveItem/name/{itemName}")]
    public void GiveItemByName(string itemName, int amount = 1, int quality = 0)
    {
        var item = ItemUtilities.GetItemByDisplayName(itemName, amount, quality);
        if (item is null)
        {
            Response.BadRequest($"Item with name '{itemName}' does not exist");
            return;
        }

        PlayerActions.GiveItem(item);
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/warpPlayer/{location}")]
    public void WarpPlayer(WarpLocation location, bool playWarpAnimation = true)
    {
        Response.Ok(PlayerActions.WarpPlayer(location, playWarpAnimation));
    }

    [Route("/actions/petFarmAnimal/{name}")]
    public void PetFarmAnimal(string name)
    {
        Response.Ok(PlayerActions.PetFarmAnimal(name));
    }
}