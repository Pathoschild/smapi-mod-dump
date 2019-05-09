using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondTheValleyExpansion.Framework.Alchemy
{
    /// <summary> the code for the alchemy feature </summary>
    class AlchemyFramework
    {
        /// <summary> if alchemy feature is unlocked </summary>
        public bool unlockedAlchemy;

        /// <summary> stores item ids that are currently in used </summary>
        public List<int> itemsInUsed = new List<int>();
        /// <summary> options for the alchemy menu </summary>
        public List<Response> alchemyMenuOptions = new List<Response>();

        /// <summary> instance of <see cref="AlchemyDataModel"/> that contains the data model for available item ID's</summary>
        private AlchemyDataModel _AlchemyDataModel = new AlchemyDataModel();
        /// <summary> the alchemy data model </summary>
        public AlchemyDataModel AlchemyData;

        /// <summary> Starts alchemy code </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        public void Alchemy(Farmer who, string ans)
        {
            switch (ans)
            {
                case "mix-ingredients":
                    this.MixIngredients(who, ans);
                    break;
                case "remove-ingredients":
                    this.RemoveIngredients(who, ans);
                    break;
                case "add-ingredients":
                    this.AddIngredients(who, ans);
                    break;
                default:
                    RefMod.ModMonitor.Log($"Looks like something went wrong with Alchemy options for {who} with {ans}", LogLevel.Error);
                    break;
            }
        }

        /// <summary> Mix all ingredients in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void MixIngredients(Farmer who, string ans)
        {

        }

        /// <summary> Remove all ingredients in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void RemoveIngredients(Farmer who, string ans)
        {

        }

        /// <summary> Add an ingredient in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void AddIngredients(Farmer who, string ans)
        {
            if (_AlchemyDataModel.itemData.ContainsKey(Game1.player.CurrentItem.ParentSheetIndex))
            {
                this.itemsInUsed.Add(Game1.player.CurrentItem.ParentSheetIndex);
                RefMod.ModHelper.Data.WriteSaveData("AlchemyItemsInUsed", itemsInUsed);
                Game1.player.removeItemFromInventory(Game1.player.CurrentItem);
            }

            else
                Game1.drawObjectDialogue(RefMod.i18n.Get("alchemy-failed.1"));
        }
    }
}
