/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraShared.Wrappers;

using CommunityToolkit.Diagnostics;

using Microsoft.Xna.Framework;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions for SObject.
/// </summary>
public static class SObjectExtensions
{
    /// <summary>
    /// Creates a TAS that represents a parabolic arc.
    /// </summary>
    /// <param name="obj">Object to throw.</param>
    /// <param name="start">Start location.</param>
    /// <param name="end">End location.</param>
    /// <param name="mp">Multiplayer instance.</param>
    /// <param name="loc">GameLocation.</param>
    /// <returns>Total time the parabolic arc will take.</returns>
    public static float ParabolicThrowItem(this SObject obj, Vector2 start, Vector2 end, Multiplayer mp, GameLocation loc)
    {
        const float gravity = 0.0025f;

        float velocity = -0.08f;
        Vector2 delta = end - start;
        if (delta.Y < 40)
        {
            // Ensure the initial velocity is sufficiently fast to make it all the way up.
            velocity -= MathF.Sqrt(2 * MathF.Abs(delta.Y + 80) * gravity);
        }
        float time = (MathF.Sqrt(Math.Max((velocity * velocity) + (gravity * delta.Y * 2f), 0)) / gravity) - (velocity / gravity);
        mp.broadcastSprites(
            loc,
            new TemporaryAnimatedSprite(
                textureName: Game1.objectSpriteSheetName,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, 16, 16),
                position: start,
                flipped: false,
                alphaFade: 0f,
                color: Color.White)
            {
                scale = Game1.pixelZoom,
                layerDepth = 1f,
                totalNumberOfLoops = 1,
                interval = time,
                acceleration = new Vector2(0f, gravity),
                motion = new Vector2(delta.X / time, velocity),
                timeBasedMotion = true,
            });
        return time;
    }

    /// <summary>
    /// Gets whether or not an SObject is a trash item.
    /// </summary>
    /// <param name="obj">SObject to check.</param>
    /// <returns>true if it's a trash item, false otherwise.</returns>
    public static bool IsTrashItem(this SObject obj)
        => obj is not null && !obj.bigCraftable.Value && (obj.ParentSheetIndex >= 168 && obj.ParentSheetIndex < 173);

    /// <summary>
    /// Returns true for an item that would be considered alcohol.
    /// </summary>
    /// <param name="obj">SObject.</param>
    /// <returns>True if alcohol.</returns>
    public static bool IsAlcoholItem(this SObject obj)
    {
        return obj.HasContextTag("alcohol_item") || obj.Name.Contains("Beer") || obj.Name.Contains("Wine") || obj.Name.Contains("Mead") || obj.Name.Contains("Pale Ale");
    }

    /// <summary>
    /// Gets the category number corresponding to an SObject's index.
    /// </summary>
    /// <param name="sObjectInd">Index of the item to check.</param>
    /// <returns>The category index if found, or 0 otherwise.</returns>
    public static int GetCategoryFromIndex(this int sObjectInd)
    {
        if (!Game1Wrappers.ObjectInfo.TryGetValue(sObjectInd, out string? data))
        {
            return 0;
        }

        ReadOnlySpan<char> cat = data.GetNthChunk('/', SObject.objectInfoTypeIndex);

        int index = cat.IndexOf(' ');
        if (index < 0)
        {
            return 0;
        }

        return int.TryParse(cat[(index + 1)..], out int categoryIndex) && categoryIndex < 0
            ? categoryIndex
            : 0;
    }

    /// <summary>
    /// Gets the public name of a bigcraftable.
    /// </summary>
    /// <param name="bigCraftableIndex">Bigcraftable.</param>
    /// <returns>public name if found.</returns>
    public static string GetBigCraftableName(this int bigCraftableIndex)
    {
        if (Game1.bigCraftablesInformation.TryGetValue(bigCraftableIndex, out string? value))
        {
            int index = value.IndexOf('/');
            if (index >= 0)
            {
                return value[..index];
            }
        }
        return "ERROR - big craftable not found!";
    }

    /// <summary>
    /// Gets the translated name of a bigcraftable.
    /// </summary>
    /// <param name="bigCraftableIndex">Index of the bigcraftable.</param>
    /// <returns>Name of the bigcraftable.</returns>
    public static string GetBigCraftableTranslatedName(this int bigCraftableIndex)
    {
        if (Game1.bigCraftablesInformation?.TryGetValue(bigCraftableIndex, out string? value) == true)
        {
            int index = value.LastIndexOf('/');
            if (index >= 0 && index < value.Length - 1)
            {
                return value[(index + 1)..];
            }
        }
        return "ERROR - big craftable not found!";
    }

    /// <summary>
    /// Consumes a recipe by teaching the player the recipe.
    /// </summary>
    /// <param name="obj">The object instance.</param>
    /// <returns>True if the recipe was taught, false otherwise.</returns>
    public static bool ConsumeRecipeImpl(this SObject obj)
    {
        Guard.IsNotNull(obj);
        Guard.IsNotNull(Game1.player);

        if (obj.IsRecipe)
        {
            string recipeName = obj.Name;

            // vanilla removes the word "Recipe" from the end
            // because ???
            int idx = recipeName.LastIndexOf("Recipe");
            if (idx > 0)
            {
                recipeName = recipeName[.. (idx - 1)];
            }

            return obj.Category == SObject.CookingCategory
                ? Game1.player.cookingRecipes.TryAdd(recipeName, 0)
                : Game1.player.craftingRecipes.TryAdd(recipeName, 0);
        }
        return false;
    }
}