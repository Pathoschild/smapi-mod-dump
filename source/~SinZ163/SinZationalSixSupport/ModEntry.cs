/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using SinZationalSixSupport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace SinZational_Six_Support
{
    public class ModEntry : Mod
    {
        private static bool IsFirstInstantiation = true;
        private static ModEntry OGInstance;
        private static bool HasBootstrapped = false;

        private static readonly Dictionary<string, BackwardsCompatManifest> Mods = new()
        {
            ["SuperAardvark.AntiSocial"] = new BackwardsCompatManifest("SuperAardvark.AntiSocial", "Anti Social NPC's", "Anti Social NPC's", "SinZSixSupport", new SemanticVersion("9.9.9"))
        };

        public ModEntry()
        {
            // Due to SMAPI checking that there is exactly one Mod class in the assembly
            // this ModEntry class is also the ModEntry class for every mod we are impersonating
            // This lets us find out that we are ourselves
            if (IsFirstInstantiation)
            {
                OGInstance = this;
                // Cannot use ModManifest yet as we haven't received it yet
                var harmony = new Harmony("SinZational.SixSupport.Bootstrap");
                harmony.Patch(
                    AccessTools.Method(Type.GetType("StardewModdingAPI.Framework.ModRegistry,StardewModdingAPI"),"Add"),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.ModRegistryAddPostfix))
                );
                IsFirstInstantiation = false;
            }
        }

        public static void ModRegistryAddPostfix(object __instance, object metadata)
        {
            if (HasBootstrapped) return;

            var manifestProp = metadata.GetType().GetProperty("Manifest");
            var OGManifest = manifestProp.GetValue(metadata) as IManifest;
            if (OGManifest.UniqueID == "SinZ.SixSupport")
            {
                HasBootstrapped = true;

                var modRegistryAddMethod = __instance.GetType().GetMethod("Add");
                var TModMetadata = Type.GetType("StardewModdingAPI.Framework.ModLoading.ModMetadata,StardewModdingAPI");
                var modMetadataAddModMethod = TModMetadata.GetMethod("SetMod", new Type[] { typeof(IMod), Type.GetType("StardewModdingAPI.Framework.ModHelpers.TranslationHelper,StardewModdingAPI") });

                var setModManifest = typeof(ModEntry).GetProperty(nameof(ModEntry.ModManifest));
                var setHelper = typeof(ModEntry).GetProperty(nameof(ModEntry.Helper));
                var setMonitor = typeof(ModEntry).GetProperty(nameof(ModEntry.Monitor));

                foreach (var manifest in Mods.Values)
                {
                    var modMetadata = Activator.CreateInstance(TModMetadata, new object[]
                    {
                        manifest.Name,
                        metadata.GetType().GetProperty("DirectoryPath").GetValue(metadata),
                        metadata.GetType().GetProperty("RootPath").GetValue(metadata),
                        manifest,
                        null, //dataRecords (info from metadata.json)
                        false //isIgnored
                    });
                    var modInstance = new ModEntry();
                    setModManifest.SetValue(modInstance, manifest);
                    setHelper.SetValue(modInstance, OGInstance.Helper);
                    // TODO: Make my own Monitor
                    setMonitor.SetValue(modInstance, OGInstance.Monitor);
                    modMetadataAddModMethod.Invoke(modMetadata, new object[] { modInstance, OGInstance.Helper.Translation });
                    modRegistryAddMethod.Invoke(__instance, new object[] { modMetadata });
                }
            }
        }

        public override void Entry(IModHelper helper)
        {
        }
    }
}
