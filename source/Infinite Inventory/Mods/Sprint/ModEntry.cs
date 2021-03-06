/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;
using System.IO;

namespace Sprint
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {
        public static Random rnd;
        public static Mod instance;
        ModConfig config;

        public bool sprinting = false;

        public int dSpeed;
        public int factor;

        public ModEntry()
        {
            instance = this;


        }

        public override void Entry(IModHelper helper)
        {
            //this.Monitor.Log("sprint - 1", LogLevel.Info);

            rnd = new Random();

            helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;
            helper.Events.Input.ButtonReleased += InputEvents_ButtonReleased;

            helper.Events.GameLoop.UpdateTicked += GameEvents_HalfSecondTick;

            helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;

        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            config = instance.Helper.Data.ReadJsonFile<ModConfig>($"Data/{Constants.SaveFolderName}.json") ?? new ModConfig();
            factor = config.SprintSpeedIncrease;

            dSpeed = Game1.player.addedSpeed;

            if (!File.Exists($"Data/{Constants.SaveFolderName}.json"))
                instance.Helper.Data.WriteJsonFile<ModConfig>($"Data/{Constants.SaveFolderName}.json", config);
        }

        private void GameEvents_HalfSecondTick(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(30) || !Context.IsWorldReady)
                return;

            if (sprinting && Game1.player.Stamina > 0.0f)
            {
                float oldst = Game1.player.Stamina;

                Game1.player.Stamina -= config.StaminaLossPerHalfSecond;

                //Game1.chatBox.addMessage($"OS: {oldst} -> S: {Game1.player.Stamina}", Color.DeepPink);
                //this.Monitor.Log("sprint - 2", LogLevel.Info);

                if (Game1.player.addedSpeed == dSpeed)
                    Game1.player.addedSpeed = dSpeed + factor;

            }
            else if (sprinting && Game1.player.Stamina <= 0.0f)
            {
                sprinting = false;
            }


            if (!sprinting && Game1.player.addedSpeed == dSpeed + factor)
            {
                Game1.player.addedSpeed = dSpeed;
            }



        }

        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null || Game1.player.Stamina <= 0f)
                return;
            
            if (e.Button == config.SprintKey)
            {
                sprinting = true;
            }
        }

        private void InputEvents_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == config.SprintKey)
            {
                sprinting = false;
            }
        }

    }

    public class ModConfig
    {
        public int SprintSpeedIncrease { get; set; } = 2;
        public float StaminaLossPerHalfSecond { get; set; } = 1.0f;
        public SButton SprintKey { get; set; } = SButton.LeftControl;

    }

}
