using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TooshiStardewMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (Context.IsWorldReady) // save is loaded
            {
                this.Monitor.Log("a trace message", LogLevel.Trace);
                this.Monitor.Log($"{Game1.player.name} pressed {e.Button}.");
            }
        }
    }

    public class ModEntry : Mod, IAssetEditor
    {
        this.Monitor.Log("a trace message for crops edit", LogLevel.Trace);
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Crops");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            asset
                .AsDictionary<int, string>()
                .Set((id, data) =>
                {
                    string[] fields = data.Split('/');
                    fields[1] = "winter";
                    return string.Join("/", fields);
                });
        }
    }
}