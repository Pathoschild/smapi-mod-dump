/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Objects;

namespace ChestFeatureSet.MoveChests
{
    public class MoveChestsModule : Module
    {
        public MoveChestsModule(ModEntry modEntry) : base(modEntry) { }

        private readonly string TempChestName = "*6uQmx.a@!H=wDrF@=+=9pf=dRyaa.R5";
        private readonly Buff MoveChestsDebuff = new Buff("MoveChestsDebuff", duration: Buff.ENDLESS, effects: new BuffEffects { Speed = { -2 } }, isDebuff: true);
        private Chest? HeldChest { get; set; }

        public override void Activate()
        {
            this.IsActive = true;

            this.Events.GameLoop.DayEnding += this.OnDayEnding;

            this.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Events.Input.ButtonReleased += this.OnButtonReleased;
        }

        public override void Deactivate()
        {
            this.IsActive = false;

            this.Events.GameLoop.DayEnding -= this.OnDayEnding;

            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
            this.Events.Input.ButtonReleased -= this.OnButtonReleased;
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (this.HeldChest != null)
            {
                var playerLocation = Game1.player.currentLocation;
                int playerTileX = (int)Game1.player.Tile.X;
                int playerTileY = (int)Game1.player.Tile.Y;

                foreach (var i in new List<int> { 0, 1, -1, 2, -2, 3, -3 })
                {
                    if (playerLocation.IsTileBlockedBy(new Vector2(playerTileX + i, playerTileY + 1))
                        || playerLocation.IsTileBlockedBy(new Vector2(playerTileX + i, playerTileY))
                        || !playerLocation.isTileOnMap(new Vector2(playerTileX + i, playerTileY)))
                        continue;

                    playerLocation.setObjectAt(playerTileX + i, playerTileY, this.HeldChest);
                    break;
                }

                foreach (var item in Game1.player.Items)
                {
                    if (item.Name != this.TempChestName)
                        continue;

                    Game1.player.Items.Remove(item);
                    this.HeldChest = null;
                    break;
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == Config.MoveChestKey)
                this.Events.Input.ButtonPressed += this.OnMouseClicked;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == Config.MoveChestKey)
                this.Events.Input.ButtonPressed -= this.OnMouseClicked;
        }

        private void OnMouseClicked(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == SButton.MouseLeft)
            {
                if (Game1.player.CurrentItem != null)
                    return;

                if (this.HeldChest != null)
                {
                    Game1.showRedMessage("Can only pick up one chest.");
                    return;
                }

                var nearbyChests = ChestExtension.GetNearbyChests(Game1.player, 2);

                if (!nearbyChests.Any())
                    return;

                this.ModEntry.Helper.Input.Suppress(e.Button);

                foreach (var chest in nearbyChests)
                {
                    if (chest.TileLocation == e.Cursor.Tile)
                    {
                        this.HeldChest = chest;

                        for (int i = 0; i < Game1.player.Items.Count; i++)
                        {
                            if (Game1.player.Items[i] != null)
                                continue;

                            if (this.HeldChest.SpecialChestType is Chest.SpecialChestTypes.None)
                                Game1.player.addItemToInventory(new Chest(true), i);
                            else if (this.HeldChest.SpecialChestType is Chest.SpecialChestTypes.BigChest)
                                Game1.player.addItemToInventory(new Chest(true, "BigChest"), i);
                            else
                            {
                                this.HeldChest = null;
                                return;
                            }

                            Game1.player.Items[i].Name = this.TempChestName;
                            Game1.player.Items[i].Quality = 4;

                            this.HeldChest.Location.objects.Remove(this.HeldChest.TileLocation);

                            // LockItems
                            this.ModEntry.LockItems?.CFSItemController.AddItem(Game1.player.Items[i]);

                            this.Events.World.ObjectListChanged += this.OnObjectListChanged;

                            if (this.Config.MoveChestsDebuff)
                                Game1.player.applyBuff(this.MoveChestsDebuff);
                            return;
                        }
                    }
                }
            }
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!Context.IsPlayerFree || this.HeldChest == null)
                return;

            var chest = e.Added.Select(p => p.Value).OfType<Chest>().LastOrDefault();

            // can not get bigChest name corrected. So skip now, later fix.
            if (chest != null && (chest.Name == this.TempChestName || chest.ItemId == "BigChest"))
            {
                chest = this.CopyChestData(chest, this.HeldChest);

                this.HeldChest = null;
                this.Events.World.ObjectListChanged -= this.OnObjectListChanged;

                if (this.Config.MoveChestsDebuff)
                    Game1.player.ClearBuffs();
            }
        }

        private Chest CopyChestData(Chest newChest, Chest CopyFrom)
        {
            newChest.Name = CopyFrom.Name;
            newChest.playerChoiceColor.Value = CopyFrom.playerChoiceColor.Value;
            newChest.heldObject.Value = CopyFrom.heldObject.Value;
            newChest.MinutesUntilReady = CopyFrom.MinutesUntilReady;
            for (int i = 0; i < CopyFrom.Items.Count; i++)
            {
                if (CopyFrom.Items[i] == null) break;
                newChest.Items.Add(CopyFrom.Items[i]);
            }
            foreach (var modData in CopyFrom.modData)
                newChest.modData.CopyFrom(modData);

            // StashToChestsData
            var stashCFSController = this.ModEntry.StashToChests != null ? this.ModEntry.StashToChests.CFSChestController : null;
            if (stashCFSController != null && stashCFSController.GetChests().Contains(CopyFrom))
            {
                stashCFSController.RemoveChest(CopyFrom);
                stashCFSController.AddChest(newChest);
            }

            // CraftFromChestsData
            var craftCFSController = this.ModEntry.CraftFromChests != null ? this.ModEntry.CraftFromChests.CFSChestController : null;
            if (craftCFSController != null && craftCFSController.GetChests().Contains(CopyFrom))
            {
                craftCFSController.RemoveChest(CopyFrom);
                craftCFSController.AddChest(newChest);
            }

            return newChest;
        }
    }
}