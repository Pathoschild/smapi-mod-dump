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