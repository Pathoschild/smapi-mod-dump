/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.GameLocations;
using FishingTrawler.Messages;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace FishingTrawler.GameLocations
{
    internal class TrawlerCabin : TrawlerLocation
    {
        private List<Location> _computerLocations;

        private const int TRAWLER_TILESHEET_INDEX = 2;
        private const string COFFEE_MACHINE_SOURCE = "murphy_cofee_machine";
        private const int BASE_COMPUTER_MILLISECONDS = 60000;
        private const int CYCLE_COMPUTER_MILLISECONDS = 30000;

        private static bool _hasLeftStartingArea;
        private static int _completedComputerCycles = 0;
        private static int _computerCooldownMilliseconds = BASE_COMPUTER_MILLISECONDS;

        internal bool hasSwiftWinds;

        public TrawlerCabin()
        {

        }

        internal TrawlerCabin(string mapPath, string name) : base(mapPath, name)
        {
            hasSwiftWinds = false;

            _computerLocations = new List<Location>();

            Layer buildingsLayer = map.GetLayer("Buildings");
            for (int x = 0; x < buildingsLayer.LayerWidth; x++)
            {
                for (int y = 0; y < buildingsLayer.LayerHeight; y++)
                {
                    Tile tile = buildingsLayer.Tiles[x, y];
                    if (tile is null)
                    {
                        continue;
                    }

                    if (tile.Properties.ContainsKey("CustomAction") && tile.Properties["CustomAction"] == "Guidance")
                    {
                        _computerLocations.Add(new Location(x, y));
                    }
                }
            }
        }

        internal override void Reset()
        {
            _completedComputerCycles = 0;
            _computerCooldownMilliseconds = BASE_COMPUTER_MILLISECONDS;
            _hasLeftStartingArea = false;
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();

            AmbientLocationSounds.addSound(new Vector2(4f, 3f), 2);

            if (miniJukeboxTrack.Value is null)
            {
                Game1.changeMusicTrack("fieldofficeTentMusic"); // Suggested tracks: Snail's Radio, Jumio Kart (Gem), Pirate Theme
            }

            // Set coffee machine animation
            setAnimatedMapTile(0, 4, new int[] { 31, 32, 33, 32 }, 90, "Front", null, TRAWLER_TILESHEET_INDEX);
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);

            if (IsComputerReady() is false)
            {
                setMapTileIndex(3, 2, -1, "Front", TRAWLER_TILESHEET_INDEX);
            }
            else
            {
                setAnimatedMapTile(3, 2, new int[] { 13, 14, 15, 16, 17 }, 90, "Front", null, TRAWLER_TILESHEET_INDEX);
            }
        }

        public override void cleanupBeforePlayerExit()
        {
            if (string.IsNullOrEmpty(miniJukeboxTrack.Value) && !string.IsNullOrEmpty(FishingTrawler.trawlerThemeSong))
            {
                FishingTrawler.SetTrawlerTheme(null);
            }
            else
            {
                FishingTrawler.SetTrawlerTheme(Game1.getMusicTrackName());
            }
            _hasLeftStartingArea = true;

            base.cleanupBeforePlayerExit();
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            string actionProperty = doesTileHaveProperty(tileLocation.X, tileLocation.Y, "CustomAction", "Buildings");

            if (actionProperty is not null)
            {
                if (actionProperty == "PathosCat")
                {
                    Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.pathos_cat"));
                    return true;
                }
                else if (actionProperty == "MurphyBusy")
                {
                    Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.murphy_busy"));
                    return true;
                }
                else if (actionProperty == "Guidance" && base.IsWithinRangeOfTile(tileLocation.X, tileLocation.Y, 1, 1, who) is true)
                {
                    if (IsComputerReady() is false)
                    {
                        base.playSound("cancel");
                        Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.computer.not_ready"), 3) { timeLeft = 1000f });
                    }
                    else
                    {
                        base.playSound("healSound");
                        AcceptPlottedCourse();
                    }
                    return true;
                }
                else if (actionProperty == "Coffee" && base.IsWithinRangeOfTile(tileLocation.X, tileLocation.Y, 1, 1, who) is true)
                {
                    if (who.buffs.AppliedBuffs.TryGetValue(COFFEE_MACHINE_SOURCE, out var coffeeBuff) is true)
                    {
                        coffeeBuff.millisecondsDuration = GetCoffeeBuff().millisecondsDuration;
                    }
                    else
                    {
                        who.applyBuff(GetCoffeeBuff());
                    }

                    if (base.IsMessageAlreadyDisplayed(FishingTrawler.i18n.Get("game_message.coffee_machine.drink")) is false)
                    {
                        Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.coffee_machine.drink")) { timeLeft = 1000f, noIcon = true });
                    }

                    base.playSound("gulp");

                    return true;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            string actionProperty = doesTileHaveProperty(xTile, yTile, "CustomAction", "Buildings");

            if (actionProperty is not null)
            {
                if (actionProperty == "PathosCat")
                {
                    if (base.IsWithinRangeOfTile(xTile, yTile, 1, 1, who) is false)
                    {
                        Game1.mouseCursorTransparency = 0.5f;
                    }
                    return true;
                }
                else if (actionProperty == "MurphyBusy")
                {
                    if (base.IsWithinRangeOfTile(xTile, yTile, 1, 1, who) is false)
                    {
                        Game1.mouseCursorTransparency = 0.5f;
                    }
                    return true;
                }
                else if (actionProperty == "Guidance")
                {
                    if (base.IsWithinRangeOfTile(xTile, yTile, 1, 1, who) is false)
                    {
                        Game1.mouseCursorTransparency = 0.5f;
                    }
                    return true;
                }
                else if (actionProperty == "Coffee")
                {
                    if (base.IsWithinRangeOfTile(xTile, yTile, 1, 1, who) is false)
                    {
                        Game1.mouseCursorTransparency = 0.5f;
                    }
                    return true;
                }
            }

            return base.isActionableTile(xTile, yTile, who);
        }

        public bool HasLeftCabin()
        {
            return _hasLeftStartingArea;
        }

        #region Guidance system event methods
        public void AcceptPlottedCourse()
        {
            if (IsComputerReady() is false)
            {
                return;
            }
            RestartComputer();

            FishingTrawler.SyncTrawler(SyncType.RestartGPS, -1, FishingTrawler.GetFarmersOnTrawler());
        }

        public int GetCooldown()
        {
            return _computerCooldownMilliseconds;
        }

        public void SetCooldown(int milliseconds)
        {
            _computerCooldownMilliseconds = milliseconds;
        }

        public void ReduceCooldown(int milliseconds)
        {
            SetCooldown(_computerCooldownMilliseconds - Math.Abs(milliseconds));
        }

        public bool IsComputerReady()
        {
            return _computerCooldownMilliseconds <= 0;
        }

        public void RestartComputer()
        {
            FishingTrawler.eventManager.IncrementTripTimer(30000);

            _completedComputerCycles += 1;
            _computerCooldownMilliseconds = (_completedComputerCycles * CYCLE_COMPUTER_MILLISECONDS) + BASE_COMPUTER_MILLISECONDS;
        }
        #endregion

        #region Coffee Machine related
        public Buff GetCoffeeBuff()
        {
            int speedBuff = 9;
            var buffEffect = new BuffEffects(new StardewValley.GameData.Buffs.BuffAttributesData() { Speed = hasSwiftWinds is true ? 3 : 2 });
            var buff = new Buff(COFFEE_MACHINE_SOURCE, FishingTrawler.i18n.Get("etc.coffee_machine"), duration: hasSwiftWinds is true ? 60000 * 3 : 60000, iconSheetIndex: speedBuff, effects: buffEffect);

            return buff;
        }
        #endregion
    }
}
