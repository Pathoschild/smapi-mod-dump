/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FlowerDanceFix
{
    public class ModEntry : Mod//, IAssetLoader
    {
        private static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            Config = Helper.ReadConfig<ModConfig>();

            // Initialize Patches
            EventPatched.Initialize(Monitor, Config, Helper);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_FDF))
            );

            Monitor.Log("Flower Dance Fix started using Harmony.", LogLevel.Trace);
        }
        public void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            /*
            //Set-up SpaceCore Methods
            var spacecore = Helper.ModRegistry.GetApi<SpaceCore.IApi>("spacechase0.SpaceCore");
            spacecore.AddEventCommand("ChangeFDF", AccessTools.Method(typeof(ModEntry), nameof(ModEntry.EventCommand_ChangeFDF)));
            */
            //Set-up Generic Mod Config Menu
            Config = Helper.ReadConfig<ModConfig>();
            var gmcm = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (gmcm is null)
            {
                return; // Missing Generic Mod Config Menu
            }
            else
            {
                gmcm.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
                gmcm.RegisterLabel(ModManifest, "Flower Dance Fix", "By elfuun");
                gmcm.RegisterParagraph(ModManifest, "Removes some of the hard-coded details of the flower dance festival main event, allowing for altered vanilla NPCs and custom NPCs to participate. Contains configurable options for dance pair generation. Only ''datable'' NPCs are considered valid for selection.");
                gmcm.RegisterSimpleOption(ModManifest, "Maximum Dance Pairs (WIP)", "(WIP) Changes number of pairs dancing ***WILL NOT WORK FOR MORE THAN 6 PAIRS***", () => Config.MaxDancePairs, (int val) => Config.MaxDancePairs = val);
                gmcm.RegisterSimpleOption(ModManifest, "NPCs Have Random Partners", "Pairs of NPCs dancing are arranged at random", () => Config.NPCsHaveRandomPartners, (bool val) => Config.NPCsHaveRandomPartners = val);
                gmcm.RegisterSimpleOption(ModManifest, "Allow Mixed-Gendered Dance Lines (WIP)", "(WIP) Pairs of NPCs dancing are random pairs of random genders- will require additional sprites", () => Config.ForceHeteroPartners, (bool val) => Config.ForceHeteroPartners = val);
                gmcm.RegisterSimpleOption(ModManifest, "Allow Non-Binary Dancers (WIP)", "(WIP) Allows NPCs that are not male or female (ie secret or unknown; gender is 2)- will require additional sprites.", () => Config.AllowNonBinaryPartners, (bool val) => Config.AllowNonBinaryPartners = val);
                gmcm.RegisterSimpleOption(ModManifest, "Allow Tourist Dancers", "Allows NPCs living outside the valley (ie they do not live in town; homeRegion is not 2).", () => Config.AllowTouristPartners, (bool val) => Config.AllowTouristPartners = val);
                gmcm.RegisterSimpleOption(ModManifest, "NPC Blacklist", "Configureable blacklist of NPCs preventing them from dancing- enclose NPC base name in quotes, deliniated by a forward slash, with no spaces.", () => Config.DancerBlackList, (string val) => Config.DancerBlackList = val);
                gmcm.RegisterSimpleOption(ModManifest, "Allow Crowd Animations", "Allows for custom crowd animations. May shuffle spectator NPCs around the main event layout.", () => Config.AllowCrowdAnimation, (bool val) => Config.AllowCrowdAnimation = val);
            }
        }
     /*   public void EventCommand_ChangeFDF(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            __instance.getActorByName(split[1]).Sprite.LoadTexture(FdfSprites[$"{split[1]}"]);
            Monitor.Log($"Changed {split[1]}'s sprites to FDF.");
            __instance.CurrentCommand++;
        }

        public IDictionary<string, string> FdfSprites = new Dictionary<string, string>();


        public bool CanLoad<T>(IAssetInfo asset)
        {
            return this.GetFdfSprite<T>(asset) != null;

        }
        public T Load<T>(IAssetInfo asset)
        {
            return (T)(object)this.GetFdfSprite<T>(asset);
        }

        public Texture2D GetFdfSprite<T>(IAssetInfo asset)
        {
            var segments = PathUtilities.GetSegments(asset.AssetName);

            bool isFdfSprite =
                typeof(T) == typeof(Texture2D)
                && segments.Length == 2
                && string.Equals(segments[0], "Characters", StringComparison.OrdinalIgnoreCase);

            if (!isFdfSprite)
                return null;

            FileInfo file = new FileInfo(Path.Combine(this.Helper.DirectoryPath, "assets", $"{segments[1]}_FDF.png"));

            string filePath = "";

            if (file.Exists)
            {
                filePath = $"assets/{segments[1]}_FDF.png";
                FdfSprites.Add($"{segments[1]}", $"{Helper.Content.GetActualAssetKey(filePath)}");
                Monitor.Log($"Added {segments[1]}'s FDF sprites to FDF sprite dictionary.", LogLevel.Trace);
            }

            return file.Exists ?
                this.Helper.Content.Load<Texture2D>($"{filePath}", ContentSource.ModFolder)
                : null;
        }
     */
    }
}


