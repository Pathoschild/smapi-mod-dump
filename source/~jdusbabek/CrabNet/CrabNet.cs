/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System;
using System.Collections.Generic;
using CrabNet.Framework;
using Microsoft.Xna.Framework;
using StardewLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CrabNet
{
    public class CrabNet : Mod
    {
        /*********
        ** Properties
        *********/
        // Local variable for config setting "keybind"
        private SButton ActionKey;

        // Local variable for config setting "loggingEnabled"
        private bool LoggingEnabled;

        // Local variable for config setting "preferredBait"
        private int BaitChoice = 685;

        // Local variable for config setting "free"
        private bool Free;

        // Local variable for config setting "chargeForBait"
        private bool ChargeForBait = true;

        // Local variable for config setting "costPerCheck"
        private int CostPerCheck = 1;

        // Local variable for config setting "costPerEmpty"
        private int CostPerEmpty = 10;

        // Local variable for config setting "whoChecks"
        private string Checker = "CrabNet";

        // Local variable for config setting "enableMessages"
        private bool EnableMessages = true;

        // Local variable for config setting "chestCoords"
        private Vector2 ChestCoords = new Vector2(73f, 14f);

        // Local variable for config setting "bypassInventory"
        private bool BypassInventory;

        // Local variable for config setting "allowFreebies"
        private bool AllowFreebies = true;

        // The cost per 1 bait, as determined from the user's bait preference
        private int BaitCost;

        // An indexed list of all messages from the dialog.xna file
        private Dictionary<string, string> AllMessages;

        // An indexed list of key dialog elements, these need to be indexed in the order in the file ie. cannot be randomized.
        private Dictionary<int, string> Dialog;

        // An indexed list of greetings.
        private Dictionary<int, string> Greetings;

        // An indexed list of all dialog entries relating to "unfinished"
        private Dictionary<int, string> UnfinishedMessages;

        // An indexed list of all dialog entries related to "freebies"
        private Dictionary<int, string> FreebieMessages;

        // An indexed list of all dialog entries related to "inventory full"
        private Dictionary<int, string> InventoryMessages;

        // An indexed list of all dialog entries related to "smalltalk".  This list is merged with a list of dialogs that are specific to your "checker"
        private Dictionary<int, string> Smalltalk;

        // Random number generator, used primarily for selecting dialog messages.
        private readonly Random Random = new Random();

        // A flag for when an item could not be deposited into either the inventory or the chest.
        private bool InventoryAndChestFull;

        // The configuration object.  Not used per-se, only to populate the local variables.
        private ModConfig Config;

        private DialogueManager DialogueManager;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.DialogueManager = new DialogueManager(this.Config, helper.Content, this.Monitor);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Enum.TryParse(this.Config.KeyBind, true, out this.ActionKey))
            {
                this.ActionKey = SButton.H;
                this.Monitor.Log($"Error parsing key binding; defaulted to {this.ActionKey}.");
            }

            this.LoggingEnabled = this.Config.EnableLogging;

            // 685, or 774
            if (this.Config.PreferredBait == 685 || this.Config.PreferredBait == 774)
                this.BaitChoice = this.Config.PreferredBait;
            else
                this.BaitChoice = 685;

            this.BaitCost = new SObject(this.BaitChoice, 1).Price;
            this.ChargeForBait = this.Config.ChargeForBait;
            this.CostPerCheck = Math.Max(0, this.Config.CostPerCheck);
            this.CostPerEmpty = Math.Max(0, this.Config.CostPerEmpty);
            this.Free = this.Config.Free;
            this.Checker = this.Config.WhoChecks;
            this.EnableMessages = this.Config.EnableMessages;
            this.ChestCoords = this.Config.ChestCoords;
            this.BypassInventory = this.Config.BypassInventory;
            this.AllowFreebies = this.Config.AllowFreebies;

            this.ReadInMessages();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == this.ActionKey)
            {
                try
                {
                    this.IterateOverCrabPots();
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Exception onKeyReleased: {ex}", LogLevel.Error);
                }
            }
        }

        private void IterateOverCrabPots()
        {
            // reset this each time invoked, it is a flag to determine if uncompleted work is due to inventory or money.
            this.InventoryAndChestFull = false;
            CrabNetStats stats = new CrabNetStats();

            foreach (GameLocation location in Game1.locations)
            {
                if (location.IsOutdoors)
                {
                    foreach (SObject obj in location.Objects.Values)
                    {
                        if (obj.Name == "Crab Pot")
                        {
                            stats.NumTotal++;

                            if (!this.Free && !this.CanAfford(Game1.player, this.CostPerCheck, stats) && !this.AllowFreebies)
                            {
                                this.Monitor.Log("Couldn't afford to check.", LogLevel.Trace);
                                stats.NotChecked++;
                                continue;
                            }

                            stats.NumChecked++;
                            stats.RunningTotal += this.CostPerCheck;

                            CrabPot pot = (CrabPot)obj;

                            if (pot.heldObject.Value != null && pot.heldObject.Value.Category != -21)
                            {
                                if (!this.Free && !this.CanAfford(Game1.player, this.CostPerEmpty, stats) && !this.AllowFreebies)
                                {
                                    this.Monitor.Log("Couldn't afford to empty.", LogLevel.Trace);
                                    stats.NotEmptied++;
                                    continue;
                                }

                                if (this.CheckForAction(Game1.player, pot, stats))
                                {
                                    stats.NumEmptied++;
                                    stats.RunningTotal += this.CostPerEmpty;
                                }
                            }
                            else
                            {
                                stats.NothingToRetrieve++;
                            }

                            if (pot.bait.Value == null && pot.heldObject.Value == null && !Game1.player.professions.Contains(11))
                            {
                                SObject b = new SObject(this.BaitChoice, 1);

                                if (!this.Free && !this.CanAfford(Game1.player, this.BaitCost, stats) && !this.AllowFreebies && this.ChargeForBait)
                                {
                                    this.Monitor.Log("Couldn't afford to bait.", LogLevel.Trace);
                                    stats.NotBaited++;
                                    continue;
                                }

                                if (this.PerformObjectDropInAction(b, Game1.player, pot))
                                {
                                    stats.NumBaited++;
                                    if (this.ChargeForBait)
                                        stats.RunningTotal += this.BaitCost;
                                }
                            }
                            else
                            {
                                stats.NothingToBait++;
                            }

                        }
                    }
                }
            }

            int totalCost = (stats.NumChecked * this.CostPerCheck);
            totalCost += (stats.NumEmptied * this.CostPerEmpty);
            if (this.ChargeForBait)
            {
                totalCost += (this.BaitCost * stats.NumBaited);
            }
            if (this.Free)
            {
                totalCost = 0;
            }

            if (this.LoggingEnabled)
            {
                this.Monitor.Log($"CrabNet checked {stats.NumChecked} pots. You used {stats.NumBaited} bait to reset.", LogLevel.Trace);
                if (!this.Free)
                    this.Monitor.Log($"Total cost was {totalCost}g. Checks: {stats.NumChecked * this.CostPerCheck}, Emptied: {stats.NumEmptied * this.CostPerEmpty}, Bait: {stats.NumBaited * this.BaitCost}", LogLevel.Trace);
            }

            if (!this.Free)
                Game1.player.Money = Math.Max(0, Game1.player.Money + (-1 * totalCost));

            if (this.EnableMessages)
            {
                this.ShowMessage(stats, totalCost);
            }
        }

        private bool CheckForAction(Farmer farmer, CrabPot pot, CrabNetStats stats)
        {
            if (!this.CanAfford(farmer, this.CostPerCheck, stats))
                return false;

            if (pot.tileIndexToShow == 714)
            {
                if (farmer.IsMainPlayer && !this.AddItemToInventory(pot.heldObject.Value, farmer, Game1.getFarm()))
                {
                    Game1.addHUDMessage(new HUDMessage("Inventory Full", Color.Red, 3500f));
                    return false;
                }
                Dictionary<int, string> dictionary = this.Helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);
                if (this.GetFishSize(pot.heldObject.Value.ParentSheetIndex, out int minSize, out int maxSize))
                    farmer.caughtFish(pot.heldObject.Value.ParentSheetIndex, Game1.random.Next(minSize, maxSize + 1));
                pot.readyForHarvest.Value = false;
                pot.heldObject.Value = null;
                pot.tileIndexToShow = 710;
                pot.bait.Value = null;
                farmer.gainExperience(1, 5);

                return true;
            }
            return false;
        }

        /// <summary>Get the minimum and maximum size for a fish.</summary>
        /// <param name="parentSheetIndex">The parent sheet index for the fish.</param>
        /// <param name="minSize">The minimum fish size.</param>
        /// <param name="maxSize">The maximum fish size.</param>
        private bool GetFishSize(int parentSheetIndex, out int minSize, out int maxSize)
        {
            minSize = -1;
            maxSize = -1;

            // get data
            Dictionary<int, string> data = this.Helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);
            if (!data.TryGetValue(parentSheetIndex, out string rawFields) || rawFields == null || !rawFields.Contains("/"))
                return false;

            // get field indexes
            string[] fields = rawFields.Split('/');
            int minSizeIndex = fields[1] == "trap"
                ? 5
                : 3;
            int maxSizeIndex = minSizeIndex + 1;
            if (fields.Length <= maxSizeIndex)
                return false;

            // parse fields
            if (!int.TryParse(fields[minSizeIndex], out minSize))
                minSize = 1;
            if (!int.TryParse(fields[maxSizeIndex], out maxSize))
                maxSize = 10;
            return true;
        }

        private bool PerformObjectDropInAction(SObject dropIn, Farmer farmer, CrabPot pot)
        {
            if (pot.bait.Value != null || farmer.professions.Contains(11))
                return false;

            pot.bait.Value = dropIn;

            return true;
        }

        private bool AddItemToInventory(SObject obj, Farmer farmer, Farm farm)
        {
            bool wasAdded = false;

            if (farmer.couldInventoryAcceptThisItem(obj) && !this.BypassInventory)
            {
                farmer.addItemToInventory(obj);
                wasAdded = true;

                if (this.LoggingEnabled)
                    this.Monitor.Log("Was able to add item to inventory.", LogLevel.Trace);
            }
            else
            {
                farm.objects.TryGetValue(this.ChestCoords, out SObject chestObj);

                if (chestObj is Chest chest)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Found a chest at {(int)this.ChestCoords.X},{(int)this.ChestCoords.Y}", LogLevel.Trace);

                    Item i = chest.addItem(obj);
                    if (i == null)
                    {
                        wasAdded = true;

                        if (this.LoggingEnabled)
                            this.Monitor.Log("Was able to add items to chest.", LogLevel.Trace);
                    }
                    else
                    {
                        this.InventoryAndChestFull = true;

                        if (this.LoggingEnabled)
                            this.Monitor.Log("Was NOT able to add items to chest.", LogLevel.Trace);
                    }

                }
                else
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Did not find a chest at {(int)this.ChestCoords.X},{(int)this.ChestCoords.Y}", LogLevel.Trace);

                    // If bypassInventory is set to true, but there's no chest: try adding to the farmer's inventory.
                    if (this.BypassInventory)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"No chest at {(int)this.ChestCoords.X},{(int)this.ChestCoords.Y}, you should place a chest there, or set bypassInventory to \'false\'.", LogLevel.Trace);

                        if (farmer.couldInventoryAcceptThisItem(obj))
                        {
                            farmer.addItemToInventory(obj);
                            wasAdded = true;

                            if (this.LoggingEnabled)
                                this.Monitor.Log("Was able to add item to inventory. (No chest found, bypassInventory set to 'true')", LogLevel.Trace);
                        }
                        else
                        {
                            this.InventoryAndChestFull = true;

                            if (this.LoggingEnabled)
                                this.Monitor.Log("Was NOT able to add item to inventory or a chest.  (No chest found, bypassInventory set to 'true')", LogLevel.Trace);
                        }
                    }
                    else
                    {
                        this.InventoryAndChestFull = true;

                        if (this.LoggingEnabled)
                            this.Monitor.Log("Was NOT able to add item to inventory or a chest.  (No chest found, bypassInventory set to 'false')", LogLevel.Trace);
                    }
                }
            }

            return wasAdded;
        }

        private bool CanAfford(Farmer farmer, int amount, CrabNetStats stats)
        {
            // Calculate the running cost (need config passed for that) and determine if additional puts you over.
            return (amount + stats.RunningTotal) <= farmer.Money;
        }


        private void ShowMessage(CrabNetStats stats, int totalCost)
        {
            string message = "";

            if (this.Checker.ToLower() == "spouse")
            {
                if (Game1.player.isMarried())
                    message += this.DialogueManager.PerformReplacement(this.Dialog[1], stats, this.Config);
                else
                    message += this.DialogueManager.PerformReplacement(this.Dialog[2], stats, this.Config);

                if (totalCost > 0 && !this.Free)
                    message += this.DialogueManager.PerformReplacement(this.Dialog[3], stats, this.Config);

                HUDMessage msg = new HUDMessage(message);
                Game1.addHUDMessage(msg);
            }
            else
            {
                NPC character = Game1.getCharacterFromName(this.Checker);
                if (character != null)
                {
                    message += this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.Greetings), stats, this.Config);
                    message += " " + this.DialogueManager.PerformReplacement(this.Dialog[4], stats, this.Config);

                    if (!this.Free)
                    {
                        this.DialogueManager.PerformReplacement(this.Dialog[5], stats, this.Config);

                        if (stats.HasUnfinishedBusiness())
                        {
                            if (this.InventoryAndChestFull)
                            {
                                message += this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.InventoryMessages), stats, this.Config);
                            }
                            else
                            {
                                if (this.AllowFreebies)
                                {
                                    message += this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.FreebieMessages), stats, this.Config);
                                }
                                else
                                {
                                    message += " " + this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.UnfinishedMessages), stats, this.Config);
                                }
                            }
                        }

                        message += this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.Smalltalk), stats, this.Config);
                        message += "#$e#";
                    }
                    else
                    {
                        message += this.DialogueManager.PerformReplacement(this.GetRandomMessage(this.Smalltalk), stats, this.Config);
                        message += "#$e#";
                    }

                    character.CurrentDialogue.Push(new Dialogue(message, character));
                    Game1.drawDialogue(character);
                }
                else
                {
                    message += this.DialogueManager.PerformReplacement(this.Dialog[6], stats, this.Config);
                    HUDMessage msg = new HUDMessage(message);
                    Game1.addHUDMessage(msg);
                }
            }
        }

        private string GetRandomMessage(Dictionary<int, string> messageStore)
        {
            int rand = this.Random.Next(1, messageStore.Count + 1);

            messageStore.TryGetValue(rand, out string value);

            if (this.LoggingEnabled)
                this.Monitor.Log($"condition met to return random unfinished message, returning:{value}", LogLevel.Trace);

            return value;
        }


        private void ReadInMessages()
        {
            try
            {
                this.AllMessages = this.Helper.Content.Load<Dictionary<string, string>>("assets/dialog");

                this.Dialog = this.DialogueManager.GetDialog("Xdialog", this.AllMessages);
                this.Greetings = this.DialogueManager.GetDialog("greeting", this.AllMessages);
                this.UnfinishedMessages = this.DialogueManager.GetDialog("unfinishedmoney", this.AllMessages);
                this.FreebieMessages = this.DialogueManager.GetDialog("freebies", this.AllMessages);
                this.InventoryMessages = this.DialogueManager.GetDialog("unfinishedinventory", this.AllMessages);
                this.Smalltalk = this.DialogueManager.GetDialog("smalltalk", this.AllMessages);

                Dictionary<int, string> characterDialog = this.DialogueManager.GetDialog(this.Checker, this.AllMessages);

                if (characterDialog.Count > 0)
                {
                    int index = this.Smalltalk.Count + 1;
                    foreach (KeyValuePair<int, string> d in characterDialog)
                    {
                        this.Smalltalk.Add(index, d.Value);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Exception loading content:{ex}", LogLevel.Error);
            }
        }
    }
}
