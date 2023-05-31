/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.Revitalize.Framework.Utilities.Ranges;
using Omegasis.Revitalize.Framework.Utilities;
using Microsoft.Xna.Framework.Input;

namespace Omegasis.Revitalize.Framework.World.Objects.Farming
{
    /// <summary>
    /// A machine that takes tree seeds and gives tree products as a result.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Farming.AutomaticTreeFarm")]
    public class AutomaticTreeFarm : ItemRecipeDropInMachine
    {
        public const string OAK_0_ANIMATION_KEY = "Oak_0";
        public const string OAK_1_ANIMATION_KEY = "Oak_1";
        public const string OAK_2_ANIMATION_KEY = "Oak_2";
        public const string OAK_3_ANIMATION_KEY = "Oak_3";

        public const string MAPLE_0_ANIMATION_KEY = "Maple_0";
        public const string MAPLE_1_ANIMATION_KEY = "Maple_1";
        public const string MAPLE_2_ANIMATION_KEY = "Maple_2";
        public const string MAPLE_3_ANIMATION_KEY = "Maple_3";

        public const string PINE_0_ANIMATION_KEY = "Pine_0";
        public const string PINE_1_ANIMATION_KEY = "Pine_1";
        public const string PINE_2_ANIMATION_KEY = "Pine_2";
        public const string PINE_3_ANIMATION_KEY = "Pine_3";

        public const string MAHOGANY_0_ANIMATION_KEY = "Mahogany_0";
        public const string MAHOGANY_1_ANIMATION_KEY = "Mahogany_1";
        public const string MAHOGANY_2_ANIMATION_KEY = "Mahogany_2";
        public const string MAHOGANY_3_ANIMATION_KEY = "Mahogany_3";

        public const string OAK_SEED_ID = "StardewValley.Oak";
        public const string MAPLE_SEED_ID = "StardewValley.Maple";
        public const string PINE_SEED_ID = "StardewValley.Pine";
        public const string MAHOGANY_SEED_ID = "StardewValley.Mahogany";

        public readonly NetString seedId = new NetString("");

        public string SeedId
        {
            get
            {
                return this.seedId.Value;
            }
            set
            {
                this.seedId.Value = value;
            }
        }

        public AutomaticTreeFarm()
        {

        }

        public AutomaticTreeFarm(BasicItemInformation Info) : this(Info, Vector2.Zero)
        {

        }

        public AutomaticTreeFarm(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.seedId);
        }

        public override void updateAnimation()
        {
            if (this.isIdle())
            {
                this.AnimationManager.playAnimation(Machine.DEFAULT_ANINMATION_KEY);
            }
            else if (this.heldItems.Count>0)
            {
                int daysRemaining = this.MinutesUntilReady / GameTimeStamp.MinutesPerDay;


                if (this.SeedId.Equals(OAK_SEED_ID))
                {
                    if (daysRemaining >=4)
                    {
                        this.AnimationManager.playAnimation(OAK_0_ANIMATION_KEY);
                    }
                    if (daysRemaining == 3 || daysRemaining==2)
                    {
                        this.AnimationManager.playAnimation(OAK_1_ANIMATION_KEY);
                    }
                    if (daysRemaining == 1 || (this.MinutesUntilReady>0 && daysRemaining==0))
                    {
                        this.AnimationManager.playAnimation(OAK_2_ANIMATION_KEY);
                    }
                    if (this.finishedProduction())
                    {
                        this.AnimationManager.playAnimation(OAK_3_ANIMATION_KEY);
                    }

                }
                if (this.SeedId.Equals(MAPLE_SEED_ID))
                {
                    if (daysRemaining >= 4)
                    {
                        this.AnimationManager.playAnimation(MAPLE_0_ANIMATION_KEY);
                    }
                    if (daysRemaining == 3 || daysRemaining == 2)
                    {
                        this.AnimationManager.playAnimation(MAPLE_1_ANIMATION_KEY);
                    }
                    if (daysRemaining == 1 || (this.MinutesUntilReady > 0 && daysRemaining == 0))
                    {
                        this.AnimationManager.playAnimation(MAPLE_2_ANIMATION_KEY);
                    }
                    if (this.finishedProduction())
                    {
                        this.AnimationManager.playAnimation(MAPLE_3_ANIMATION_KEY);
                    }
                }
                if (this.SeedId.Equals(PINE_SEED_ID))
                {
                    if (daysRemaining >= 4)
                    {
                        this.AnimationManager.playAnimation(PINE_0_ANIMATION_KEY);
                    }
                    if (daysRemaining == 3 || daysRemaining == 2)
                    {
                        this.AnimationManager.playAnimation(PINE_1_ANIMATION_KEY);
                    }
                    if (daysRemaining == 1 || (this.MinutesUntilReady > 0 && daysRemaining == 0))
                    {
                        this.AnimationManager.playAnimation(PINE_2_ANIMATION_KEY);
                    }
                    if (this.finishedProduction())
                    {
                        this.AnimationManager.playAnimation(PINE_3_ANIMATION_KEY);
                    }
                }
                if (this.SeedId.Equals(MAHOGANY_SEED_ID))
                {
                    if (daysRemaining >= 4)
                    {
                        this.AnimationManager.playAnimation(MAHOGANY_0_ANIMATION_KEY);
                    }
                    if (daysRemaining == 3 || daysRemaining == 2)
                    {
                        this.AnimationManager.playAnimation(MAHOGANY_1_ANIMATION_KEY);
                    }
                    if (daysRemaining == 1 || (this.MinutesUntilReady > 0 && daysRemaining == 0))
                    {
                        this.AnimationManager.playAnimation(MAHOGANY_2_ANIMATION_KEY);
                    }
                    if (this.finishedProduction())
                    {
                        this.AnimationManager.playAnimation(MAHOGANY_3_ANIMATION_KEY);
                    }
                }
            }
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            bool elapsed= base.minutesElapsed(minutes, environment);
            this.updateAnimation();
            return elapsed;
        }

