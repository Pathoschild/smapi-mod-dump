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
using StardewWebApi.Game;
using StardewWebApi.Game.Items;
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.V1;

[Route("/api/v1/ui")]
public class UIController : ApiControllerBase
{
    [Route("/actions/showHudMessage")]
    public void ShowHUDMessage(string message, HUDMessageType? type = null, int? duration = null)
    {
        UIUtilities.ShowHUDMessage(message, type, duration);
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/showHudMessage/item")]
    public void ShowHUDMessageForItem(
        string message,
        int? duration = null,
        string? itemId = null,
        string? itemName = null,
        int amount = 1,
        int quality = 0,
        string? typeKey = null
    )
    {
        Item? item;
        if (itemId is not null)
        {
            item = ItemUtilities.GetItemByFullyQualifiedId(itemId, amount, quality);
            if (item is null)
            {
                Response.BadRequest($"Item with ID '{itemId}' does not exist");
                return;
            }
        }
        else if (itemName is not null)
        {
            item = ItemUtilities.GetItemByDisplayName(itemName, amount, quality);
            if (item is null)
            {
                Response.BadRequest($"Item with name '{itemName}' does not exist");
                return;
            }
        }
        else
        {
            Response.BadRequest($"Either {nameof(itemId)} or {nameof(itemName)} must be specified");
            return;
        }

        UIUtilities.ShowHUDMessageForItem(
            item,
            message,
            duration,
            typeKey
        );
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/showHudMessage/large")]
    public void ShowLargeHUDMessage(string message)
    {
        UIUtilities.ShowLargeHUDMessage(message);
        Response.Ok(new ActionResult(true));
    }

    [Route("/actions/showChatMessage")]
    public void ShowChatMessage(string message, ChatMessageType type = ChatMessageType.None)
    {
        UIUtilities.ShowChatMessage(message, type);
        Response.Ok(new ActionResult(true));
    }
}