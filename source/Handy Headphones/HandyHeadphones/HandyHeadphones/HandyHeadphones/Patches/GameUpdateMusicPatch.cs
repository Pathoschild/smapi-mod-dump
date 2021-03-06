/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;

namespace HandyHeadphones.Patches
{
    [HarmonyPatch]
    class GameChangeUpdateMusicPatch
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static IModHelper helper = ModEntry.modHelper;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.updateMusic));
        }

        internal static bool Prefix()
        {
            if (Game1.soundBank == null)
            {
                return true;
            }

            Hat playerHat = Game1.player.hat;
            if (playerHat != null && (playerHat.Name == "Headphones" || playerHat.Name == "Earbuds" || playerHat.Name == "Studio Headphones") && Game1.CurrentEvent is null)
            {
                string song_to_play = Game1.player.currentLocation.miniJukeboxTrack.Value;
                if (string.IsNullOrEmpty(song_to_play))
                {
                    return false;
                }

                if (Game1.currentSong.Name == song_to_play)
                {
                    if (Game1.musicPlayerVolume < Game1.options.musicVolumeLevel || Game1.ambientPlayerVolume < Game1.options.ambientVolumeLevel)
                    {
                        if (Game1.musicPlayerVolume < Game1.options.musicVolumeLevel)
                        {
                            Game1.musicPlayerVolume = Math.Min(1f, Game1.musicPlayerVolume += 0.01f);
                            if (Game1.game1.IsMainInstance)
                            {
                                Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
                            }
                        }
                        if (Game1.ambientPlayerVolume < Game1.options.ambientVolumeLevel)
                        {
                            Game1.ambientPlayerVolume = Math.Min(1f, Game1.ambientPlayerVolume += 0.015f);
                            if (Game1.game1.IsMainInstance)
                            {
                                Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
                            }
                        }
                    }
                    else if (Game1.currentSong != null && !Game1.currentSong.IsPlaying && !Game1.currentSong.IsStopped)
                    {
                        Game1.currentSong = Game1.soundBank.GetCue(Game1.currentSong.Name);
                        if (Game1.game1.IsMainInstance)
                        {
                            Game1.currentSong.Play();
                        }
                    }
                    return false;
                }

                Game1.musicPlayerVolume = Math.Max(0f, Math.Min(Game1.options.musicVolumeLevel, Game1.musicPlayerVolume - 0.01f));
                Game1.ambientPlayerVolume = Math.Max(0f, Math.Min(Game1.options.musicVolumeLevel, Game1.ambientPlayerVolume - 0.01f));
                if (Game1.game1.IsMainInstance)
                {
                    Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
                    Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
                }
                if (Game1.musicPlayerVolume != 0f || Game1.ambientPlayerVolume != 0f || Game1.currentSong == null)
                {
                    return false;
                }
                if (song_to_play.Equals("none"))
                {
                    Game1.jukeboxPlaying = false;
                    Game1.currentSong.Stop(AudioStopOptions.Immediate);

                    return false;
                }
                else if ((Game1.options.musicVolumeLevel != 0f || Game1.options.ambientVolumeLevel != 0f) && (!song_to_play.Equals("rain") || Game1.endOfNightMenus.Count == 0))
                {
                    if (Game1.game1.IsMainInstance)
                    {
                        Game1.currentSong.Stop(AudioStopOptions.Immediate);
                        Game1.currentSong.Dispose();
                    }
                    Game1.currentSong = Game1.soundBank.GetCue(song_to_play);
                    if (Game1.game1.IsMainInstance)
                    {
                        Game1.currentSong.Play();
                        monitor.Log($"Playing: {Game1.currentLocation.miniJukeboxTrack.Value}", LogLevel.Trace);
                    }
                    if (Game1.game1.IsMainInstance && Game1.currentSong != null && Game1.currentSong.Name.Equals("rain") && Game1.currentLocation != null)
                    {
                        if (Game1.IsRainingHere())
                        {
                            if (Game1.currentLocation.IsOutdoors)
                            {
                                Game1.currentSong.SetVariable("Frequency", 100f);
                            }
                            else if (!Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                            {
                                Game1.currentSong.SetVariable("Frequency", 15f);
                            }
                        }
                        else if (Game1.eventUp)
                        {
                            Game1.currentSong.SetVariable("Frequency", 100f);
                        }
                    }
                }
                else
                {
                    Game1.currentSong.Stop(AudioStopOptions.Immediate);
                }
                Game1.currentTrackOverrideable = false;
                Game1.requestedMusicDirty = false;

                monitor.Log($"Player selected: {Game1.currentLocation.miniJukeboxTrack.Value}", LogLevel.Trace);
                return false;
            }

            return true;
        }
    }
}
