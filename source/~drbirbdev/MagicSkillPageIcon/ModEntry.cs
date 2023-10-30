/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Reflection;
using BirbShared;
using BirbShared.Mod;
using HarmonyLib;
using SpaceCore;
using StardewModdingAPI;

namespace MagicSkillPageIcon
{

    class ModEntry : Mod
    {
        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiAsset]
        internal static Assets Assets;

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this, true);
        }
    }

    [HarmonyPatch("Magic.Framework.Skills.Skill", "GetName")]
    class Skill_Constructor
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.Magic");
        }

        public static void Postfix(Skills.Skill __instance)
        {
            try
            {
                if (__instance.SkillsPageIcon is null)
                {
                    __instance.SkillsPageIcon = ModEntry.Assets.SkillPageIcon;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }
}
