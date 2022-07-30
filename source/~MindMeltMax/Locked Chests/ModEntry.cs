/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using LockChests.Patches;
using LockChests.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Linq;
using System.Collections.Generic;

using SObject = StardewValley.Object;

namespace LockChests
{
    public class ModEntry : Mod
    {
        public static ITranslationHelper ITranslations;
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            ITranslations = Helper.Translation;
            IConfig = Helper.ReadConfig<Config>();

            Helper.Events.Display.MenuChanged += onMenuChanged;
            Helper.Events.Input.ButtonPressed += onButtonDown;
            Helper.Events.Multiplayer.PeerConnected += onPlayerJoin;

            Patcher.Patch();
        }

        private void onMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is not null and ItemGrabMenu grabMenu)
            {
                Item? source = Helper.Reflection.GetField<Item>(grabMenu, "sourceItem").GetValue();
                if (source is not null and Chest c && c.playerChest.Value && c.modData.ContainsKey("LockedChests.Owner") && c.modData.ContainsKey("LockedChests.Lock") && c.modData.ContainsKey("LockedChests.AccessIds"))
                {
                    long owner = Convert.ToInt64(c.modData["LockedChests.Owner"]);
                    Lock chestLock = Enum.Parse<Lock>(c.modData["LockedChests.Lock"]);
                    List<long> accessIds = Json.Read<List<long>>(c.modData["LockedChests.AccessIds"]) ?? new List<long>() { Game1.player.UniqueMultiplayerID };
                    if (owner != Game1.player.UniqueMultiplayerID)
                        if (chestLock == Lock.ReadOnly && !accessIds.Any(x => x == Game1.player.UniqueMultiplayerID))
                            grabMenu.inventory.highlightMethod = grabMenu.ItemsToGrabMenu.highlightMethod = (i) => false;
                }
            }
        }

        private void onButtonDown(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove) return;
            if (IConfig.Open.Any(x => x == e.Button))
            {
                var OatT = ObjectAt(Game1.player.GetGrabTile());
                if (OatT is not null and Chest c && c.playerChest.Value)
                {
                    long owner = -1;
                    if (c.modData.ContainsKey("LockedChests.Owner"))
                        owner = Convert.ToInt64(c.modData["LockedChests.Owner"]);
                    if (owner == Game1.player.UniqueMultiplayerID || owner == -1)
                        Game1.activeClickableMenu = new OwnerSettingsMenu(c);
                    else
                        Game1.activeClickableMenu = new DialogueBox(string.Format(ITranslations.Get("No_Access_Dialogue"), Game1.getFarmerMaybeOffline(owner).Name));
                }
            }
        }

        private void onPlayerJoin(object? sender, PeerConnectedEventArgs e)
        {
            if (e.Peer.Mods is null || !e.Peer.Mods.Any(x => x.ID == Helper.ModRegistry.ModID))
                Game1.chatBox.addErrorMessage($"{Game1.getFarmer(e.Peer.PlayerID).Name} does not have Locked Chests installed and will be able to access all chests");
            if (Game1.activeClickableMenu is OwnerSettingsMenu menu)
                menu.updateLayout();
        }

        private SObject ObjectAt(Vector2 pos) => Game1.currentLocation.Objects.Pairs.FirstOrDefault(x => x.Key == pos).Value;

        internal static void TransferOwner(Farmer newOwner, Chest current)
        {
            current.modData["LockedChests.Owner"] = $"{newOwner.UniqueMultiplayerID}";
            if (IConfig.LockOnTransfer)
                current.modData["LockedChests.Lock"] = $"{Lock.Locked}";
            current.modData["LockedChests.AccessIds"] = $"[{newOwner.UniqueMultiplayerID}]";
        }
    }

    public enum Lock
    {
        Open,
        ReadOnly,
        Locked
    }
}
