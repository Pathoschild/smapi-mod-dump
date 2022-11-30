/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SoundTweaker
{
    public partial class ModEntry
    {

        [HarmonyPatch(typeof(AudioEmitter))]
        [HarmonyPatch(MethodType.Constructor)]
        public class AudioEmitter_Patch
        {
            public static void Postfix(AudioEmitter __instance)
            {
                if (!Config.ModEnabled)
                    return;
            }
        }
        [HarmonyPatch(typeof(AudioEmitter), nameof(AudioEmitter.Position))]
        [HarmonyPatch(MethodType.Getter)]
        public class AudioEmitter_Position_Patch
        {
            public static void Postfix(ref Vector3 __result)
            {
                if (!Config.ModEnabled)
                    return;
                __result = new Vector3(Game1.random.Next(0, 2) - 1, Game1.random.Next(0, 2) - 1, Game1.random.Next(0, 2) - 1);
            }
        }
        [HarmonyPatch(typeof(Cue), nameof(Cue.Apply3D))]
        public class Cue_Apply3D_Patch
        {
            public static void Postfix(AudioListener listener, AudioEmitter emitter)
            {
                if (!Config.ModEnabled)
                    return;
                var x = 1;
            }
        }
        [HarmonyPatch(typeof(Cue), "PlaySoundInstance")]
        public class Cue_PlaySoundInstance_Patch
        {
            public static void Prefix(SoundEffectInstance sound_instance, int variant_index)
            {
                if (!Config.ModEnabled)
                    return;
                var x = 1;
            }
        }
        [HarmonyPatch(typeof(SoundEffectInstance), "PlatformPlay")]
        public class SoundEffectInstance_PlatformPlay_Patch
        {
            public static void Prefix(SoundEffectInstance __instance, ref bool ___is3D, FAudio.F3DAUDIO_DSP_SETTINGS ___dspSettings, ref bool ____isDynamic, ref float ____pan, ref float ____volume, ref float ____pitch)
            {
                if (!Config.ModEnabled)
                    return;

                var src = ___dspSettings.SrcChannelCount;
                var dst = ___dspSettings.DstChannelCount;
                ___is3D = true;
                ____pan = -1;
                ____volume = 10000;
                AccessTools.Method(typeof(SoundEffectInstance), "InitDSPSettings").Invoke(__instance, new object[] { 2U });
                
            }
        }
        [HarmonyPatch(typeof(SoundEffectInstance), "InitDSPSettings")]
        public class SoundEffectInstance_InitDSPSettings_Patch
        {
            public static void Prefix(SoundEffectInstance __instance, uint srcChannels, ref bool ___is3D, FAudio.F3DAUDIO_DSP_SETTINGS ___dspSettings, ref bool ____isDynamic, ref float ____pan, ref float ____volume, ref float ____pitch)
            {
                if (!Config.ModEnabled)
                    return;
                ___is3D = true;
                ____pan = 1;
            }
            public static void Postfix(SoundEffectInstance __instance, uint srcChannels, ref bool ___is3D, FAudio.F3DAUDIO_DSP_SETTINGS ___dspSettings, ref bool ____isDynamic, ref float ____pan, ref float ____volume, ref float ____pitch)
            {
                if (!Config.ModEnabled)
                    return;
            }
        }
        [HarmonyPatch(typeof(Cue), "UpdateRpcCurves")]
        public class Cue_UpdateRpcCurves_Patch
        {
            public static void Postfix(Cue __instance, XactSoundBankSound ____currentXactSound)
            {
                if (!Config.ModEnabled)
                    return;
                //AccessTools.FieldRefAccess<Cue, float>(__instance, "_rpcVolume") = 10f;
            }
        }
    }
}