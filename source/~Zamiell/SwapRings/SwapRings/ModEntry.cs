/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace SwapRings
{
    public class ModEntry : Mod
    {
        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new ModConfig(),
                save: () => this.Helper.WriteConfig(config)
            );

            configMenu.AddKeybindList(
                this.ModManifest,
                () => config.Hotkey,
                (KeybindList val) => config.Hotkey = val,
                () => "Swap Hotkey",
                () => "The hotkey to swap the rings."
            );
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (config.Hotkey.IsDown())
            {
                SwapRings();
            }
        }

        private void SwapRings()
        {
            var ringIndexes = GetInventoryRingIndexes();

            if (ringIndexes.Count < 1)
            {
                return;
            }

            var firstRingIndex = ringIndexes[0];
            SwapRing(true, firstRingIndex);

            // We could also use "crit", which is the sound effect for equipping a ring.
            Game1.playSound("toolSwap");

            if (ringIndexes.Count < 2)
            {
                return;
            }

            var secondRingIndex = ringIndexes[1];
            SwapRing(false, secondRingIndex);
            // We already played a sound.
        }

        // This does not include equipped rings.
        private List<int> GetInventoryRingIndexes()
        {
            List<int> ringIndexes = new List<int>();

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                var item = Game1.player.Items[i];

                if (item is Ring ring)
                {
                    ringIndexes.Add(i);
                }
            }

            return ringIndexes;
        }

        private void SwapRing(bool left, int newRingIndex)
        {
            var newItem = Game1.player.Items[newRingIndex];
            if (newItem is Ring newRing)
            {
                if (left)
                {
                    var oldRing = Game1.player.leftRing.Value;
                    Game1.player.Equip(newRing, Game1.player.leftRing);
                    Game1.player.Items[newRingIndex] = oldRing;
                }
                else
                {
                    var oldRing = Game1.player.rightRing.Value;
                    Game1.player.Equip(newRing, Game1.player.rightRing);
                    Game1.player.Items[newRingIndex] = oldRing;
                }
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
