using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static SunscreenMod.Flags;

namespace SunscreenMod
{
    /// <summary>Manages a player's sunburn level, new burn level, and sun damage.</summary>
    public class Sunburn
    {
        static IModHelper Helper => ModEntry.Instance.Helper;
        static IMonitor Monitor => ModEntry.Instance.Monitor;
        static ModConfig Config => ModConfig.Instance;


        static readonly ITranslationHelper i18n = Helper.Translation;


        /// <summary>Maximum allowable level of sunburn or new burn damage.</summary>
        public const int MAX_LEVEL = 3; //Make sure this matches the list in Flags.cs

        /// <summary>Sun damage threshold for triggering new burn if healthy.</summary>
        public const int SUN_DAMAGE_THRESHOLD = 4000;
        /// <summary>Sun damage threshold for additional new burn if already sunburnt or damaged</summary>
        public const int EXTRA_DAMAGE_THRESHOLD = 2500;

        readonly string SKIN_COLOR_FLAG = $"{FLAG_BASE}NormalSkinColor";  //Without the index number
        readonly string SKIN_COLOR_REGEX = $"^{FLAG_BASE}NormalSkinColor_([0-9]+)$";  //Capturing group gets the index number

        /// <summary>Current level of active sunburn damage from 0-3. Setter handles validation, skin color changes and flag data.</summary>
        public int SunburnLevel
        {
            get { return _sunburnLevel; }
            internal set
            {
                value = Math.Max(0, Math.Min(MAX_LEVEL, value)); //Clamp to 0-3 inclusive
                if (_sunburnLevel == 0) //Value WAS zero
                {
                    //Save default skin color
                    SaveNormalSkinFlag(Game1.player.skin.Value + 1); //Adjust for zero-based indexing
                }

                //Add or update debuffs
                ActivateSunburnDebuff(value);

                //Set sunburn level
                _sunburnLevel = value;
                SaveLevelFlag(SUNBURN_LEVEL_FLAG, value);

                //Change skin color appropriately
                int newSkinColor;
                if (value == 0 || !Config.SkinColorChange)
                {
                    newSkinColor = GetNormalSkinFlag() ?? Game1.player.skin.Value; //Undo sunburn or undo skin color effects
                    RemoveNormalSkinFlag();
                }
                else if (Context.IsMultiplayer)
                {
                    newSkinColor = Config.BurnSkinColorIndex[value - 1];
                }
                else
                {
                    newSkinColor = Config.BurnSkinColorIndex[0];
                    Helper.Content.InvalidateCache("Characters\\Farmer\\skinColors"); //Necessary for single player colors
                }
                int skinColorIndex = newSkinColor - 1; //Adjust for zero-based indexing
                Game1.player.changeSkinColor(skinColorIndex, true);

            }
        }
        private int _sunburnLevel = 0;

        /// <summary>Current level of newly accumulated sunburn damage from 0-3. Setter handles validation and flag data.</summary>
        public int NewBurnDamageLevel
        {
            get { return _newBurnDamageLevel; }
            internal set
            {
                value = Math.Max(0, Math.Min(MAX_LEVEL, value)); //Clamp to 0-3 inclusive
                _newBurnDamageLevel = value;
                SaveLevelFlag(NEW_BURN_LEVEL_FLAG, value);
            }
        }
        private int _newBurnDamageLevel = 0;

        /// <summary>Accumulated points of UV damage leading to new burn development. Setter handles rollover when threshold is hit.</summary>
        public int SunDamageCounter
        {
            get { return _sunDamageCounter; }
            private set
            {
                int damageCount = value;
                int threshold = HasSunDamage() ? EXTRA_DAMAGE_THRESHOLD : SUN_DAMAGE_THRESHOLD;
                if (damageCount >= threshold)
                {
                    NewBurnDamageLevel += 1;
                    damageCount %= threshold;
                }
                _sunDamageCounter = damageCount;
            }
        }
        private int _sunDamageCounter = 0;

        /// <summary>Initializes a class instance with the player's current sunburn and new burn data from mail flags.</summary>
        public Sunburn()
        {
            SunburnLevel = GetPlayerSunburnLevel(Game1.player);
            NewBurnDamageLevel = GetPlayerNewBurnDamageLevel(Game1.player);
        }

