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
using Netcode;
using StardewModdingAPI;
using StardewRoguelike.Extensions;
using StardewRoguelike.Patches;
using StardewRoguelike.TerrainFeatures;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewRoguelike.ChallengeFloors
{
    internal class PickAPath : ChallengeBase
    {
        private enum RoomType
        {
            Merchant = 0,
            HealingStatue = 1,
            Monsters = 2,
            CurseTeller = 3,
            Wheel = 4,
            JojaCola = 5,
            Cauldron = 6
        }

        public override List<string> MapPaths => new() { "custom-pickapath" };

        public override Vector2? GetSpawnLocation(MineShaft mine) => new(6, 49);

        private readonly NetEnum<RoomType> InitializedRoomType = new();

        private readonly NetEnum<RoomType> InaccessibleRoomType = new();

        private readonly NetBool IsLeftSide = new();

        private readonly NetBool ShouldSpawnRooms = new();

        private MineShaft Location = null!;

        private DwarfGate gateLeft = null!;

        private DwarfGate gateRight = null!;

        private GambaMenu? gambaMenu = null;

        private bool initialized = false;

        private bool initializedOtherRoom = false;

        private bool canSpawnLadder = false;

        private bool spawnedLadder = false;

        private int tickCounter = 0;

        public CurseType? CurseToAdd { get; private set; }

        public ShopMenu CurrentShop { get; private set; } = null!;

        public PickAPath() : base() { }

        protected override void InitNetFields()
        {
            base.InitNetFields();
            NetFields.AddFields(InitializedRoomType, InaccessibleRoomType, IsLeftSide, ShouldSpawnRooms);
        }

        public void OnGateOpen(Point point)
        {
            foreach (DwarfGate gate in Location.get_MineShaftDwarfGates())
                gate.get_DwarfGateDisabled().Value = true;
        }

        public override void Initialize(MineShaft mine)
        {
            Location = mine;

            var validRoomTypes = new List<RoomType>((IEnumerable<RoomType>)Enum.GetValues(typeof(RoomType)));

            RoomType otherRoom = validRoomTypes[Roguelike.FloorRng.Next(validRoomTypes.Count)];

            if (!Context.IsMultiplayer && Game1.player.health == Game1.player.maxHealth)
                validRoomTypes.Remove(RoomType.HealingStatue);
            if (!Context.IsMultiplayer && !Curse.HasAnyCurse())
                validRoomTypes.Remove(RoomType.Cauldron);

            RoomType roomType;
            do
            {
                roomType = validRoomTypes[Roguelike.FloorRng.Next(validRoomTypes.Count)];
            } while (roomType == otherRoom);

            InitializedRoomType.Value = roomType;
            InaccessibleRoomType.Value = otherRoom;

            gateLeft = mine.CreateDwarfGate(1, new(15, 37), new(15, 40));
            gateRight = mine.CreateDwarfGate(1, new(24, 37), new(24, 40));

            gateLeft.pressEvent.onEvent += OnGateOpen;
            gateLeft.pressEvent.onEvent += InitializeRoomLeftSide;
            gateRight.pressEvent.onEvent += OnGateOpen;
            gateRight.pressEvent.onEvent += InitializeRoomRightSide;
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Location = mine;

            CurseToAdd = Curse.GetRandomUniqueCurse(Roguelike.FloorRng);
            CurrentShop = new(Merchant.GetMerchantStock(0.5f, Roguelike.FloorRng), context: "Blacksmith", on_purchase: OpenShopPatch.OnPurchase);
            CurrentShop.setUpStoreForContext();
        }

        private void SpawnRoomType(RoomType roomType, MineShaft mine, Point originPoint)
        {
            switch (roomType)
            {
                case RoomType.Merchant:
                    SpawnMerchant(mine, originPoint);
                    break;
                case RoomType.HealingStatue:
                    SpawnHealingStatue(mine, originPoint);
                    break;
                case RoomType.CurseTeller:
                    SpawnCurseTeller(mine, originPoint);
                    break;
                case RoomType.Wheel:
                    SpawnWheel(mine, originPoint);
                    break;
                case RoomType.JojaCola:
                    SpawnJojaCola(mine, originPoint);
                    break;
            }
        }

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return canSpawnLadder;
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!initialized && ShouldSpawnRooms.Value)
            {
                Point originPoint = IsLeftSide.Value ? new(8, 7) : new(27, 7);
                SpawnRoomType(InitializedRoomType.Value, mine, originPoint);

                initialized = true;
            }
            else if (!initializedOtherRoom && ShouldSpawnRooms.Value)
            {
                Point originPoint = !IsLeftSide.Value ? new(8, 7) : new(27, 7);
                SpawnRoomType(InaccessibleRoomType.Value, mine, originPoint);

                initializedOtherRoom = true;
            }

            if (!Context.IsMainPlayer || !initialized || !initializedOtherRoom)
                return;

            tickCounter++;
            if (tickCounter > 60)
                tickCounter = 0;

            if (InitializedRoomType.Value == RoomType.Monsters && !spawnedLadder && tickCounter == 0)
            {
                if (mine.EnemyCount == 0)
                    SpawnLadder();
            }
            else if (InitializedRoomType.Value != RoomType.Monsters && !spawnedLadder)
                SpawnLadder();
        }

        public override bool PerformAction(MineShaft mine, string action, Farmer who, Location tileLocation)
        {
            if (action is not null && who.IsLocalPlayer)
            {
                string[] actionParams = action.Split(' ');
                if (actionParams[0] == "GambaWheel")
                {
                    gambaMenu ??= new GambaMenu();
                    Game1.activeClickableMenu = gambaMenu;
                    return true;
                }
                else if (actionParams[0] == "HealingBear")
                {
                    Game1.player.health = Game1.player.maxHealth;
                    Game1.playSound("yoba");
                    return true;
                }
                else if (actionParams[0] == "CursesDiscounted" && !mine.get_MineShaftUsedFortune())
                {
                    if (Curse.HasAllCurses() || !CurseToAdd.HasValue)
                    {
                        Game1.drawObjectDialogue("You have every otherworldly power known to mankind.");
                        return true;
                    }

                    int hpNeeded = 10;
                    int goldNeeded = 350;

                    var responses = new Response[3]
                    {
                    new Response("YesGold", $"Yes [{goldNeeded}G]"),
                    new Response("YesHP", $"Yes [{hpNeeded} Max HP]"),
                    new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
                    };

                    mine.createQuestionDialogue("Would you like me to grant you an otherworldly ability for a *super* low price?", responses, "discountedCursePurchase");
                    return true;
                }
            }

            return false;
        }

        public override bool AnswerDialogueAction(MineShaft mine, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer.StartsWith("discountedCursePurchase"))
            {
                int hpNeeded = 10;
                int goldNeeded = 350;

                bool paid = false;

                if (questionAndAnswer == "discountedCursePurchase_YesHP")
                {
                    if (Game1.player.maxHealth > hpNeeded)
                    {
                        Roguelike.TrueMaxHP -= hpNeeded;
                        paid = true;
                    }
                    else
                        Game1.drawObjectDialogue("You do not have enough HP.");
                }
                else if (questionAndAnswer == "discountedCursePurchase_YesGold")
                {
                    if (Game1.player.Money >= goldNeeded)
                    {
                        Game1.player.Money -= goldNeeded;
                        paid = true;
                    }
                    else
                        Game1.drawObjectDialogue("You do not have enough money.");
                }

                if (paid && CurseToAdd is not null)
                {
                    Curse.AddCurse(CurseToAdd.Value);
                    Game1.playSound("debuffSpell");
                    mine.set_MineShaftUsedFortune(true);
                }
            }

            return false;
        }

        private void SpawnLadder()
        {
            canSpawnLadder = true;

            if (IsLeftSide.Value)
                Location.createLadderAt(new(6, 14));
            else
                Location.createLadderAt(new(34, 14));

            spawnedLadder = true;
        }

        public void InitializeRoomLeftSide(Point point)
        {
            IsLeftSide.Value = true;
            InitializeRooms();
        }

        public void InitializeRoomRightSide(Point point)
        {
            InitializeRooms();
        }

        public void InitializeRooms()
        {
            if (InitializedRoomType.Value == RoomType.Monsters)
                SpawnMonsters(Location, IsLeftSide.Value ? new(8, 7) : new(27, 7));
            else if (InaccessibleRoomType.Value == RoomType.Monsters)
                SpawnMonsters(Location, !IsLeftSide.Value ? new(8, 7) : new(27, 7));

            if (InitializedRoomType.Value == RoomType.Cauldron)
                SpawnCauldron(Location, IsLeftSide.Value ? new(8, 7) : new(27, 7));
            else if (InaccessibleRoomType.Value == RoomType.Cauldron)
                SpawnCauldron(Location, !IsLeftSide.Value ? new(8, 7) : new(27, 7));

            ShouldSpawnRooms.Value = true;
        }

        public void SpawnMerchant(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.SetTile(xOrigin + 1, yOrigin - 1, "Front", "townInterior", 885);
            mine.SetTile(xOrigin + 4, yOrigin - 1, "Front", "townInterior", 1042);
            mine.SetTile(xOrigin + 2, yOrigin, "Front", "townInterior", 918);
            mine.SetTile(xOrigin + 3, yOrigin, "Front", "townInterior", 919);

            mine.SetTile(xOrigin + 1, yOrigin, "Buildings", "townInterior", 917);
            mine.SetTile(xOrigin + 4, yOrigin, "Buildings", "townInterior", 1074);
            mine.SetTile(xOrigin + 1, yOrigin + 1, "Buildings", "townInterior", 1104);
            mine.SetTile(xOrigin + 2, yOrigin + 1, "Buildings", "townInterior", 1105);
            mine.SetTile(xOrigin + 3, yOrigin + 1, "Buildings", "townInterior", 1105);
            mine.SetTile(xOrigin + 4, yOrigin + 1, "Buildings", "townInterior", 1106);

            mine.setTileProperty(xOrigin + 2, yOrigin + 1, "Buildings", "Action", "Buy RoguelikeDiscounted");
            mine.setTileProperty(xOrigin + 3, yOrigin + 1, "Buildings", "Action", "Buy RoguelikeDiscounted");

            foreach (Character character in mine.characters)
            {
                if (character.Name == "Marlon")
                    return;
            }

            NPC marlon = Game1.getCharacterFromName("Marlon");
            marlon.setTileLocation(new Vector2(xOrigin + 3, yOrigin));
            mine.addCharacter(marlon);
        }

        public void SpawnHealingStatue(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.SetTile(xOrigin + 2, yOrigin + 5, "Buildings", "townInterior", 1472);
            mine.SetTile(xOrigin + 3, yOrigin + 5, "Buildings", "townInterior", 1473);
            mine.SetTile(xOrigin + 2, yOrigin + 4, "Buildings", "townInterior", 1440);
            mine.SetTile(xOrigin + 3, yOrigin + 4, "Buildings", "townInterior", 1441);
            mine.SetTile(xOrigin + 2, yOrigin + 3, "Front", "townInterior", 1408);
            mine.SetTile(xOrigin + 3, yOrigin + 3, "Front", "townInterior", 1409);
            mine.SetTile(xOrigin + 2, yOrigin + 2, "Front", "townInterior", 1376);
            mine.SetTile(xOrigin + 3, yOrigin + 2, "Front", "townInterior", 1377);

            mine.setTileProperty(xOrigin + 2, yOrigin + 5, "Buildings", "Action", "HealingBear");
            mine.setTileProperty(xOrigin + 3, yOrigin + 5, "Buildings", "Action", "HealingBear");
        }

        public void SpawnMonsters(MineShaft mine, Point topLeftTile)
        {
            if (!Context.IsMainPlayer)
                return;

            int monstersToSpawn = 8;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                monstersToSpawn += 3;

            while (monstersToSpawn > 0)
            {
                Vector2 tileLocation = new(
                    topLeftTile.X + Game1.random.Next(0, 6),
                    topLeftTile.Y + Game1.random.Next(0, 6)
                );
                Monster monster = mine.BuffMonsterIfNecessary(mine.getMonsterForThisLevel(mine.mineLevel, (int)tileLocation.X, (int)tileLocation.Y));
                Roguelike.AdjustMonster(mine, ref monster);

                mine.characters.Add(monster);
                monstersToSpawn--;
            }
        }

        public void SpawnCauldron(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.terrainFeatures.Add(new(xOrigin + 1, yOrigin + 3), new CleansingCauldron());
        }

        public void SpawnCurseTeller(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.SetTile(xOrigin + 2, yOrigin + 1, "Front", "z_Festivals", 473);
            mine.SetTile(xOrigin + 3, yOrigin + 1, "Front", "z_Festivals", 474);
            mine.SetTile(xOrigin + 2, yOrigin + 2, "Buildings", "z_Festivals", 505);
            mine.SetTile(xOrigin + 3, yOrigin + 2, "Buildings", "z_Festivals", 506);

            mine.setTileProperty(xOrigin + 2, yOrigin + 2, "Buildings", "Action", "CursesDiscounted");
            mine.setTileProperty(xOrigin + 3, yOrigin + 2, "Buildings", "Action", "CursesDiscounted");
        }

        public void SpawnWheel(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.SetTile(xOrigin + 1, yOrigin + 2, "Front", "z_Festivals", 276);
            mine.SetTile(xOrigin + 2, yOrigin + 2, "Front", "z_Festivals", 29);
            mine.SetTile(xOrigin + 3, yOrigin + 2, "Front", "z_Festivals", 30);

            mine.SetTile(xOrigin + 1, yOrigin + 3, "Buildings", "z_Festivals", 308);
            mine.SetTile(xOrigin + 2, yOrigin + 3, "Buildings", "z_Festivals", 61);
            mine.SetTile(xOrigin + 3, yOrigin + 3, "Buildings", "z_Festivals", 62);

            mine.setTileProperty(xOrigin + 2, yOrigin + 3, "Buildings", "Action", "GambaWheel");
            mine.setTileProperty(xOrigin + 3, yOrigin + 3, "Buildings", "Action", "GambaWheel");
        }

        public void SpawnJojaCola(MineShaft mine, Point topLeftTile)
        {
            var (xOrigin, yOrigin) = topLeftTile;

            mine.SetTile(xOrigin + 1, yOrigin, "Buildings", "townInterior", 1476);
            mine.SetTile(xOrigin + 2, yOrigin, "Buildings", "townInterior", 1477);

            mine.SetTile(xOrigin + 1, yOrigin - 1, "Front", "townInterior", 1444);
            mine.SetTile(xOrigin + 2, yOrigin - 1, "Front", "townInterior", 1445);
            mine.SetTile(xOrigin + 1, yOrigin - 2, "Front", "townInterior", 1412);
            mine.SetTile(xOrigin + 2, yOrigin - 2, "Front", "townInterior", 1413);

            mine.SetTile(xOrigin, yOrigin - 3, "Front", "townInterior", 2017);
            mine.SetTile(xOrigin + 1, yOrigin - 3, "Front", "townInterior", 2018);
            mine.SetTile(xOrigin + 2, yOrigin - 3, "Front", "townInterior", 2019);
            mine.SetTile(xOrigin + 3, yOrigin - 3, "Front", "townInterior", 2020);
            mine.SetTile(xOrigin + 4, yOrigin - 3, "Front", "townInterior", 2021);

            mine.setTileProperty(xOrigin + 1, yOrigin, "Buildings", "Action", "ColaMachine");
            mine.setTileProperty(xOrigin + 2, yOrigin, "Buildings", "Action", "ColaMachine");
        }
    }
}
