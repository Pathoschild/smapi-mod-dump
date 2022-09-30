/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewRoguelike.Extensions;
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

        public override Vector2? SpawnLocation => new(6, 49);

        private NetInt InitializedRoomType = new(-1);

        private NetInt InaccessibleRoomType = new(-1);

        private NetBool IsLeftSide = new();

        private MineShaft Location;

        private DwarfGate gateLeft;

        private DwarfGate gateRight;

        private GambaMenu gambaMenu = null;

        private bool initialized = false;

        private bool initializedOtherRoom = false;

        private bool canSpawnLadder = false;

        private bool spawnedLadder = false;

        private int tickCounter = 0;

        public PickAPath() : base() { }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(InitializedRoomType, InaccessibleRoomType, IsLeftSide);
        }

        public void OnGateOpen(Point point)
        {
            foreach (DwarfGate gate in Location.get_MineShaftDwarfGates())
                gate.get_DwarfGateDisabled().Value = true;
        }

        public override void Initialize(MineShaft mine)
        {
            Location = mine;

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

        public override bool ShouldSpawnLadder(MineShaft mine) => canSpawnLadder;

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!initialized && InitializedRoomType.Value >= 0)
            {
                Point originPoint = IsLeftSide.Value ? new(8, 7) : new(27, 7);

                RoomType roomType = (RoomType)InitializedRoomType.Value;
                SpawnRoomType(roomType, mine, originPoint);

                initialized = true;
            }
            else if (!initializedOtherRoom && InaccessibleRoomType.Value >= 0)
            {
                Point originPoint = !IsLeftSide.Value ? new(8, 7) : new(27, 7);

                RoomType roomType = (RoomType)InaccessibleRoomType.Value;
                SpawnRoomType(roomType, mine, originPoint);

                initializedOtherRoom = true;
            }

            if (!Context.IsMainPlayer || !initialized || !initializedOtherRoom)
                return;

            tickCounter++;
            if (tickCounter > 60)
                tickCounter = 0;

            if (InitializedRoomType.Value == (int)RoomType.Monsters && !spawnedLadder && tickCounter == 0)
            {
                if (mine.EnemyCount == 0)
                    SpawnLadder();
            }
            else if (InitializedRoomType.Value != (int)RoomType.Monsters && !spawnedLadder)
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

            InitializedRoomType.Value = (int)roomType;
            InaccessibleRoomType.Value = (int)otherRoom;

            if (roomType == RoomType.Monsters)
                SpawnMonsters(Location, IsLeftSide.Value ? new(8, 7) : new(27, 7));
            else if (otherRoom == RoomType.Monsters)
                SpawnMonsters(Location, !IsLeftSide.Value ? new(8, 7) : new(27, 7));

            if (roomType == RoomType.Cauldron)
                SpawnCauldron(Location, IsLeftSide.Value ? new(8, 7) : new(27, 7));
            else if (otherRoom == RoomType.Cauldron)
                SpawnCauldron(Location, !IsLeftSide.Value ? new(8, 7) : new(27, 7));
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

            mine.setTileProperty(xOrigin + 2, yOrigin + 2, "Buildings", "Action", "Curses");
            mine.setTileProperty(xOrigin + 3, yOrigin + 2, "Buildings", "Action", "Curses");
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
