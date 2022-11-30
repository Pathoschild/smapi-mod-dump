/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BlueberryMushroomMachine.Editors
{
    internal static class EventsEditor
    {
        public static bool ApplyEdit(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(@"Characters/Dialogue/Robin"))
            {
                e.Edit((asset) =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    const string key = "event.4637.0000.0000";
                    if (!data.ContainsKey(key))
                        data.Add(key, ModEntry.Instance.i18n.Get(key));
                });
                return true;
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(@"Data/Events/Farm"))
            {
                e.Edit(ApplyEventEdit);
                return true;
            }
            return false;
        }

        public static void ApplyEventEdit(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            var json = ModEntry.Instance.Helper.ModContent.Load<IDictionary<string, string>>
                (ModValues.EventsPath);

            foreach (var key in json.Keys)
            {
                if (key.StartsWith("46370001"))
                {
                    if (Game1.player.HouseUpgradeLevel >= 3)
                    {
                        Log.D("Event conditions:" +
                              $" disabled=[{ModEntry.Instance.Config.DisabledForFruitCave}]" +
                              $" caveChoice=[{Game1.MasterPlayer.caveChoice}]",
                            ModEntry.Instance.Config.DebugMode);
                        if (ModEntry.Instance.Config.DisabledForFruitCave
                            && Game1.MasterPlayer.caveChoice.Value != 2)
                            return;

                        if (!data.ContainsKey(key))
                        {
                            var value = string.Format(
                                json[key],
                                ModEntry.Instance.i18n.Get("event.4637.0001.0000"),
                                ModEntry.Instance.i18n.Get("event.4637.0001.0001"),
                                ModEntry.Instance.i18n.Get("event.4637.0001.0002"),
                                ModEntry.Instance.i18n.Get("event.4637.0001.0003"),
                                ModValues.PropagatorInternalName);
                            Log.D($"Injecting event.",
                                ModEntry.Instance.Config.DebugMode);
                            data.Add(key, value);
                        }
                    }
                }
            }
        }
    }
}
