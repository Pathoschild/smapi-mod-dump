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
