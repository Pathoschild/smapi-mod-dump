/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using MoonShared.APIs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using BirbCore.Attributes;
using HarmonyLib;
using MoonShared.Patching;
using ArchaeologySkill.Objects.Water_Shifter;
using StardewModdingAPI.Events;
using SpaceShared.APIs;

namespace ArchaeologySkill
{
    [SMod]
    public class ModEntry : Mod
    {
        [SMod.Instance]
        internal static ModEntry Instance;

        internal static Config Config;
        internal static Assets Assets;
        internal static bool XPDisplayLoaded => ModEntry.Instance.Helper.ModRegistry.IsLoaded("Shockah.XPDisplay");
        internal static bool JsonAssetsLoaded => ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
        internal static bool DynamicGameAssetsLoaded => ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");

        internal readonly List<Func<Item, (int? SkillIndex, string? SpaceCoreSkillName)?>> ToolSkillMatchers =
        [
            o => o is Hoe ? (null, "moonslime.Archaeology") : null,
            o => o is Pan ? (null, "moonslime.Archaeology") : null
        ];

        public ITranslationHelper I18N => this.Helper.Translation;
        internal static MoonShared.APIs.IJsonAssetsApi JAAPI;
        internal static IDynamicGameAssetsApi DGAAPI;
        internal static IXPDisplayApi XpAPI;

        public static Dictionary<string, List<string>> ItemDefinitions;
        public static readonly IList<string> BonusLootTable = [];
        public static readonly IList<string> ArtifactLootTable = [];
        public static readonly IList<string> WaterSifterLootTable = [];





        public override void Entry(IModHelper helper)
        {
            Instance = this;

            Parser.ParseAll(this);
        }

    }
}
