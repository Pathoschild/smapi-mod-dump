/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;

namespace BuildableGreenhouse.Compatibility
{
    partial class ModCompatibility
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static IManifest Manifest;
        private static ModConfig Config;

        public static Dictionary<String, Building> Greenhouses;

        public static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
        {
            Monitor = monitor;
            Helper = helper;
            Manifest = manifest;
            Config = helper.ReadConfig<ModConfig>();

            Greenhouses = new Dictionary<String, Building>();
        }
    }
}