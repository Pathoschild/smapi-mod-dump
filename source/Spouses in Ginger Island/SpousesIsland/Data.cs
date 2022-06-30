/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class SGIData
    {
        internal static void DialoguesInternational(AssetRequestedEventArgs e, ModConfig Config)
        {
            //ALL translation keys are placeholders
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Abigail") && Config.Allow_Abigail == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Abigail.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Abigail.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Abigail.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Alex") && Config.Allow_Alex == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Alex.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Alex.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Alex.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Elliott") && Config.Allow_Elliott == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Elliott.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Elliott.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Elliott.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Emily") && Config.Allow_Emily == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Emily.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Emily.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Emily.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Haley") && Config.Allow_Haley == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Haley.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Haley.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Haley.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Harvey") && Config.Allow_Harvey == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Harvey.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Harvey.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Harvey.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Krobus") && Config.Allow_Krobus == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Krobus.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Krobus.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Krobus.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Leah") && Config.Allow_Leah == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Leah.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Leah.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Leah.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Maru") && Config.Allow_Maru == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Maru.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Maru.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Maru.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Penny") && Config.Allow_Penny == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Penny.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Penny.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Penny.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sam") && Config.Allow_Sam == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sam.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sam.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sam.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sebastian") && Config.Allow_Sebastian == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sebastian.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sebastian.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sebastian.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Shane") && Config.Allow_Shane == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Shane.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Shane.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Shane.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Claire") && Config.Allow_Claire == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Claire.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Claire.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Claire.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Lance") && Config.Allow_Lance == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Lance.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Lance.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Lance.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Wizard") && Config.Allow_Magnus == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Wizard.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Wizard.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Wizard.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Olivia") && Config.Allow_Olivia == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Olivia.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Olivia.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Olivia.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sophia") && Config.Allow_Sophia == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sophia.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sophia.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Sophia.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Victor") && Config.Allow_Victor == true)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = ModEntry.ModHelper.Translation.Get("Dialogue.Victor.arrival");
                    data["marriage_loc1"] = ModEntry.ModHelper.Translation.Get("Dialogue.Victor.LocA");
                    data["marriage_loc3"] = ModEntry.ModHelper.Translation.Get("Dialogue.Victor.LocB");
                });
            }

        }
        internal static bool IsSpouseEnabled(string name, ModConfig Config)
        {
            switch (name)
            {
                case "Abigail":
                    return Config.Allow_Abigail;
                case "Alex":
                    return Config.Allow_Alex;
                case "Elliott":
                    return Config.Allow_Elliott;
                case "Emily":
                    return Config.Allow_Emily;
                case "Haley":
                    return Config.Allow_Haley;
                case "Harvey":
                    return Config.Allow_Harvey;
                case "Krobus":
                    return Config.Allow_Krobus;
                case "Leah":
                    return Config.Allow_Leah;
                case "Maru":
                    return Config.Allow_Maru;
                case "Penny":
                    return Config.Allow_Penny;
                case "Sam":
                    return Config.Allow_Sam;
                case "Sebastian":
                    return Config.Allow_Sebastian;
                case "Shane":
                    return Config.Allow_Shane;
                case "Claire":
                    return Config.Allow_Claire;
                case "Lance":
                    return Config.Allow_Lance;
                case "Olivia":
                    return Config.Allow_Olivia;
                case "Sophia":
                    return Config.Allow_Sophia;
                case "Victor":
                    return Config.Allow_Victor;
                case "Wizard":
                    return Config.Allow_Magnus;
                default:
                    return false;
            }
        }
    }
}