        public override List<Item> getMachineOutputs(bool AddToPlayersInventory, bool DropAsItemDebris, bool ShowInventoryFullError)
        {
            List<Item> items= base.getMachineOutputs(AddToPlayersInventory, DropAsItemDebris, ShowInventoryFullError);
            if (!this.hasItemsInHeldItemQueue())
            {
                this.SeedId = "";
                this.updateAnimation();
            }
            return items;
        }

        public override CraftingResult processInput(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            if (string.IsNullOrEmpty(this.getCraftingRecipeBookId()) || this.isWorking() || this.finishedProduction())
            {
                return new CraftingResult(false);
            }

            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = this.getListOfValidRecipes(inputItems, who, ShowRedMessage);

            if (validRecipes.Count > 0)
            {
                return this.onSuccessfulRecipeFound(validRecipes.First().Key, validRecipes.First().Value, who);
            }

            return new CraftingResult(false);
        }

        /// <summary>
        /// Generate the list of potential recipes based on the contents of the farmer's inventory.
        /// </summary>
        /// <param name="inputItems"></param>
        /// <returns></returns>
        public override List<ProcessingRecipe> getListOfPotentialRecipes(IList<Item> inputItems, Farmer who = null)
        {
            List<ProcessingRecipe> possibleRecipes = new List<ProcessingRecipe>();
            possibleRecipes.AddRange(base.getListOfPotentialRecipes(inputItems)); //Still allow getting recipes from recipe books and prefer those first.

            //Attempt to generate recipes automatically from items passed in.
            foreach (Item item in inputItems)
            {
                if (item == null) continue;

                ProcessingRecipe recipe = this.createProcessingRecipeFromItem(item, who);
                if (recipe != null)
                {
                    possibleRecipes.Add(recipe);
                }
            }
            return possibleRecipes;
        }



        public virtual ProcessingRecipe createProcessingRecipeFromItem(Item item, Farmer who = null)
        {
            ItemReference input = new ItemReference(item.getOne());
            if (!(item is StardewValley.Object))
            {
                return null;
            }

            string objectId = RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(item.ParentSheetIndex);


            List<LootTableEntry> outputs = new List<LootTableEntry>();


            outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Sap), new IntRange(5, 5), new DoubleRange(0, 100)));
            outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Wood), new IntRange(12, 16), new DoubleRange(0, 100)));


            IntRange chance=new IntRange(0,100);
            int val=chance.getRandomInclusive();

            if(new DoubleRange(50, 75).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 1));
            }

            if (new DoubleRange(75, 82).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 2));
            }

            if (new DoubleRange(82, 88).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 3));
            }

            if (new DoubleRange(88, 93).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 4));
            }

            if (new DoubleRange(93, 97).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 5));
            }

            if (new DoubleRange(97, 99).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 7));
            }

            if (new DoubleRange(99, 100).containsInclusive(val))
            {
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), 10));
            }

            //Seeds have a chance of not being produced.
            IntRange stackSizeRange = new IntRange(0, 2);
            int stackSize = stackSizeRange.getRandomInclusive();

            int timeToGrow = GameTimeStamp.MinutesPerDay * 5;

            if (objectId.Equals(RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(Enums.SDVObject.PineCone)))
            {
                if (stackSize > 0)
                {
                    outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.PineCone), stackSize, new DoubleRange(0, 100)));
                }

                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.PineTar), 1, new DoubleRange(0, 100)));

                this.SeedId = PINE_SEED_ID;
                return new ProcessingRecipe(input.RegisteredObjectId, new GameTimeStamp(timeToGrow), input, outputs);

            }

            if (objectId.Equals(RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(Enums.SDVObject.MapleSeed)))
            {
                if (stackSize > 0)
                {
                    outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.MapleSeed), stackSize, new DoubleRange(0, 100)));
                }

                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.MapleSyrup), 1, new DoubleRange(0, 100)));

                this.SeedId = MAPLE_SEED_ID;
                return new ProcessingRecipe(input.RegisteredObjectId, new GameTimeStamp(timeToGrow), input, outputs);
            }

            if (objectId.Equals(RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(Enums.SDVObject.Acorn)))
            {

                if (stackSize > 0)
                {
                    outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Acorn), stackSize, new DoubleRange(0, 100)));
                }

                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.OakResin), 1, new DoubleRange(0, 100)));

                this.SeedId = OAK_SEED_ID;
                return new ProcessingRecipe(input.RegisteredObjectId, new GameTimeStamp(timeToGrow), input, outputs);
            }

            if (objectId.Equals(RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(Enums.SDVObject.MahoganySeed)))
            {
                outputs.Clear();
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Sap), new IntRange(5, 5), new DoubleRange(0, 100)));
                outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.Hardwood), new IntRange(10, 10), new DoubleRange(0, 100)));

                if (stackSize > 0)
                {
                    outputs.Add(new LootTableEntry(new ItemReference(Enums.SDVObject.MahoganySeed), stackSize, new DoubleRange(0, 100)));
                }

                this.SeedId = MAHOGANY_SEED_ID;
                return new ProcessingRecipe(input.RegisteredObjectId, new GameTimeStamp(timeToGrow), input, outputs);
            }

            return null;
        }



        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.Ship);
        }

        public override Item getOne()
        {
            return new AutomaticTreeFarm(this.basicItemInformation.Copy());
        }

    }
}
