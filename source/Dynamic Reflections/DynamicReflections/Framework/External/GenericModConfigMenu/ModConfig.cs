/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Models.Settings;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.External.GenericModConfigMenu
{
    public class ModConfig
    {
        public bool AreWaterReflectionsEnabled { get; set; } = true;
        public bool AreMirrorReflectionsEnabled { get; set; } = true;
        public bool ArePuddleReflectionsEnabled { get; set; } = true;
        public bool AreNPCReflectionsEnabled { get; set; } = true;
        public bool AreSkyReflectionsEnabled { get; set; } = true;

        public WaterSettings WaterReflectionSettings { get; set; } = new WaterSettings();
        public PuddleSettings PuddleReflectionSettings { get; set; } = new PuddleSettings();
        public SkySettings SkyReflectionSettings { get; set; } = new SkySettings();
        public int MeteorShowerNightChance { get; set; } = 10;

        public Dictionary<string, WaterSettings> LocalWaterReflectionSettings { get; set; } = new Dictionary<string, WaterSettings>();
        public Dictionary<string, PuddleSettings> LocalPuddleReflectionSettings { get; set; } = new Dictionary<string, PuddleSettings>();
        public Dictionary<string, SkySettings> LocalSkyReflectionSettings { get; set; } = new Dictionary<string, SkySettings>();
        public SButton QuickMenuKey { get; set; } = SButton.R;

        public WaterSettings GetCurrentWaterSettings(GameLocation location)
        {
            if (location is null || LocalWaterReflectionSettings is null || LocalWaterReflectionSettings.ContainsKey(location.NameOrUniqueName) is false || LocalWaterReflectionSettings[location.NameOrUniqueName] is null || LocalWaterReflectionSettings[location.NameOrUniqueName].OverrideDefaultSettings is false)
            {
                return WaterReflectionSettings;
            }

            return LocalWaterReflectionSettings[location.NameOrUniqueName];
        }

        public PuddleSettings GetCurrentPuddleSettings(GameLocation location)
        {
            if (location is null || LocalPuddleReflectionSettings is null || LocalPuddleReflectionSettings.ContainsKey(location.NameOrUniqueName) is false || LocalPuddleReflectionSettings[location.NameOrUniqueName] is null || LocalPuddleReflectionSettings[location.NameOrUniqueName].OverrideDefaultSettings is false)
            {
                return PuddleReflectionSettings;
            }

            return LocalPuddleReflectionSettings[location.NameOrUniqueName];
        }

        public SkySettings GetCurrentSkySettings(GameLocation location)
        {
            if (location is null || LocalSkyReflectionSettings is null || LocalSkyReflectionSettings.ContainsKey(location.NameOrUniqueName) is false || LocalSkyReflectionSettings[location.NameOrUniqueName] is null || LocalSkyReflectionSettings[location.NameOrUniqueName].OverrideDefaultSettings is false)
            {
                return SkyReflectionSettings;
            }

            return LocalSkyReflectionSettings[location.NameOrUniqueName];
        }
    }
}
