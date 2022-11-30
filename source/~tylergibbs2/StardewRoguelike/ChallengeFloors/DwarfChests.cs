/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewRoguelike.VirtualProperties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike.ChallengeFloors
{
    internal class DwarfChests : ChallengeBase
    {
        public NetCollection<NetLong> alreadyUsed = new();

        private bool spawnedDwarf = false;

        private ChestDwarf dwarf;

        public DwarfChests() : base() { }

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields.AddFields(alreadyUsed);
        }

        public override List<string> MapPaths => new() { "custom-dwarf" };

        public override Vector2? GetSpawnLocation(MineShaft mine) => new(6, 5);

        public override bool ShouldSpawnLadder(MineShaft mine) => false;

        public void SpawnLocalChests(MineShaft mine)
        {
            MerchantFloor merchantFloor = Merchant.GetNextMerchantFloor(mine);

            Vector2 leftMostChest = new(7f, 12f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 chestSpot = new(leftMostChest.X + (i * 3), leftMostChest.Y);
                List<Item> chestItems = new() { merchantFloor.PickAnyRandom(Roguelike.FloorRng) };

                Chest chest = new(0, chestItems, chestSpot)
                {
                    Tint = Color.White
                };
                mine.overlayObjects.Add(chestSpot, chest);
            }

            Game1.playSound("axchop");
        }

        public void RemoveLocalChests(MineShaft mine)
        {
            List<Vector2> toRemove = mine.overlayObjects
                .Where(kvp => kvp.Value is Chest && (kvp.Value as Chest).frameCounter.Value != 2)
                .Select(kvp => kvp.Key).ToList();

            foreach (Vector2 v in toRemove)
            {
                Chest chest = mine.overlayObjects[v] as Chest;
                Item chestItem = chest.items[0];
                Sign sign = new(v, 39);
                sign.displayItem.Value = chestItem;
                sign.displayType.Value = chestItem is Ring ? 4 : 1;
                mine.overlayObjects[v] = sign;
            }
        }

        public override bool AnswerDialogueAction(MineShaft mine, string questionAndAnswer, string[] questionParams)
        {
            int hpNeeded = Game1.player.maxHealth / 2;

            if (questionAndAnswer == "dwarfPurchase_Yes")
            {
                if (Game1.player.health > hpNeeded)
                {
                    Game1.player.health -= hpNeeded;
                    Game1.playSound("ow");
                    SpawnLocalChests(mine);
                    alreadyUsed.Add(Game1.player.uniqueMultiplayerID);
                }
                else
                    Game1.drawObjectDialogue(I18n.Merchant_NotEnoughHP());
            }

            return false;
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            int chestCount = 0;
            foreach (SObject obj in mine.overlayObjects.Values)
            {
                if (obj is Chest)
                    chestCount++;
            }
            if (chestCount != 0 && chestCount < 3)
                RemoveLocalChests(mine);

            if (!Context.IsMainPlayer)
                return;

            if (!spawnedDwarf)
            {
                Texture2D portrait = Game1.content.Load<Texture2D>("Portraits/Dwarf");
                dwarf = new(mine, new(10, 15), portrait);
                mine.addCharacter(dwarf);
                spawnedDwarf = true;
            }
        }
    }

    internal class ChestDwarf : NPC
    {
        public ChestDwarf() : base() { }

        public ChestDwarf(GameLocation location, Vector2 tileLocation, Texture2D portrait) : base(new AnimatedSprite($"Characters/Dwarf", 0, 16, 24), tileLocation * 64, location.Name, 0, "ChestChoiceNPC", null, portrait, eventActor: false) { }

        public NetCollection<NetLong> GetUsedFarmers()
        {
            if (currentLocation is not MineShaft mine)
                throw new Exception("ChestDwarf is not in a MineShaft.");

            DwarfChests challenge = (DwarfChests)mine.get_MineShaftChallengeFloor().Value;
            return challenge.alreadyUsed;
        }

        public override bool checkAction(Farmer who, GameLocation location)
        {
            if (GetUsedFarmers().Contains(who.uniqueMultiplayerID))
                return false;

            int hpNeeded = who.maxHealth / 2;
            location.createQuestionDialogue(StardewValley.Dialogue.convertToDwarvish("I have some items for sale. Do you want to buy one for ") + $"{hpNeeded} HP?", location.createYesNoResponses(), "dwarfPurchase");

            return true;
        }
    }
}
