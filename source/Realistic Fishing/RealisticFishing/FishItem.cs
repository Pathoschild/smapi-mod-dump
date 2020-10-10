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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

using PyTK.CustomElementHandler;
using Newtonsoft.Json;

namespace RealisticFishing
{

    public class FishItem : StardewValley.Object, ICustomObject, ISyncableElement
    {
        public const int saleModifier = 8;

        public int Id;
        public List<FishModel> FishStack = new List<FishModel>();
        public String Description;

        public Boolean recoveredFromInventory = false;

        // Used to set a FishStack of a FishItem in a chest when the FishItem was removed from the inventory
        // and when a partial stack is added to a stack in a chest
        // this technique is used because there is no event handler for "AddedToChest"
        public static FishItem itemToAdd;

        // Set in FishItem.getOne().  Used to set the correct FishStack of an item that was partially removed
        // from the inventory and put into a chest AND to set the correct FishStack of the newly added 
        // FishItem in an inventory that had one stack worth of an item removed from a chest
        public static FishItem itemInChestToFix;

        // The FishItem in a chest that has to be updated when FishItem.addToStack() is called
        public static FishItem itemToChange;

        // Used for all other cases where the inventory interacts with a chest
        public static FishItem itemInChestToUpdate;

        public PySync syncObject { get; set; }

        public FishItem() 
        {
            if (syncObject == null)
            {
                syncObject = new PySync(this);
                syncObject.init();
            }

            this.build();
        }

        public FishItem(int id) 
            : base(id, 1, false, -1, 1) 
        {
            if (syncObject == null)
            {
                syncObject = new PySync(this);
                syncObject.init();
            }

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();
            this.FishStack.Add(new FishModel(-1, this.Name, -1, -1, 0, 1));
        }

        public FishItem(int id, FishModel fish) 
            : base(id, 1, false, -1, fish.quality)
        {
            if (syncObject == null)
            {
                syncObject = new PySync(this);
                syncObject.init();
            }

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();

            this.FishStack.Add(fish);
        }

        public object getReplacement()
        {
            Chest replacement = new Chest(true);
            return replacement;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("Id", this.Id.ToString());
            savedata.Add("FishStack", JsonConvert.SerializeObject(this.FishStack));
            savedata.Add("quality", this.quality.ToString());
            return savedata;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.Id = int.Parse(additionalSaveData["Id"]);
            this.ParentSheetIndex = this.Id;
            this.quality.Value = int.Parse(additionalSaveData["quality"]);
            this.FishStack = JsonConvert.DeserializeObject<List<FishModel>>(additionalSaveData["FishStack"]);
            this.Stack = this.FishStack.Count;

            string str;
            Game1.objectInformation.TryGetValue(this.Id, out str);
            if (str != null)
            {
                string[] strArray = str.Split('/');
                this.name = strArray[0];
                this.price.Value = Convert.ToInt32(strArray[1]);
                this.edibility.Value = Convert.ToInt32(strArray[2]);
                this.Description = strArray[5];
            }
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {

            int id = int.Parse(additionalSaveData["Id"]);

            List<FishModel> fishStack = JsonConvert.DeserializeObject<List<FishModel>>(additionalSaveData["FishStack"]);

            FishItem fish = new FishItem(id);
            fish.FishStack = fishStack;
            return (ICustomObject)fish;
        }

        public void build() {
            this.Name = "test update";
            this.Description = "test desc";
        }

        public Dictionary<string, string> getSyncData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("Description", this.Description);
            savedata.Add("FishStack", JsonConvert.SerializeObject(this.FishStack));
            return savedata;
        }

        public void sync(Dictionary<string, string> syncData)
        {
            this.FishStack = JsonConvert.DeserializeObject<List<FishModel>>(syncData["FishStack"]);
            this.Description = syncData["Description"];
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public void AddToInventory()
        {
            this.syncObject.MarkDirty();

            Item item = this;
            FishItem fishItem = item as FishItem;

            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                // Adding to a non-empty inventory slot
                if (index < Game1.player.items.Count && Game1.player.items[index] != null && (Game1.player.items[index].maximumStackSize() != -1 && Game1.player.items[index].getStack() < Game1.player.items[index].maximumStackSize()) && Game1.player.items[index].Name.Equals(item.Name) && ((!(item is StardewValley.Object) || !(Game1.player.items[index] is StardewValley.Object) || (item as StardewValley.Object).quality.Value == (Game1.player.items[index] as StardewValley.Object).quality.Value && (item as StardewValley.Object).ParentSheetIndex == (Game1.player.items[index] as StardewValley.Object).ParentSheetIndex) && item.canStackWith(Game1.player.items[index])))
                {
                    (Game1.player.items[index] as FishItem).Stack += fishItem.Stack;
                    (Game1.player.items[index] as FishItem).FishStack.AddRange(fishItem.FishStack);
                    (Game1.player.items[index] as FishItem).syncObject.MarkDirty();
                    return;
                }
            }
            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                // Adding to an empty inventory slot
                if (Game1.player.items.Count > index && Game1.player.items[index] == null)
                {
                    Game1.player.items[index] = item;
                    return;
                }
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override Item getOne() {

            this.checkIfStackIsWrong();
            this.syncObject.MarkDirty();

            if (this.FishStack.Count > 0) {
                
                FishItem one = new FishItem(this.Id, this.FishStack[this.FishStack.Count - 1]);
                FishItem.itemInChestToFix = one;
                FishItem.itemInChestToUpdate = this;

                return (Item)one;
            } else {
                throw new MissingMemberException();
            }
        }

        public override string getDescription()
        {

            if (this.FishStack.Count == 1) {
                return this.Description + " This one is " + ((int)Math.Round(this.FishStack[0].length)).ToString() + " in. long.";
            }

            string lengths = "";

            int count = 0;
            int max = 10;

            foreach (FishModel fish in this.FishStack) {

                if (count == 0) {
                    lengths += ((int)Math.Round(fish.length)).ToString();
                } else {
                    lengths += ", " + ((int)Math.Round(fish.length)).ToString();
                }

                count++;

                if (count == max) {
                    break;
                }
            }

            if (count >= max) {
                return this.Description + "This stack contains " + this.Name + " of length: \n" + lengths + "\n...(truncated)";
            } else {
                return this.Description + "This stack contains " + this.Name + " of length: \n" + lengths;   
            }
        }

        public override int salePrice() {

            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (Math.Round(fish.length) / FishItem.saleModifier) * (fish.quality + 1);
            }

            p /= this.FishStack.Count;

            return (int)Math.Round(p);
        }

        public override int sellToStorePrice()
        {
            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (Math.Round(fish.length) / FishItem.saleModifier) * (fish.quality + 1);
            }

            p /= this.FishStack.Count;



            return (int)Math.Round(p);
        }

        public override int addToStack(int amount) {

            this.syncObject.MarkDirty();

            FishItem.itemToChange = this;

            return base.addToStack(amount);
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other) && other is FishItem;
        }

        public void checkIfStackIsWrong() {
            
            if (this.FishStack.Count > this.Stack)
            {
                this.FishStack.RemoveRange(this.Stack, this.FishStack.Count - this.Stack);
            }
        }
    }
}