        /// <summary>Constructs a Buff instance to match the ongoing effects of sunburn.</summary>
        /// <param name="level">Level of sunburn damage</param>
        private Buff NewSunburnDebuff(int level)
        {
            level = Math.Max(1, Math.Min(MAX_LEVEL, level));
            //int staminaDebuff = Config.EnergyLossPerLevel * level * -1;
            //int defenseDebuff = Config.DefenseLossPerLevel * level * -1;
            int speedDebuff = Config.SunburnSpeedDebuff ? -1 : 0;
            int inGameMinutes = 60 * 40; //40 hours, more than plenty to last the day
            /*return new Buff(0, 0, 0, 0, 0, 0, 0,
                staminaDebuff, 0, speedDebuff, defenseDebuff, 0,
                inGameMinutes, $"{FlagBase}Sunburn", $"Sunburn ({severity})"); //Broken buff */

            string severity = i18n.Get("Severity.Mild");
            if (level > 1) severity = i18n.Get("Severity.Moderate");
            if (level > 2) severity = i18n.Get("Severity.Severe");

            return new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speedDebuff, 0, 0,
                inGameMinutes, $"{FLAG_BASE}Sunburn", i18n.Get("Sunburn.DebuffSource", new { severity })); //Working buff
        }

        /// <summary>Resets and/or activates a Buff matching the input level of sunburn damage.</summary>
        /// <param name="level">Level of sunburn damage</param>
        public void ActivateSunburnDebuff(int level)
        {
            List<Buff> todelete = new List<Buff>();
            foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
            {
                if (buff.source == $"{FLAG_BASE}Sunburn")
                {
                    if (Config.DebugMode) Monitor.Log($"Found buff to remove: {buff.which} | {buff.description}.", LogLevel.Debug);
                    buff.removeBuff();
                    todelete.Add(buff);
                }
            }
            foreach (Buff buff in todelete) Game1.buffsDisplay.otherBuffs.Remove(buff);
            if (Config.DebugMode) Monitor.Log($"Max energy right now is {Game1.player.maxStamina}.", LogLevel.Debug);
            if (level > 0)
            {
                Game1.buffsDisplay.addOtherBuff(NewSunburnDebuff(level));
                if (Config.DebugMode) Monitor.Log($"Activated a level {level} sunburn debuff. Max energy is {Game1.player.maxStamina}", LogLevel.Debug);
            }
        }

        /// <summary>Maintains current sunburn Buff remaining times when called.</summary>
        public void MaintainSunburnDebuffs()
        {
            foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
            {
                if (buff.source == $"{FLAG_BASE}Sunburn")
                {
                    buff.millisecondsDuration = 50; //50 milliseconds (like CJB cheats does it)
                }
            }
        }

        /// <summary>Display an HUD info message with the current sunburn level, or "healed" if zero.</summary>
        public void DisplaySunburnStatus()
        {
            int level = SunburnLevel;
            //HUD Message
            if (level == 0)
            {
                string messagetext = i18n.Get("Sunburn.Healed");
                Game1.addHUDMessage(new HUDMessage(messagetext, 4)); //Stamina heal message type
                Monitor.Log($"Burn status: {messagetext}", LogLevel.Info);
            }
            else if (level > 0)
            {
                string severity = i18n.Get("Severity.Mild");
                if (level > 1) severity = i18n.Get("Severity.Moderate");
                if (level > 2) severity = i18n.Get("Severity.Severe");

                string messagetext = i18n.Get("Sunburn.SunburnAlert", new { severity });
                Game1.addHUDMessage(new HUDMessage(messagetext, 2)); //Exclamation mark message type
                Monitor.Log($"Burn status: {messagetext}", LogLevel.Info);
            }
        }

        /// <summary>Heals one sunburn level, adds new burn to current, and resets new burn and sun damage counters.</summary>
        public void UpdateForNewDay()
        {
            if (SunburnLevel > 0)
            {
                SunburnLevel--; //Heal one level

                //To avoid confusion, only message about healing progress if no new damage was taken.
                if (NewBurnDamageLevel == 0)
                {
                    string messagetext = i18n.Get("NewDay.BurnHealing");
                    if (SunburnLevel != 0) //Only if there is lingering sunburn damage
                    {
                        Game1.addHUDMessage(new HUDMessage(messagetext, 4)); //Stamina heal message type
                    }
                    Monitor.Log($"New day: {messagetext}", LogLevel.Debug);
                }
            }
            if (NewBurnDamageLevel > 0)
            {
                SunburnLevel += NewBurnDamageLevel; //Activate new damage

                string messagetext = i18n.Get("NewDay.NewSunburn");
                Game1.addHUDMessage(new HUDMessage(messagetext, 2)); //Exclamation mark message type
                Monitor.Log($"New day: {messagetext}", LogLevel.Debug);
            }
            //Reset values for new day
            NewBurnDamageLevel = 0;
            SunDamageCounter = 0;
        }

