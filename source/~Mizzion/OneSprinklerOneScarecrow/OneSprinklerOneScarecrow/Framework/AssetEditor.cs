/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace OneSprinklerOneScarecrow.Framework
{
    internal class AssetEditor : IAssetEditor
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        private ITranslationHelper _translate;
        private Config _config;

        //Translations
        private readonly string haxorSprinklerName,
            haxorSprinklerDescription,
            haxorScarecrowName,
            haxorScarecrowDescription;

        public AssetEditor(IModHelper helper, IMonitor monitor, ITranslationHelper translate, Config config)
        {
            _helper = helper;
            _monitor = monitor;
            _translate = translate;
            _config = config;
           
        }


        public bool CanEdit<T>(IAssetInfo asset)
        {
            //Check to see if we can edit the assets. Will be done soon.
            return asset.AssetNameEquals("Maps/springobjects") || asset.AssetNameEquals("TileSheets/Craftables") || asset.AssetNameEquals("Data/ObjectInformation") || asset.AssetNameEquals("Data/BigCraftablesInformation") || asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            //Lets get the translations. See if this fixes the bloody issues
            HaxorSprinkler.TranslatedName = _translate.Get("haxorsprinkler.name");
            HaxorSprinkler.TranslatedDescription = _translate.Get("haxorsprinkler.description");
            HaxorScarecrow.TranslatedName = _translate.Get("haxorscarecrow.name");
            HaxorScarecrow.TranslatedDescription = _translate.Get("haxorscarecrow.description");

            //Do the edits here. Will be done soon
            if (asset.AssetNameEquals("Maps/springobjects")){
                Texture2D sprinkler =
                    _helper.Content.Load<Texture2D>("Assets/HaxorSprinkler.png", ContentSource.ModFolder);
                Texture2D oldImage = asset.AsImage().Data;
                
                asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice,
                    oldImage.Width,
                    System.Math.Max(oldImage.Height, 1200 / 24 * 16)));
                asset.AsImage().PatchImage(oldImage);
                asset.AsImage().PatchImage(sprinkler, targetArea: this.GetRectangle(HaxorSprinkler.ParentSheetIndex));
            }
            else if (asset.AssetNameEquals("TileSheets/Craftables"))
            {
                Texture2D scarecrow = _helper.Content.Load<Texture2D>("Assets/HaxorScareCrow.png");
                Texture2D oldImage = asset.AsImage().Data;

                asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice,
                    oldImage.Width,
                    System.Math.Max(oldImage.Height, 1200 / 8 * 32)));
                asset.AsImage().PatchImage(oldImage);
                asset.AsImage().PatchImage(scarecrow, targetArea: this.GetRectangleCraftables(HaxorScarecrow.ParentSheetIndex));
            }
            else if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                string ass = $"{HaxorSprinkler.Name}/{HaxorSprinkler.Price}/{HaxorSprinkler.Edibility}/{HaxorSprinkler.Type} {HaxorSprinkler.Category}/{HaxorSprinkler.TranslatedName}/{HaxorSprinkler.TranslatedDescription}";
                asset.AsDictionary<int, string>().Data.Add(HaxorSprinkler.ParentSheetIndex, ass);
                    
                _monitor.Log($"Added Name: {HaxorSprinkler.Name}({HaxorSprinkler.TranslatedName}) Id: {HaxorSprinkler.ParentSheetIndex}.\r\n {ass}");
            }
            else if (asset.AssetNameEquals("Data/BigCraftablesInformation"))
            {
                asset.AsDictionary<int, string>().Data.Add(HaxorScarecrow.ParentSheetIndex, $"{HaxorScarecrow.Name}/{HaxorScarecrow.Price}/{HaxorScarecrow.Edibility}/{HaxorScarecrow.Type} {HaxorScarecrow.Category}/{HaxorScarecrow.TranslatedDescription}/true/false/0/{HaxorScarecrow.TranslatedName}");
                _monitor.Log($"Added Name: {HaxorScarecrow.Name}({HaxorScarecrow.TranslatedName}). Id: {HaxorScarecrow.ParentSheetIndex}");
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
        
                /*
                    
                    Sprinkler
                    {390, 100}, //Easy Mode = 100 Stone
                    {386, 10} // Hard Mode = 10 Iridium Ore
        
                    Scarecrow
                    {388, 100}, //Easy Mode = 100 Wood
                    {337, 10} // Hard Mode = 10 Iridium Bars
                 
                 */
/*
                var curData = asset.AsDictionary<string, string>();
                //bool isEn = asset.Locale == "en";
                string isEnSprik = asset.Locale != "en" ? $"/{HaxorSprinkler.TranslatedName}" : "";
                string isEnScare = asset.Locale != "en" ? $"/{HaxorScarecrow.TranslatedName}" : "";
                _monitor.Log("Made it to the else");
                string sprinklerIngredientsOut = !_config.ActivateHarderIngredients ? $"390 100/Home/{HaxorSprinkler.ParentSheetIndex}/false/null{isEnSprik}" : $"386 10/Home/{HaxorSprinkler.ParentSheetIndex}/false/null{isEnSprik}";
                string scarecrowIngredientsOut = !_config.ActivateHarderIngredients ? $"388 100/Home/{HaxorScarecrow.ParentSheetIndex}/true/null{isEnScare}" : $"337 10/Home/{HaxorScarecrow.ParentSheetIndex}/true/null{isEnScare}";

                if (curData.Data.ContainsKey("Haxor Sprinkler"))
                    curData.Data["Haxor Sprinkler"] = sprinklerIngredientsOut;
                if (curData.Data.ContainsKey("Haxor Scarecrpw"))
                    curData.Data["Haxor Scarecrow"] = scarecrowIngredientsOut;
                if (!curData.Data.ContainsKey("Haxor Sprinkler") && !curData.Data.ContainsKey("Haxor Scarecrow"))
                {
                    //Didn't find the recipes, now we add them
                    try
                    {
                        curData.Data.Add("Haxor Sprinkler", sprinklerIngredientsOut);
                        curData.Data.Add("Haxor Scarecrow", scarecrowIngredientsOut);
                        _monitor.Log($"Added Haxor Sprinkler Recipe: {sprinklerIngredientsOut}");
                        _monitor.Log($"Added Haxor Scarecrow: {scarecrowIngredientsOut}");
                    }
                    catch (Exception ex)
                    {
                        _monitor.Log($"There was an error editing crafting recipes. {ex.ToString()}");
                    }
                       
                }
            }
        }

        //Custom Methods
        public Rectangle GetRectangle(int id)
        {
            return new Rectangle(id % 24 * 16, id / 24 * 16, 16, 16);
        }

        public Rectangle GetRectangleCraftables(int id)
        {
            return new Rectangle(id % 8 * 16, id / 8 * 32, 16, 32);
        }
    }
}*/
