/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public interface IRecipePart : IItemRequest {
        /// <summary>This recipe part's sprite.</summary>
        ISprite Sprite { get; }

        /// <summary>Gets the name of this recipe part.</summary>
        /// <returns>The name of this recipe part.</returns>
        string GetDisplayName();

        /// <summary>Tries to create an instance of this recipe part.</summary>
        /// <param name="result">An item which matches this recipe part.</param>
        /// <returns>True if successfully created, false otherwise.</returns>
        bool TryCreateOne(out Item result);
    }
}