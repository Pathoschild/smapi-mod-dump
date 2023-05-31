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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Misc
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Misc.StatueOfPerfectionTracking")]
    public class StatueOfPerfecionTracking : CustomObject
    {

        public StatueOfPerfecionTracking()
        {

        }

        public StatueOfPerfecionTracking(BasicItemInformation ItemInfo) : base(ItemInfo)
        {

        }

        public StatueOfPerfecionTracking(BasicItemInformation ItemInfo, Vector2 TilePosition) : base(ItemInfo, TilePosition)
        {

        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (Game1.menuUp || Game1.currentMinigame != null) return false;

            Game1.playSound("qi_shop");
            StringBuilder sb = new StringBuilder();
            bool smallWindow = Game1.viewport.Height < 850;
            List<string> brokenUpDialogue = new List<string>();


            //Add all lines of dialogue below.
            //Add more statistics below!

            Dictionary<string, string> strings = JsonContentPackUtilities.LoadStringDictionaryFile(Path.Combine(StringsPaths.UI, "Objects", "StatueOfPerfectionTracking.json"));


            List<string> lines = new List<string>();

            lines.Add(Utility.loadStringShort("UI", "PT_Title") + "^");
            lines.Add(strings["LineDivide"]);
            lines.Add(Utility.loadStringShort("UI", "PT_Shipped") + ": " + this.FormatCompletionLine((Farmer farmer) => (float)Math.Floor(Utility.getFarmerItemsShippedPercent(farmer) * 100f)) + "%^");
            lines.Add(strings["Missing"]);

            foreach (KeyValuePair<int, string> kvp in Game1.objectInformation)
            {
                string typeString = kvp.Value.Split('/')[3];
                string displayName = kvp.Value.Split('/')[0];
                if (!typeString.Contains("Arch") && !typeString.Contains("Fish") && !typeString.Contains("Mineral") && !typeString.Substring(typeString.Length - 3).Equals("-2") && !typeString.Contains("Cooking") && !typeString.Substring(typeString.Length - 3).Equals("-7") && StardewValley.Object.isPotentialBasicShippedCategory(kvp.Key, typeString.Substring(typeString.Length - 3)))
                {
                    if (!who.basicShipped.ContainsKey(kvp.Key))
                    {
                        lines.Add("  -" + displayName + "^");
                    }
                }
            }

            lines.Add(Utility.loadStringShort("UI", "PT_Obelisks") + ": " + Math.Min(Utility.numObelisksOnFarm(), 4) + "/4^");
            lines.Add(Utility.loadStringShort("UI", "PT_GoldClock") + ": " + (Game1.getFarm().isBuildingConstructed("Gold Clock") ? Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes") : Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
            lines.Add(Utility.loadStringShort("UI", "PT_MonsterSlayer") + ": " + this.FormatCompletionLine((Farmer farmer) => farmer.hasCompletedAllMonsterSlayerQuests.Value, Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
            lines.Add(Utility.loadStringShort("UI", "PT_GreatFriends") + ": " + this.FormatCompletionLine((Farmer farmer) => (float)Math.Floor(Utility.getMaxedFriendshipPercent(farmer) * 100f)) + "%^");
            lines.Add(Utility.loadStringShort("UI", "PT_FarmerLevel") + ": " + this.FormatCompletionLine((Farmer farmer) => Math.Min(farmer.Level, 25)) + "/25^");
            lines.Add(Utility.loadStringShort("UI", "PT_Stardrops") + ": " + this.FormatCompletionLine((Farmer farmer) => Utility.foundAllStardrops(farmer), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
            lines.Add(Utility.loadStringShort("UI", "PT_Cooking") + ": " + this.FormatCompletionLine((Farmer farmer) => (float)Math.Floor(Utility.getCookedRecipesPercent(farmer) * 100f)) + "%^");

            lines.Add(strings["Missing"]);
            Dictionary<string, string> cookingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            foreach (KeyValuePair<string, string> v in cookingRecipes)
            {
                if (who.cookingRecipes.ContainsKey(v.Key))
                {
                    int recipe = Convert.ToInt32(v.Value.Split('/')[2].Split(' ')[0]);
                    string displayName = v.Key;
                    if (!who.recipesCooked.ContainsKey(recipe))
                    {
                        lines.Add("  -" + displayName + "^");
                    }
                }
            }

            lines.Add(Utility.loadStringShort("UI", "PT_Crafting") + ": " + this.FormatCompletionLine((Farmer farmer) => (float)Math.Floor(Utility.getCraftedRecipesPercent(farmer) * 100f)) + "%^");

            lines.Add(strings["Missing"]);
            Dictionary<string, string> craftingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
            foreach (string s in craftingRecipes.Keys)
            {
                string displayName = s;
                if (!(s == "Wedding Ring") && who.craftingRecipes.ContainsKey(s) && who.craftingRecipes[s] == 0)
                {
                    lines.Add("  -" + displayName + "^");
                }
            }

            lines.Add(Utility.loadStringShort("UI", "PT_Fish") + ": " + this.FormatCompletionLine((Farmer farmer) => (float)Math.Floor(Utility.getFishCaughtPercent(farmer) * 100f)) + "%^");
            lines.Add(strings["Missing"]);
            foreach (KeyValuePair<int, string> v in Game1.objectInformation)
            {
                string displayName = v.Value.Split('/')[0];
                if (v.Value.Split('/')[3].Contains("Fish") && (v.Key < 167 || v.Key > 172) && (v.Key < 898 || v.Key > 902))
                {
                    if (!who.fishCaught.ContainsKey(v.Key))
                    {
                        lines.Add("  -" + displayName + "^");
                    }
                }
            }

            lines.Add(Utility.loadStringShort("UI", "PT_GoldenWalnut") + ": " + Math.Min(Game1.netWorldState.Value.GoldenWalnutsFound, 130) + "/" + 130 + "^");
            lines.Add(strings["LineDivide"]);
            lines.Add(Utility.loadStringShort("UI", "PT_Total") + ": " + Math.Floor(Utility.percentGameComplete() * 100f) + "%^");

            for (int i = 0; i < lines.Count; i++)
            {
                sb.AppendLine(lines[i]);
                //For smaller windows we need to break up the dialogue.
                if (smallWindow && i % 7 == 0 && i != 0)
                {
                    sb.AppendLine("...");
                    brokenUpDialogue.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
                //For larger windows we can have more dialogue.
                if (!smallWindow && i % 12 == 0 && i != 0)
                {
                    sb.AppendLine("...");
                    brokenUpDialogue.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
                //Add any remaining lines to the broken up dialogue.
                if (i == lines.Count - 1)
                {
                    brokenUpDialogue.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
            }

            if (smallWindow)
            {
                Game1.multipleDialogues(brokenUpDialogue.ToArray());
            }
            else
            {
                Game1.drawDialogueNoTyping(sb.ToString());
            }

            return true;
        }

        public virtual string FormatCompletionLine(Func<Farmer, float> check)
        {
            KeyValuePair<Farmer, float> kvp = Utility.GetFarmCompletion(check);
            if (kvp.Key == Game1.player)
            {
                return kvp.Value.ToString();
            }
            return "(" + kvp.Key.Name + ") " + kvp.Value;
        }

        public virtual string FormatCompletionLine(Func<Farmer, bool> check, string true_value, string false_value)
        {
            KeyValuePair<Farmer, bool> kvp = Utility.GetFarmCompletion(check);
            if (kvp.Key == Game1.player)
            {
                if (!kvp.Value)
                {
                    return false_value;
                }
                return true_value;
            }
            return "(" + kvp.Key.Name + ") " + (kvp.Value ? true_value : false_value);
        }

        public static string loadStringShort(string fileWithinStringsFolder, string key)
        {
            return Game1.content.LoadString("Strings\\" + fileWithinStringsFolder + ":" + key);
        }

        public override Item getOne()
        {
            return new StatueOfPerfecionTracking(this.basicItemInformation.Copy());
        }
    }
}
