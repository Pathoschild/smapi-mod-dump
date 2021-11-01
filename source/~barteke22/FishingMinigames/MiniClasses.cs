/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FishingMinigames
{
    public class MinigameMessage
    {
        public long multiplayerID;
        public string stage;
        public string voiceType;
        public float voicePitch;
        public bool drawAttachments;
        public int whichFish;
        public int fishQuality;
        public int maxFishSize;
        public float fishSize;
        public float itemSpriteSize;
        public int stack;
        public bool recordSize;
        public bool furniture;
        public Rectangle sourceRect;
        public int x;
        public int y;
        public int oldFacingDirection;


        public MinigameMessage()
        {
            this.multiplayerID = -1L;
            this.stage = null;
            this.sourceRect = new Rectangle();
        }

        public MinigameMessage(Farmer whichPlayer, string stage, string voiceType, float voicePitch, bool drawAttachments, int whichFish, int fishQuality, int maxFishSize, float fishSize, float itemSpriteSize, int stack, bool recordSize, bool furniture, Rectangle sourceRect, int x, int y, int oldFacingDirection)
        {
            this.multiplayerID = whichPlayer.UniqueMultiplayerID;
            this.stage = stage;
            this.voiceType = voiceType;
            this.voicePitch = voicePitch;
            this.drawAttachments = drawAttachments;
            this.whichFish = whichFish;
            this.fishQuality = fishQuality;
            this.maxFishSize = maxFishSize;
            this.fishSize = fishSize;
            this.itemSpriteSize = itemSpriteSize;
            this.stack = stack;
            this.recordSize = recordSize;
            this.furniture = furniture;
            this.sourceRect = sourceRect;
            this.x = x;
            this.y = y;
            this.oldFacingDirection = oldFacingDirection;
        }
    }


    class MinigameColor
    {
        public Color color;
        public Vector2 pos = new Vector2(0f);
        public int whichSlider = 0;
    }
    class DummyMenu : StardewValley.Menus.IClickableMenu
    {
        public DummyMenu()
        {
            //this is just to prevent other mods from interfering with minigames
        }
    }


    //[HarmonyPatch(typeof(Tool), "getDescription")]
    class HarmonyPatches
    {
        public static void getDescription_Nets(ref string __result, ref Tool __instance)
        {
            try
            {
                if (__instance is StardewValley.Tools.FishingRod && __instance.UpgradeLevel != 1)//bamboo+ (except training)
                {
                    string desc = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14042");

                    desc += "\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.Name);

                    if (__instance.UpgradeLevel > 1)//fiber/iridium
                    {
                        if (__instance.attachments[0] != null)
                        {
                            desc += "\n\n" + __instance.attachments[0].DisplayName + ((__instance.attachments[0].Quality == 0) ? "" : " (" + FishingMinigames.ModEntry.translate.Get("Mods.Infinite") + ")")
                                   + ":\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.attachments[0].Name);
                        }
                        if (__instance.attachments[1] != null)
                        {
                            desc += "\n\n" + __instance.attachments[1].DisplayName + ((__instance.attachments[1].Quality == 0) ? "" : " (" + FishingMinigames.ModEntry.translate.Get("Mods.Infinite") + ")")
                                   + ":\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.attachments[1].Name);
                        }
                    }
                    if (desc.EndsWith("\n")) desc = desc.Substring(0, desc.Length - 1);
                    __result = Game1.parseText(desc, Game1.smallFont, desc.Length * 10);
                }
            }
            catch (System.Exception e)
            {
                Log.Error("Error in harmony patch: " + e.Message);
            }
        }
    }
}
