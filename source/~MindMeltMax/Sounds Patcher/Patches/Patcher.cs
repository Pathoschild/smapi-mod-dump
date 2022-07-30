/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sounds_Patcher.Patches
{
    public class Patcher
    {
        private static IModHelper Helper;
        private static HarmonyInstance Instance;

        public static void Init(IModHelper helper)
        {
            Helper = helper;
            Instance = HarmonyInstance.Create(Helper.ModRegistry.ModID);

            #region Game1

            //Game1.playSound
            Instance.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(Game1SoundPatches), nameof(Game1SoundPatches.playSound_prefix))
            );

            //Game1.playSoundPitched
            Instance.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.playSoundPitched), new[] { typeof(string), typeof(int) }),
                prefix: new HarmonyMethod(typeof(Game1SoundPatches), nameof(Game1SoundPatches.playSoundPitched_prefix))
            );

            //Game1.playItemNumberSelectSound
            Instance.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.playItemNumberSelectSound)),
                prefix: new HarmonyMethod(typeof(Game1SoundPatches), nameof(Game1SoundPatches.playItemNumberSelectSound_prefix))
            );

            //Game1.changeMusicTrack
            Instance.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.changeMusicTrack), new[] { typeof(string), typeof(bool), typeof(Game1.MusicContext)}),
                prefix: new HarmonyMethod(typeof(Game1SoundPatches), nameof(Game1SoundPatches.changeMusicTrack_prefix))
            );

            #endregion

            #region GameLocation

            //GameLocation.playSound
            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.playSound), new[] { typeof(string), typeof(NetAudio.SoundContext) }),
                prefix: new HarmonyMethod(typeof(GameLocationSoundPatches), nameof(GameLocationSoundPatches.playSound_prefix))
            );

            //GameLocation.playSoundPitched
            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.playSoundPitched), new[] { typeof(string), typeof(int), typeof(NetAudio.SoundContext) }),
                prefix: new HarmonyMethod(typeof(GameLocationSoundPatches), nameof(GameLocationSoundPatches.playSoundPitched_prefix))
            );

            //GameLocation.playSoundAt
            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.playSoundAt), new[] { typeof(string), typeof(Vector2), typeof(NetAudio.SoundContext) }),
                prefix: new HarmonyMethod(typeof(GameLocationSoundPatches), nameof(GameLocationSoundPatches.playSoundAt_prefix))
            );

            //GameLocation.localSound
            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSound), new[] { typeof(string)}),
                prefix: new HarmonyMethod(typeof(GameLocationSoundPatches), nameof(GameLocationSoundPatches.localSound_prefix))
            );

            //GameLocation.localSoundAt
            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSoundAt), new[] { typeof(string), typeof(Vector2)}),
                prefix: new HarmonyMethod(typeof(GameLocationSoundPatches), nameof(GameLocationSoundPatches.localSoundAt_prefix))
            );

            #endregion

            #region ISoundBank

            //ISoundBank.PlayCue 1
            Instance.Patch(
                original: AccessTools.Method(typeof(SoundBankWrapper), nameof(SoundBankWrapper.PlayCue), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(SoundBankPatches), nameof(SoundBankPatches.PlayCue_prefix_1))
            );

            //ISoundBank.PlayCue 2
            Instance.Patch(
                original: AccessTools.Method(typeof(SoundBankWrapper), nameof(SoundBankWrapper.PlayCue), new[] { typeof(string), typeof(AudioListener), typeof(AudioEmitter) }),
                prefix: new HarmonyMethod(typeof(SoundBankPatches), nameof(SoundBankPatches.PlayCue_prefix_2))
            );

            #endregion

            #region ICue

            //ICue.Play
            Instance.Patch(
                original: AccessTools.Method(typeof(CueWrapper), nameof(CueWrapper.Play)),
                prefix: new HarmonyMethod(typeof(CuePatches), nameof(CuePatches.Play_prefix))
            );

            #endregion
        }
    }
}
