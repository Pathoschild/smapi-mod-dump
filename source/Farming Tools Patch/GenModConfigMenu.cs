/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Torsang/FarmingToolsPatch
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericModConfigMenu;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Runtime.CompilerServices;

namespace FarmingToolsPatch
{
    internal static class GenModConfigMenu
    {
        public static void configurate ( IMod mod, IGenericModConfigMenuApi cfgMenu )
        {

            // register mod
            cfgMenu.Register
            (
                mod: mod.ModManifest,
                reset: () => ModEntry.config = new ModConfig(),
                save: () => mod.Helper.WriteConfig ( ModEntry.config )
            );

            cfgMenu.AddSectionTitle
            (
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get ( "hotkey-options" )
            );
            cfgMenu.AddBoolOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "hotkey-toggle" ),
                tooltip: () => mod.Helper.Translation.Get ( "hktoggle-tooltip" ),
                getValue: () => ModEntry.config.hKeyBool,
                setValue: value => ModEntry.config.hKeyBool = value
            );
            cfgMenu.AddKeybindList
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "length-inc-btn" ),
                tooltip: () => mod.Helper.Translation.Get ( "length-ib-tooltip" ),
                getValue: () => ModEntry.config.incLengthBtn,
                setValue: value => ModEntry.config.incLengthBtn = value
            );
            cfgMenu.AddKeybindList
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "radius-inc-btn" ),
                tooltip: () => mod.Helper.Translation.Get ( "radius-ib-tooltip" ),
                getValue: () => ModEntry.config.incRadiusBtn,
                setValue: value => ModEntry.config.incRadiusBtn = value
            );
            cfgMenu.AddKeybindList
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "length-dec-btn" ),
                tooltip: () => mod.Helper.Translation.Get ( "length-db-tooltip" ),
                getValue: () => ModEntry.config.decLengthBtn,
                setValue: value => ModEntry.config.decLengthBtn = value
            );
            cfgMenu.AddKeybindList
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "radius-dec-btn" ),
                tooltip: () => mod.Helper.Translation.Get ( "radius-db-tooltip" ),
                getValue: () => ModEntry.config.decRadiusBtn,
                setValue: value => ModEntry.config.decRadiusBtn = value
            );
            cfgMenu.AddKeybindList
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "cycle-charge-lvl" ),
                tooltip: () => mod.Helper.Translation.Get ( "cycle-cl-tooltip" ),
                getValue: () => ModEntry.config.cyclePwrLvl,
                setValue: value => ModEntry.config.cyclePwrLvl = value
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "reset-default" ),
                tooltip: () => mod.Helper.Translation.Get ( "reset-tooltip" ),
                getValue: () => ModEntry.config.resetTime,
                setValue: value => ModEntry.config.resetTime = value,
                min: 1,
                max: 5,
                interval: 1
            );

            cfgMenu.AddSectionTitle
            (
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get ( "iridium-section" )
            );
            cfgMenu.AddBoolOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "iridium-bool" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-bool-tooltip" ),
                getValue: () => ModEntry.config.iBool,
                setValue: value => ModEntry.config.iBool = value
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-length" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-l-tooltip" ),
                getValue: () => ModEntry.config.iLength,
                setValue: value => ModEntry.config.iLength = value,
                min: 1,
                max: ModEntry.toolMax,
                interval: 1
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-radius" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-r-tooltip" ),
                getValue: () => ModEntry.config.iRadius,
                setValue: value => ModEntry.config.iRadius = value,
                min: 0,
                max: ModEntry.toolMax,
                interval: 1
            );

            cfgMenu.AddSectionTitle
            (
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get ( "gold-section" )
            );
            cfgMenu.AddBoolOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "gold-bool" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-bool-tooltip" ),
                getValue: () => ModEntry.config.gBool,
                setValue: value => ModEntry.config.gBool = value
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-length" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-l-tooltip" ),
                getValue: () => ModEntry.config.gLength,
                setValue: value => ModEntry.config.gLength = value,
                min: 1,
                max: ModEntry.toolMax,
                interval: 1
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-radius" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-r-tooltip" ),
                getValue: () => ModEntry.config.gRadius,
                setValue: value => ModEntry.config.gRadius = value,
                min: 0,
                max: ModEntry.toolMax,
                interval: 1
            );

            cfgMenu.AddSectionTitle
            (
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get ( "steel-section" )
            );
            cfgMenu.AddBoolOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "steel-bool" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-bool-tooltip" ),
                getValue: () => ModEntry.config.sBool,
                setValue: value => ModEntry.config.sBool = value
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-length" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-l-tooltip" ),
                getValue: () => ModEntry.config.sLength,
                setValue: value => ModEntry.config.sLength = value,
                min: 1,
                max: ModEntry.toolMax,
                interval: 1
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-radius" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-r-tooltip" ),
                getValue: () => ModEntry.config.sRadius,
                setValue: value => ModEntry.config.sRadius = value,
                min: 0,
                max: ModEntry.toolMax,
                interval: 1
            );

            cfgMenu.AddSectionTitle
            (
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get ( "copper-section" )
            );
            cfgMenu.AddBoolOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "copper-bool" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-bool-tooltip" ),
                getValue: () => ModEntry.config.cBool,
                setValue: value => ModEntry.config.cBool = value
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-length" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-l-tooltip" ),
                getValue: () => ModEntry.config.cLength,
                setValue: value => ModEntry.config.cLength = value,
                min: 1,
                max: ModEntry.toolMax,
                interval: 1
            );
            cfgMenu.AddNumberOption
            (
                mod: mod.ModManifest,
                name: () => mod.Helper.Translation.Get ( "tool-radius" ),
                tooltip: () => mod.Helper.Translation.Get ( "tool-r-tooltip" ),
                getValue: () => ModEntry.config.cRadius,
                setValue: value => ModEntry.config.cRadius = value,
                min: 0,
                max: ModEntry.toolMax,
                interval: 1
            );
        }
    }
}
