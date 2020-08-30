using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SunscreenMod.Flags;

namespace SunscreenMod
{
    public class Sunburn
    {
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static ModConfig Config => ModConfig.Instance;


        protected static ITranslationHelper i18n = Helper.Translation;


        public const int MAXLEVEL = 3; //Make sure this matches the list in Flags.cs

        public const int SUNDAMAGETHRESHOLD = 4000;
        public const int EXTRADAMAGETHRESHOLD = 3000;

        public string SKINCOLORFLAG = $"{FlagBase}NormalSkinColor_";  //Without the index number
        public string SKINCOLORREGEX = $"^{FlagBase}NormalSkinColor_([0-9]+)$";  //Capturing group gets the index number

        public int SunburnLevel {
            get { return _sunburnLevel; }
            internal set 
            {
                value = Math.Max(0, Math.Min(MAXLEVEL, value)); //Clamp to 0-3 inclusive
                if (_sunburnLevel == 0) //Value WAS zero
                {
                    //Save default skin color
                    SaveNormalSkinFlag(Game1.player.skin.Value + 1); //Adjust for zero-based indexing
                }

                //Add or update debuffs
                ActivateSunburnDebuff(value);

                //Set sunburn level
                _sunburnLevel = value;
                SaveLevelFlag("SunburnLevel", value);

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

        public int NewBurnDamageLevel {
            get { return _newBurnDamageLevel; }
            internal set 
            {
                value = Math.Max(0, Math.Min(MAXLEVEL, value)); //Clamp to 0-3 inclusive
                _newBurnDamageLevel = value;
                SaveLevelFlag("NewBurnDamageLevel", value);
            }
        }
        private int _newBurnDamageLevel = 0;

        public int SunDamageCounter
        {
            get { return _sunDamageCounter; }
            private set
            {
                int damageCount = value;
                int threshold = HasSunDamage() ? EXTRADAMAGETHRESHOLD : SUNDAMAGETHRESHOLD;
                if (damageCount >= threshold)
                {
                    NewBurnDamageLevel += 1;
                    damageCount %= threshold;
                }
                _sunDamageCounter = damageCount;
            }
        }
        private int _sunDamageCounter = 0;

        public Sunburn()
        {
            SunburnLevel = GetPlayerSunburnLevel(Game1.player);
            NewBurnDamageLevel = GetPlayerNewBurnDamageLevel(Game1.player);
        }

        private Buff NewSunburnDebuff(int level)
        {
            level = Math.Max(1, Math.Min(MAXLEVEL, level));
            //int staminaDebuff = Config.EnergyLossPerLevel * level * -1;
            //int defenseDebuff = Config.DefenseLossPerLevel * level * -1;
            int speedDebuff = Config.SunburnSpeedDebuff? -1 : 0;
            string severity = "mild"; if (level > 1) severity = "moderate"; if (level > 2) severity = "severe";
            int inGameMinutes = 60 * 40; //40 hours, more than plenty to last the day
            /*return new Buff(0, 0, 0, 0, 0, 0, 0, 
                staminaDebuff, 0, speedDebuff, defenseDebuff, 0,
                inGameMinutes, $"{FlagBase}Sunburn", $"Sunburn ({severity})"); //Broken buff */
            return new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speedDebuff, 0, 0,
                inGameMinutes, $"{FlagBase}Sunburn", $"Sunburn ({severity})"); //Working buff
        }

        public void ActivateSunburnDebuff(int level)
        {
            List<Buff> todelete = new List<Buff>();
            foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
            {
                if (buff.source == $"{FlagBase}Sunburn")
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

        public void MaintainSunburnDebuffs()
        {
            foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
            {
                if (buff.source == $"{FlagBase}Sunburn")
                {
                    buff.millisecondsDuration = 900000; //15 minutes
                }
            }
        }

        public void DisplaySunburnStatus()
        {
            int level = SunburnLevel;
            //HUD Message
            if (level == 0)
            {
                Game1.addHUDMessage(new HUDMessage(i18n.Get("Sunburn.Healed"), 4)); //Stamina heal message type
            }
            else if (level > 0)
            {
                string severity = "mild"; if (level > 1) severity = "moderate"; if (level > 2) severity = "severe";
                Game1.addHUDMessage(new HUDMessage(i18n.Get("Sunburn.SunburnAlert", new { severity }), 2)); //Exclamation mark message type
            }
        }

        public void UpdateForNewDay()
        {
            SunburnLevel--; //Heal one level
            SunburnLevel += NewBurnDamageLevel; //Activate new damage
            NewBurnDamageLevel = 0;
            SunDamageCounter = 0;
        }

        public void CheckForBurnDamage(SDVTime time)
        {
            //TODO? maybe check for sunscreen here
            int newDamage = UVIndex.UVIntensityAt(time);
            SunDamageCounter += newDamage;
            if (Config.DebugMode) Monitor.Log($"New burn damage level is {NewBurnDamageLevel} | SunDamageCounter is at {SunDamageCounter}", LogLevel.Debug);
        }

        private void SaveNormalSkinFlag(int skinValue)
        {
            RemoveNormalSkinFlag();
            Game1.player.mailReceived.Add($"{SKINCOLORFLAG}{skinValue}");
        }

        private void RemoveNormalSkinFlag()
        {
            List<string> todelete = new List<string>();
            foreach (string flag in Game1.player.mailReceived) //Remove old flags
            {
                if (Regex.IsMatch(flag, SKINCOLORREGEX)) todelete.Add(flag);
            }
            foreach (string flag in todelete) Game1.player.mailReceived.Remove(flag);
        }

        public int? GetNormalSkinFlag()
        {
            int? value = null;
            foreach (string flag in Game1.player.mailReceived) //Remove old flags
            {
                Match m = Regex.Match(flag, SKINCOLORREGEX);
                if (m.Success) value = int.Parse(m.Groups[1].ToString()); //Match groups are not zero-indexed
            }
            return value;
        }

        private void SaveLevelFlag(string flagType, int level)
        {
            for (int i = 1;  i <= MAXLEVEL; i++) //Remove old flags
            {
                if (i != level) RemoveFlag($"{flagType}_{i}");
            }
            if (level > 0) //Don't add any flag if level is zero
            {
                AddFlag($"{flagType}_{level}"); //No need to worry about duplicates, AddFlag() takes care of it
            }
        }

        public bool IsSunburnt()
        {
            if (SunburnLevel > 0)
                return true;
            return false;
        }
        public bool HasSunDamage() //True if there is a sunburn OR existing new burn
        {
            if (SunburnLevel > 0 || NewBurnDamageLevel > 0)
                return true;
            return false;
        }

        public bool IsPlayerSunburnt(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            if (GetPlayerSunburnLevel(who) == 0)
                return false;
            return true;
        }

        public int GetPlayerSunburnLevel(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            for (int i = 1; i <= MAXLEVEL; i++)
            {
                if (HasFlag($"SunburnLevel_{i}", who)) return i;
            }
            return 0;
        }

        public int GetPlayerNewBurnDamageLevel(Farmer who) //Best used with other players, since **THIS** player can just check the property.
        {
            for (int i = 1; i <= MAXLEVEL; i++)
            {
                if (HasFlag($"NewBurnDamageLevel_{i}", who)) return i;
            }
            return 0;
        }

    }
}
