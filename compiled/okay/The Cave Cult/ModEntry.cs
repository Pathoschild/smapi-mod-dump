using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;
using StardewValley.Monsters;
using static System.Formats.Asn1.AsnWriter;
using StardewValley.Characters;
using StardewValley.GameData.LocationContexts;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using xTile.Dimensions;

namespace CaveCultCode
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        public int numberOfKills = 0;
        public int neededKills = 5;

        int numOfMonstersLastFrame = 0;




        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Player.Warped += Player_Warped;
        }



        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            //if (e.Button == SButton.V)
            //{
            //    SpawnWeapon();
            //}
            //if (e.Button == SButton.B)
            //{
            //    this.Monitor.Log($"{Game1.player.CurrentTool.DisplayName} has {numberOfKills}", LogLevel.Debug);
            //}
            //if (e.Button == SButton.N)
            //{
            //    numberOfKills = 10;
            //}
            //if (e.Button == SButton.P)
            //{
            //    activateAltar(10);
            //}
            if (e.Button == SButton.MouseLeft)
            {
                if (Game1.player.CurrentTool.DisplayName == "Ritual Dagger")
                {
                    if (Game1.currentCursorTile == new Vector2(21, 31))
                    {
                        activateAltar(numberOfKills);
                    }
                }
            }

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void OnUpdateTicked(object? sender, EventArgs e)
        {   
            
            if (Game1.currentLocation != null)
            {
                int numOfMonsters = 0;
                foreach (NPC npc in Game1.currentLocation.characters)
                {
                    if (npc is Monster)
                    {
                        numOfMonsters++;
                    }
                }

                if (numOfMonsters > numOfMonstersLastFrame)
                {
                    numOfMonstersLastFrame = numOfMonsters;
                    this.Monitor.Log($"Monsters added. Total monsters: {numOfMonstersLastFrame}", LogLevel.Debug);
                }

                if (numOfMonsters < numOfMonstersLastFrame)
                {
                    if (Game1.player.CurrentTool.DisplayName == "Ritual Dagger")
                    {
                        switch (numberOfKills)
                        {
                            case 0:
                                Game1.addHUDMessage(new HUDMessage("The monster's energy seems to be absorbed into the dagger", 2));
                                break;
                            case 4:
                                Game1.addHUDMessage(new HUDMessage("The dagger's energy seems to be growing", 2));
                                break;
                            case 9:
                                Game1.addHUDMessage(new HUDMessage("The dagger hums with mysterious energy", 2));
                                break;
                            case 14:
                                Game1.addHUDMessage(new HUDMessage("The dagger has started glowing", 2));
                                break;
                            case 19:
                                Game1.addHUDMessage(new HUDMessage("The dagger is overflowing with energy", 2));
                                break;
                        }
                        numberOfKills += numOfMonstersLastFrame - numOfMonsters;
                        this.Monitor.Log($"{numberOfKills} monsters killed. Total kills: {numberOfKills}", LogLevel.Debug);
                    }
                    numOfMonstersLastFrame = numOfMonsters;
                }
            }
        }

        public void SpawnWeapon()
        {

            MeleeWeapon weapon = new MeleeWeapon("65");
            weapon.ParentSheetIndex = 65;
            Game1.player.addItemToInventory(weapon);
        }

        public void activateAltar(int numberOfKills)
        {
            if (numberOfKills > 5)
            {
                this.numberOfKills = 0;
                Game1.addHUDMessage(new HUDMessage("Something happened", 2));
                rainTomorrow(Game1.player);

            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Hmmm doesn't seem to be working... I should come back when I've charged it up more.", 2));
            }
        }

        private void rainTomorrow(Farmer who)
        {
            GameLocation currentLocation = who.currentLocation;
            string text = currentLocation.GetLocationContextId();
            LocationContextData locationContext = currentLocation.GetLocationContext();
            if (!locationContext.AllowRainTotem)
            {
                Game1.showRedMessageUsingLoadString("Strings\\UI:Item_CantBeUsedHere");
                return;
            }

            if (locationContext.RainTotemAffectsContext != null)
            {
                text = locationContext.RainTotemAffectsContext;
            }

            bool flag = false;
            if (text == "Default")
            {
                if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season))
                {
                    Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = "Rain");
                    flag = true;
                }
            }
            else
            {
                currentLocation.GetWeather().WeatherForTomorrow = "Rain";
                flag = true;
            }

            if (flag)
            {
                Game1.pauseThenMessage(2000, Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"));
            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            Monster[] slimes = new Monster[3];

            if(e.NewLocation.Name == "Custom_Cave_Level1")
            {
               // Game1.hudMessages.Add(new HUDMessage("Welcome to" + e.NewLocation.Name, HUDMessage.error_type));
                slimes[0] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(20 * 64, 25 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[1] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(26 * 64, 26 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[2] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(30 * 64, 28 * 64), Microsoft.Xna.Framework.Color.BlueViolet);

                foreach(Monster slime in slimes)
                {
                    Game1.currentLocation.characters.Add(slime);
                }

                Array.Clear(slimes);
            }

            if (e.NewLocation.Name == "Custom_Cave_Level2")
            {
               // Game1.hudMessages.Add(new HUDMessage("Welcome to" + e.NewLocation.Name, HUDMessage.error_type));
                slimes[0] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(9 * 64, 15 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[1] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(16 * 64, 16 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[2] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(25 * 64, 13 * 64), Microsoft.Xna.Framework.Color.BlueViolet);

                foreach (Monster slime in slimes)
                {
                    Game1.currentLocation.characters.Add(slime);
                }

                Array.Clear(slimes);
            }

            if (e.NewLocation.Name == "Custom_Cave_Level3")
            {
               // Game1.hudMessages.Add(new HUDMessage("Welcome to" + e.NewLocation.Name, HUDMessage.error_type));
                slimes[0] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(10 * 64, 19 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[1] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(18 * 64, 20 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
                slimes[2] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(26 * 64, 17 * 64), Microsoft.Xna.Framework.Color.BlueViolet);

                foreach (Monster slime in slimes)
                {
                    Game1.currentLocation.characters.Add(slime);
                }

                Array.Clear(slimes);
            }
        }
    }
}