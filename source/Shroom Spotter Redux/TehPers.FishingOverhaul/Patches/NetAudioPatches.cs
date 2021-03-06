/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TehPers.FishingOverhaul.Patches {

    public class NetAudioPatches {
        public static bool Prefix(string audioName) {
            if (audioName == "FishHit" && Game1.player.CurrentTool is FishingRod rod && !ModFishing.Instance.Overrider.OverridingCatch.Contains(rod)) {
                ModFishing.Instance.Monitor.Log($"Prevented {audioName} cue from playing.", LogLevel.Trace);
                return false;
            }

            return true;
        }
    }
}
