/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule.Framework;

using System.Linq;
using StardewMods.Common.Integrations.OrdinaryCapsule;
using StardewMods.OrdinaryCapsule.Framework.Models;

/// <inheritdoc />
public sealed class Api : IOrdinaryCapsuleApi
{
    private readonly IModHelper _helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Api" /> class.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    public Api(IModHelper helper)
    {
        this._helper = helper;
    }

    /// <inheritdoc />
    public void RegisterItem(ICapsuleItem item)
    {
        var capsuleItems = this._helper.ModContent.Load<CapsuleItems>("assets/items.json");
        var existingItem = capsuleItems.FirstOrDefault(capsuleItem => capsuleItem.ContextTags.SetEquals(item.ContextTags));
        if (existingItem is not null)
        {
            return;
        }

        capsuleItems.Add(item);
        this._helper.Data.WriteJsonFile("assets/items.json", capsuleItems);
    }
}