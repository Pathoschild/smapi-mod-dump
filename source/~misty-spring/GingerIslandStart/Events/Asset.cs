/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using GingerIslandStart.Additions;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Weapons;

namespace GingerIslandStart.Events;

public static class Asset
{
    private static string Id => ModEntry.Id;
    private static string Translate(string msg) => ModEntry.Help.Translation.Get(msg, new{ playerName = Game1.player.displayName});
    
    /// <summary>
    /// Changes the island south map.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <see cref="StardewValley.Locations.BoatTunnel"/>
    /// <see cref="StardewValley.Locations.IslandSouth"/>
    internal static void Requested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.BaseName.Equals("Maps/WillysBoat"))
        {
            e.LoadFrom(() => Game1.content.Load<Texture2D>("LooseSprites/WillysBoat"), AssetLoadPriority.Low);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Data/Weapons"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string,WeaponData>();
                var extraWeapons = Weapons.Create();

                foreach (var weapon in extraWeapons)
                {
                    editor.Data.Add(weapon);
                }
            });
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Strings/Weapons"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string,string>();
                var extraWeapons = Weapons.GetNames();

                foreach (var weapon in extraWeapons)
                {
                    editor.Data.Add(weapon);
                }
            });
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Strings/Objects"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string,string>();
                editor.Data.TryAdd("GISurvivalFish_Name", Translate("SurvivalFish_Name"));
                editor.Data.TryAdd("GISurvivalFish_Description", Translate("SurvivalFish_Desc"));
            });
        }

        if (e.NameWithoutLocale.BaseName.Equals("Data/Events/IslandSouthEastCave"))
        {
            e.LoadFrom( () => new Dictionary<string,string>(), AssetLoadPriority.Low);
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string, string>();
                var msg = Translate("BackpackUpgraded");
                var eventData = $"continue/-100 -100/farmer 27 7 2/pause 1500/playSound clank/pause 900/playSound clank/pause 200/playSound throw/pause 200/playSound clank/pause 300/playSound shadowHit/pause 200/playSound slingshot/pause 700/playSound shadowHit/pause 200/playSound clank/pause 1000/playSound getNewSpecialItem/message \"{msg}\"/end";
                editor.Data.Add("GingerIslandStart_UpgradePackage", eventData);
            });
        }
        
        if (Game1.player == null)
            return;

        if (e.NameWithoutLocale.BaseName.Equals("Data/Events/IslandSouth"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string, string>();
                var zero = Translate("Intro_0");
                var first = Translate("Intro_1");
                var second = Translate("Intro_2");
                var third = Translate("Intro_3");
                var fourth = Translate("Intro_4");
                
                var introEvent = $"ocean/-100 -100/farmer 20 31 2/pause 1500/showFrame 5/message \"{zero}\"/pause 1000/message \"{first}\"/pause 1200/message \"{second}\"/pause 100/viewport 20 31/pause 200/pause 1000/showFrame 4/pause 100/showFrame 0/pause 700/jump farmer/pause 500/faceDirection farmer 1/pause 200/faceDirection farmer 3/pause 200/faceDirection farmer 2/pause 1000/message \"{third}\"/pause 500/emote farmer 16/pause 700/message \"{fourth}\"/pause 500/emote farmer 40/pause 1000/end";
                
                editor.Data.Add($"GingerIslandStart_AltIntro/n {Id}_gotGiftWarped", introEvent);
            });
        }

        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;

        if (e.NameWithoutLocale.BaseName.Equals("Data/Shops"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsDictionary<string,ShopData>();
                
                var dwarfShop = ResourceShop.Create();
                editor.Data.Add($"{Id}_Dwarf", dwarfShop);

                var pirateShop = PirateShop.Create();
                editor.Data.Add($"{Id}_Pirate", pirateShop);
                
                var pirateRecovery = PirateShop.CreateRecoveryShop();
                editor.Data.Add($"{Id}_PirateRecovery", pirateRecovery);

                if (editor.Data.TryGetValue("IslandTrade", out var islandTrader) && !Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
                {
                    var shopExtra = ModEntry.ShopMultiplier switch
                    {
                        0.5 => 0,
                        2 => 2,
                        _ => 1
                    };
                    
                    islandTrader.Items.Add(new ShopItemData { Id = "Sugar", ItemId = "(O)245", TradeItemId = "(O)832", TradeItemAmount = 1 + shopExtra});
                    islandTrader.Items.Add(new ShopItemData { Id = "Milk 50%", ItemId = "(O)184", TradeItemId = "(O)404", TradeItemAmount = 1 + shopExtra, Condition = "SYNCED_RANDOM day halfchance 0.5", AvoidRepeat = true});
                    islandTrader.Items.Add(new ShopItemData { Id = "Milk", ItemId = "(O)184", TradeItemId = "(O)834", TradeItemAmount = 1 + shopExtra, AvoidRepeat = true});
                    islandTrader.Items.Add(new ShopItemData { Id = "Truffle oil", ItemId = "(O)155", TradeItemId = "(O)832", TradeItemAmount = 1 + shopExtra, Condition = "PLAYER_FORAGING_LEVEL Current 9"});
                }
                
                if(!editor.Data.TryGetValue("VolcanoShop", out var volcanoShop))
                    return;

                var newItem = new ShopItemData
                {
                    Id = "GeodeCrusher",
                    ItemId = "(BC)182",
                    TradeItemId = "(O)336",
                    TradeItemAmount = 2,
                    Price = (int)(5000 * ModEntry.ShopMultiplier),
                    AvailableStock = 1,
                    UseObjectDataPrice = false,
                    CustomFields = new Dictionary<string, string>
                    {
                        { ItemExtensions.Additions.ModKeys.ExtraTradesKey, "(O)390 50, (O)72 1" }
                    },
                    Condition = "!PLAYER_STAT Current geodesCracked 1"
                };

                volcanoShop.Items.Insert(0, newItem);
            });
        }

        if (e.NameWithoutLocale.BaseName.Equals("Maps/Island_Resort"))
        {
            if (Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
                return;
            
            e.Edit(asset =>
            {
                var editor = asset.AsMap();

                var back = editor.Data.GetLayer("Buildings");
                back.Tiles[14, 7].Properties.Remove("Action");
            });
        }

        if (!e.NameWithoutLocale.BaseName.Equals("Maps/Island_S"))
            return;
        
        e.Edit(asset =>
        {
            var editor = asset.AsMap();
            
            if (Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
                return;
            
            var back = editor.Data.GetLayer("Back");
            back.Tiles[19, 43].Properties.Remove("TouchAction");
            back.Tiles[20,44].Properties.Add("Passable", "T");
            back.Tiles[22,42].Properties.Add("Passable", "T");

            Location.NeedsEdit = true;
        });
    }
}