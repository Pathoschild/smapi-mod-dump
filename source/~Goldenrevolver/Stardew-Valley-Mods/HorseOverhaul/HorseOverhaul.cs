/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Characters;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using static StableOverlayTextures;

    public enum SeasonalVersion
    {
        None,
        Sonr,
        Gwen,
        Magimatica,
        Lumi
    }

    // TODO: UI info suite 2 compatibility (issue #313)

    public class HorseOverhaul : Mod
    {
        //// the relative stable coordinates

        //// (tile.x, tile.y), (tile.x+1, tile.y), (tile.x+2, tile.y), (tile.x+3, tile.y)
        //// (tile.x, tile.y+1), (tile.x+1, tile.y+1), (tile.x+2, tile.y+1), (tile.x+3, tile.y+1)

        private readonly PerScreen<List<HorseWrapper>> horses = new(createNewState: () => new List<HorseWrapper>());

        private readonly PerScreen<bool> dayJustStarted = new(createNewState: () => false);

        internal readonly PerScreen<bool> isDoingHorseWarp = new(createNewState: () => false);

        public List<HorseWrapper> Horses { get => horses.Value; }

        public IBetterRanchingApi BetterRanchingApi { get; set; }

        public HorseOverhaulConfig Config { get; set; }

        private static IManifest Manifest { get; set; }

        internal string GwenOption { get; set; } = "1";

        internal SeasonalVersion SeasonalVersion { get; set; }

        internal bool UsingMyStableTextures { get; set; }
        internal bool UsingIncompatibleTextures { get; set; }

        private IRawTextureData repairTroughOverlay;

        internal IRawTextureData RepairTroughOverlay
        {
            get => repairTroughOverlay; set
            {
                repairTroughOverlay = value;
                filledTroughTexture = null;
                emptyTroughTexture = null;
            }
        }

        private IRawTextureData filledTroughOverlay;

        internal IRawTextureData FilledTroughOverlay
        {
            get => filledTroughOverlay; set
            {
                filledTroughOverlay = value;
                filledTroughTexture = null;
            }
        }

        private Texture2D filledTroughTexture;

        internal Texture2D FilledTroughTexture
        {
            get
            {
                if (filledTroughTexture == null)
                {
                    filledTroughTexture = RepairTroughOverlay == null ? GetCurrentStableTexture(this) : MergeTextures(RepairTroughOverlay, GetCurrentStableTexture(this));

                    if (FilledTroughOverlay != null)
                    {
                        filledTroughTexture = MergeTextures(FilledTroughOverlay, filledTroughTexture, SeasonalVersion == SeasonalVersion.Lumi);
                    }

                    filledTroughTexture.Name = ModManifest.UniqueID + ".FilledTrough";
                }

                return filledTroughTexture;
            }
        }

        private IRawTextureData emptyTroughOverlay;

        internal IRawTextureData EmptyTroughOverlay
        {
            get => emptyTroughOverlay; set
            {
                emptyTroughOverlay = value;
                emptyTroughTexture = null;
            }
        }

        private Texture2D emptyTroughTexture;

        internal Texture2D EmptyTroughTexture
        {
            get
            {
                if (emptyTroughTexture == null)
                {
                    emptyTroughTexture = RepairTroughOverlay == null ? GetCurrentStableTexture(this) : MergeTextures(RepairTroughOverlay, GetCurrentStableTexture(this));

                    if (EmptyTroughOverlay != null)
                    {
                        emptyTroughTexture = MergeTextures(EmptyTroughOverlay, emptyTroughTexture, SeasonalVersion == SeasonalVersion.Lumi);
                    }

                    emptyTroughTexture.Name = ModManifest.UniqueID + ".EmptyTrough";
                }

                return emptyTroughTexture;
            }
        }

        public Texture2D SaddleBagOverlay { get; set; }

        public bool IsUsingHorsemanship { get; set; } = false;

        private const int maximumSaddleBagPositionsChecked = 10;

        public static readonly string saddleBagBookNonQID = $"{Manifest?.UniqueID}.SaddleBagBook";
        public static readonly string saddleBagBookQID = $"(O){saddleBagBookNonQID}";

        public override void Entry(IModHelper helper)
        {
            Manifest = this.ModManifest;
            Config = Helper.ReadConfig<HorseOverhaulConfig>();

            HorseOverhaulConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate
            {
                CheckForKeybindConflict();
                HorseOverhaulConfig.SetUpModConfigMenu(Config, this);
                BetterRanchingApi = SetupBetterRanching();
            };

            SoundModule.SetupSounds(this);

            helper.Events.GameLoop.SaveLoaded += delegate { OnSaveLoaded(); };
            helper.Events.GameLoop.Saving += delegate { ResetHorses(); };
            helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
            helper.Events.GameLoop.UpdateTicked += delegate { LateDayStarted(); };

            helper.Events.Display.RenderedWorld += delegate { OnRenderedWorld(); };

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

            helper.Events.Content.AssetRequested += (_, e) => SaddleBagAccess.ApplySaddleBagUnlockChanges(e, this);
            helper.Events.Content.AssetReady += InvalidateStableTroughTexture;

            helper.Events.Input.ButtonPressed += (_, e) => ButtonHandling.OnButtonPressed(this, e);
            helper.Events.Input.ButtonsChanged += (_, _) => ButtonHandling.OnButtonsChanged(this);

            Patcher.PatchAll(this);
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        private void InvalidateStableTroughTexture(object sender, AssetReadyEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Buildings/Stable"))
            {
                filledTroughTexture = null;
                emptyTroughTexture = null;
            }
        }

        // this is legacy code for backwards compatibility, as we now patch GetSpriteWidthForPositioning
        private static void ResetHorses()
        {
            Utility.ForEachBuilding(delegate (Building building)
            {
                // also do it for tractors
                if (building is Stable stable)
                {
                    Horse horse = stable.getStableHorse();

                    if (horse != null)
                    {
                        horse.forceOneTileWide.Value = false;
                    }
                }

                return true;
            }, false);
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e == null || e.FromModID != ModManifest.UniqueID)
            {
                return;
            }

            var message = e.ReadAs<StateMessage>();

            if (message != null)
            {
                if (e.Type == StateMessage.SyncRequestType && Context.IsMainPlayer)
                {
                    foreach (var horse in Horses)
                    {
                        Helper.Multiplayer.SendMessage(new StateMessage(horse), StateMessage.SyncType, modIDs: new[] { ModManifest.UniqueID }, new[] { e.FromPlayerID });
                    }
                }
                else
                {
                    foreach (var horse in Horses)
                    {
                        if (horse?.Stable?.HorseId == message.HorseID)
                        {
                            if (e.Type == StateMessage.GotFoodType)
                            {
                                horse.GotFed = true;
                                horse.Friendship = message.Friendship;
                            }

                            if (e.Type == StateMessage.GotWaterType)
                            {
                                horse.GotWater = true;
                                horse.Friendship = message.Friendship;
                            }

                            if (e.Type == StateMessage.GotPettedType)
                            {
                                horse.WasPet = true;
                                horse.Friendship = message.Friendship;
                            }

                            if (e.Type == StateMessage.GotHeaterType)
                            {
                                horse.HasHeater = true;
                                horse.Friendship = message.Friendship;
                            }

                            if (e.Type == StateMessage.SyncType)
                            {
                                horse.WasPet = message.WasPet;
                                horse.GotFed = message.GotFed;
                                horse.GotWater = message.GotWater;
                                horse.HasHeater = message.HasHeater;
                                horse.Friendship = message.Friendship;

                                if (message.StableID.HasValue)
                                {
                                    horse.SaddleBag = GetSaddleBag(() => horse.Stable, message.StableID.Value);
                                }

                                horse?.Stable?.resetTexture();
                            }
                        }
                    }
                }
            }
        }

        private void CheckForKeybindConflict()
        {
            try
            {
                if (Helper.ModRegistry.IsLoaded("CJBok.CheatsMenu"))
                {
                    var data = Helper.ModRegistry.Get("CJBok.CheatsMenu");

                    var path = data.GetType().GetProperty("DirectoryPath");

                    if (path?.GetValue(data) != null)
                    {
                        var list = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "OpenMenuKey" }, data.Manifest.Name, false);

                        if (list["OpenMenuKey"].ToLower() == "p")
                        {
                            // whether the pet menu is set to the default
                            if (Config.PetMenuKey.IsBound && Config.PetMenuKey.Keybinds != null)
                            {
                                var conflictLessBinds = new List<Keybind>();
                                bool foundConflict = false;

                                foreach (var keybind in Config.PetMenuKey.Keybinds)
                                {
                                    if (keybind?.Buttons?.Length == 1 && keybind.Buttons[0] == SButton.P)
                                    {
                                        foundConflict = true;
                                    }
                                    else
                                    {
                                        conflictLessBinds.Add(keybind);
                                    }
                                }

                                if (foundConflict)
                                {
                                    DebugLog("Unassigned pet menu key because cjb cheats menu is bound to the same key.");
                                    Config.PetMenuKey = new KeybindList(conflictLessBinds.ToArray());
                                    this.Helper.WriteConfig(Config);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnSaveLoaded()
        {
            var horseIDs = new List<Guid>();

            Utility.ForEachBuilding(delegate (Building building)
            {
                if (building is Stable stable && !stable.IsTractorGarage())
                {
                    horseIDs.Add(stable.HorseId);
                }

                return true;
            }, false);

            if (horseIDs.Count != horseIDs.Distinct().Count())
            {
                ErrorLog("There appear to exist multiple stables with the same ID (which is supposed to be unique). Is this an old save? This will cause a lot of issues with this mod. I recommend to destroy and rebuild the stables to get new unique IDs.");
            }

            filledTroughOverlay = null;
            repairTroughOverlay = null;
            emptyTroughOverlay = null;

            SetOverlays(this);

            filledTroughTexture = null;
            emptyTroughTexture = null;
        }

        private IBetterRanchingApi SetupBetterRanching()
        {
            if (Helper.ModRegistry.IsLoaded("BetterRanching") && !Helper.ModRegistry.Get("BetterRanching").Manifest.Version.IsOlderThan("1.8.1"))
            {
                return Helper.ModRegistry.GetApi<IBetterRanchingApi>("BetterRanching");
            }

            return null;
        }

        internal Dictionary<string, string> ReadConfigFile(string path, string modFolderPath, string[] options, string modName, bool isNonString)
        {
            string fullPath = Path.Combine(modFolderPath, PathUtilities.NormalizePath(path));

            var result = new Dictionary<string, string>();

            try
            {
                string fullText = File.ReadAllText(fullPath).ToLower();
                var split = fullText.Split('\"');
                int offset = isNonString ? 1 : 2;

                for (int i = 0; i < split.Length; i++)
                {
                    foreach (var option in options)
                    {
                        if (option.ToLower() == split[i].Trim() && i + offset < split.Length)
                        {
                            string optionText = split[i + offset].Trim();

                            result.Add(option, optionText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog($"There was an exception while {ModManifest.Name} was reading the config for {modName}:", e);
            }

            return result;
        }

        private void LateDayStarted()
        {
            if (!Context.IsWorldReady || !dayJustStarted.Value)
            {
                return;
            }

            dayJustStarted.Value = false;

            // is local coop farmhand
            if (Context.IsOnHostComputer && !Context.IsMainPlayer)
            {
                return;
            }

            if (SeasonalVersion == SeasonalVersion.Sonr)
            {
                EmptyTroughOverlay = Helper.ModContent.Load<IRawTextureData>($"assets/sonr/overlay_empty_{Game1.currentSeason.ToLower()}.png");
            }
            else if (SeasonalVersion == SeasonalVersion.Gwen)
            {
                if (Game1.IsWinter && Game1.isSnowing && GwenOption == "1")
                {
                    EmptyTroughOverlay = Helper.ModContent.Load<IRawTextureData>($"assets/gwen/overlay_1_snow_peta.png");
                }
                else
                {
                    EmptyTroughOverlay = Helper.ModContent.Load<IRawTextureData>($"assets/gwen/overlay_{GwenOption}.png");
                }
            }
            else if (SeasonalVersion == SeasonalVersion.Lumi)
            {
                emptyTroughTexture = null;
            }

            // call this even if water or sprite changes are disabled to reset the texture
            // the overridden method makes sure to not change the sprite if the config disallows it
            Utility.ForEachBuilding(delegate (Building building)
            {
                if (building is Stable stable && !stable.IsTractorGarage())
                {
                    // empty the water troughs
                    if (Context.IsMainPlayer && stable?.modData?.ContainsKey($"{ModManifest.UniqueID}/gotWater") == true)
                    {
                        stable.modData.Remove($"{ModManifest.UniqueID}/gotWater");
                    }

                    stable.resetTexture();
                }

                return true;
            }, false);
        }

        private void OnDayStarted()
        {
            // for LateDayStarted, so we can change the textures after content patcher did
            dayJustStarted.Value = true;

            Horses.Clear();

            // can't use ForEachBuilding, because we need the location
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                foreach (var building in location.buildings)
                {
                    if (building is not Stable stable || stable.IsTractorGarage())
                    {
                        continue;
                    }

                    Chest saddleBag = null;
                    int? stableID = null;

                    // non host players get the saddle bag from the sync request
                    if (Context.IsMainPlayer)
                    {
                        stable.modData.TryGetValue($"{ModManifest.UniqueID}/stableID", out string modData);

                        if (!string.IsNullOrEmpty(modData) && int.TryParse(modData, out int parsedID))
                        {
                            stableID = parsedID;
                            saddleBag = GetSaddleBag(() => stable, stableID.Value);
                        }

                        if (Config.SaddleBag && saddleBag == null)
                        {
                            stableID = null;

                            if (TryCreateNewSaddleBag(stable, out var newSaddleBag, out int newStableID))
                            {
                                saddleBag = newSaddleBag;
                                stableID = newStableID;
                            }
                        }
                    }

                    Horses.Add(new HorseWrapper(stable, this, saddleBag, stableID));

                    if (Context.IsMainPlayer && Config.HorseHeater && Game1.IsWinter)
                    {
                        if (CheckForHeater(location, stable))
                        {
                            var horseW = Horses.Where(h => h?.Stable?.HorseId == stable.HorseId).FirstOrDefault();

                            horseW?.AddHeaterBonus();
                        }
                    }
                }

                return true;
            }, false);

            if (Context.IsMainPlayer)
            {
                Utility.ForEachCharacter(delegate (NPC npc)
                {
                    if (npc is not Pet pet)
                    {
                        return true;
                    }

                    if (pet.modData?.ContainsKey($"{ModManifest.UniqueID}/gotFed") == true)
                    {
                        pet.modData.Remove($"{ModManifest.UniqueID}/gotFed");
                    }

                    return true;
                });
            }
            else
            {
                Helper.Multiplayer.SendMessage(new StateMessage(), "syncRequest", modIDs: new[] { ModManifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            }
        }

        public bool CheckForHeater(GameLocation location, Stable stable)
        {
            // this stable skin includes a heater, so we give the player the bonus for free
            if (SeasonalVersion == SeasonalVersion.Magimatica)
            {
                return true;
            }

            foreach (var item in location.Objects.Values)
            {
                if (item == null || !item.Name.Equals("Heater"))
                {
                    continue;
                }

                // check if the heater is on any tile of the stable or one tile around it
                if (stable.tileX.Value - 1 <= item.TileLocation.X && item.TileLocation.X <= stable.tileX.Value + 4)
                {
                    if (stable.tileY.Value - 1 <= item.TileLocation.Y && item.TileLocation.Y <= stable.tileY.Value + 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Chest GetSaddleBag(Func<Stable> stable, int stableID)
        {
            Game1.getFarm().Objects.TryGetValue(new Vector2(stableID, 0), out StardewValley.Object value);

            if (value != null && value is Chest chest)
            {
                chest.modData[$"{ModManifest.UniqueID}/isSaddleBag"] = "true";

                if (Config.SaddleBag)
                {
                    return chest;
                }
                else if (Context.IsMainPlayer)
                {
                    if (stable.Invoke().modData.ContainsKey($"{ModManifest.UniqueID}/stableID"))
                    {
                        stable.Invoke().modData.Remove($"{ModManifest.UniqueID}/stableID");
                    }

                    if (chest.Items.Count > 0)
                    {
                        foreach (var item in chest.Items)
                        {
                            Game1.player.team.returnedDonations.Add(item);
                            Game1.player.team.newLostAndFoundItems.Value = true;
                        }

                        chest.Items.Clear();
                    }

                    Game1.getFarm().Objects.Remove(new Vector2(stableID, 0));
                }
            }
            else if (Context.IsMainPlayer)
            {
                if (Config.SaddleBag)
                {
                    ErrorLog("Stable says there is a saddle bag chest, but I couldn't find it!");
                }
                else if (stable.Invoke().modData.ContainsKey($"{ModManifest.UniqueID}/stableID"))
                {
                    stable.Invoke().modData.Remove($"{ModManifest.UniqueID}/stableID");
                }
            }

            return null;
        }

        private bool TryCreateNewSaddleBag(Stable stable, out Chest saddleBag, out int stableID)
        {
            saddleBag = null;
            stableID = -1;

            if (!Context.IsMainPlayer)
            {
                return false;
            }

            // find position for the new chest
            stableID = -1;

            for (int i = 0; i < maximumSaddleBagPositionsChecked; i++)
            {
                if (!Game1.getFarm().Objects.ContainsKey(new Vector2(i, 0)))
                {
                    stableID = i;
                    break;
                }
            }

            if (stableID == -1)
            {
                ErrorLog("Couldn't find a spot to place the saddle bag chest");
                return false;
            }

            saddleBag = new Chest(true, new Vector2(stableID, 0));
            stable.modData[$"{ModManifest.UniqueID}/stableID"] = stableID.ToString();
            saddleBag.modData[$"{ModManifest.UniqueID}/isSaddleBag"] = "true";
            Game1.getFarm().Objects.Add(new Vector2(stableID, 0), saddleBag);

            return true;
        }

        private void OnRenderedWorld()
        {
            if (Config.Petting && BetterRanchingApi != null && !Game1.eventUp)
            {
                float yOffset = Game1.tileSize / 3;
                int mult = Config.ThinHorse ? -1 : 1;

                foreach (HorseWrapper horseWrapper in Horses.Where(h => h?.Horse?.currentLocation == Game1.currentLocation))
                {
                    BetterRanchingApi.DrawHeartBubble(Game1.spriteBatch, horseWrapper.Horse.Position.X, horseWrapper.Horse.Position.Y - yOffset, mult * horseWrapper.Horse.Sprite.getWidth(), () => !horseWrapper.WasPet);
                }
            }
        }
    }
}