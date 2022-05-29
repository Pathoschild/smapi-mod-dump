/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuAddProfessionDescriptionsPatch()
    {
        Original = RequireMethod<LevelUpMenu>("addProfessionDescriptions");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions,
        string professionName)
    {
        try
        {
            if (!Enum.TryParse<Profession>(professionName, out var profession)) return true; // run original logic

            descriptions.Add(ModEntry.ModHelper.Translation.Get(professionName + ".name." +
                                                                (Game1.player.IsMale ? "male" : "female")));

            var skillIndex = (int) profession / 6;
            var currentLevel = Game1.player.GetUnmodifiedSkillLevel(skillIndex);
            descriptions.AddRange(ModEntry.ModHelper.Translation
                .Get(professionName + ".desc" +
                     (Game1.player.HasProfession(profession, true) ||
                      Game1.activeClickableMenu is LevelUpMenu && currentLevel > 10
                         ? ".prestiged"
                         : string.Empty)).ToString()
                .Split('\n'));

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}