/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using StardewValley;
using SpaceCore;
using ItemPipes.Framework.Util;

namespace ItemPipes.Framework.Items.Recipes
{
    [XmlType("Mods_sergiomadd.ItemPipes_IronPipeRecipe")]

    public class IronPipeRecipe : CustomCraftingRecipe
    {
        //public string IDName { get; set; }
        public override string Description => "Test";
        public override IngredientMatcher[] Ingredients =>
            new IngredientMatcher[]
            {
                new ObjectIngredientMatcher(390, 2)
            };
        public string ItemTexturePath { get; set; }
        public override Texture2D IconTexture => 
            ModEntry.helper.Content.Load<Texture2D>("assets/Pipes/IronPipe/IronPipe_Item.png");

        public override Rectangle? IconSubrect => new Rectangle(0, 0, 16, 16);

        public IronPipeRecipe()
        {
            /*
            Printer.Info("CREANDO RECIPE");
            IDName = "IronPipe";
            Description = "Type: Connector Pipe\nThe link between IO pipes. It moves items at 2 tiles-1 second.";
            ItemTexturePath = $"assets/Pipes/{IDName}/{IDName}_Item.png";
            IconTexture = ModEntry.helper.Content.Load<Texture2D>(ItemTexturePath);
            IconSubrect = new Rectangle(0, 0, 16, 16);
            Ingredients = new IngredientMatcher[] 
            {
                new ObjectIngredientMatcher(2, 5)
            };
            */
        }
        public override Item CreateResult()
        {
            Printer.Info("CREANDO ITEM DE RECIPE");
            return new IronPipeItem();
        }
    }
}
