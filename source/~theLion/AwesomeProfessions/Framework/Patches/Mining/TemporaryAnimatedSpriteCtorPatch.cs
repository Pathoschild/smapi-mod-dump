/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class TemporaryAnimatedSpriteCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TemporaryAnimatedSpriteCtorPatch()
    {
        Original = RequireConstructor<TemporaryAnimatedSprite>(typeof(int), typeof(float), typeof(int), typeof(int),
            typeof(Vector2), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist bomb radius.</summary>
    [HarmonyPostfix]
    private static void TemporaryAnimatedSpriteCtorPostfix(ref TemporaryAnimatedSprite __instance, Farmer owner)
    {
        if (owner.HasProfession(Profession.Demolitionist)) ++__instance.bombRadius;
        if (owner.HasProfession(Profession.Demolitionist, true)) ++__instance.bombRadius;
    }

    #endregion harmony patches
}