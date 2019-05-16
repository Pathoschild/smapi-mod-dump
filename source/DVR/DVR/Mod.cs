using Microsoft.Xna.Framework;
using PyTK.CustomTV;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVR
{
    public class MyDialogue : DialogueBox
    {
        public MyDialogue(string dialogue, List<Response> responses, int width = 1200) : base(dialogue, responses, width) { }
    }

    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += DayStarted;
            CustomTVMod.addChannel("fish", Helper.Translation.Get("fish_channel"), showFishChannel);
            CustomTVMod.changeAction("rerun", showRerun);
        }

        private void showRerun(TV tv, TemporaryAnimatedSprite tas, Farmer f, string s)
        {
            var screenInfo = Helper.Reflection.GetField<TemporaryAnimatedSprite>(tv, "screen");
            var strs = getWeeklyRecipe();
            screenInfo.SetValue(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(602, 361, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)(tv.boundingBox.Bottom - 1) / 10000f + 1E-05f, 0f, Color.White, tv.getScreenSizeModifier(), 0f, 0f, 0f, false));
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13127")));
            Game1.afterDialogues = new Game1.afterFadeFunction(() =>
            {
                Game1.multipleDialogues(strs);
                Game1.afterDialogues = new Game1.afterFadeFunction(() => tv.turnOffTV());
            });
        }

        private void showFishChannel(TV tv, TemporaryAnimatedSprite tas, Farmer frm, string s)
        {
            var screenInfo = Helper.Reflection.GetField<TemporaryAnimatedSprite>(tv, "screen");
            var fish = fishForToday[timesViewedFish];
            timesViewedFish = (timesViewedFish + 1) % fishForToday.Count;
            var pos = tv.getScreenPosition();
            var scale = tv.getScreenSizeModifier() + 1;
            pos.X += 7 * scale;
            pos.Y += (float)(scale * 1.4);

            screenInfo.SetValue(new TemporaryAnimatedSprite(Game1.objectSpriteSheetName, GameLocation.getSourceRectForObject(fish.ID), 150f, 1, 999999, pos, false, false, (float)(tv.boundingBox.Bottom - 1) / 10000f + 1E-05f, 0f, Color.White, scale, 0f, 0f, 0f, false));

            Game1.drawObjectDialogue(Game1.parseText(Helper.Translation.Get("fish_intro")));
            Game1.afterDialogues = new Game1.afterFadeFunction(() =>
            {
                Game1.multipleDialogues(fish.GetTvText(Helper.Translation).ToArray());
                Game1.afterDialogues = new Game1.afterFadeFunction(() => tv.turnOffTV());
            });

        }

        private int todaysRecipe = 0;
        private IList<Fish> fishForToday = null;
        private int timesViewedFish = 0;

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            todaysRecipe = 0;
            timesViewedFish = 0;
            fishForToday = Fishies.GetFishForToday();
        }


        // straight from decompilation with a few modifications, in commented block
        protected virtual string[] getWeeklyRecipe()
        {
            string str;
            string str1;
            string str2;
            string[] text = new string[2];
            // modifications start
            int maxWeek = (int)Game1.stats.DaysPlayed / 7;
            Dictionary<string, string> cookingRecipeChannel = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
            var whichWeek = maxWeek;
            if (todaysRecipe != 0)
            {
                whichWeek = todaysRecipe;
            }
            else
            {
                var possible = cookingRecipeChannel.Where(x => int.Parse(x.Key) <= maxWeek).ToList();
                var unknown = possible.Where(x => !Game1.player.cookingRecipes.ContainsKey(x.Value.Split('/')[0])).ToList();
                if (unknown.Any())
                {
                    possible = unknown;
                }
                Random r = new Random((int)(Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2));
                whichWeek = int.Parse(possible[r.Next(possible.Count)].Key);
                todaysRecipe = whichWeek;
            }
            // modifications end
            try
            {
                string recipeName = cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[0];
                text[0] = cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[1];
                if (!CraftingRecipe.cookingRecipes.ContainsKey(recipeName))
                {
                    string[] strArrays = text;
                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                    {
                        str = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", recipeName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", recipeName));
                    }
                    else
                    {
                        str = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' }).Last<string>()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' }).Last<string>()));
                    }
                    strArrays[1] = str;
                }
                else
                {
                    string[] split = CraftingRecipe.cookingRecipes[recipeName].Split(new char[] { '/' });
                    string[] strArrays1 = text;
                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                    {
                        str1 = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", recipeName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", recipeName));
                    }
                    else
                    {
                        str1 = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", split[(int)split.Length - 1]) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", split[(int)split.Length - 1]));
                    }
                    strArrays1[1] = str1;
                }
                if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
                {
                    Game1.player.cookingRecipes.Add(recipeName, 0);
                }
            }
            catch (Exception exception)
            {
                string recipeName = cookingRecipeChannel["1"].Split(new char[] { '/' })[0];
                text[0] = cookingRecipeChannel["1"].Split(new char[] { '/' })[1];
                string[] strArrays2 = text;
                if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                {
                    str2 = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel["1"].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", recipeName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", recipeName));
                }
                else
                {
                    str2 = (Game1.player.cookingRecipes.ContainsKey(cookingRecipeChannel["1"].Split(new char[] { '/' })[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", cookingRecipeChannel["1"].Split(new char[] { '/' }).Last<string>()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", cookingRecipeChannel["1"].Split(new char[] { '/' }).Last<string>()));
                }
                strArrays2[1] = str2;
                if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
                {
                    Game1.player.cookingRecipes.Add(recipeName, 0);
                }
            }
            return text;
        }
    }

}
