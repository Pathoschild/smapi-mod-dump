/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CyanFireUK/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using GenericModConfigMenu;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;




namespace ChangeHorseSounds
{

    public sealed class ChangeHorseSoundsModConfig
    {
     public bool ReplaceSounds { get; set; } = true;
     public bool PlayOnce { get; set; } = false;

    }
    public class ModEntry : Mod
    {
        public static IModHelper SHelper { get; private set; }
        private static ChangeHorseSoundsModConfig config;


        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Content.AssetRequested += Content_AssetRequested;

            SHelper = helper;

            config = SHelper.ReadConfig<ChangeHorseSoundsModConfig>();


            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                              original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSound)),
                              prefix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.localSound_prefix))
                                );

            harmony.Patch(
                              original: AccessTools.Method(typeof(FarmerSprite), "checkForFootstep"),
                              postfix: new HarmonyMethod(typeof(FarmerSpritePatches), nameof(FarmerSpritePatches.checkForFootstep_postfix))
                                );
        }


        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {

                var configMenu = SHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                configMenu.Register(
                   mod: ModManifest,
                   reset: () => config = new ChangeHorseSoundsModConfig(),
                   save: () => SHelper.WriteConfig(config)
                   );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Replace Horse Footstep Sounds",
                    tooltip: () => "Select whether to replace horse footstep sounds or not",
                    getValue: () => config.ReplaceSounds,
                    setValue: value => config.ReplaceSounds = value
                    );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Play Sounds Once",
                    tooltip: () => "Plays the sound once and then loops when it reaches the end of the sound file",
                    getValue: () => config.PlayOnce,
                    setValue: value => config.PlayOnce = value
                    );

        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (config.PlayOnce == true)
            {
               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customStone").instanceLimit = 1;
               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customStone").limitBehavior = CueDefinition.LimitBehavior.FailToPlay;

               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customWoody").instanceLimit = 1;
               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customWoody").limitBehavior = CueDefinition.LimitBehavior.FailToPlay;

               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customThud").instanceLimit = 1;
               Game1.soundBank.GetCueDefinition(ModManifest.UniqueID + "_customThud").limitBehavior = CueDefinition.LimitBehavior.FailToPlay;
            }

        }


        public void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/AudioChanges"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, AudioCueData> data = editor.AsDictionary<string, AudioCueData>().Data;

                    if (config.ReplaceSounds == true)
                    {
                        if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", "horsecustomstone.wav")))
                        {
                            AddCueData(data, ModManifest.UniqueID + "_customStone", "horsecustomstone.wav", loop: false);
                        }
                        if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", "horsecustomwoody.wav")))
                        {
                            AddCueData(data, ModManifest.UniqueID + "_customWoody", "horsecustomwoody.wav", loop: false);
                        }
                        if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", "horsecustomthud.wav")))
                        {
                            AddCueData(data, ModManifest.UniqueID + "_customThud", "horsecustomthud.wav", loop: false);
                        }
                    }
                });
            }
        }

        private void AddCueData(IDictionary<string, AudioCueData> data, string id, string filename, bool loop)
        {
            string path = Path.Combine(Helper.DirectoryPath, "assets", filename);

            data[id] = new AudioCueData
            {
                Id = id,
                FilePaths = new(1) { path },
                Category = "Sound",
                Looped = loop,
                StreamedVorbis = false
            };
        }



        [HarmonyPatch(typeof(GameLocation), "localSound")]
        public class SoundPatches
        {

            public static void localSound_prefix(GameLocation __instance, ref string audioName, Vector2? position)
            {

                foreach (Farmer who in __instance.farmers)
                {
                    if (config.ReplaceSounds == true && Game1.soundBank.Exists("CF.ChangeHorseSounds_customStone") && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && who.mount != null && who.mount.rider != null && position != null && who.mount.Tile == position)
                    {
                        audioName = "CF.ChangeHorseSounds_customStone";
                    }
                    if (config.ReplaceSounds == true && Game1.soundBank.Exists("CF.ChangeHorseSounds_customWoody") && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && who.mount != null && who.mount.rider != null && position != null && who.mount.Tile == position)
                    {
                        audioName = "CF.ChangeHorseSounds_customWoody";
                    }
                    if (config.ReplaceSounds == true && Game1.soundBank.Exists("CF.ChangeHorseSounds_customThud") && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && who.mount != null && who.mount.rider != null && position != null && who.mount.Tile == position)
                    {
                        audioName = "CF.ChangeHorseSounds_customThud";
                    }
                }            
            }
        }

        [HarmonyPatch(typeof(FarmerSprite), "checkForFootstep")]
        public class FarmerSpritePatches
        {

            public static void checkForFootstep_postfix(FarmerSprite __instance)
            {

                if (Context.IsMultiplayer)
                if (__instance.Owner is Farmer farmer && farmer.isRidingHorse() || __instance.Owner == null || __instance.Owner.currentLocation != Game1.currentLocation)
                    return;
                Farmer owner = __instance.Owner as Farmer;
                Vector2 key = owner != null ? owner.Tile : Game1.player.Tile;
                if (Game1.currentLocation.IsOutdoors || Game1.currentLocation.Name.ToLower().Contains("mine") || Game1.currentLocation.Name.ToLower().Contains("cave") || Game1.currentLocation.IsGreenhouse)
                {
                    string str = Game1.currentLocation.doesTileHaveProperty((int)key.X, (int)key.Y, "Type", "Buildings");
                    if (str == null || str.Length < 1)
                        str = Game1.currentLocation.doesTileHaveProperty((int)key.X, (int)key.Y, "Type", "Back");
                    if (str != null)
                    {
                        if (!(str == "Dirt"))
                        {
                            if (!(str == "Stone"))
                            {
                                if (!(str == "Grass"))
                                {
                                    if (str == "Wood")
                                        __instance.currentStep = "woodyStep";
                                }
                                else
                                    __instance.currentStep = Game1.currentLocation.GetSeason() == Season.Winter ? "snowyStep" : "grassyStep";
                            }
                            else
                                __instance.currentStep = "stoneStep";
                        }
                        else
                            __instance.currentStep = "sandyStep";
                    }
                }
                else
                    __instance.currentStep = "thudStep";
                if ((__instance.currentSingleAnimation >= 32 && __instance.currentSingleAnimation <= 56 || __instance.currentSingleAnimation >= 128 && __instance.currentSingleAnimation <= 152) && __instance.currentAnimationIndex % 4 == 0)
                {
                    string cueName = owner.currentLocation.getFootstepSoundReplacement(__instance.currentStep);
                    if (owner.onBridge.Value)
                    {
                        if (owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(owner.Position, 384))
                            cueName = "thudStep";
                        owner.bridge?.OnFootstep(owner.Position);
                    }
                    TerrainFeature terrainFeature;
                    if (Game1.currentLocation.terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature is Flooring flooring)
                        cueName = flooring.getFootstepSound();
                    Vector2 position = owner.Position;
                    if (owner.shouldShadowBeOffset)
                        position += owner.drawOffset;
                    if (!(cueName == "sandyStep"))
                    {
                        if (cueName == "snowyStep")
                            Game1.currentLocation.temporarySprites.Add(TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(position.X + 24f + (float)(Game1.random.Next(-4, 4) * 4), position.Y + 8f + (float)(Game1.random.Next(-4, 4) * 4)), false, false, position.Y / 1E+07f, 0.01f, Color.White, (float)(3.0 + Game1.random.NextDouble()), 0.0f, owner.FacingDirection == 1 || owner.FacingDirection == 3 ? -0.7853982f : 0.0f, 0.0f));
                    }
                    else
                    {
                        Game1.currentLocation.temporarySprites.Add(TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + 16f + (float)Game1.random.Next(-8, 8), position.Y + (float)(Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextBool(), position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, (float)(0.75 + (double)Game1.random.Next(-3, 4) * 0.0500000007450581), 0.0f, 0.0f, 0.0f));
                        TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + 16f + (float)Game1.random.Next(-4, 4), position.Y + (float)(Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextBool(), position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, (float)(0.550000011920929 + (double)Game1.random.Next(-3, 4) * 0.0500000007450581), 0.0f, 0.0f, 0.0f);
                        temporaryAnimatedSprite.delayBeforeAnimationStart = 20;
                        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
                    }
                    if (cueName != null && owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(owner.Position, 384) && (owner == Game1.player || !LocalMultiplayer.IsLocalMultiplayer(true)))
                    {
                        Game1.playSound(cueName);
                        if (owner.boots.Value != null && owner.boots.Value.ItemId == "853")
                            Game1.playSound("jingleBell");
                    }
                    foreach (Trinket trinketItem in owner.trinketItems)
                        trinketItem?.OnFootstep(owner);
                    if (owner.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                        return;
                    Game1.stats.takeStep();
                }
                else
                {
                    if ((__instance.currentSingleAnimation < 0 || __instance.currentSingleAnimation > 24) && (__instance.currentSingleAnimation < 96 || __instance.currentSingleAnimation > 120))
                        return;
                    if (owner.onBridge.Value && __instance.currentAnimationIndex % 2 == 0)
                    {
                        if (owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(owner.Position, 384) && (owner == Game1.player || !LocalMultiplayer.IsLocalMultiplayer(true)))
                            Game1.playSound("thudStep");
                        owner.bridge?.OnFootstep(owner.Position);
                        foreach (Trinket trinketItem in owner.trinketItems)
                            trinketItem?.OnFootstep(owner);
                    }
                    if (__instance.currentAnimationIndex != 0 || owner.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                        return;
                    Game1.stats.takeStep();
                }
            }
        }
    }
}
