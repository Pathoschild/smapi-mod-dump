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
    using HarmonyLib;
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

    public enum SeasonalVersion
    {
        None,
        Sonr,
        Gwen,
        Magimatica
    }

    public static class ExtensionMethods
    {
        public static bool IsTractor(this Horse horse)
        {
            return horse?.modData.ContainsKey("Pathoschild.TractorMod") == true || horse?.Name.StartsWith("tractor/") == true;
        }

        public static bool IsGarage(this Stable stable)
        {
            return stable != null && (stable.buildingType.Value == "TractorGarage" || stable.maxOccupants.Value == -794739);
        }
    }

    public class HorseOverhaul : Mod
    {
        //// the relative stable coordinates

        //// (tile.x, tile.y), (tile.x+1, tile.y), (tile.x+2, tile.y), (tile.x+3, tile.y)
        //// (tile.x, tile.y+1), (tile.x+1, tile.y+1), (tile.x+2, tile.y+1), (tile.x+3, tile.y+1)

        private readonly List<SButton> mouseButtons = new() { SButton.MouseLeft, SButton.MouseRight, SButton.MouseMiddle, SButton.MouseX1, SButton.MouseX2 };

        private readonly PerScreen<List<HorseWrapper>> horses = new(createNewState: () => new List<HorseWrapper>());

        private readonly PerScreen<bool> dayJustStarted = new(createNewState: () => false);

        private string gwenOption = "1";

        private bool usingMyTextures = false;

        private SeasonalVersion seasonalVersion = SeasonalVersion.None;

        public List<HorseWrapper> Horses { get => horses.Value; }

        public IBetterRanchingApi BetterRanchingApi { get; set; }

        public HorseConfig Config { get; set; }

        public Texture2D CurrentStableTexture => usingMyTextures ? Helper.Content.Load<Texture2D>("assets/stable.png") : Helper.Content.Load<Texture2D>("Buildings/Stable", ContentSource.GameContent);

        public Texture2D FilledTroughTexture => FilledTroughOverlay == null ? CurrentStableTexture : MergeTextures(FilledTroughOverlay, CurrentStableTexture);

        public Texture2D EmptyTroughTexture => EmptyTroughOverlay == null ? CurrentStableTexture : MergeTextures(EmptyTroughOverlay, CurrentStableTexture);

        public Texture2D SaddleBagOverlay { get; set; }

        public bool IsUsingHorsemanship { get; set; } = false;

        private Texture2D FilledTroughOverlay { get; set; }

        private Texture2D EmptyTroughOverlay { get; set; }

        //// TODO horse race festival

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<HorseConfig>();

            HorseConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate
            {
                CheckForKeybindConflict();
                HorseConfig.SetUpModConfigMenu(Config, this);
                BetterRanchingApi = SetupBetterRanching();
            };

            SoundModule.SetupSounds(this);

            Helper.Events.GameLoop.SaveLoaded += delegate { SetOverlays(); };

            Helper.Events.GameLoop.Saving += delegate { ResetHorses(); };
            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
            helper.Events.GameLoop.UpdateTicked += delegate { LateDayStarted(); };
            helper.Events.Display.RenderedWorld += OnRenderedWorld;

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

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

        private static Texture2D MergeTextures(Texture2D overlay, Texture2D oldTexture)
        {
            if (overlay == null || oldTexture == null)
            {
                return oldTexture;
            }

            int count = overlay.Width * overlay.Height;
            var newData = new Color[count];
            overlay.GetData(newData);
            var origData = new Color[count];
            oldTexture.GetData(origData);

            if (newData == null || origData == null)
            {
                return oldTexture;
            }

            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = newData[i].A != 0 ? newData[i] : origData[i];
            }

            oldTexture.SetData(newData);
            return oldTexture;
        }

        // this is legacy code for backwards compatibility, as we now patch GetSpriteWidthForPositioning
        private static void ResetHorses()
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                // also do it for tractors
                if (building is Stable stable && stable.getStableHorse() != null)
                {
                    stable.getStableHorse().forceOneTileWide.Value = false;
                }
            }
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

        private void SetOverlays()
        {
            var horseIDs = new List<Guid>();

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Stable stable && !stable.IsGarage())
                {
                    horseIDs.Add(stable.HorseId);
                }
            }

            if (horseIDs.Count != horseIDs.Distinct().Count())
            {
                ErrorLog("There appear to exist multiple stables with the same ID (which is supposed to be unique). Is this an old save? This will cause a lot of issues with this mod. I recommend to destroy and rebuild the stables to get new unique IDs.");
            }

            if (Config.SaddleBag && Config.VisibleSaddleBags != SaddleBagOption.Disabled.ToString())
            {
                SaddleBagOverlay = Helper.Content.Load<Texture2D>($"assets/saddlebags_{Config.VisibleSaddleBags.ToLower()}.png");
                IsUsingHorsemanship = Helper.ModRegistry.IsLoaded("red.horsemanship");
            }

            if (!Config.Water || Config.DisableStableSpriteChanges)
            {
                return;
            }

            seasonalVersion = SeasonalVersion.None;

            usingMyTextures = false;

            FilledTroughOverlay = null;

            if (Helper.ModRegistry.IsLoaded("sonreirblah.JBuildings"))
            {
                // seasonal overlays are assigned in LateDayStarted
                EmptyTroughOverlay = null;

                seasonalVersion = SeasonalVersion.Sonr;
                return;
            }

            if (Helper.ModRegistry.IsLoaded("Oklinq.CleanStable"))
            {
                EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/overlay_empty.png", ContentSource.ModFolder);

                return;
            }

            if (Helper.ModRegistry.IsLoaded("Elle.SeasonalBuildings"))
            {
                var data = Helper.ModRegistry.Get("Elle.SeasonalBuildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var list = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "color palette", "stable" }, data.Manifest.Name, false);

                    if (list["stable"].ToLower() != "false")
                    {
                        EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/elle/overlay_empty_{list["color palette"]}.png", ContentSource.ModFolder);

                        return;
                    }
                }
            }

            if (Helper.ModRegistry.IsLoaded("Elle.SeasonalVanillaBuildings"))
            {
                var data = Helper.ModRegistry.Get("Elle.SeasonalVanillaBuildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var list = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "stable" }, data.Manifest.Name, false);

                    if (list["stable"].ToLower() == "true")
                    {
                        FilledTroughOverlay = Helper.Content.Load<Texture2D>($"assets/overlay_filled_tone.png", ContentSource.ModFolder);
                        EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/overlay_empty_tone.png", ContentSource.ModFolder);

                        return;
                    }
                }
            }

            if (Helper.ModRegistry.IsLoaded("Gweniaczek.Medieval_stables"))
            {
                IModInfo data = Helper.ModRegistry.Get("Gweniaczek.Medieval_stables");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "stableOption" }, data.Manifest.Name, false);

                    SetupGwenTextures(dict);

                    return;
                }
            }

            if (Helper.ModRegistry.IsLoaded("Gweniaczek.Medieval_buildings"))
            {
                var data = Helper.ModRegistry.Get("Gweniaczek.Medieval_buildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "buildingsReplaced", "stableOption" }, data.Manifest.Name, false);

                    if (dict["buildingsReplaced"].Contains("stable"))
                    {
                        SetupGwenTextures(dict);

                        return;
                    }
                }
            }

            if (Helper.ModRegistry.IsLoaded("magimatica.SeasonalVanillaBuildings") || Helper.ModRegistry.IsLoaded("red.HudsonValleyBuildings"))
            {
                EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/overlay_empty_no_bucket.png", ContentSource.ModFolder);

                seasonalVersion = SeasonalVersion.Magimatica;

                return;
            }

            // no compatible texture mod found so we will use mine
            usingMyTextures = true;

            EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/overlay_empty.png", ContentSource.ModFolder);
        }

        private void SetupGwenTextures(Dictionary<string, string> dict)
        {
            if (dict["stableOption"] == "4")
            {
                FilledTroughOverlay = Helper.Content.Load<Texture2D>($"assets/gwen/overlay_{dict["stableOption"]}_full.png", ContentSource.ModFolder);
            }

            EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/gwen/overlay_{dict["stableOption"]}.png", ContentSource.ModFolder);

            seasonalVersion = SeasonalVersion.Gwen;
            gwenOption = dict["stableOption"];
        }

        private IBetterRanchingApi SetupBetterRanching()
        {
            if (Helper.ModRegistry.IsLoaded("BetterRanching") && !Helper.ModRegistry.Get("BetterRanching").Manifest.Version.IsOlderThan("1.8.1"))
            {
                return Helper.ModRegistry.GetApi<IBetterRanchingApi>("BetterRanching");
            }

            return null;
        }

        private Dictionary<string, string> ReadConfigFile(string path, string modFolderPath, string[] options, string modName, bool isNonString)
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

            if (seasonalVersion == SeasonalVersion.Sonr)
            {
                EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/sonr/overlay_empty_{Game1.currentSeason.ToLower()}.png", ContentSource.ModFolder);
            }
            else if (seasonalVersion == SeasonalVersion.Gwen)
            {
                if (Game1.IsWinter && Game1.isSnowing)
                {
                    EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/gwen/overlay_1_snow_peta.png", ContentSource.ModFolder);
                }
                else
                {
                    EmptyTroughOverlay = Helper.Content.Load<Texture2D>($"assets/gwen/overlay_{gwenOption}.png", ContentSource.ModFolder);
                }
            }

            // call this even if water or sprite changes are disabled to reset the texture
            // the overridden method makes sure to not change the sprite if the config disallows it
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Stable stable && !stable.IsGarage())
                {
                    // empty the water troughs
                    if (Context.IsMainPlayer && stable?.modData?.ContainsKey($"{ModManifest.UniqueID}/gotWater") == true)
                    {
                        stable.modData.Remove($"{ModManifest.UniqueID}/gotWater");
                    }

                    stable.resetTexture();
                }
            }
        }

        private void OnDayStarted()
        {
            // for LateDayStarted, so we can change the textures after content patcher did
            dayJustStarted.Value = true;

            Horses.Clear();

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Stable stable)
                {
                    if (stable.IsGarage())
                    {
                        continue;
                    }

                    Chest saddleBag = null;
                    int? stableID = null;

                    // non host players get the saddle bag from the sync request
                    if (Context.IsMainPlayer)
                    {
                        stable.modData.TryGetValue($"{ModManifest.UniqueID}/stableID", out string modData);

                        if (!string.IsNullOrEmpty(modData))
                        {
                            stableID = int.Parse(modData);

                            saddleBag = GetSaddleBag(() => stable, stableID.Value);
                        }

                        if (Config.SaddleBag && saddleBag == null)
                        {
                            stableID = CreateNewSaddleBag(ref stable, ref saddleBag);
                        }
                    }

                    Horses.Add(new HorseWrapper(stable, this, saddleBag, stableID));

                    if (Context.IsMainPlayer && Config.HorseHeater)
                    {
                        CheckForHeater(stable);
                    }
                }
            }

            if (Context.IsMainPlayer)
            {
                if (Game1.player.hasPet())
                {
                    Pet pet = Game1.player.getPet();

                    if (pet?.modData?.ContainsKey($"{ModManifest.UniqueID}/gotFed") == true)
                    {
                        pet.modData.Remove($"{ModManifest.UniqueID}/gotFed");
                    }
                }
            }
            else
            {
                Helper.Multiplayer.SendMessage(new StateMessage(), "syncRequest", modIDs: new[] { ModManifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            }
        }

        private void CheckForHeater(Stable stable)
        {
            var horse = stable.getStableHorse();

            if (horse != null && Game1.IsWinter)
            {
                // this stable skin includes a heater, so we give the player the bonus for free
                if (seasonalVersion == SeasonalVersion.Magimatica)
                {
                    HorseWrapper horseW = null;
                    Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).Do(h => horseW = h);

                    if (horseW != null)
                    {
                        horseW.AddHeaterBonus();
                    }

                    return;
                }

                var farmObjects = Game1.getFarm().Objects.Values;

                foreach (var item in farmObjects)
                {
                    if (item != null && item.Name.Equals("Heater"))
                    {
                        // check if the heater is on any tile of the stable or one tile around it
                        if (stable.tileX.Value - 1 <= item.TileLocation.X && item.TileLocation.X <= stable.tileX.Value + 4)
                        {
                            if (stable.tileY.Value - 1 <= item.TileLocation.Y && item.TileLocation.Y <= stable.tileY.Value + 2)
                            {
                                HorseWrapper horseW = null;
                                Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).Do(h => horseW = h);

                                if (horseW != null)
                                {
                                    horseW.AddHeaterBonus();
                                }

                                return;
                            }
                        }
                    }
                }
            }
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

                    if (chest.items.Count > 0)
                    {
                        foreach (var item in chest.items)
                        {
                            Game1.player.team.returnedDonations.Add(item);
                            Game1.player.team.newLostAndFoundItems.Value = true;
                        }

                        chest.items.Clear();
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

        private int? CreateNewSaddleBag(ref Stable stable, ref Chest saddleBag)
        {
            if (!Context.IsMainPlayer)
            {
                return null;
            }

            // find position for the new chest
            int stableID = -1;

            for (int i = 0; i < 10; i++)
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
                return null;
            }

            saddleBag = new Chest(true, new Vector2(stableID, 0));
            stable.modData[$"{ModManifest.UniqueID}/stableID"] = stableID.ToString();
            saddleBag.modData[$"{ModManifest.UniqueID}/isSaddleBag"] = "true";
            Game1.getFarm().Objects.Add(new Vector2(stableID, 0), saddleBag);

            return stableID;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || Config.DisableMainSaddleBagAndFeedKey)
            {
                return;
            }

            if (e.Button.IsUseToolButton())
            {
                ////bool wasController = e.Button.TryGetController(out _);
                bool ignoreMousePosition = !mouseButtons.Contains(e.Button);
                Point cursorPosition = Game1.getMousePosition();

                bool interacted = Feeding.CheckHorseInteraction(this, Game1.currentLocation, cursorPosition.X + Game1.viewport.X, cursorPosition.Y + Game1.viewport.Y, ignoreMousePosition);

                if (!interacted)
                {
                    Feeding.CheckPetInteraction(this, cursorPosition.X + Game1.viewport.X, cursorPosition.Y + Game1.viewport.Y, ignoreMousePosition);
                }
            }
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }

            // this is done in buttonsChanged instead of buttonPressed as recommended
            // in the documentation: https://stardewcommunitywiki.com/Modding:Modder_Guide/APIs/Input#KeybindList
            if (Config.HorseMenuKey.JustPressed())
            {
                OpenHorseMenu();
                return;
            }

            if (Config.PetMenuKey.JustPressed())
            {
                OpenPetMenu();
                return;
            }

            if (Config.AlternateSaddleBagAndFeedKey.JustPressed())
            {
                bool interacted = Feeding.CheckHorseInteraction(this, Game1.currentLocation, 0, 0, true);

                if (!interacted)
                {
                    Feeding.CheckPetInteraction(this, 0, 0, true);
                }
            }
        }

        private void OpenHorseMenu()
        {
            HorseWrapper horse = null;

            Horses.Where(h => h?.Horse?.getOwner() == Game1.player && h?.Horse?.getName() == Game1.player.horseName.Value).Do(h => horse = h);

            if (horse != null)
            {
                Game1.activeClickableMenu = new HorseMenu(this, horse);
            }
        }

        private void OpenPetMenu()
        {
            if (Game1.player.hasPet())
            {
                Pet pet = Game1.player.getPet();

                if (pet != null)
                {
                    Game1.activeClickableMenu = new PetMenu(this, pet);
                }
            }
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
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