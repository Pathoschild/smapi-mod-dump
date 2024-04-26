/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Integrations;
using Microsoft.Xna.Framework;

namespace FishCatalogue
{
    internal class ModConfig
    {
        public int HUD_X_Position { get; set; } = 10;
        public int HUD_Y_Position { get; set; } = 10;
        public int HUD_Columns { get; set; } = 2;
        public bool HideUncaughtFish { get; set; } = true;
        public bool ShowFishNames { get; set; } = true;


        public ModConfig()
        {
            // Initialise variables here
            HUD_X_Position = 10;
            HUD_Y_Position = 10;
            HUD_Columns = 2;
            HideUncaughtFish = true;
            ShowFishNames = true;
        }

        public Vector2 HudPosition()
        {
            return new Vector2(HUD_X_Position, HUD_Y_Position);
        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Instance.Config = new ModConfig(),
                save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );

            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgHudXPosition_Name,
                tooltip: I18n.CfgHudXPosition_Desc,
                getValue: () => this.HUD_X_Position,
                setValue: (val) => this.HUD_X_Position = (int)(val)
            );

            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgHudYPosition_Name,
                tooltip: I18n.CfgHudYPosition_Desc,
                getValue: () => this.HUD_Y_Position,
                setValue: (val) => this.HUD_Y_Position = (int)(val)
            );

            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgHudColumns_Name,
                tooltip: I18n.CfgHudColumns_Desc,
                getValue: () => this.HUD_Columns,
                setValue: (val) => this.HUD_Columns = (int)(val),
                min: 1,
                max: 10,
                interval: 1
            );

            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgHideUncaughtFish_Name,
                tooltip: I18n.CfgHideUncaughtFish_Desc,
                getValue: () => this.HideUncaughtFish,
                setValue: (val) => this.HideUncaughtFish = val
            );

            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgShowFishName_Name,
                tooltip: I18n.CfgShowFishName_Desc,
                getValue: () => this.ShowFishNames,
                setValue: (val) => this.ShowFishNames = val
            );
        }

        public static string displayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }
    }
}


