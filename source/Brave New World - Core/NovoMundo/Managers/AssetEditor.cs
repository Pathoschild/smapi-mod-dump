/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace NovoMundo.Managers
{
    public class Asset_Editor
    {
        public void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add(new KeyValuePair<string, string>("NMFarm1", "-1/-1/-1/-1/-1/-1/-1/-1/382 .05 770 .1 390 .25 330 1"));
                    data.Add(new KeyValuePair<string, string>("NMFarm2", "-1/-1/-1/-1/-1/-1/-1/-1/382 .05 770 .1 390 .25 330 1"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add(new KeyValuePair<string, string>("nmQuarry", "none"));
                    data.Add(new KeyValuePair<string, string>("nmCinema", "none"));
                    data.Add(new KeyValuePair<string, string>("nmFarm", "none"));
                    data.Add(new KeyValuePair<string, string>("nmLake", "none"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add(new KeyValuePair<string, string>("nmATM", $"adult/polite/neutral/neutral/male/non-datable/null/Other/null//Custom_nmnpctroom 15 14/{ModEntry.ModHelper.Translation.Get("NPCDataATMName")}"));
                    data.Add(new KeyValuePair<string, string>("nmMorris", $"adult/polite/neutral/neutral/male/non-datable/null/Other/null//Custom_nmnpctroom 14 14/{ModEntry.ModHelper.Translation.Get("NPCDataMorrisName")}"));
                    data.Add(new KeyValuePair<string, string>("nmBuilder1", "adult/polite/neutral/neutral/male/non-datable/null/Other/null//Custom_nmnpctroom 13 14/Builder"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Portraits/nmATM"))
            {
                e.LoadFrom(() => ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/nmATMportraitsbrown.png"), AssetLoadPriority.Low);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Portraits/nmMorris"))
            {
                e.LoadFrom(() => ModEntry.ModHelper.GameContent.Load<Texture2D>("Portraits/Morris"),  AssetLoadPriority.Low);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Portraits/nmBuilder1"))
            {
                e.LoadFrom(() => ModEntry.ModHelper.GameContent.Load<Texture2D>("Portraits/Robin"), AssetLoadPriority.Low);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/nmMorris") || e.NameWithoutLocale.IsEquivalentTo("Characters/nmATM"))
            {
                e.LoadFrom(() => ModEntry.ModHelper.GameContent.Load<Texture2D>("Characters/Morris"), AssetLoadPriority.Low);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/nmBuilder1"))
            {
                e.LoadFrom(() => ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/nmBuilder1.png"), AssetLoadPriority.Low);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/nmBuilder1"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add(new KeyValuePair<string, string>("spring", "610 Custom_nmnpctroom 13 13 1"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/AntiSocialNPCs"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add(new KeyValuePair<string, string>("nmATM", "true"));
                    data.Add(new KeyValuePair<string, string>("nmMorris", "true"));
                    data.Add(new KeyValuePair<string, string>("nmBuilder1", "true"));
                });
            }
        }
    }
}




