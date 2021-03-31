/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.Reflection.Emit;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewHack.AlwaysScrollMap
{

    public class ModConfig {
        /** Whether the mod is enabled upon load. */
        public bool EnabledIndoors = true;
        public bool EnabledOutdoors = false;
        /** Which key should be used to toggle always scroll map. */
        public SButton ToggleScroll = SButton.OemSemicolon;
    }
    
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public static bool Enabled() {
            if (Game1.currentLocation.IsOutdoors)
                return getInstance().config.EnabledOutdoors;
            else
                return getInstance().config.EnabledIndoors;
        }

        public override void HackEntry(IModHelper helper) {
            Patch((Microsoft.Xna.Framework.Point p)=>Game1.UpdateViewPort(false, p), Game1_UpdateViewPort);
            
            Helper.Events.Input.ButtonPressed += ToggleScroll;
        }
        
        protected override void InitializeApi(GenericModConfigMenuAPI api)
        {
            api.RegisterSimpleOption(ModManifest, "Enabled indoors", "Always scroll map indoors", () => config.EnabledIndoors, (bool val) => config.EnabledIndoors = val);
            api.RegisterSimpleOption(ModManifest, "Enabled outdoors", "Always scroll map outdoors", () => config.EnabledOutdoors, (bool val) => config.EnabledOutdoors = val);
            api.RegisterSimpleOption(ModManifest, "Toggle", "Toggle scrolling in current location", () => config.ToggleScroll, (SButton val) => config.ToggleScroll = val);
        }

        private void ToggleScroll(object sender, ButtonPressedEventArgs e) {
            if (e.Button.Equals(config.ToggleScroll)) {
                if (Game1.currentLocation.IsOutdoors) {
                    config.EnabledOutdoors ^= true;
                } else {
                    config.EnabledIndoors ^= true;
                }
            }
        }

        void Game1_UpdateViewPort()
        {
            var range = FindCode(
                // if (!Game1.viewportFreeze ...
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.viewportFreeze))
            );
            range.Extend(
                // if (Game1.currentLocation.forceViewportPlayerFollow)
                // (Note: currentLocation is a field on PC and a property on android).
                Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.forceViewportPlayerFollow)),
                OpCodes.Brfalse
            );
            // Encapsulate with if (!State.Enabled) {
            range.Prepend(
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.Enabled)),
                Instructions.Brtrue(AttachLabel(range.End[0]))
            );
            range.ReplaceJump(2, range[0]);
        }
    }
}

