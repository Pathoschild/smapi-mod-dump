/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kevinforrestconnors/RealisticFishing
**
*************************************************/

using System;
using RealiticFishing.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using RealisticFishing;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewValley.Objects;

namespace RealiticFishing
{
    /// <summary>The main entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        public static Random rand = new Random();

        // Used to detect if the player is fishing.
        private SBobberBar Bobber;

        // Handles the event handler logic.
        private bool BeganFishingGame = false;
        private bool EndedFishingGame = false;
        private bool PlayerReceivedFish = false;
        private int whichFish;

        // Which direction the player was facing when they were fishing.  Used in ThrowFish.
        private int FishingDirection;

        // The last fish caught.
        public Item FishCaught;

        // The FishingRod information
        public bool fishWasCaught = false;

        // The list of Tuple<name, uniqueID> of all the fish caught today. 
        public List<Tuple<string, int>> AllFishCaughtToday;

        // How many fish have been caught today.
        public int NumFishCaughtToday;

        // How many fish the player can catch each day.
        public int FishQuota = 10;

        // The class instance of the saved FishPopulation data
        public FishPopulation fp;
        // The population data structure
        public Dictionary<String, List<FishModel>> population;

        public bool inventoryWasReconstructed = false;
        public bool chestsWereReconstructed = false;
        public bool questsWereReconstructed = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += this.MenuEvents_MenuClosed;
            GameEvents.UpdateTick += this.GameEvents_OnUpdateTick;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterCreate += this.SaveEvents_AfterCreate;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;

            Tests.ModEntryInstance = this;

