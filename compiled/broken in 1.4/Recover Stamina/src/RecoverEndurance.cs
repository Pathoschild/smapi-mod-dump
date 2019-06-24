using System;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace lperkins2.RecoverEndurance
{
    public class ModEntry : Mod {
        private string DataFilePath => Path.Combine("data", $"{Constants.SaveFolderName}.json");
        
        public class EnduranceData {
            public float lastStamina;
            public float staminaUsed;
            public float bsStamina;
            
            public int lastToD;
            public int currentToD;
            public int ticker;
            public bool wasExhausted = false;
        }
        
        public IModHelper ModHelper;
        public EnduranceData enduranceData;
        
        public float lastStamina {
            get { return this.enduranceData.lastStamina; }
            set { this.enduranceData.lastStamina = value; }
        }
        
        public float staminaUsed {
            get { return this.enduranceData.staminaUsed; }
            set { this.enduranceData.staminaUsed = value; }
        }
        
        public float bsStamina {
            get { return this.enduranceData.bsStamina; }
            set { this.enduranceData.bsStamina = value; }
        }
        
        public int lastToD {
            get { return this.enduranceData.lastToD; }
            set { this.enduranceData.lastToD = value; }
        }
        
        public int currentToD {
            get { return this.enduranceData.currentToD; }
            set { this.enduranceData.currentToD = value; }
        }
        
        public int ticker {
            get { return this.enduranceData.ticker; }
            set { this.enduranceData.ticker = value; }
        }
        
        public bool wasExhausted {
            get { return this.enduranceData.wasExhausted; }
            set { this.enduranceData.wasExhausted = value; }
        }
        
        
        public ModEntry() {
            enduranceData = new EnduranceData();
        }
        
        public int modulo(int x, int y) {
            return ((x % y) + y) % y;
        }
        
        public float hoursTillSix(int time) {
            int minutes = 100 - time % 100;
            int hours = modulo(600 - (time - minutes), 2400) / 100;
            if (hours == 0 && minutes > 0) {
                hours = 23;
            }
            return hours - minutes / 60.0f;
        }
        
        private void _staminaIncreased(float dStamina) {
            float penalty = this.staminaUsed / (Game1.player.MaxStamina * 4) * dStamina;
            if (this.wasExhausted) {
                penalty *= 2;
            }
            if (penalty > dStamina) {
                penalty = dStamina;
            }
            Game1.player.Stamina -= penalty;
        }
        
        public override void Entry(IModHelper helper) {
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged;
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            this.ModHelper = helper;
        }
        
        
        private void GameEvents_UpdateTick(object sender, EventArgs e) {
            if (!Context.IsWorldReady)
                return;
            
            float dStamina = Game1.player.Stamina - this.lastStamina;
            
            if (dStamina > 0) {
                this._staminaIncreased(dStamina);
            }
            else if (dStamina < 0) {
                this.staminaUsed -= dStamina;
            }
            
            this.lastStamina = Game1.player.Stamina;
            if (this.lastStamina < 0) {
                this.wasExhausted = true;
            }
        }
        
        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            this.ticker++;
            if (Game1.player.isInBed) {
                this.staminaUsed -= 1.0f / 1.05f;
                if (this.staminaUsed < 0) {
                    this.staminaUsed = 0;
                }
            }
            if (this.ticker >= 10) {
                this.ticker = 0;
                Game1.player.Stamina += 1;
            }
            
        }
        
        private void SaveEvents_BeforeSave(object sender, EventArgs e) {
            this.bsStamina = Game1.player.Stamina;
            this.ModHelper.WriteJsonFile(this.DataFilePath, this.enduranceData);
        }
        
        private void SaveEvents_AfterLoad(object sender, EventArgs e) {
            this.enduranceData = this.ModHelper.ReadJsonFile<EnduranceData>(this.DataFilePath);
            if (this.enduranceData==null) {
                this.enduranceData = new EnduranceData();
                this.staminaUsed = 0;
                this.lastStamina = Game1.player.Stamina;
                this.lastToD = Game1.timeOfDay;
                this.ticker = 0;
                this.bsStamina = 0;
            }
            
            
        }
        
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e) {
            if (this.bsStamina==0){
                return;
            }
            float timePassed = this.hoursTillSix(this.lastToD);
            this.rest(timePassed);
            
            
            if (this.wasExhausted) {
                Game1.player.Stamina /= 2;
                this.staminaUsed += Game1.player.Stamina;
            }
            
            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina);
            if (timePassed > 0) {
                this.wasExhausted = false;
            }
        }
        
        private void rest(float timePassed) {
            this.staminaUsed -= timePassed * 40 * (Game1.player.MaxStamina / 270);
            if (this.staminaUsed < 0) {
                this.staminaUsed = 0;
            }
            Game1.player.Stamina = this.bsStamina + timePassed * 45f * (Game1.player.MaxStamina / 270);
            this._staminaIncreased(timePassed * 45f * (Game1.player.MaxStamina / 270));
            
        }
        
        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e) {
            //~ if (Game1.timeOfDay == 630) {
                //~ Game1.timeOfDay=2000;
            //~ }
            if (!Context.IsWorldReady)
                return;
            this.lastToD = this.currentToD;
            this.currentToD = Game1.timeOfDay;
            
        }
    }
}
