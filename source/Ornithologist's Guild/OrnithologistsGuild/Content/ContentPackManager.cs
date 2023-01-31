/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;

namespace OrnithologistsGuild.Content
{
    public class ContentPackManager
    {
        private const string FILENAME = "content.json";

        public static List<ContentPackDef> ContentPackDefs = new List<ContentPackDef>();

        public static Dictionary<string, BirdieDef> BirdieDefs = new Dictionary<string, BirdieDef>();

        public static void Initialize()
        {
            ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        public static void LoadExternal()
        {
            foreach (IContentPack contentPack in ModEntry.Instance.Helper.ContentPacks.GetOwned())
            {
                Load(contentPack);
            }
        }

        public static void Load(IContentPack contentPack)
        {
            ModEntry.Instance.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Info);

            if (!contentPack.HasFile(FILENAME))
            {
                ModEntry.Instance.Monitor.Log(@$"Error in content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}: a ""{FILENAME}"" file is required", LogLevel.Error);
                return;
            }

            ContentPackDef contentPackDef;
            try
            {
                contentPackDef = contentPack.ReadJsonFile<ContentPackDef>(FILENAME);
                if (contentPackDef.FormatVersion != ContentPackDef.FORMAT_VERSION)
                {
                    throw new Exception($"FormatVersion must be {ContentPackDef.FORMAT_VERSION} (got {contentPackDef.FormatVersion})");
                }

                contentPackDef.ContentPack = contentPack;
                ContentPackDefs.Add(contentPackDef);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log(@$"Error in content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}: {e.ToString()}", LogLevel.Error);
                return;
            }

            foreach (var birdieDef in contentPackDef.Birdies)
            {
                try
                {
                    birdieDef.UniqueID = $"{contentPack.Manifest.UniqueID}.birdie.{birdieDef.ID}";
                    birdieDef.ContentPackDef = contentPackDef;

                    birdieDef.LoadSoundAssets();

                    BirdieDefs.Add(birdieDef.UniqueID, birdieDef);
                }
                catch (Exception e)
                {
                    ModEntry.Instance.Monitor.Log(@$"Error in content pack: {contentPackDef.ContentPack.Manifest.Name} {contentPackDef.ContentPack.Manifest.Version} (in {birdieDef.ID}): {e.ToString()}", LogLevel.Error);
                }
            }

            ModEntry.Instance.Monitor.Log($"Read {contentPackDef.Birdies.Length} birdies from {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Info);
        }

        public static void LoadBuiltIn()
        {
            IContentPack contentPack = ModEntry.Instance.Helper.ContentPacks.CreateTemporary(
               directoryPath: Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "assets", "content-pack"),
               id: "BuiltIn",
               name: "Ornithologist's Guild birds",
               description: "A variety of North American birds created specifically for Ornithologist's Guild.",
               author: "Ivy",
               version: ModEntry.Instance.ModManifest.Version
            );

            Load(contentPack);
        }

        public static void LoadVanilla()
        {
            IContentPack contentPack = ModEntry.Instance.Helper.ContentPacks.CreateTemporary(
               directoryPath: Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "assets", "content-pack-vanilla"),
               id: "Vanilla",
               name: "Stardew Valley birds",
               description: "Built-in Stardew Valley birds.",
               author: "ConcernedApe",
               version: ModEntry.Instance.ModManifest.Version
            );

            Load(contentPack);
        }

        private static void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs _)
        {
            // Parse conditions on second tick
            if (ModEntry.CP.IsConditionsApiReady)
            {
                ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;

                // Parse all conditions
                foreach (var contentPackDef in ContentPackDefs)
                {
                    foreach (var birdieDef in contentPackDef.Birdies)
                    {
                        try
                        {
                            birdieDef.ParseConditions();
                        } catch (Exception e)
                        {
                            ModEntry.Instance.Monitor.Log(@$"Error in content pack: {contentPackDef.ContentPack.Manifest.Name} {contentPackDef.ContentPack.Manifest.Version} (in {birdieDef.ID}): {e.ToString()}", LogLevel.Error);
                        }
                    }
                }
            }
        }
    }
}
