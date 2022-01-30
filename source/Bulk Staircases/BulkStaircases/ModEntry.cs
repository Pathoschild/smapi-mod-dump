/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BananaFruit1492/BulkStaircases
**
*************************************************/

using BulkStaircases.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;

namespace BulkStaircases
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// Field name of treasure room field, see MineShaft.cs
        /// </summary>
        private static readonly string TREASUREFIELDNAME = "netIsTreasureRoom";

        /// <summary>
        /// Property name of quarry dungeon state, see MineShaft.cs
        /// </summary>
        private static readonly string QUARRYPROPERTYNAME = "isQuarryArea";

        /// <summary>
        /// Property name of monster area state, see MineShaft.cs
        /// </summary>
        private static readonly string MONSTERAREAPROPERTYNAME = "isMonsterArea";

        /// <summary>
        /// Property name of dinosaur area state, see MineShaft.cs
        /// </summary>
        private static readonly string DINOSAURAREAPROPERTYNAME = "isDinoArea";

        /// <summary>
        /// String for mines, see constructor of MineShaft.cs
        /// </summary>
        private static readonly string UNDERGROUNDMINESTRING = "UndergroundMine";

        private ModConfig Config;

        private static readonly string STAIRCASENAME = "Staircase";

        /// <summary>
        /// Level 100 in skull cavern
        /// </summary>
        private static readonly int SKULLCAVERNLEVEL100FLOOR = 220;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Config.ToggleKey.JustPressed())
                return;
            if (!Context.CanPlayerMove)
                return;
            GameLocation location = Game1.currentLocation;
            if (location is not MineShaft shaft)
                return;
            Farmer player = Game1.player;
            if (player == null)
                return;
            Item heldItem = player.CurrentItem;
            if (heldItem == null)
                return;
            if (heldItem.Name != ModEntry.STAIRCASENAME)
                return;
            if(shaft.mineLevel == MineShaft.bottomOfMineLevel)
            {
                Game1.addHUDMessage(new HUDMessage($"You're already at the bottom of the mines", 3));
                return;
            }
            if(shaft.mineLevel == MineShaft.quarryMineShaft)
            {
                Game1.addHUDMessage(new HUDMessage($"Can't use staircases here", 3));
                return;
            }
            var numStairsCanBeUsed = heldItem.Stack - Config.NumberOfStaircasesToLeaveInStack;
            if (numStairsCanBeUsed <= 0)
            {
                Game1.addHUDMessage(new HUDMessage($"Only {heldItem.Stack} staircases left", 3));
                return;
            }
            int maxLevelsToDescend;
            // normal mine
            if (shaft.mineLevel >= 0 && shaft.mineLevel < MineShaft.bottomOfMineLevel)
            {
                if(shaft.mineLevel + numStairsCanBeUsed > MineShaft.bottomOfMineLevel)
                {
                    maxLevelsToDescend = MineShaft.bottomOfMineLevel - shaft.mineLevel;
                }
                else
                {
                    maxLevelsToDescend = numStairsCanBeUsed;
                }
            }
            // skull cavern
            else
            {
                maxLevelsToDescend = numStairsCanBeUsed;
            }
            int actualLevelsToDescend = 0;
            LocationRequest levelToDescendTo;
            if (this.SkipAllLevels())
            {
                actualLevelsToDescend = maxLevelsToDescend;
                levelToDescendTo = this.GetLocationRequestForMineLevel(shaft.mineLevel + actualLevelsToDescend);
            }
            else if (this.OnlyNotSkipLevel100SkullCavern())
            {
                if (121 <= shaft.mineLevel && shaft.mineLevel < ModEntry.SKULLCAVERNLEVEL100FLOOR)
                {
                    actualLevelsToDescend = Math.Min(maxLevelsToDescend, ModEntry.SKULLCAVERNLEVEL100FLOOR - shaft.mineLevel);
                }
                else
                {
                    actualLevelsToDescend = maxLevelsToDescend;
                }
                levelToDescendTo = this.GetLocationRequestForMineLevel(shaft.mineLevel + actualLevelsToDescend);
            }
            // only actually calculate level if need be
            else
            {
                do
                {
                    actualLevelsToDescend++;
                    levelToDescendTo = this.GetLocationRequestForMineLevel(shaft.mineLevel + actualLevelsToDescend);
                }
                while (SkipLevel(levelToDescendTo) && actualLevelsToDescend < maxLevelsToDescend);
            }
            warpFarmer(levelToDescendTo);
            if (heldItem.Stack > actualLevelsToDescend)
            {
                MineShaft.numberOfCraftedStairsUsedThisRun += actualLevelsToDescend;
                heldItem.Stack -= actualLevelsToDescend;
            }
            else
            {
                player.removeItemFromInventory(heldItem);
            }
        }

        /// <summary>
        /// Checks if only skull cavern level 100 is not to be skipped
        /// </summary>
        /// <returns></returns>
        private bool OnlyNotSkipLevel100SkullCavern()
        {
            return this.Config.SkipTreasureLevels
                && this.Config.SkipSlimeLevels
                && this.Config.SkipQuarryDungeonLevels
                && this.Config.SkipMonsterLevels
                && this.Config.SkipDinosaurLevels
                && this.Config.SkipMushroomLevels
                && !this.Config.SkipLevel100SkullCavern;
        }

        /// <summary>
        /// Checks if all levels are to be skipped
        /// </summary>
        /// <returns></returns>
        private bool SkipAllLevels()
        {
            return this.Config.SkipTreasureLevels
                && this.Config.SkipSlimeLevels
                && this.Config.SkipQuarryDungeonLevels
                && this.Config.SkipMonsterLevels
                && this.Config.SkipDinosaurLevels
                && this.Config.SkipMushroomLevels
                && this.Config.SkipLevel100SkullCavern;
        }

        /// <summary>
        /// True if the level is to be skipped.
        /// False otherwise.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool SkipLevel(LocationRequest request)
        {
            var location = request.Location;
            if (location is MineShaft mine)
            {
                if (!this.Config.SkipTreasureLevels)
                {
                    IReflectedField<NetBool> treasureField = Helper.Reflection.GetField<NetBool>(mine, TREASUREFIELDNAME);
                    if(treasureField != null)
                    {
                        NetBool val = treasureField.GetValue();
                        bool isTreasure = val.Value;
                        if (isTreasure)
                            return false;
                    }
                }
                if (!this.Config.SkipSlimeLevels)
                {
                    if (mine.isLevelSlimeArea())
                        return false;
                }
                if (!this.Config.SkipQuarryDungeonLevels)
                {
                    if (IsBoolPropertyTrue(QUARRYPROPERTYNAME, mine))
                        return false;
                }
                if (!this.Config.SkipMonsterLevels)
                {
                    if (IsBoolPropertyTrue(MONSTERAREAPROPERTYNAME, mine))
                        return false;
                }
                if (!this.Config.SkipDinosaurLevels)
                {
                    if (IsBoolPropertyTrue(DINOSAURAREAPROPERTYNAME, mine))
                        return false;
                }
                if (!this.Config.SkipLevel100SkullCavern)
                {
                    if (mine.mineLevel == SKULLCAVERNLEVEL100FLOOR)
                        return false;
                }
                if (!this.Config.SkipMushroomLevels)
                {
                    if (MineShaft.mushroomLevelsGeneratedToday.Contains(mine.mineLevel))
                        return false;
                }
            }
            return true;
        }

        private bool IsBoolPropertyTrue(string propertyName, object tobeCheckedForProperty)
        {
            IReflectedProperty<bool> dinosaurAreaProperty = Helper.Reflection.GetProperty<bool>(tobeCheckedForProperty, propertyName);
            if (dinosaurAreaProperty != null)
            {
                return dinosaurAreaProperty.GetValue();
            }
            return false;
        }
        private void warpFarmer(LocationRequest request)
        {
            // constants taken from Game1.enterMine
            Game1.warpFarmer(request, 6, 6, 2);
        }
        
        /// <summary>
        /// Gets the location request for the given mine level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private LocationRequest GetLocationRequestForMineLevel(int level)
        {
            return Game1.getLocationRequest(UNDERGROUNDMINESTRING + level);
        }
    }
}
