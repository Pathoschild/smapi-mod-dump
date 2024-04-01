/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace DialogueBoxRedesign
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
    }
    
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        public float BoxBackgroundOpacity { get; set; } = 0.8f;
        
        /// <summary>
        /// Whether to move the character portrait to the left of the text.
        /// </summary>
        public bool ShowPortraitOnTheLeft { get; set; } = false;
        
        /// <summary>
        /// Whether to show the speaker's name above the portrait.
        /// </summary>
        public bool ShowSpeakerName { get; set; } = true;
        
        /// <summary>
        /// Whether to show the friendship jewel.
        /// </summary>
        public bool ShowFriendshipJewel { get; set; } = true;

        /// <summary>
        /// Setup the Generic Mod Config Menu API.
        /// </summary>
        public static void SetUpModConfigMenu(ModConfig config, ModEntry mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            var manifest = mod.ModManifest;

            api.Register(manifest, () =>
            {
                config = new ModConfig();
                mod.SaveConfig(config);
            }, () => mod.SaveConfig(config));
            
            api.AddSectionTitle(manifest, () => "Appearance");

            api.AddNumberOption(manifest, () => config.BoxBackgroundOpacity, val => config.BoxBackgroundOpacity = val,
                () => "Box Background Opacity", null, 0.0f, 1.0f, 0.01f);
            
            api.AddBoolOption(manifest, () => config.ShowPortraitOnTheLeft, val => config.ShowPortraitOnTheLeft = val,
                () => "Show Portrait On The Left",
                () => "Whether to move the character portrait to the left of the text.");
            api.AddBoolOption(manifest, () => config.ShowSpeakerName, val => config.ShowSpeakerName = val,
                () => "Show Speaker Name", 
                () => "Whether to show the speaker's name above the portrait.");
            api.AddBoolOption(manifest, () => config.ShowFriendshipJewel, val => config.ShowFriendshipJewel = val,
                () => "Show Friendship Jewel", 
                () => "Whether to show the friendship jewel.");
        }
    }
}