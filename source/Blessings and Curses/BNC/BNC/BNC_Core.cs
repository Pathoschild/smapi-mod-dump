using BNC.Configs;
using Bookcase.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using static BNC.Spawner;

namespace BNC
{
    public class BNC_Core : Mod
    {
        public static SaveFile BNCSave = new SaveFile();
        public static readonly string saveFileName = "bncsave";
        public static IModHelper helper;
        public static IMonitor Logger;
        public static Config config = new Config();

        public override void Entry(IModHelper helperIn)
        {
            helper = helperIn;
            Logger = this.Monitor;
            config = helper.ReadConfig<Config>();

            if(config.Enable_Twitch_Integration)
                TwitchIntergration.LoadConfig(helperIn);

            MineEvents.MineLevelChanged += MineBuffManager.mineLevelChanged;
            BookcaseEvents.GameQuaterSecondTick.Add(QuaterSecondUpdate);
            BookcaseEvents.GameFullSecondTick.Add(FullSecondTick);
             TimeEvents.AfterDayStarted += NewDayEvent;
            SaveEvents.AfterSave += SaveEvent;
            SaveEvents.AfterLoad += LoadEvent;
            SaveEvents.BeforeSave += BeforeSaveEvent;
            SaveEvents.AfterReturnToTitle += OnReturnToTitle;
            BuffManager.Init();
            MineBuffManager.Init();
            Spawner.Init();

            //////if(DebugMode)
            //InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
        }

        
        Spawner spawner = new Spawner();
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            this.Monitor.Log(e.Button.ToString());
            if (e.Button.Equals(SButton.B)) {
                this.Monitor.Log("B was pressed");
                Array values = Enum.GetValues(typeof(TwitchMobType));
                    Random random = new Random();
                    TwitchMobType randomType = (TwitchMobType)values.GetValue(random.Next(values.Length));


                    string name = "test name"+ random.Next();
                    Spawner.AddMonsterToSpawnFromType(randomType, name);



                    //Junimo j = new TwitchJunimo(Vector2.Zero);
                    //j.Name = "test name" + (i > 0 ? "'s npc" : "");
                    //j.collidesWithOtherCharacters.Value = false;
                    //Spawner.addSubToSpawn(j);
            }
        }

        private void OnReturnToTitle(object sender, EventArgs e)
        {
            BNCSave.clearData();
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            BNCSave.LoadModData(helper);
        }

        private void BeforeSaveEvent(object sender, EventArgs e)
        {
            Spawner.ClearMobs();
        }

        private void SaveEvent(object sender, EventArgs e) {
            BNCSave.SaveModData(helper);
        }

        private void NewDayEvent(object sender, EventArgs e) {
            if (!Context.IsWorldReady)
                return;
            if (BNC_Core.config.Random_Day_Buffs)
                BuffManager.UpdateDay();
        }

        private void QuaterSecondUpdate(Bookcase.Events.Event args)
        {
            if (!Context.IsWorldReady)
                return;
            BuffManager.UpdateTick();
            Spawner.UpdateTick();
        }

        private void FullSecondTick(Bookcase.Events.Event args)
        {
            if (!Context.IsWorldReady)
                return;
            MineBuffManager.UpdateTick();
        }
    }
}
