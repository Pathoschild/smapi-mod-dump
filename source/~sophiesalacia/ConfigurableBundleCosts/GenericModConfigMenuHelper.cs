/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using StardewValley.Menus;

namespace ConfigurableBundleCosts;

internal class GenericModConfigMenuHelper
{

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            mod: Globals.Manifest,
            reset: () => Globals.Config = new ModConfig(),
            save: () => Globals.Helper.WriteConfig(Globals.Config)
        );

        /* Joja section */

        Globals.GmcmApi.AddBoolOption(
            mod: Globals.Manifest,
            name: () => "Enabled: Joja",
            tooltip: () => "If disabled, vanilla costs will be used.",
            getValue: () => Globals.Config.Joja.ApplyValues,
            setValue: val => Globals.Config.Joja.ApplyValues = val
        );

        Globals.GmcmApi.AddSectionTitle(
            mod: Globals.Manifest,
            text: () => "Joja"
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Membership Cost",
            tooltip: () => "Cost to purchase a Joja membership",
            getValue: () => Globals.Config.Joja.MembershipCost,
            setValue: val => Globals.Config.Joja.MembershipCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Movie Theater Cost",
            tooltip: () => "Cost to purchase the Movie Theater from Joja",
            getValue: () => Globals.Config.Joja.MovieTheaterCost,
            setValue: val => Globals.Config.Joja.MovieTheaterCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Bus Cost",
            tooltip: () => "Cost to repair the bus to Calico Desert",
            getValue: () => Globals.Config.Joja.BusCost,
            setValue: val => Globals.Config.Joja.BusCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Minecarts Cost",
            tooltip: () => "Cost to repair the minecart system around town",
            getValue: () => Globals.Config.Joja.MinecartsCost,
            setValue: val => Globals.Config.Joja.MinecartsCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Bridge Cost",
            tooltip: () => "Cost to repair the bridge to the quarry",
            getValue: () => Globals.Config.Joja.BridgeCost,
            setValue: val => Globals.Config.Joja.BridgeCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Greenhouse Cost",
            tooltip: () => "Cost to repair the greenhouse on the farm",
            getValue: () => Globals.Config.Joja.GreenhouseCost,
            setValue: val => Globals.Config.Joja.GreenhouseCost = val
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Panning Cost",
            tooltip: () => "Cost to remove the glittering boulder on the mountain",
            getValue: () => Globals.Config.Joja.PanningCost,
            setValue: val => Globals.Config.Joja.PanningCost = val
        );

        /* Vault section */
        
        Globals.GmcmApi.AddParagraph(
            mod: Globals.Manifest,
            text: () => "\n");

        Globals.GmcmApi.AddBoolOption(
            mod: Globals.Manifest,
            name: () => "Enabled: Vault",
            tooltip: () => "If disabled, vanilla costs will be used.",
            getValue: () => Globals.Config.Vault.ApplyValues,
            setValue: val => Globals.Config.Vault.ApplyValues = val
        );

        Globals.GmcmApi.AddSectionTitle(
            mod: Globals.Manifest,
            text: () => "Vault"
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Vault Bundle 1 Cost",
            tooltip: () => "Cost of Vault Bundle 1",
            getValue: () => Globals.Config.Vault.Bundle1,
            setValue: val => Globals.Config.Vault.Bundle1 = val,
            fieldId: "bundle1"
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Vault Bundle 2 Cost",
            tooltip: () => "Cost of Vault Bundle 2",
            getValue: () => Globals.Config.Vault.Bundle2,
            setValue: val => Globals.Config.Vault.Bundle2 = val,
            fieldId: "bundle2"
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Vault Bundle 3 Cost",
            tooltip: () => "Cost of Vault Bundle 3",
            getValue: () => Globals.Config.Vault.Bundle3,
            setValue: val => Globals.Config.Vault.Bundle3 = val,
            fieldId: "bundle3"
        );

        Globals.GmcmApi.AddNumberOption(
            mod: Globals.Manifest,
            name: () => "Vault Bundle 4 Cost",
            tooltip: () => "Cost of Vault Bundle 4",
            getValue: () => Globals.Config.Vault.Bundle4,
            setValue: val => Globals.Config.Vault.Bundle4 = val,
            fieldId: "bundle4"
        );

        Globals.GmcmApi.OnFieldChanged(
            mod: Globals.Manifest,
            onChange: UpdateBundleDataIfNecessary
        );
    }

    private static void UpdateBundleDataIfNecessary(string fieldId, object newValue)
    {
        Log.Trace("updating bundle data");
        BundleManager.CheckBundleData();
    }
}