        /// <summary>Calculates current UV intensity and adds that value to the damage counter.</summary>
        public void CheckForBurnDamage(SDVTime time)
        {
            //TODO? maybe check for sunscreen here
            int newDamage = UVIndex.UVIntensityAt(time);
            SunDamageCounter += newDamage;
            if (Config.DebugMode) Monitor.Log($"New burn damage level is {NewBurnDamageLevel} | SunDamageCounter is at {SunDamageCounter}", LogLevel.Debug);
        }

        /// <summary>Saves data about the player's normal skin color as a mail flag.</summary>
        /// <param name="skinValue">The normal skin color value.</param>
        private void SaveNormalSkinFlag(int skinValue)
        {
            RemoveNormalSkinFlag();
            Game1.player.mailReceived.Add($"{SKIN_COLOR_FLAG}_{skinValue}");
        }

        /// <summary>Clears data about normal skin color value from the player's mail flags.</summary>
        private void RemoveNormalSkinFlag()
        {
            List<string> todelete = new List<string>();
            foreach (string flag in Game1.player.mailReceived) //Remove old flags
            {
                if (Regex.IsMatch(flag, SKIN_COLOR_REGEX)) todelete.Add(flag);
            }
            foreach (string flag in todelete) Game1.player.mailReceived.Remove(flag);
        }

        /// <summary>Retrieves data about the player's normal skin color from their mail flags.</summary>
        public int? GetNormalSkinFlag()
        {
            int? value = null;
            foreach (string flag in Game1.player.mailReceived) //Remove old flags
            {
                Match m = Regex.Match(flag, SKIN_COLOR_REGEX);
                if (m.Success) value = int.Parse(m.Groups[1].ToString()); //Match groups are not zero-indexed
            }
            return value;
        }

        /// <summary>Saves data about a player's sunburn level or new burn level as a mail flag. Removes other flags of same type.</summary>
        /// <param name="flagType">The flag value, not including FlagBase prefix or level suffix.</param>
        /// <param name="level">The level value to save as a suffix.</param>
        private void SaveLevelFlag(string flagType, int level)
        {
            for (int i = 1; i <= MAX_LEVEL; i++) //Remove old flags
            {
                if (i != level) RemoveFlag($"{flagType}_{i}");
            }
            if (level > 0) //Don't add any flag if level is zero
            {
                AddFlag($"{flagType}_{level}"); //No need to worry about duplicates, AddFlag() takes care of it
            }
        }

        /// <summary>Checks if the current player has an active sunburn.</summary>
        public bool IsSunburnt()
        {
            if (SunburnLevel > 0)
                return true;
            return false;
        }

        /// <summary>Checks if the current player has an active sunburn OR existing new burn.</summary>
        public bool HasSunDamage()
        {
            if (SunburnLevel > 0 || NewBurnDamageLevel > 0)
                return true;
            return false;
        }

        /// <summary>Checks if a player has an active sunburn.</summary>
        /// <param name="who">The player to check.</param>
        public bool IsPlayerSunburnt(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            if (GetPlayerSunburnLevel(who) == 0)
                return false;
            return true;
        }

        /// <summary>Return's a player's active sunburn level.</summary>
        /// <param name="who">The player to check.</param>
        public int GetPlayerSunburnLevel(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            for (int i = 1; i <= MAX_LEVEL; i++)
            {
                if (HasFlag($"{SUNBURN_LEVEL_FLAG}_{i}", who)) return i;
            }
            return 0;
        }

        /// <summary>Return's a player's new burn damage level.</summary>
        /// <param name="who">The player to check.</param>
        public int GetPlayerNewBurnDamageLevel(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            for (int i = 1; i <= MAX_LEVEL; i++)
            {
                if (HasFlag($"{NEW_BURN_LEVEL_FLAG}_{i}", who)) return i;
            }
            return 0;
        }

    }
}