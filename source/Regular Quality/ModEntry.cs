using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace RegularQuality
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        private ModConfig Config;
        private const int MAX_ITEM_STACK_SIZE = 999;
        private const int BUNDLE_INGREDIENT_FIELDS = 3;
        private const char BUNDLE_FIELD_SEPARATOR = '/';
        private const char BUNDLE_INGREDIENT_SEPARATOR = ' ';

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            if ( Config.BundleIngredientQualityMultiplicator < 0 )
            {
                this.Monitor.Log("Error in config.json: \"RarityMultiplicator\" must be at least 0.", LogLevel.Error);
                this.Monitor.Log("Deactivating mod", LogLevel.Error);
                return;
            }

            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
            //helper.Events.Display.RenderedWorld += RenderedWorld;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Bundles"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // update community center bundles (mainly the quality crops bundle, but be compatible with other mods)
            if (asset.AssetNameEquals("Data/Bundles"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                // https://stackoverflow.com/a/31767807
                // .ToList is part of System.Linq
                // Without it, the loop would error after an assignment to a dictionary element
                foreach ( string key in data.Keys.ToList() )
                {
                    string[] fields = data[key].Split(BUNDLE_FIELD_SEPARATOR);
                    string[] bundleIngredients = fields[2].Split(BUNDLE_INGREDIENT_SEPARATOR);
                    for (int i = 0; i < bundleIngredients.Length / BUNDLE_INGREDIENT_FIELDS; ++i)
                    {
                        int indexItemId   = i * BUNDLE_INGREDIENT_FIELDS;
                        int indexQuantity = i * BUNDLE_INGREDIENT_FIELDS + 1;
                        int indexQuality  = i * BUNDLE_INGREDIENT_FIELDS + 2;

                        string itemId   = bundleIngredients[indexItemId];
                        string quantity = bundleIngredients[indexQuantity];
                        string quality  = bundleIngredients[indexQuality];

                        // itemId -1 is a gold purchase, don't change anything here
                        if (itemId == "-1") continue;

                        // quality is already regular, no adjustment needed
                        if (quality == "0") continue;

                        // adjust amount -> multiply by rarity
                        int intQuality = int.Parse(quality);
                        int intQuantity = int.Parse(quantity);
                        int newQuantity = intQuantity + intQuantity * intQuality * Config.BundleIngredientQualityMultiplicator;
                        if (newQuantity > MAX_ITEM_STACK_SIZE)
                        {
                            this.Monitor.Log($"A bundle ingredient would have exceeded the maximum stack size of {MAX_ITEM_STACK_SIZE}. It has been limited to {MAX_ITEM_STACK_SIZE}.", LogLevel.Warn);
                            this.Monitor.Log($"Bundle: {key} | itemId: {itemId} | adjusted quantity = {newQuantity} (= {quantity} + {quantity} * {quality} * {Config.BundleIngredientQualityMultiplicator}", LogLevel.Warn);
                            newQuantity = MAX_ITEM_STACK_SIZE;
                        }

                        bundleIngredients[indexQuantity] = newQuantity.ToString();
                        bundleIngredients[indexQuality] = "0";
                    }

                    string newData = string.Join(BUNDLE_INGREDIENT_SEPARATOR.ToString(), bundleIngredients);

                    // nothing changed, no need to touch the data dictionary
                    if ( newData == fields[2]) continue;

                    fields[2] = newData;
                    data[key] = string.Join(BUNDLE_FIELD_SEPARATOR.ToString(), fields);
                }
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised AFTER the player receives an item.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            // only update the inventory of the target player
            if ( !e.IsLocalPlayer )
            {
                return;
            }

            IEnumerator<Item> enumerator = e.Added.GetEnumerator();
            while (enumerator.MoveNext())
            {
                // not an item with a quality property, skip
                if (!(enumerator.Current is StardewValley.Object)) return;

                StardewValley.Object item = (StardewValley.Object)enumerator.Current;

                // quality is already regular, nothing to do
                // otherwise the below code would "autosort" items to the first free slot when manually organizing the inventory
                if ( item.Quality == 0 ) return;

                // remove quality
                // because this happens only AFTER the item was added to the inventory,
                // make a best effort to stack the item with an already existing stack
                Game1.player.removeItemFromInventory(item);
                item.Quality = 0;
                Game1.player.addItemToInventory(item);
            }
        }

        /// <summary>Raised for every frame.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            // regular quality fish
            if( Game1.activeClickableMenu == null && Game1.player.CurrentTool is FishingRod )
            {
                FishingRod rod = (FishingRod)Game1.player.CurrentTool;
                if (rod.fishCaught)
                {
                    this.Helper.Reflection.GetField<int>(rod, "fishQuality", true).SetValue(0);
                }
            }

            // regular quality forage/crops
            // quality is determined on pickup, not on spawn
            // this would require Harmony to patch the Crop.harvest method
        }
    }
}