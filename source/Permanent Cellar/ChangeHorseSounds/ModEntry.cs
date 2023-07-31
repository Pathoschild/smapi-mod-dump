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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using GenericModConfigMenu;
using HarmonyLib;
using StardewValley;
using StardewValley.Characters;


namespace ChangeHorseSounds
{

    public sealed class ChangeHorseSoundsModConfig
    {
     public string HorseName { get; set; } = "";

    }
    public class ModEntry : Mod
    {
        public static IModHelper SHelper { get; private set; }
        private static ChangeHorseSoundsModConfig config;


        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            SHelper = helper;

            config = SHelper.ReadConfig<ChangeHorseSoundsModConfig>();


            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                              original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSoundAt)),
                              prefix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.localSoundAt_prefix))
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

                configMenu.AddTextOption(
                    mod: ModManifest,
                    name: () => "Horse Name:",
                    tooltip: () => "The name of the horse that should have its sound changed",
                    getValue: () => config.HorseName,
                    setValue: value => config.HorseName = value
                    );

        }


        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string path1 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustom.wav");
            string path2 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomstone.wav");
            string path3 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomwoody.wav");
            string path4 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomthud.wav");
            string path5 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustom.wav");
            string path6 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomstone.wav");
            string path7 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomwoody.wav");
            string path8 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomthud.wav");
            string path9 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustom.wav");
            string path10 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomstone.wav");
            string path11 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomwoody.wav");
            string path12 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomthud.wav");

            if (File.Exists(path1))
            {
                CueDefinition cue_definition = new CueDefinition();
                cue_definition.name = "horseCustom";

                SoundEffect horseCustom;
                FileStream fs = new FileStream(path1, FileMode.Open);
                horseCustom = SoundEffect.FromStream(fs);

                cue_definition.SetSound(horseCustom, 5, false, false);
                Game1.soundBank.AddCue(cue_definition);
                
            }
            if (File.Exists(path2))
            {
                CueDefinition cue_definition2 = new CueDefinition();
                cue_definition2.name = "horseCustomStone";

                SoundEffect horseCustomStone;
                FileStream fs = new FileStream(path2, FileMode.Open);
                horseCustomStone = SoundEffect.FromStream(fs);

                cue_definition2.SetSound(horseCustomStone, 5, false, false);
                Game1.soundBank.AddCue(cue_definition2);

            }
            if (File.Exists(path3))
            {
                CueDefinition cue_definition3 = new CueDefinition();
                cue_definition3.name = "horseCustomWoody";

                SoundEffect horseCustomWoody;
                FileStream fs = new FileStream(path3, FileMode.Open);
                horseCustomWoody = SoundEffect.FromStream(fs);

                cue_definition3.SetSound(horseCustomWoody, 5, false, false);
                Game1.soundBank.AddCue(cue_definition3);

            }
            if (File.Exists(path4))
            {
                CueDefinition cue_definition4 = new CueDefinition();
                cue_definition4.name = "horseCustomThud";

                SoundEffect horseCustomThud;
                FileStream fs = new FileStream(path4, FileMode.Open);
                horseCustomThud = SoundEffect.FromStream(fs);

                cue_definition4.SetSound(horseCustomThud, 5, false, false);
                Game1.soundBank.AddCue(cue_definition4);

            }
            if (File.Exists(path5))
            {
                CueDefinition cue_definition5 = new CueDefinition();
                cue_definition5.name = "tractorCustom";

                SoundEffect tractorCustom;
                FileStream fs = new FileStream(path5, FileMode.Open);
                tractorCustom = SoundEffect.FromStream(fs);

                cue_definition5.SetSound(tractorCustom, 5, false, false);
                Game1.soundBank.AddCue(cue_definition5);

            }
            if (File.Exists(path6))
            {
                CueDefinition cue_definition6 = new CueDefinition();
                cue_definition6.name = "tractorCustomStone";

                SoundEffect tractorCustomStone;
                FileStream fs = new FileStream(path6, FileMode.Open);
                tractorCustomStone = SoundEffect.FromStream(fs);

                cue_definition6.SetSound(tractorCustomStone, 5, false, false);
                Game1.soundBank.AddCue(cue_definition6);

            }
            if (File.Exists(path7))
            {
                CueDefinition cue_definition7 = new CueDefinition();
                cue_definition7.name = "tractorCustomWoody";

                SoundEffect tractorCustomWoody;
                FileStream fs = new FileStream(path7, FileMode.Open);
                tractorCustomWoody = SoundEffect.FromStream(fs);

                cue_definition7.SetSound(tractorCustomWoody, 5, false, false);
                Game1.soundBank.AddCue(cue_definition7);

            }
            if (File.Exists(path8))
            {
                CueDefinition cue_definition8 = new CueDefinition();
                cue_definition8.name = "tractorCustomThud";

                SoundEffect tractorCustomThud;
                FileStream fs = new FileStream(path8, FileMode.Open);
                tractorCustomThud = SoundEffect.FromStream(fs);

                cue_definition8.SetSound(tractorCustomThud, 5, false, false);
                Game1.soundBank.AddCue(cue_definition8);

            }
            if (File.Exists(path9))
            {
                CueDefinition cue_definition9 = new CueDefinition();
                cue_definition9.name = "motorcycleCustom";

                SoundEffect motorcycleCustom;
                FileStream fs = new FileStream(path9, FileMode.Open);
                motorcycleCustom = SoundEffect.FromStream(fs);

                cue_definition9.SetSound(motorcycleCustom, 5, false, false);
                Game1.soundBank.AddCue(cue_definition9);

            }
            if (File.Exists(path10))
            {
                CueDefinition cue_definition10 = new CueDefinition();
                cue_definition10.name = "motorcycleCustomStone";

                SoundEffect motorcycleCustomStone;
                FileStream fs = new FileStream(path10, FileMode.Open);
                motorcycleCustomStone = SoundEffect.FromStream(fs);

                cue_definition10.SetSound(motorcycleCustomStone, 5, false, false);
                Game1.soundBank.AddCue(cue_definition10);

            }
            if (File.Exists(path11))
            {
                CueDefinition cue_definition11 = new CueDefinition();
                cue_definition11.name = "motorcycleCustomWoody";

                SoundEffect motorcycleCustomWoody;
                FileStream fs = new FileStream(path11, FileMode.Open);
                motorcycleCustomWoody = SoundEffect.FromStream(fs);

                cue_definition11.SetSound(motorcycleCustomWoody, 5, false, false);
                Game1.soundBank.AddCue(cue_definition11);

            }
            if (File.Exists(path12))
            {
                CueDefinition cue_definition12 = new CueDefinition();
                cue_definition12.name = "motorcycleCustomThud";

                SoundEffect motorcycleCustomThud;
                FileStream fs = new FileStream(path12, FileMode.Open);
                motorcycleCustomThud = SoundEffect.FromStream(fs);

                cue_definition12.SetSound(motorcycleCustomThud, 5, false, false);
                Game1.soundBank.AddCue(cue_definition12);

            }
        }



     [HarmonyPatch(typeof(GameLocation), "localSoundAt")]
        public class SoundPatches
        {
            public static IEnumerable<Horse> GetHorsesIn(GameLocation location)
            {
                if (!Context.IsMultiplayer)
                {
                    return from h in location.characters.OfType<Horse>()
                           select h;
                }
                return (from h in location.characters.OfType<Horse>()
                        select h).Concat(from player in (IEnumerable<Farmer>)location.farmers
                                         where player.mount != null
                                         select player.mount).Distinct();
            }

            static string path1 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustom.wav");
            static string path2 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomstone.wav");
            static string path3 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomwoody.wav");
            static string path4 = Path.Combine(SHelper.DirectoryPath, "assets", "horsecustomthud.wav");
            static string path5 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustom.wav");
            static string path6 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomstone.wav");
            static string path7 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomwoody.wav");
            static string path8 = Path.Combine(SHelper.DirectoryPath, "assets", "tractorcustomthud.wav");
            static string path9 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustom.wav");
            static string path10 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomstone.wav");
            static string path11 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomwoody.wav");
            static string path12 = Path.Combine(SHelper.DirectoryPath, "assets", "motorcyclecustomthud.wav");


            public static void localSoundAt_prefix(GameLocation __instance, ref string audioName, Vector2 position)
            {

                if (!Context.IsMultiplayer && File.Exists(path1) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && string.Equals(Game1.player.mount.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase))
                {
                    audioName = "horseCustom";
                }
                if (!Context.IsMultiplayer && File.Exists(path2) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && string.Equals(Game1.player.mount.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase))
                {
                    audioName = "horseCustomStone";
                }
                if (!Context.IsMultiplayer && File.Exists(path3) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && string.Equals(Game1.player.mount.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase))
                {
                    audioName = "horseCustomWoody";
                }
                if (!Context.IsMultiplayer && File.Exists(path4) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && string.Equals(Game1.player.mount.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase))
                {
                    audioName = "horseCustomThud";
                }
                if (!Context.IsMultiplayer && File.Exists(path5) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("tractor"))
                {
                    audioName = "tractorCustom";
                }
                if (!Context.IsMultiplayer && File.Exists(path6) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("tractor"))
                {
                    audioName = "tractorCustomStone";
                }
                if (!Context.IsMultiplayer && File.Exists(path7) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("tractor"))
                {
                    audioName = "tractorCustomWoody";
                }
                if (!Context.IsMultiplayer && File.Exists(path8) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("tractor"))
                {
                    audioName = "tractorCustomThud";
                }
                if (!Context.IsMultiplayer && File.Exists(path9) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("motorcycle"))
                {
                    audioName = "motorcycleCustom";
                }
                if (!Context.IsMultiplayer && File.Exists(path10) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("motorcycle"))
                {
                    audioName = "tractorCustomStone";
                }
                if (!Context.IsMultiplayer && File.Exists(path11) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("motorcycle"))
                {
                    audioName = "tractorCustomWoody";
                }
                if (!Context.IsMultiplayer && File.Exists(path12) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && Game1.player.mount.getTileLocation() == position && Game1.player.mount.rider != null && Game1.player.mount.Name.StartsWith("motorcycle"))
                {
                    audioName = "tractorCustomThud";
                }
                foreach (Horse horse1 in GetHorsesIn(__instance))
                {
                    if (Context.IsMultiplayer && File.Exists(path1) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && string.Equals(horse1.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase) & !horse1.Name.Equals(""))
                    {
                        audioName = "horseCustom";
                    }
                    if (Context.IsMultiplayer && File.Exists(path2) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && string.Equals(horse1.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase) & !horse1.Name.Equals(""))
                    {
                        audioName = "horseCustomStone";
                    }
                    if (Context.IsMultiplayer && File.Exists(path3) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && string.Equals(horse1.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase) & !horse1.Name.Equals(""))
                    {
                        audioName = "horseCustomWoody";
                    }
                    if (Context.IsMultiplayer && File.Exists(path4) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && string.Equals(horse1.Name, config.HorseName, StringComparison.InvariantCultureIgnoreCase) & !horse1.Name.Equals(""))
                    {
                        audioName = "horseCustomThud";
                    }
                    if (Context.IsMultiplayer && File.Exists(path5) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("tractor"))
                    {
                        audioName = "tractorCustom";
                    }
                    if (Context.IsMultiplayer && File.Exists(path6) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("tractor"))
                    {
                        audioName = "tractorCustomStone";
                    }
                    if (Context.IsMultiplayer && File.Exists(path7) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("tractor"))
                    {
                        audioName = "tractorCustomWoody";
                    }
                    if (Context.IsMultiplayer && File.Exists(path8) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("tractor"))
                    {
                        audioName = "tractorCustomThud";
                    }
                    if (Context.IsMultiplayer && File.Exists(path9) && audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("motorcycle"))
                    {
                        audioName = "motorcycleCustom";
                    }
                    if (Context.IsMultiplayer && File.Exists(path10) && audioName.Equals("stoneStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("motorcycle"))
                    {
                        audioName = "motorcycleCustomStone";
                    }
                    if (Context.IsMultiplayer && File.Exists(path11) && audioName.Equals("woodyStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("motorcycle"))
                    {
                        audioName = "motorcycleCustomWoody";
                    }
                    if (Context.IsMultiplayer && File.Exists(path12) && audioName.Equals("thudStep", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name.StartsWith("motorcycle"))
                    {
                        audioName = "motorcycleCustomThud";
                    }
                }
            }
        }
    }
}
