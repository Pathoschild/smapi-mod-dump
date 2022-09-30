/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule;

using System.Collections.Generic;
using System.Linq;
using StardewMods.Common.Integrations.OrdinaryCapsule;
using StardewMods.OrdinaryCapsule.Models;

/// <inheritdoc />
public sealed class OrdinaryCapsuleApi : IOrdinaryCapsuleApi
{
    private readonly IModHelper _helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OrdinaryCapsuleApi" /> class.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    public OrdinaryCapsuleApi(IModHelper helper)
    {
        this._helper = helper;
    }

    /// <inheritdoc />
    public void RegisterItem(HashSet<string> contextTags, int productionTime, string? sound)
    {
        var capsuleItems = this._helper.ModContent.Load<List<CapsuleItem>>("assets/items.json");
        var existingItem = capsuleItems.FirstOrDefault(capsuleItem => capsuleItem.ContextTags.SetEquals(contextTags));
        if (existingItem is not null)
        {
            return;
        }

        capsuleItems.Add(new(contextTags, productionTime, sound));
        this._helper.Data.WriteJsonFile("assets/items.json", capsuleItems);
    }
}