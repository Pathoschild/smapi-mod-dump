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
[Route("/api/v1/players")]
public class PlayersController : ApiControllerBase
{
    [Route("/main")]
    public void GetMainPlayerInfo()
    {
        Response.Ok(Player.Main);
    }

    [Route("/main/inventory")]
    public void GetMainPlayerInventory()
    {
        Response.Ok(Player.Main.GetInventory());
    }

    [Route("/main/actions/refillEnergy")]
    public void RefillEnergy()
    {
        Player.Main.RefillEnergy();
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/passOut")]
    public void PassOut()
    {
        Player.Main.PassOut();
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/fullyHeal")]
    public void FullyHeal()
    {
        Player.Main.FullyHeal();
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/knockOut")]
    public void KnockOut()
    {
        Player.Main.KnockOut();
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/giveMoney/{amount}")]
    public void GiveMoney(int amount = 1000)
    {
        Player.Main.GiveMoney(amount);
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/giveItem/id/{itemId}")]
    public void GiveItemById(string itemId, int amount = 1, int quality = 0)
    {
        var item = ItemUtilities.GetItemByFullyQualifiedId(itemId, amount, quality);
        if (item is null)
        {
            Response.BadRequest($"Item with ID '{itemId}' does not exist");
            return;
        }

        Player.Main.GiveItem(item);
        Response.Ok(new ActionResult(true, BasicItem.FromItem(item)));
    }

    [Route("/main/actions/giveItem/name/{itemName}")]
    public void GiveItemByName(string itemName, int amount = 1, int quality = 0)
    {
        var item = ItemUtilities.GetItemByDisplayName(itemName, amount, quality);
        if (item is null)
        {
            Response.BadRequest($"Item with name '{itemName}' does not exist");
            return;
        }

        Player.Main.GiveItem(item);
        Response.Ok(new ActionResult(true));
    }

    [Route("/main/actions/warpPlayer/{location}")]
    public void WarpPlayer(WarpLocation location, bool playWarpAnimation = true)
    {
        Response.Ok(Player.Main.WarpPlayer(location, playWarpAnimation));
    }

    [Route("/main/actions/petFarmAnimal/{name}")]
    public void PetFarmAnimal(string name)
    {
        Response.Ok(Player.Main.PetFarmAnimal(name));
    }
}