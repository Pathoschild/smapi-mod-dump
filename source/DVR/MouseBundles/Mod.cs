/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseBundles
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += MenuChanged;
        }



        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {

            var shop = e.NewMenu as StardewValley.Menus.ShopMenu;
            if (shop == null || Game1.currentLocation.Name != "Forest" || !shop.potraitPersonDialogue.Contains("hats")) return;

            // no bundles for Joja members
            if (Game1.player.hasOrWillReceiveMail("JojaMember")) return;
            CommunityCenter communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            if (communityCenter.areAllAreasComplete()) return;

            IReflectedField<Dictionary<Item, int[]>> inventoryInformation = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(shop, "itemPriceAndStock");
            Dictionary<Item, int[]> itemPriceAndStock = inventoryInformation.GetValue();
            IReflectedField<List<Item>> forSaleInformation = this.Helper.Reflection.GetField<List<Item>>(shop, "forSale");
            List<Item> forSale = forSaleInformation.GetValue();

            foreach (var bundle in GetBundles())
            {
                if (communityCenter.isBundleComplete(bundle.ID))
                    continue;

                foreach (var ing in bundle.Ingredients)
                {
                    if (communityCenter.bundles[bundle.ID][ing.Index]) continue;
                    int itemId = ing.ItemID;
                    var objectToAdd = new StardewValley.Object(Vector2.Zero, itemId, ing.Stack);
                    objectToAdd.Quality = ing.Quality;
                    if (objectToAdd.Name.Contains("rror"))
                    {
                        continue;
                    }
                    itemPriceAndStock.Add(objectToAdd, new int[2] { 5000, ing.Stack });
                    forSale.Add(objectToAdd);
                }
            }

            inventoryInformation.SetValue(itemPriceAndStock);
            forSaleInformation.SetValue(forSale);
        }

        internal IEnumerable<BundleModel> GetBundles()
        {
            IDictionary<string, string> data = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");
            foreach (var entry in data)
            {
                // parse key
                string[] keyParts = entry.Key.Split('/');
                string area = keyParts[0];
                int id = int.Parse(keyParts[1]);

                // parse bundle info
                string[] valueParts = entry.Value.Split('/');
                string name = valueParts[0];
                string reward = valueParts[1];
                string displayName = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en
                    ? name // field isn't present in English
                    : valueParts.Last(); // number of fields varies, but display name is always last

                // parse ingredients
                List<BundleIngredientModel> ingredients = new List<BundleIngredientModel>();
                string[] ingredientData = valueParts[2].Split(' ');
                for (int i = 0; i < ingredientData.Length; i += 3)
                {
                    int index = i / 3;
                    int itemID = int.Parse(ingredientData[i]);
                    int stack = int.Parse(ingredientData[i + 1]);
                    int quality = int.Parse(ingredientData[i + 2]);
                    ingredients.Add(new BundleIngredientModel(index, itemID, stack, quality));
                }

                // create bundle
                yield return new BundleModel(id, name, displayName, area, reward, ingredients);
            }
        }
    }

    internal class BundleModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique bundle ID.</summary>
        public int ID { get; }

        /// <summary>The bundle name.</summary>
        public string Name { get; }

        /// <summary>The translated bundle name.</summary>
        public string DisplayName { get; }

        /// <summary>The community center area containing the bundle.</summary>
        public string Area { get; }

        /// <summary>The unparsed reward description, which can be parsed with <see cref="StardewValley.Utility.getItemFromStandardTextDescription"/>.</summary>
        public string RewardData { get; }

        /// <summary>The required item ingredients.</summary>
        public BundleIngredientModel[] Ingredients { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">The unique bundle ID.</param>
        /// <param name="name">The bundle name.</param>
        /// <param name="displayName">The translated bundle name.</param>
        /// <param name="area">The community center area containing the bundle.</param>
        /// <param name="rewardData">The unparsed reward description.</param>
        /// <param name="ingredients">The required item ingredients.</param>
        public BundleModel(int id, string name, string displayName, string area, string rewardData, IEnumerable<BundleIngredientModel> ingredients)
        {
            this.ID = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.Area = area;
            this.RewardData = rewardData;
            this.Ingredients = ingredients.ToArray();
        }
    }

    internal class BundleIngredientModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The ingredient's index in the bundle.</summary>
        public int Index { get; }

        /// <summary>The required item's parent sprite index (or -1 for a monetary bundle).</summary>
        public int ItemID { get; }

        /// <summary>The number of items required.</summary>
        public int Stack { get; }

        /// <summary>The required item quality.</summary>
        public int Quality { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="index">The ingredient's index in the bundle.</param>
        /// <param name="itemID">The required item's parent sprite index (or -1 for a monetary bundle).</param>
        /// <param name="stack">The number of items required.</param>
        /// <param name="quality">The required item quality.</param>
        public BundleIngredientModel(int index, int itemID, int stack, int quality)
        {
            this.Index = index;
            this.ItemID = itemID;
            this.Stack = stack;
            this.Quality = quality;
        }
    }
}
