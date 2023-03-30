/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace DialaTarotCSharp;

internal class AssetManager
{
    internal static void LoadOrEditAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("sophie.DialaTarot/CardBack"))
            e.LoadFromModFile<Texture2D>("Assets/cardBack.png", AssetLoadPriority.Medium);
        else if (e.NameWithoutLocale.IsEquivalentTo("sophie.DialaTarot/Event"))
            e.LoadFrom(
                () => new Dictionary<string, string>()
                {
                    ["Event"] = "none/-100 -100/farmer -100 -100 0/globalFadeToClear/skippable/pause 1000/cutscene DialaTarot/pause 1000/end"
                }, AssetLoadPriority.Medium);

        for (int i = 1; i <= TarotCard.Names.Count; i++)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"sophie.DialaTarot/Card{i}"))
                e.LoadFromModFile<Texture2D>($"Assets/{TarotCard.Names[i]}.png", AssetLoadPriority.Medium);
        }
    }
}
