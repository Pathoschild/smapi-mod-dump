using System;
using System.Reflection.Emit;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StardewHack.AlwaysScrollMap
{
    static class State {
        public static bool Enabled = true;
    }

    public class ModConfig {
        /** Whether the mod is enabled upon load. */
        public bool Enabled = true;
        /** Which key should be used to toggle always scroll map. */
        public SButton ToggleScroll = SButton.OemSemicolon;
    }
    
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void Entry(IModHelper helper) {
            base.Entry(helper);
            InputEvents.ButtonPressed += ToggleScroll;
            State.Enabled = config.Enabled;
        }

        private void ToggleScroll(object sender, EventArgs e) {
            var ev = (EventArgsInput)e;
            if (ev.Button.Equals(config.ToggleScroll)) {
                State.Enabled ^= true;
            }
        }

        [BytecodePatch("StardewValley.Game1::UpdateViewPort")]
        void Game1_UpdateViewPort()
        {
            var range = FindCode(
                // if (!Game1.viewportFreeze ...
                Instructions.Ldsfld(typeof(StardewValley.Game1), "viewportFreeze")
            );
            range.Extend(
                // if (Game1.currentLocation.forceViewportPlayerFollow)
                Instructions.Ldsfld(typeof(StardewValley.Game1), "currentLocation"),
                Instructions.Ldfld(typeof(StardewValley.GameLocation), "forceViewportPlayerFollow"),
                OpCodes.Brfalse
            );
            // Encapsulate with if (!State.Enabled) {
            range.Prepend(
                Instructions.Ldsfld(typeof(StardewHack.AlwaysScrollMap.State), "Enabled"),
                Instructions.Brtrue(AttachLabel(range.End[0]))
            );
        }
    }
}

