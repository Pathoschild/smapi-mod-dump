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
using StardewWebApi.Server;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Api.V1;

[RequireLoadedGame]
[Route("/api/v1/items")]
public class ItemsController : ApiControllerBase
{
    [Route("/")]
    public void GetAllItems()
    {
        Response.Ok(ItemUtilities.GetAllItems().Select(i => BasicItem.FromItem(i)));
    }

    [Route("/id/{itemId}")]
    public void GetItemById(string itemId)
    {
        Response.Ok(BasicItem.FromItem(ItemUtilities.GetItemByFullyQualifiedId(itemId)));
    }

    [Route("/type/{itemType}")]
    public void GetAllItemsByType(string itemType)
    {
        Response.Ok(ItemUtilities.GetAllItemsByType(itemType).Select(i => BasicItem.FromItem(i)));
    }

    [Route("/type/{itemType}/id/{itemId}")]
    public void GetItemByTypeAndId(string itemType, string itemId)
    {
        Response.Ok(BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId(itemType, itemId)));
    }
}