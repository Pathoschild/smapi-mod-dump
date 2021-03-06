/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Stardew-Valley-Mods
**
*************************************************/

//System
using System;
using System.Collections.Generic;
using System.Linq;
//Stardew API
using StardewModdingAPI;
using StardewModdingAPI.Events;
//Stardew
using StardewValley;
//Framework
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Survivalistic
{
    class ModEntry : Mod
    {
        #region Variables
        //Instance Variable
        public static ModEntry instance;

        //Multiplier Variables
        public float staminaDecayMult = 1;
        public float hungerDecayMult = 0.003f;
        public float thirstDecayMult = 0.005f;

        //Color Variables
        private Color hungerColor;
        private Color thirstColor;

        //Texture Variables
        private Texture2D texHungerBar, texThirstBar, texFiller;
        
        //Progress Bar Variables
        private int defaultProgressBarSize = 168;

        //Status Variables
        private float maxHunger = 100;
        private float maxThirst = 100;
        private float hungerPercentage, thirstPercentage;

        //Position Variables
        private int barPosX, barPosY;

        //References
        private Farmer player;

        //Controlation Variables
        public bool canUpdate = true;
        private bool findItemInList = false;
        private bool isEating = false;
        private bool canWarnHunger = true, canWarnThirst = true;
        private bool searchingToolItem = false;

        //Data Variables
        public static SaveData data;
        public static ModConfig config;

        //Storage Variables
        public IDictionary<string, string> compatibleFoodMods, compatibleToolMods;
        public IDictionary<string, string> actionCost;
        public IDictionary<string, string> itemList;
        public IDictionary<string, string>[] foodModCompatibility;
        public IDictionary<string, string>[] toolModCompatibility;

        //Language Variables
        public IDictionary<string, string> language;
        #endregion

        public override void Entry(IModHelper helper)
        {
            //Set instance variable
            instance = this;

            //Load config file
            config = this.Helper.ReadConfig<ModConfig>();

            //Load mod compatibility list and itemlists
            compatibleFoodMods = helper.Content.Load<Dictionary<string, string>>("compatibility/compatibility_foodmods.json", ContentSource.ModFolder);
            //Load mod compatibility tools
            compatibleToolMods = helper.Content.Load<Dictionary<string, string>>("compatibility/compatibility_toolsmods.json", ContentSource.ModFolder);
            //Load the customs item and tool lists
            checkCompatibleMods(helper);
            //Load the item list
            itemList = helper.Content.Load<Dictionary<string, string>>("assets/vanilla_lists/vanilla_itemlist.json", ContentSource.ModFolder);
            //Load action cost list
            actionCost = helper.Content.Load<Dictionary<string, string>>("assets/vanilla_lists/vanilla_toolslist.json", ContentSource.ModFolder);
            //Load current language file
            language = helper.Content.Load<Dictionary<string, string>>("lang/" + config.Language + ".json", ContentSource.ModFolder);

            //Links the events
            helper.Events.Display.RenderedHud += this.onRenderedHud;
            helper.Events.GameLoop.UpdateTicking += this.onUpdate;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.DayEnding += this.onDayEnding;
            helper.Events.Multiplayer.ModMessageReceived += this.onModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += this.onPeerContextReceived;

            //Get the assets
            texHungerBar = helper.Content.Load<Texture2D>("assets/sprites/texHungerBar.png", ContentSource.ModFolder);
            texThirstBar = helper.Content.Load<Texture2D>("assets/sprites/texThirstBar.png", ContentSource.ModFolder);
            texFiller = helper.Content.Load<Texture2D>("assets/sprites/texFiller.png", ContentSource.ModFolder);

            //Set bar filler colors
            hungerColor = config.HungerBarColor;
            thirstColor = config.ThirstBarColor;
        }

        //Check for compatible mods in the list
        private void checkCompatibleMods(IModHelper helper)
        {
            int foundMods = 0;
            int countSupport = 0;
            
            //Check for the quantity of loaded food mods
            foreach (string t in compatibleFoodMods.Keys)
            {
                if (this.Helper.ModRegistry.IsLoaded(t))
                {
                    foundMods++;
                }
            }
            foodModCompatibility = new IDictionary<string, string>[foundMods];
            foreach (string t in compatibleFoodMods.Keys)
            {
                if (this.Helper.ModRegistry.IsLoaded(t))
                {
                    foodModCompatibility[countSupport] = helper.Content.Load<Dictionary<string, string>>("compatibility/compatibility_itemlists/" + compatibleFoodMods[t] + ".json", ContentSource.ModFolder);
                    countSupport++;
                }
            }

            //Reset mods found count
            foundMods = 0;
            countSupport = 0;

            //Check for the quantity of loaded tools mods
            foreach (string t in compatibleToolMods.Keys)
            {
                if (this.Helper.ModRegistry.IsLoaded(t))
                {
                    foundMods++;
                }
            }
            toolModCompatibility = new IDictionary<string, string>[foundMods];
            foreach (string t in compatibleToolMods.Keys)
            {
                if (this.Helper.ModRegistry.IsLoaded(t))
                {
                    toolModCompatibility[countSupport] = helper.Content.Load<Dictionary<string, string>>("compatibility/compatibility_toolscost/" + compatibleToolMods[t] + ".json", ContentSource.ModFolder);
                    countSupport++;
                }
            }
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Set default variable informations
            player = Game1.player;

            //Get the status information from the save
            if (Context.IsMainPlayer)
            {
                data = Helper.Data.ReadSaveData<SaveData>($"Ophaneom.Survivalistic.{Game1.player.UniqueMultiplayerID}") ?? new SaveData();
            }
        }

        //Get the player connection before the approvation
        private void onPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            if (Game1.IsServer)
            {
                data = Helper.Data.ReadSaveData<SaveData>($"Ophaneom.Survivalistic.{e.Peer.PlayerID}") ?? new SaveData();
                Helper.Multiplayer.SendMessage(data, "MESSAGE_KEY", null, new long[] { e.Peer.PlayerID });
            }
        }

        //Updates player information
        private void updateInformation()
        {
            data.SyncToHost();
        }

        //Send to all players the necessity to save the status information
        public void onModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "MESSAGE_KEY")
            {
                var _data = e.ReadAs<SaveData>();
                if (Context.IsMainPlayer)
                {
                    Helper.Data.WriteSaveData<SaveData>($"Ophaneom.Survivalistic{e.FromPlayerID}", _data);
                }
                else
                {
                    data = _data;
                }
            }
        }

        //Reset the status when day starting
        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            data.hunger = maxHunger;
            data.thirst = maxThirst;
        }

        private void onRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Context.IsWorldReady && Game1.CurrentEvent == null)
            {
                //Gets the actual size of viewport
                int sizeX = Game1.uiViewport.Width;
                int sizeY = Game1.uiViewport.Height;

                //Sets the bar positions
                setBarsPosition(sizeX, sizeY);

                //Draw the bars
                e.SpriteBatch.Draw(texHungerBar, new Rectangle(barPosX, barPosY - 240, texHungerBar.Width * 4, texHungerBar.Height * 4), Color.White);
                e.SpriteBatch.Draw(texThirstBar, new Rectangle(barPosX - 60, barPosY - 240, texThirstBar.Width * 4, texThirstBar.Height * 4), Color.White);

                //Draw the fillers
                e.SpriteBatch.Draw(texFiller, new Vector2(barPosX + 36, barPosY - 25), new Rectangle(0, 0, texFiller.Width * 4, (int)hungerPercentage), hungerColor, 3.138997f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 1f);
                e.SpriteBatch.Draw(texFiller, new Vector2(barPosX - 24, barPosY - 25), new Rectangle(0, 0, texFiller.Width * 4, (int)thirstPercentage), thirstColor, 3.138997f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 1f);
            }
        }

        //Set the bars position
        private void setBarsPosition(int uiSizeX, int uiSizeY)
        {
            //Store the current location name info
            string playerCurrentLocation = player.currentLocation.Name;
            switch (config.Position)
            {
                //Sets the position to bottom-right / Adjusts bar position if the players health are active
                case "bottom-right":
                    if (playerCurrentLocation.Contains("UndergroundMine") || playerCurrentLocation.Contains("VolcanoDungeon") || player.health < player.maxHealth)
                    {
                        barPosX = uiSizeX - 171;
                    }
                    else
                    {
                        barPosX = uiSizeX - 116;
                    }
                    barPosY = uiSizeY;
                    break;

                //Sets the position to bottom-left
                case "bottom-left":
                    barPosX = 70;
                    barPosY = uiSizeY;
                    break;

                //Sets the position to middle-right
                case "middle-right":
                    barPosX = uiSizeX - 56;
                    barPosY = (uiSizeY / 2) + 75;
                    break;

                //Sets the position to middle-left
                case "middle-left":
                    barPosX = 70;
                    barPosY = (uiSizeY / 2) + 75;
                    break;

                //Sets the position to top-right / Adjusts bar position if the player had any buff
                case "top-right":
                    barPosX = uiSizeX - 365;
                    if (player.appliedSpecialBuffs != null)
                    {
                        barPosY = 325;
                    }
                    else
                    {
                        barPosY = 290;
                    }
                    break;

                //Sets the position to top-left / Adjusts bar position if player are inside some mine/dungeon level
                case "top-left":
                    barPosX = 70;
                    if (playerCurrentLocation.Contains("UndergroundMine"))
                    {
                        barPosY = 320;
                    }
                    else if (playerCurrentLocation.Contains("VolcanoDungeon") && playerCurrentLocation != "VolcanoDungeon0")
                    {
                        barPosY = 320;
                    }
                    else
                    {
                        barPosY = 260;
                    }
                    break;

                //Sets the position to a custom position
                case "custom":
                    barPosX = config.CustomPositionX;
                    barPosY = config.CustomPositionY;
                    break;
            }
        }

        private void onUpdate(object sender, UpdateTickingEventArgs e)
        {
            checkPauseStatus();
            if (canUpdate)
            {
                updateInformation();

                //Call the percentage updates
                calculatePercentages();

                //Consumes hunger and thirsty based on the multipliers
                consumesHunger();
                consumesThirst();

                //Check if player are consuming something
                checkEating();

                //Verify if the player was starving or thirsting
                if (data.hunger <= 0 || data.thirst <= 0)
                {
                    damageLoop();
                }

                //Normalize hunger and thirst status
                statusNormalizer();

                //Checks if hunger or thrist are below a minimum value, needing to send a warning message
                sendWarningBanner();

                //Check if the player are doing some action
                actionPenalty();
            }
        }

        //Check the pause status of the game
        private void checkPauseStatus()
        {
            if (Context.IsWorldReady && Game1.CurrentEvent == null && Game1.activeClickableMenu == null)
            {
                canUpdate = true;
            }
            else
            {
                //If the server has more than 1 player (host) it will keep updating
                if (Multiplayer.AllPlayers > 1)
                {
                    canUpdate = true;
                }
                else
                {
                    canUpdate = false;
                }
            }
        }

        //Those 2 voids updates the hunger and thirst percentages
        private void calculatePercentages()
        {
            hungerPercentage = (data.hunger / maxHunger) * defaultProgressBarSize;
            thirstPercentage = (data.thirst / maxThirst) * defaultProgressBarSize;
        }

        //Verify if player needs to receive damage
        private void damageLoop()
        {
            //Deals the damage
            if (player.stamina > 0)
            {
                player.stamina -= (int)staminaDecayMult;
            }
        }

        //Consumes hunger and thirst
        private void consumesHunger()
        {
            data.hunger -= hungerDecayMult;
        }
        private void consumesThirst()
        {
            data.thirst -= thirstDecayMult;
        }

        //Normalize the actual status if are above max status or below minimum
        private void statusNormalizer()
        {
            if (data.hunger > maxHunger)
            {
                data.hunger = maxHunger;
            }
            else if (data.hunger < 0)
            {
                data.hunger = 0;
            }

            if (data.thirst > maxThirst)
            {
                data.thirst = maxThirst;
            }
            else if (data.thirst < 0)
            {
                data.thirst = 0;
            }
        }

        //Verify if the player are eating/drinking something
        private void checkEating()
        {
            if (player.isEating && !isEating)
            {
                isEating = true;
                inflictStatusIncrease();
            }

            if (!player.isEating)
            {
                isEating = false;
            }
        }

        //Increase the player status based on the food status
        private void inflictStatusIncrease()
        {
            //Verify if the player is eating
            findItemInList = false;
            int lastHunger, lastThirst;

            //Verify if the tool base name are in vanilla_itemlist.json
            foreach (string t in itemList.Keys)
            {
                if (player.itemToEat.Name == t)
                {
                    List<string> idstatus = itemList[t].Split('/').ToList<string>();

                    lastHunger = (int)data.hunger;
                    lastThirst = (int)data.thirst;

                    data.hunger += Int32.Parse(idstatus[0]);
                    data.thirst += Int32.Parse(idstatus[1]);

                    this.Monitor.Log($"Food display name: {player.itemToEat.DisplayName}", LogLevel.Info);
                    this.Monitor.Log($"Item internal name: {player.itemToEat.Name}", LogLevel.Info);
                    this.Monitor.Log($"Hunger added: {Int32.Parse(idstatus[0])}", LogLevel.Info);
                    this.Monitor.Log($"Thirst added: {Int32.Parse(idstatus[1])}", LogLevel.Info);

                    findItemInList = true;
                    sendStatusIncreaseBanner(lastHunger, lastThirst, Int32.Parse(idstatus[0]), Int32.Parse(idstatus[1]));
                    break;
                }
            }

            //Verify if the item base name are in some custom.json
            if (!findItemInList)
            {
                foreach (IDictionary<string, string> itemlist in foodModCompatibility)
                {
                    foreach (string t in itemlist.Keys)
                    {
                        if (player.itemToEat.Name == t)
                        {
                            List<string> idstatus = itemlist[t].Split('/').ToList<string>();

                            lastHunger = (int)data.hunger;
                            lastThirst = (int)data.thirst;

                            data.hunger += Int32.Parse(idstatus[0]);
                            data.thirst += Int32.Parse(idstatus[1]);

                            this.Monitor.Log($"Food display name: {player.itemToEat.DisplayName}", LogLevel.Info);
                            this.Monitor.Log($"Item internal name: {player.itemToEat.Name}", LogLevel.Info);
                            this.Monitor.Log($"Hunger added: {Int32.Parse(idstatus[0])}", LogLevel.Info);
                            this.Monitor.Log($"Thirst added: {Int32.Parse(idstatus[1])}", LogLevel.Info);

                            sendStatusIncreaseBanner(lastHunger, lastThirst, Int32.Parse(idstatus[0]), Int32.Parse(idstatus[1]));
                            break;
                        }
                    }
                }
            }
        }

        //Shows the message about hunger and thirst gain before eating/drinking
        private void sendStatusIncreaseBanner(int lastHunger, int lastThirst, int foodHungerValue, int foodThirstValue)
        {
            int diffHunger = lastHunger + foodHungerValue;
            int diffThirst = lastThirst + foodThirstValue;

            //Send the hunger message
            if (diffHunger <= maxHunger && foodThirstValue != 0)
            {
                Game1.addHUDMessage(new HUDMessage($"{language["HungerWord"]} + {foodHungerValue}", 4));
            }
            else if (diffHunger > maxHunger)
            {
                Game1.addHUDMessage(new HUDMessage($"{language["HungerWord"]} + {maxHunger - lastHunger}", 4));
            }

            //Send the hunger message
            if (diffThirst <= maxThirst && foodThirstValue != 0)
            {
                Game1.addHUDMessage(new HUDMessage($"{language["ThirstWord"]} + {foodThirstValue}", 4));
            }
            else if (diffThirst > maxThirst)
            {
                Game1.addHUDMessage(new HUDMessage($"{language["ThirstWord"]} + {maxThirst - lastThirst}", 4));
            }
        }

        //Send a warning to player when hunger or thirst are low
        private void sendWarningBanner()
        {
            //Send starving warning
            if (data.hunger <= 15)
            {
                if (canWarnHunger)
                {
                    canWarnHunger = false;
                    Game1.addHUDMessage(new HUDMessage($"{language["StarvingMessage"]}", 3));
                }
            }
            //Resets state
            else
            {
                canWarnHunger = true;
            }
            //Send thirsting warning
            if (data.thirst <= 15)
            {
                if (canWarnThirst)
                {
                    canWarnThirst = false;
                    Game1.addHUDMessage(new HUDMessage($"{language["ThirstingMessage"]}", 3));
                }
            }
            //Resets state
            else
            {
                canWarnThirst = true;
            }
        }

        //Decreases hunger/thirst when player are doing some action
        private void actionPenalty()
        {
            if (player.UsingTool)
            {
                //Lock the search
                if (!searchingToolItem)
                {
                    searchingToolItem = true;
                    bool findToolInList = false;

                    //Verify if the tool base name are in vanilla_toolslist.json
                    foreach (string t in actionCost.Keys)
                    {
                        if (player.CurrentTool.BaseName == t)
                        {
                            List<string> statusCost = actionCost[t].Split('/').ToList<string>();

                            data.hunger -= float.Parse(statusCost[0]);
                            data.thirst -= float.Parse(statusCost[1]);

                            this.Monitor.Log($"Tool display name: {player.CurrentTool.DisplayName}", LogLevel.Info);
                            this.Monitor.Log($"Item internal name: {player.CurrentTool.BaseName}", LogLevel.Info);
                            this.Monitor.Log($"Hunger penalty: {float.Parse(statusCost[0])}", LogLevel.Info);
                            this.Monitor.Log($"Thirst penalty: {float.Parse(statusCost[1])}", LogLevel.Info);

                            findToolInList = true;
                            break;
                        }
                    }
                    //Verify if the tool base name are in some custom.json
                    if (!findToolInList)
                    {
                        foreach (IDictionary<string, string> toollist in toolModCompatibility)
                        {
                            foreach (string t in toollist.Keys)
                            {
                                if (player.CurrentTool.BaseName == t)
                                {
                                    List<string> statusCost = toollist[t].Split('/').ToList<string>();

                                    data.hunger -= float.Parse(statusCost[0]);
                                    data.thirst -= float.Parse(statusCost[1]);

                                    this.Monitor.Log($"Tool display name: {player.CurrentTool.DisplayName}", LogLevel.Info);
                                    this.Monitor.Log($"Item internal name: {player.CurrentTool.BaseName}", LogLevel.Info);
                                    this.Monitor.Log($"Hunger penalty: {float.Parse(statusCost[0])}", LogLevel.Info);
                                    this.Monitor.Log($"Thirst penalty: {float.Parse(statusCost[1])}", LogLevel.Info);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            //Resets the lock
            else if (!player.UsingTool)
            {
                searchingToolItem = false;
            }
        }
    }
}