            if (Tests.ShouldRunTests)
            {
                GameEvents.EighthUpdateTick += Tests.GameEvents_OnUpdateTick;
                ControlEvents.KeyPressed += Tests.ControlEvents_KeyPressed;
            }
        }

        /*********
        ** Private methods
        *********/

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
                return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
            {

                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                foreach (Tuple<int, string, int, int, int> f in fp.AllFish) {
                    data["demetrius" + f.Item2.Trim()] = "Dear @,^ I was conducting a field study on " + f.Item2.Trim() + " the other day, and I discovered that" +
                                                                                                          " the population is in decline. ^ To prevent a fishery collapse, please release any large " + f.Item2.Trim() + 
                                                  " you catch until the population is stable again. ^ -Demetrius";

                    data["demetrius2" + f.Item2.Trim()] = "Dear @,^ It looks like the population of " + f.Item2.Trim() + "is stable again. ^ -Demetrius";
                }
            }
        }

        /* GameEvents_OnUpdateTick
        * Triggers every time the menu changes.
        * Handles the setup of the SBobberBar, used to detect if the player is fishing.
        */
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu is BobberBar bobberBarMenu) {
                this.Bobber = SBobberBar.ConstructFromBaseClass(bobberBarMenu);
            } 

            // Player just caught a fish, but inventory is full
            if (e.NewMenu is ItemGrabMenu itemGrabMenu && e.PriorMenu is DialogueBox && FishItem.itemToAdd != null) {
                
                Item itemToChange = null;

                foreach (Item item in itemGrabMenu.ItemsToGrabMenu.actualInventory) {

                    // Item is a fish
                    if (item.Category == -4) {
                        itemToChange = item;
                    }
                }

                if (itemToChange != null) {
                    itemGrabMenu.ItemsToGrabMenu.actualInventory.Remove(itemToChange);
                    itemGrabMenu.ItemsToGrabMenu.actualInventory.Add(FishItem.itemToAdd);
                }
            }
        }

        /* GameEvents_OnUpdateTick
        * Triggers every time the menu changes.
        * Detects the treasure menu, allowing custom fish to be generated even if treasure is caught too.
        */
        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
        }

        /* ControlEvents_KeyPressed
         * Triggers every a key is pressed.
         * Used to play/pause tests.
         */
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed.Equals(Keys.O) && Tests.ShouldRunTests)
            {
                Tests.RunningTests = !Tests.RunningTests;
            }
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {

            if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu && e.Button == SButton.MouseLeft) {

                Item sourceItem = this.Helper.Reflection.GetField<Item>(itemGrabMenu, "sourceItem").GetValue();

                if (sourceItem is Chest || (itemGrabMenu.behaviorOnItemGrab?.Target as Chest)?.fridge == true) {

                    int x = (int)e.Cursor.ScreenPixels.X;
                    int y = (int)e.Cursor.ScreenPixels.Y;

                    if (itemGrabMenu.inventory.getItemAt(x, y) == null)
                    {

                        Item item = itemGrabMenu.ItemsToGrabMenu.leftClick(x, y, itemGrabMenu.heldItem, false);

                        if (item is FishItem fishItem)
                        {
                            if (itemGrabMenu.trashCan.containsPoint(x, y))
                            {
                                item = null;
                            }
                            else
                            {
                                fishItem.AddToInventory();
                            }

                            FishItem.itemInChestToUpdate = null;

                        }
                        else
                        {
                            // Item is deleted as a result of ItemToGrabMenu.leftClick(), so add it to inventory
                            Item itemWasAdded = itemGrabMenu.inventory.tryToAddItem(item);

                            // Inventory was full, so keep it in chest
                            if (itemWasAdded != null)
                            {
                                itemGrabMenu.ItemsToGrabMenu.tryToAddItem(item);
                            }
                        }
                    }                    
                }
            }
        }

        /* TimeEvents_AfterDayStarted
         * Triggers at the beginning of each day.
         * Regenerates X fish, where X corresponds in number and type to the 
         * fish that the player caught yesterday.
         */
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e) {

            List<String> changedFish = new List<String>();

            foreach (Tuple<string, int> tuple in this.AllFishCaughtToday) {
                string fishName = tuple.Item1;

                changedFish.Add(fishName);

                List<FishModel> fishOfType;
                this.population.TryGetValue(fishName, out fishOfType);

                int numFishOfType = fishOfType.Count;
                int selectedFish = ModEntry.rand.Next(0, numFishOfType);

                fishOfType.RemoveAt(selectedFish);
                fishOfType.Add(fishOfType[selectedFish].MakeBaby());

                this.population[fishName] = fishOfType;
            }

            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();
        }

        /* SaveEvents_AfterCreate
         * Triggers after a save file is updated. 
         * Used to retrieve data and seed the population of fish.
         */
        private void SaveEvents_AfterCreate(object sender, EventArgs e) 
        {

            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();
      
            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;
            this.fp.CurrentFishIDCounter = instance.CurrentFishIDCounter;

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);
        }

        /* SaveEvents_AfterLoad
        * Triggers after a save file is loaded.
        * Used to retrieve data and sometimes seed fish population.
        */
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;
            this.fp.CurrentFishIDCounter = instance.CurrentFishIDCounter;

            // Recover items in inventory
            if (!this.inventoryWasReconstructed) 
            {
                this.RecoverItemsInInventory(instance);
            }

            // Recover items in chests
            if (!this.chestsWereReconstructed)
            {
                this.RecoverItemsInChests(instance);
            }

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            if (Tests.ShouldRunTests) {
                Tests.RunningTests = true;
            }
        }

        /* SaveEvents_BeforeSave
        * Triggers before a save file is saved.
        * Used to serialize the data into the save file.
        */
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json");

            instance.NumFishCaughtToday = this.NumFishCaughtToday;
            instance.AllFishCaughtToday = this.AllFishCaughtToday;
            instance.fp = this.fp;
            instance.population = this.fp.population;
            instance.CurrentFishIDCounter = this.fp.CurrentFishIDCounter;

            foreach (Tuple<int, string, int, int, int> fish in this.fp.AllFish)
            {
                if (this.fp.IsAverageFishBelowValue(fish.Item2))
                {
                    // Fish is endangered.  the value in the dictionary represents that the mail message from Demetrius has not yet been sent
                    if (!instance.endangeredFish.ContainsKey(fish.Item2)) {
                        instance.endangeredFish.Add(fish.Item2, false);
                        Game1.addMailForTomorrow("demetrius" + fish.Item2);
                        instance.endangeredFish[fish.Item2] = true;
                    } 
                } else {
                    // Fish is no longer endangered
                    if (instance.endangeredFish.ContainsKey(fish.Item2)) {

                        if (instance.endangeredFish[fish.Item2] == true) {
                            Game1.addMailForTomorrow("demetrius2" + fish.Item2);
                        }

                        instance.endangeredFish.Remove(fish.Item2);
                    }
                }
            }

            instance.inventory.Clear();

            // Save items in inventory
            for (int index = 0; index < Game1.player.maxItems; ++index)
            {
                if (index < Game1.player.items.Count && Game1.player.items[index] != null)
                {
                    Item item = Game1.player.items[index];
                    if (item is FishItem) {
                        instance.inventory.Add(new Tuple<int, List<FishModel>>(item.ParentSheetIndex, (item as FishItem).FishStack));
                        Game1.player.removeItemFromInventory(item);
                    }
                }
            }

            instance.chests.Clear();

            // Save items in chests
            foreach (GameLocation location in Game1.locations) {

                var chestsInLocation = new Dictionary<Vector2, List<Tuple<int, List<FishModel>>>>();

                foreach (StardewValley.Object worldObject in location.objects.Values) {

                    var fishInChest = new List<Tuple<int, List<FishModel>>>();

                    if (worldObject is Chest) {

                        foreach (Item item in (worldObject as Chest).items) {

                            if (item is FishItem) {
                                fishInChest.Add(new Tuple<int, List<FishModel>>(item.ParentSheetIndex, (item as FishItem).FishStack));
                                (worldObject as Chest).items.Remove(item);
                            }
                        }

                        chestsInLocation.Add(worldObject.TileLocation, fishInChest);
                    }
                }

                instance.chests.Add(location.Name, chestsInLocation);
            }

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.inventoryWasReconstructed = false;
            this.chestsWereReconstructed = false;
            this.questsWereReconstructed = false;
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            if (!this.inventoryWasReconstructed)
            {
                this.RecoverItemsInInventory(instance);
            }

            // Recover items in chests
            if (!this.chestsWereReconstructed)
            {
                this.RecoverItemsInChests(instance);
            }

        }

        /* GameEvents_OnUpdateTick
         * Triggers 60 times per second.  
         * Use one of the methods here https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events#Game for other time durations
         */
        private void GameEvents_OnUpdateTick(object sender, EventArgs e)
        {

            if (Game1.player.CurrentTool is FishingRod rod)
            {

                if (rod.fishCaught && !this.PlayerReceivedFish)
                {

                    // Prevents the mod from giving the player 1 fish per tick ;)
                    this.PlayerReceivedFish = true;

                    this.whichFish = this.Helper.Reflection.GetField<int>(rod, "whichFish").GetValue();

                    // construct a temporary fish item to figure out what the caught fish's name is
                    FishItem tempFish = new FishItem(this.whichFish);

                    if (tempFish.Category == -4) { // is a fish
                        
                        // get the list of fish in the Population with that name
                        List<FishModel> fishOfType;
                        this.population.TryGetValue(tempFish.Name, out fishOfType);

                        // get a random fish of that type from the population
                        int numFishOfType = fishOfType.Count;
                        int selectedFishIndex = ModEntry.rand.Next(0, numFishOfType);
                        FishModel selectedFish = fishOfType[selectedFishIndex];

                        this.Helper.Reflection.GetField<int>(rod, "fishSize").SetValue((int)Math.Round(selectedFish.length));

                        // store a new custom fish item
                        Item customFish = (Item)new FishItem(this.whichFish, selectedFish);
                        FishItem.itemToAdd = customFish as FishItem;
                        this.FishCaught = customFish;

                        // make sure the fish in the ocean will be regenerated at the end of the day
                        this.AllFishCaughtToday.Add(new Tuple<string, int>(selectedFish.name, selectedFish.uniqueID));

                        // Prompt the player to throw back the fish
                        this.PromptThrowBackFish(selectedFish.name, selectedFish.uniqueID);  
                    }
                }
            }


            if (Game1.activeClickableMenu is BobberBar && this.Bobber != null) {

                SBobberBar bobber = this.Bobber;

                if (!this.BeganFishingGame) {
                    this.OnFishingBegin();
                    this.PlayerReceivedFish = false;
                    this.BeganFishingGame = true;
                }

            } else if (this.EndedFishingGame) {
                
                this.OnFishingEnd();
                this.EndedFishingGame = false;

            } else if (this.BeganFishingGame) {{}
                    
                this.EndedFishingGame = true;
                this.BeganFishingGame = false;
            }
        }

        /* PlayerEvents_InventoryChanged
         * Triggers every time the inventory changes.
         * Calls PromptThrowBackFish if the player just gained a fish and also just finished fishing.
         * If the player caught treasure, waits until the player gains the fish and this executes again when the player gains the fish.
         */
        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {
            
            foreach (ItemStackChange i in e.Added) 
            {

                // Item.Category == -4 tests if item is a fish.
                if (i.Item.Category == -4)
                {
                    if (!(i.Item is FishItem fishItem))
                    {
                        if (!this.fp.TrapFish.Contains(i.Item.Name)) {
                            Game1.player.removeItemFromInventory(i.Item);
                        }

                    } else {

                        fishItem.syncObject.MarkDirty();

                        if (fishItem.recoveredFromInventory)
                        {
                            fishItem.recoveredFromInventory = false;
                        } else {

                            if (FishItem.itemInChestToUpdate != null) {

                                fishItem.FishStack = FishItem.itemInChestToUpdate.FishStack.GetRange(FishItem.itemInChestToUpdate.FishStack.Count - i.Item.Stack, i.Item.Stack);
                                FishItem.itemInChestToUpdate.checkIfStackIsWrong();
                                FishItem.itemInChestToUpdate = null;

                            }
                        }
                    }
                }
            }

            foreach (ItemStackChange i in e.Removed) 
            {
                // Item.Category == -4 tests if item is a fish.
                if (i.Item.Category == -4) 
                {

                    if (i.Item is FishItem fishItem) {

                        fishItem.syncObject.MarkDirty();

                        FishItem.itemToAdd = new FishItem(fishItem.Id);
                        FishItem.itemToAdd.FishStack = fishItem.FishStack;
                    }
                }
            }

            foreach (ItemStackChange i in e.QuantityChanged)
            {
                // Item.Category == -4 tests if item is a fish.
                if (i.Item.Category == -4)
                {
                    if (!(i.Item is FishItem fishItem))
                    {
                        // Used to remove the basic fish item without removing the FishItem custom item
                        i.Item.Stack -= i.StackChange;
                    }
                    else
                    {
                        fishItem.syncObject.MarkDirty();

                        if (i.StackChange < 0)
                        {

                            int numRemoved = Math.Abs(i.StackChange);

                            FishItem.itemToAdd = new FishItem(fishItem.Id);
                            FishItem.itemToAdd.FishStack = fishItem.FishStack.GetRange(fishItem.FishStack.Count - numRemoved, numRemoved);

                            if (FishItem.itemInChestToFix != null) {
                                FishItem.itemInChestToFix.FishStack = FishItem.itemToAdd.FishStack;
                                FishItem.itemInChestToFix = null;
                            }

                            fishItem.FishStack.RemoveRange(fishItem.FishStack.Count - numRemoved, numRemoved);

                        } else if (i.StackChange > 0) {

                            if (i.StackChange == 1) {
                                
                                if (FishItem.itemInChestToFix != null)
                                {
                                    FishItem.itemInChestToUpdate.checkIfStackIsWrong();
                                    fishItem.FishStack.AddRange(FishItem.itemInChestToFix.FishStack);
                                    FishItem.itemInChestToUpdate = null;
                                    FishItem.itemInChestToFix = null;
                                }
                            } else {

                                if (Game1.activeClickableMenu is ItemGrabMenu && FishItem.itemInChestToUpdate != null)
                                {
                                    fishItem.FishStack.AddRange(FishItem.itemInChestToUpdate.FishStack.GetRange(FishItem.itemInChestToUpdate.FishStack.Count - i.StackChange, i.StackChange));
                                    FishItem.itemInChestToUpdate.checkIfStackIsWrong();
                                    FishItem.itemInChestToUpdate = null;
                                    FishItem.itemInChestToFix = null;
                                }
                            }
                        }
                    }
                }
            }

            if (FishItem.itemToChange != null) {

                FishItem.itemToChange.FishStack.AddRange(FishItem.itemToAdd.FishStack);
                FishItem.itemToChange = null;
            }
        }

        /* OnFishingEnd
        * Triggers once when the fishing minigame starts.  
        */
        private void OnFishingBegin() {
            this.FishingDirection = Game1.player.FacingDirection;
        }

        /* OnFishingEnd
         * Triggers once after the player catches a fish (not on trash)
         */
        private void OnFishingEnd() {
            
        }

        /* PromptThrowBackFish
         * Triggers every time the player catches a fish while they are still under the quota.
         * Calls ThrowBackFish as a callback to handle the choice made.
         */
        private void PromptThrowBackFish(string fishName, int fishID) {

            if (this.NumFishCaughtToday >= this.FishQuota) {
                
                this.ThrowBackFish(Game1.player, "Yes");

            } else {
                String dialogue = "You have caught " + this.NumFishCaughtToday + " fish today.  You are permitted to keep 10 fish per day.  Throw it back?";

                Response[] answerChoices = new[]
                {
                    new Response("Yes", "Yes"),
                    new Response("No", "No")
                };

                Game1.currentLocation.createQuestionDialogue(dialogue, answerChoices, new GameLocation.afterQuestionBehavior(delegate (Farmer who, string whichAnswer) {
                    if (this.ThrowBackFish(who, whichAnswer)) {
                        
                        List<FishModel> fishOfType;
                        this.population.TryGetValue(fishName, out fishOfType);

                        FishModel selectedFish = fishOfType.Find((FishModel obj) => obj.uniqueID == fishID);
                        fishOfType.Remove(selectedFish);
                    }
                }));
            }
        }

        /* ThrowBackFish
         * Triggers every time the player catches a fish while they are still under the quota.
         * If whichAnswer == "Yes", removes the fish from the inventory and calls ThrowFish
         */
        private bool ThrowBackFish(Farmer who, string whichAnswer) {

            if (whichAnswer == "Yes") {

                Item fish = new FishItem(this.FishCaught.ParentSheetIndex);

                if (this.AllFishCaughtToday.Count > 0) {
                    this.AllFishCaughtToday.RemoveAt(this.AllFishCaughtToday.Count - 1);
                }

                this.ThrowFish(fish, who.getStandingPosition(), this.FishingDirection, (GameLocation)null, -1);
                return true;

            } else if (whichAnswer == "No") {

                this.NumFishCaughtToday++;

                (this.FishCaught as FishItem).AddToInventory();

                if (this.NumFishCaughtToday == this.FishQuota) {
                    Game1.addHUDMessage(new HUDMessage("You have reached the fishing limit for today."));
                }
                return false;
            } else {
                return false;
            }
        }

        /* ThrowFish
         * Drops the fish that was just caught 192 pixels in front of the player, almost always into the water.
         * TODO: Make it so that the item always lands in the water, or that the item is destroyed if it doesn't.
         */
        private void ThrowFish(Item fish, Vector2 origin, int direction, GameLocation location, int groundLevel = -1) {
            if (location == null)
                location = Game1.currentLocation;
            Vector2 targetLocation = new Vector2(origin.X, origin.Y);

            switch (direction)
            {
                case -1:
                    targetLocation = Game1.player.getStandingPosition();
                    break;
                case 0: // up
                    origin.Y -= 192f;
                    targetLocation.Y += 192f;
                    break;
                case 1: // right
                    origin.X += 192f;
                    targetLocation.X -= 192f;
                    break;
                case 2: // down
                    origin.Y += 192f;
                    targetLocation.Y -= 192f;
                    break;
                case 3: // left
                    origin.X -= 192f;
                    targetLocation.X += 192f;
                    break;
            }

            Debris debris = new Debris(-2, 1, origin, targetLocation, 0.1f); // (int debrisType, int numberOfChunks, Vector2 debrisOrigin, Vector2 playerPosition, float velocityMultiplyer)
            debris.item = fish;
            if (groundLevel != -1)
                debris.chunkFinalYLevel = groundLevel;
            location.debris.Add(debris);
        }

        private void RecoverItemsInChests(RealisticFishingData instance) {
            
            this.chestsWereReconstructed = true;

            foreach (GameLocation location in Game1.locations)
            {

                if (instance.chests.ContainsKey(location.Name))
                {
                    var chestsInLocation = instance.chests[location.Name];

                    foreach (StardewValley.Object worldObject in location.objects.Values)
                    {

                        if (worldObject is Chest)
                        {
                            if (chestsInLocation.ContainsKey(worldObject.TileLocation))
                            {

                                foreach (Tuple<int, List<FishModel>> f in chestsInLocation[worldObject.TileLocation])
                                {
                                    int id = f.Item1;
                                    var fishStack = f.Item2;

                                    var itemToBeAdded = new FishItem(id, fishStack[0]);
                                    itemToBeAdded.Stack = fishStack.Count;
                                    itemToBeAdded.FishStack = fishStack;

                                    (worldObject as Chest).items.Add(itemToBeAdded);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RecoverItemsInInventory(RealisticFishingData instance) {

            this.inventoryWasReconstructed = true;

            foreach (Tuple<int, List<FishModel>> f in instance.inventory)
            {
                int id = f.Item1;
                var fishStack = f.Item2;

                var itemToBeAdded = new FishItem(id, fishStack[0]);
                itemToBeAdded.Stack = fishStack.Count;
                itemToBeAdded.FishStack = fishStack;
                itemToBeAdded.recoveredFromInventory = true;

                itemToBeAdded.AddToInventory();
            }
        }
    }
}