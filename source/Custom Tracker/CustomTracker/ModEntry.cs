/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>This mod instance.</summary>
        internal static ModEntry Instance;

        /// <summary>The mod's config.json settings.</summary>
        internal static ModConfig Config;

        /// <summary>The "address" of the custom tracker's texture in Stardew's content manager.</summary>
        string TrackerLoadString { get; } = "LooseSprites\\CustomTracker";
        /// <summary>The "address" of the "forage mode" tracker's background texture in Stardew's content manager.</summary>
        string BackgroundLoadString { get; } = "LooseSprites\\CustomTrackerForageBG";

        /// <summary>The loaded texture for the custom tracker.</summary>
        Texture2D Spritesheet { get; set; } = null;
        /// <summary>The loaded texture for the "forage mode" tracker's background.</summary>
        Texture2D Background { get; set; } = null;
        /// <summary>A rectangle describing the spritesheet location of the custom tracker.</summary>
        Rectangle SpriteSource { get; set; }
        /// <summary>A rectangle describing the spritesheet location of the "forage mode" tracker's background.</summary>
        Rectangle BackgroundSource { get; set; }

        /// <summary>True if forage icons are being displayed instead of the custom tracker. If the tracker texture cannot be loaded, this mode may activate as a fallback.</summary>
        bool ForageIconMode { get; set; } = false;

        /// <summary>A set of object IDs that should be tracked. Should be populated from the mod's config.json settings.</summary>
        HashSet<int> TrackedObjectIDs { get; set; } = new HashSet<int>();
        /// <summary>A set of object names that should be tracked. Should be populated from the mod's config.json settings.</summary>
        HashSet<string> TrackedObjectNames { get; set; } = new HashSet<string>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this; //provide a global reference to this mod's utilities

            Config = helper.ReadConfig<ModConfig>(); //load the mod's config.json file
            if (Config == null) //if loading failed
                return;

            //populate object sets from config settings, if applicable
            if (Config.OtherTrackedObjects != null)
            {
                foreach (object entry in Config.OtherTrackedObjects) //for each entry in the list of extra objects to track
                {
                    if (int.TryParse(entry.ToString(), out int objectID)) //if this entry can be converted into an integer, treat it as an object ID
                    {
                        TrackedObjectIDs.Add(objectID); //add it to the set of tracked IDs
                    }
                    else if (entry is string objectName) //if this is a non-integer string, treat it as an object name
                    {
                        TrackedObjectNames.Add(objectName.ToLower()); //add it to the set of tracked names
                    }
                    else //if this entry couldn't be parsed
                    {
                        Monitor.Log($"Failed to recognize this entry in the config.json \"OtherTrackedObjects\" list: {entry.ToString()}", LogLevel.Info);
                    }
                }
            }

            //register the mod's SMAPI events
            helper.Events.Display.RenderedHud += Display_RenderedHud;
            helper.Events.Display.RenderingHud += Display_RenderingHud;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            helper.Events.Display.RenderedActiveMenu += GMCM.Enable;
        }
    }
}
