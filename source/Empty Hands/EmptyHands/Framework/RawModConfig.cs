/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace Pathoschild.Stardew.EmptyHands.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class RawModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keyboard input map.</summary>
        public InputMapConfiguration<string> Keyboard { get; set; }

        /// <summary>The controller input map.</summary>
        public InputMapConfiguration<string> Controller { get; set; }

        /// <summary>Whether to check for updates to the mod.</summary>
        public bool CheckForUpdates { get; set; } = true;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct a default instance.</summary>
        public RawModConfig()
        {
            this.Keyboard = new InputMapConfiguration<string>
            {
                SetToNothing = Keys.OemTilde.ToString()
            };
            this.Controller = new InputMapConfiguration<string>
            {
                SetToNothing = ""
            };
        }

        /// <summary>Get a parsed representation of the mod configuration.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public ModConfig GetParsed(IMonitor monitor)
        {
            return new ModConfig
            {
                Keyboard = new InputMapConfiguration<Keys>
                {
                    SetToNothing = this.TryParse(monitor, this.Keyboard.SetToNothing, Keys.OemTilde)
                },
                Controller = new InputMapConfiguration<Buttons>
                {
                    SetToNothing = this.TryParse<Buttons>(monitor, this.Controller.SetToNothing)
                }
            };
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse a raw enum value.</summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="raw">The raw value.</param>
        /// <param name="defaultValue">The default value if it can't be parsed.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        private T TryParse<T>(IMonitor monitor, string raw, T defaultValue = default(T)) where T : struct
        {
            // empty
            if (string.IsNullOrWhiteSpace(raw))
                return defaultValue;

            // valid enum
            T parsed;
            if (Enum.TryParse(raw, true, out parsed))
                return parsed;

            // invalid
            monitor.Log($"Couldn't parse '{raw}' from config.json as a {typeof(T).Name} value, using default value of {defaultValue}.", LogLevel.Warn);
            return defaultValue;
        }
    }
}
