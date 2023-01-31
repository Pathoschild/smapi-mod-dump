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
using System.Collections.Generic;
using BirbShared;
using BirbShared.APIs;
using BirbShared.Command;
using BirbShared.Config;
using HarmonyLib;
using StardewModdingAPI;

namespace GameboyArcade
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal static Config Config;

        internal static Dictionary<string, Content> LoadedContentPacks = new Dictionary<string, Content>();
        internal static Dictionary<string, string> BigCraftableIDMap = new Dictionary<string, string>();

        internal static IDynamicGameAssetsApi DynamicGameAssets;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);
            Config = helper.ReadConfig<Config>();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            this.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived_SaveRequest;
            this.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived_LoadRequest;

        }

        public override object GetApi()
        {
            return new GameboyArcadeAPIImpl();
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            new Harmony(this.ModManifest.UniqueID).PatchAll();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();

            if (this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                DynamicGameAssets = this.Helper.ModRegistry
                    .GetApi<IDynamicGameAssetsApi>
                    ("spacechase0.DynamicGameAssets");
                if (DynamicGameAssets is null)
                {
                    Log.Error("Can't access the Dynamic Game Assets API. Is the mod installed correctly?");
                }
            }

            this.LoadContentPacks();
        }

        /// <summary>
        /// Allow remote players to load ROM saves which are marked as shared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Multiplayer_ModMessageReceived_LoadRequest(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "LoadRequest")
            {
                string minigameId = e.ReadAs<string>();
                SaveState loaded = this.Helper.Data.ReadJsonFile<SaveState>($"data/{minigameId}/{Constants.SaveFolderName}/file.json");
                this.Helper.Multiplayer.SendMessage<SaveState>(loaded, "LoadReceive", new string[] { this.ModManifest.UniqueID }, new long[] { e.FromPlayerID });
            }
        }

        /// <summary>
        /// Allow remote players to save ROM saves which are marked as shared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Multiplayer_ModMessageReceived_SaveRequest(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type.StartsWith("SaveRequest "))
            {
                string minigameId = e.Type.Substring(12);
                if (!LoadedContentPacks.ContainsKey(minigameId))
                {
                    Log.Error($"{e.FromPlayerID} sent save request for {minigameId}, but no such minigame exists for host computer!");
                    return;
                }
                SaveState save = e.ReadAs<SaveState>();
                this.Helper.Data.WriteJsonFile<SaveState>($"data/{minigameId}/{Constants.SaveFolderName}/file.json", save);
            }
        }

        private void LoadContentPacks()
        {
            foreach (IContentPack pack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    List<Content> contents = pack.ReadJsonFile<List<Content>>("content.json");
                    if (contents is null || contents.Count == 0)
                    {
                        Log.Error($"{pack.Manifest.UniqueID}: content.json was missing!");
                        continue;
                    }

                    foreach (Content content in contents)
                    {
                        if (content is null || content.Name is null || content.Name == "")
                        {
                            Log.Error($"{pack.Manifest.UniqueID}: Content entry was missing name");
                            continue;
                        }
                        if (!pack.HasFile(content.FilePath))
                        {
                            Log.Error($"{pack.Manifest.UniqueID}: {content.Name} rom file was missing {content.FilePath}");
                            continue;
                        }
                        content.ContentPack = pack;

                        content.UniqueID = $"{pack.Manifest.UniqueID}.{content.ID}";

                        LoadedContentPacks.Add(content.UniqueID, content);

                        if (content.DGAID is not null && DynamicGameAssets is not null)
                        {
                            if (DynamicGameAssets.SpawnDGAItem(content.DGAID) is not null)
                            {
                                BigCraftableIDMap.Add(content.DGAID, content.UniqueID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{pack.Manifest.UniqueID}: Failed to parse content.json\n{e}");
                }
            }
        }
    }
}
