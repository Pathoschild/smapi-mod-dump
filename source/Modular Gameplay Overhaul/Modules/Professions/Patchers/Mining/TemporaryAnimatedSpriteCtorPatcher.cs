/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class TemporaryAnimatedSpriteCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TemporaryAnimatedSpriteCtorPatcher"/> class.</summary>
    internal TemporaryAnimatedSpriteCtorPatcher()
    {
        this.Target = this.RequireConstructor<TemporaryAnimatedSprite>(
            typeof(int),
            typeof(float),
            typeof(int),
            typeof(int),
            typeof(Vector2),
            typeof(bool),
            typeof(bool),
            typeof(GameLocation),
            typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist bomb radius + allow manual detonation.</summary>
    [HarmonyPostfix]
    private static void TemporaryAnimatedSpriteCtorPostfix(TemporaryAnimatedSprite __instance, Farmer owner)
    {
        if (!owner.HasProfession(Profession.Demolitionist))
        {
            return;
        }

        __instance.bombRadius++;
        if (owner.HasProfession(Profession.Demolitionist, true))
        {
            __instance.bombRadius++;
        }

        if (!ProfessionsModule.Config.ControlsUi.ModKey.IsDown())
        {
            return;
        }

        __instance.totalNumberOfLoops = int.MaxValue;
        EventManager.Enable<ManualDetonationUpdateTickedEvent>();
    }

    #endregion harmony patches
}
