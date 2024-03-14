/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass.Patches;

/// <summary>Contains patches for patching game code in the <see cref="Grass"/> class.</summary>
internal class GrassPatch
{
    /*********
    ** Internal Methods
    *********/
    /// <summary>The prefix for the <see cref="Grass.reduceBy(int, bool)"/> method.</summary>
    /// <param name="showDebris">Whether debris should be drawn.</param>
    /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
    /// <param name="__result">The return value of the method being patched.</param>
    /// <returns><see langword="true"/> if the original method should get ran; otherwise <see langword="false"/> (depending on the mod configuration).</returns>
    /// <remarks>This is used so animals won't eat grass if the configuration forbids them from it.</remarks>
    internal static bool ReduceByPrefix(bool showDebris, Grass __instance, ref bool __result)
    {
        // ensure animals aren't allowed to eat grass
        if (ModEntry.Instance.Config.CanAnimalsEatGrass)
            return true;

        // reimplement the method and force the return value to be false (meaning the grass won't get destroyed)
        if (showDebris)
        {
            var tileLocation = __instance.Tile;
            Game1.createRadialDebris(Game1.currentLocation, __instance.textureName(), new Rectangle(2, 8, 8, 8), 1, ((int)tileLocation.X + 1) * 64, ((int)tileLocation.Y + 1) * 64, Game1.random.Next(6, 14), (int)tileLocation.Y + 1, Color.White, 4);
        }
        __result = false;
        return false;
    }

    /// <summary>The prefix for the <see cref="Grass.seasonUpdate(bool)"/> method.</summary>
    /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
    /// <param name="__result">Whether all the grass should be killed (this is the return value of the original method).</param>
    /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
    /// <remarks>This is used to determine if grass should get killed at the beginning of a new season based on the mod configuration.</remarks>
    internal static bool SeasonUpdatePrefix(Grass __instance, ref bool __result)
    {
        __result = Game1.GetSeasonForLocation(__instance.Location) switch
        {
            Season.Spring => !ModEntry.Instance.Config.CanGrassLiveInSpring,
            Season.Summer => !ModEntry.Instance.Config.CanGrassLiveInSummer,
            Season.Fall => !ModEntry.Instance.Config.CanGrassLiveInFall,
            Season.Winter => !ModEntry.Instance.Config.CanGrassLiveInWinter
        };

        // recalculate the new textures for grass
        __instance.loadSprite();
        __instance.setUpRandom();

        return false;
    }

    /// <summary>The prefix for the <see cref="Grass.loadSprite"/> method.</summary>
    /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
    /// <remarks>This is used to load the custom grass sprite.</remarks>
    internal static bool LoadSpritePrefix(Grass __instance)
    {
        var spritePool = ModEntry.Instance.GetSpritePoolBySeason(Game1.GetSeasonForLocation(__instance.Location));

        __instance.texture = new Lazy<Texture2D>(() => spritePool.Atlas);
        __instance.grassSourceOffset.Value = 0;

        return false;
    }

    /// <summary>The transpiler for the <see cref="Grass.performToolAction(Tool, int, Vector2)"/> method.</summary>
    /// <param name="instructions">The IL instructions.</param>
    /// <returns>The new IL instructions.</returns>
    /// <remarks>This is used to change the colour of the break animation of grass in winter.</remarks>
    internal static IEnumerable<CodeInstruction> PerformToolActionTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && instruction.operand == typeof(Color).GetMethod("get_Green", BindingFlags.Public | BindingFlags.Static))
                instruction.operand = typeof(Color).GetMethod("get_DarkTurquoise", BindingFlags.Public | BindingFlags.Static);

            yield return instruction;
        }
    }

    /// <summary>The post fix for the <see cref="Grass.setUpRandom"/> method.</summary>
    /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
    /// <remarks>This is used for setting the MoreGrass version of the 'whichWeed' member which ensures the custom sprite is drawn correctly.</remarks>
    internal static void SetupRandomPostFix(Grass __instance)
    {
        var spritePool = ModEntry.Instance.GetSpritePoolBySeason(Game1.GetSeasonForLocation(__instance.Location));
        
        var defaultSpriteOnly = Utilities.ShouldForceGrassToDefault(__instance.Location.Name) || Game1.random.NextDouble() < (ModEntry.Instance.Config.PercentCoverageOfDefaultGrass / 100f);
        
        for (int i = 0; i < 4; i++)
        {
            var spriteId = spritePool.GetRandomSpriteId(defaultSpriteOnly);

            var offset = SpritePool.GetOffsetFromSpriteId(spriteId);
            __instance.modData[ModDataConstants.GrassOffsetXBase + i] = offset.X.ToString();
            __instance.modData[ModDataConstants.GrassOffsetYBase + i] = offset.Y.ToString();
        }
    }

    /// <summary>The prefix for the <see cref="Grass.draw(SpriteBatch)"/> method.</summary>
    /// <param name="spriteBatch">The sprite batch to draw the grass to.</param>
    /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
    /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
    /// <remarks>This is used to draw the grass sprites.</remarks>
    internal static bool DrawPrefix(SpriteBatch spriteBatch, Grass __instance)
    {
        var offset1 = (int[])typeof(Grass).GetField("offset1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var offset2 = (int[])typeof(Grass).GetField("offset2", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var offset3 = (int[])typeof(Grass).GetField("offset3", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var offset4 = (int[])typeof(Grass).GetField("offset4", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var shakeRotation = (float)typeof(Grass).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var shakeRandom = (double[])typeof(Grass).GetField("shakeRandom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
        var flip = (bool[])typeof(Grass).GetField("flip", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

        var tileLocation = __instance.Tile;
        for (int i = 0; i < __instance.numberOfWeeds.Value; i++)
        {
            var globalPosition = i != 4
                ? tileLocation * 64f + new Vector2(x: i % 2 * 64 / 2 + offset3[i] * 4 - 4 + 30, y: i / 2 * 64 / 2 + offset4[i] * 4 + 40)
                : tileLocation * 64f + new Vector2(x: 16 + offset1[i] * 4 - 4 + 30, y: 16 + offset2[i] * 4 + 40);

            spriteBatch.Draw(
                texture: __instance.texture.Value,
                position: Game1.GlobalToLocal(Game1.viewport, globalPosition),
                sourceRectangle: new Rectangle(int.Parse(__instance.modData[ModDataConstants.GrassOffsetXBase + i]), int.Parse(__instance.modData[ModDataConstants.GrassOffsetYBase + i]), 15, 20),
                color: Color.White,
                rotation: shakeRotation / (float)(shakeRandom[i] + 1),
                origin: new Vector2(7.5f, 17.5f),
                scale: 4f,
                effects: flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: (globalPosition.Y + 16 - 20) / 10000f + globalPosition.X / 10000000f
            );
        }

        return false;
    }
}
