/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/



using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Enchantments;
using System;
using NuclearBombLocations;
using StardewModdingAPI;
using StardewValley.BellsAndWhistles;
using StardewValley.Events;
using StardewValley.Characters;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace NuclearBombLocations

{
    [HarmonyPatch(typeof(Tree), nameof(Tree.performToolAction))]
    public static class TreeToolActionPatch
    {
        public static void Prefix(Tree __instance, Tool t, int explosion)
        {

            if (t is Axe)
            {
                if (Game1.currentLocation is ClairabelleLagoon)
                {
                    __instance.health.Value = 999999999999;
                    Game1.player.Money -= 10;
                    Game1.addHUDMessage(new HUDMessage("Ranger Anabelle tickets you for illegal wood harvesting in the national park! You got what was coming to you!!!", 1));
                }
            }
            else if (explosion >= 0)
            {
                if (Game1.currentLocation is ClairabelleLagoon)
                {
                    __instance.health.Value = 999999999999;
                    Game1.player.Money -= 10;
                    Game1.addHUDMessage(new HUDMessage("Ranger Anabelle tickets you for illegal wood harvesting in the national park! You got what was coming to you!!!", 1));

                }
            }
        }
    }

    /*

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.loadMap))]
    public static class GameLocationsLoadPatch
    {
        public static void Prefix(GameLocation __instance, string mapPath, bool force_reload = false)
        {

            if (__instance is not AtarraMountainTop)
            {
                return;
            }


            {
                __instance.waterTiles = new WaterTiles(__instance.map.GetLayer("AlwaysFront2").LayerWidth, __instance.map.GetLayer("AlwaysFront2").LayerHeight);
                bool foundAnyWater;
                foundAnyWater = false;
                for (int x = 0; x < __instance.map.GetLayer("AlwaysFront2").LayerWidth; x++)

                {
                    for (int y = 0; y < __instance.map.GetLayer("AlwaysFront2").LayerHeight; y++)
                    {
                        string water_property;
                        water_property = __instance.doesTileHaveProperty(x, y, "Water", "AlwaysFront2");
                        if (water_property != null)
                        {
                            foundAnyWater = true;
                           
                           
                            {
                                __instance.waterTiles[x, y] = true;
                            }
                        }
                    }
                }
            }

            // Maybe foreach tile???  


        }
    }


    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawWater))]

    public static class GameLocationsWaterDrawPatch
    // public virtual void drawWater(SpriteBatch b)
    {
        public static void prefix(GameLocation __instance, SpriteBatch b)
        {

            if( __instance is not AtarraMountainTop )
            {
                return;
            }
            __instance.currentEvent?.drawUnderWater(b);
        if (__instance.waterTiles == null)
        {
            return;
        }
        for (int y = Math.Max(0, Game1.viewport.Y / 64 - 1); y < Math.Min(__instance.map.GetLayer("AlwaysFront2").LayerHeight, (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2); y++)
        {
            for (int x = Math.Max(0, Game1.viewport.X / 64 - 1); x < Math.Min(__instance.map.GetLayer("AlwaysFront2").LayerWidth, (Game1.viewport.X + Game1.viewport.Width) / 64 + 1); x++)
            {
                if (__instance.waterTiles.waterTiles[x, y].isWater && __instance.waterTiles.waterTiles[x, y].isVisible)
                {
                        __instance.drawWaterTile(b, x, y);
                }
            }
        }
        }
    }


    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawWater))]

    public static class GameLocationsWaterDrawTilePatch
    // public virtual void drawWater(SpriteBatch b)
    {
        public static void prefix(GameLocation __instance, SpriteBatch b, int x, int y, Color color)
        {

            if (__instance is not AtarraMountainTop)
            {
                return;
            }

          //  public void drawWaterTile(SpriteBatch b, int x, int y, Color color)
            {
                bool num;
                num = y == __instance.map.GetLayer("AlwaysFront2").LayerHeight - 1 || __instance.waterTiles[x, y + 1];
                bool topY;
                topY = y == 0 || __instance.waterTiles[x, y - 1];
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - (int)((!topY) ? __instance.waterPosition : 0f))), new Microsoft.Xna.Framework.Rectangle(__instance.waterAnimationIndex * 64, 2064 + (((x + y) % 2 != 0) ? ((!__instance.waterTileFlip) ? 128 : 0) : (__instance.waterTileFlip ? 128 : 0)) + (topY ? ((int)__instance.waterPosition) : 0), 64, 64 + (topY ? ((int)(0f - __instance.waterPosition)) : 0)), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
                if (num)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y + 1) * 64 - (int)__instance.waterPosition)), new Microsoft.Xna.Framework.Rectangle(__instance.waterAnimationIndex * 64, 2064 + (((x + (y + 1)) % 2 != 0) ? ((!__instance.waterTileFlip) ? 128 : 0) : (__instance.waterTileFlip ? 128 : 0)), 64, 64 - (int)(64f - __instance.waterPosition) - 1), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
                }
            }

        }
    }

    */

            /*
            [HarmonyPatch(typeof(NPC), nameof(NPC.isGaySpouse))]
            public static class GayNPCPatch
            {
                public static void NPC__isGaySpouse__Prefix(
                          StardewValley.NPC __instance,
                          ref bool __result)
                {

                    if (!__instance.Name.Equals("MermaidRangerMarisol"))
                        return;

                    if (__instance.Name.Equals("MermaidRangerMarisol") && Game1.player.isMale
                            )
                    {
                        __result = true;
                    }
                    else if
                        (__instance.Name.Equals("MermaidRangerMarisol") && !Game1.player.isMale
                            )
                    {
                        __result = false;
                    }
                }
            }   
            */


            [HarmonyPatch(typeof(QuestionEvent), nameof(QuestionEvent.setUp))]
    public static class QuestionEventCPatch
    {
        public static bool QuestionEvent_setUp_Prefix(int ___whichQuestion, ref bool __result)
        {
            if (___whichQuestion == 1)
            {
                if (Game1.player.spouse.Equals("MermaidRangerMarisol"))
                {
                    __result = true;
                    return false;
                }
                Response[] answers = new Response[]
                {
                    new Response("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
                    new Response("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No"))
                };

                if (!Game1.player.Gender.Equals(Gender.Male))
                {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion"), answers, new GameLocation.afterQuestionBehavior(ModEntry.answerPregnancyQuestion));
                }
                else
                {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion_Adoption" ), answers, new GameLocation.afterQuestionBehavior(ModEntry.answerPregnancyQuestion));
                }
                Game1.messagePause = true;
                __result = false;
                return false;
            }
            return true;
        }

    }
    

    [HarmonyPatch(typeof(BirthingEvent), nameof(BirthingEvent.setUp))]
    public static class BirthingEventSetup
    {
        public static bool BirthingEvent_setUp_Prefix(ref bool ___isMale, ref string ___message, ref bool __result)
        {

            if (!Game1.player.spouse.Equals("MermaidRangerMarisol"))
                return false;

           
            Game1.player.CanMove = false;
            ___isMale = false;
            if (Game1.player.Gender.Equals(Gender.Male))
            {
                ___message = Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", Lexicon.getGenderedChildTerm(___isMale));
            }
            else 
            {
                ___message = Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", Lexicon.getGenderedChildTerm(___isMale));
            }
           
            __result = false;
            return false;
        }
    }

    /*
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.UpdateWhenCurrentLocation))]
    public static class Game1UpdateDungeonLocationsPatch
    {
        public static void Postfix(SlimeTent __instance, GameTime time)
        {
            //if (Game1.menuUp && !Game1.IsMultiplayer)
            //{
            //return;
            //}
            if (Game1.IsClient)
            {
                return;
            }
            //MermaidTrain mermaidTrain = new MermaidTrain();
            //mermaidTrain.Update(time);

            //SlimeTent.UpdateWhenCurrentLocation();

        }
    }*/

}