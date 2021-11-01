/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    internal static class HarmonyPatch_UntimedSO
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        private readonly static List<string> NPCNames = new List<string> { 
            "Acorn", "Aguar", "Alissa", "Anton", "Ariah", "Belinda", "Bert", "Blair", "Bliss", "Bryle", "Carmen",
            "Corine", "Daia", "Ezekiel", "Faye", "Flor", "Freddie", "Helen", "Ian", "Irene", "Jeric", "Jio", "Keahi",
            "Kenneth", "Kiarra", "Kimpoi", "Kiwi", "Lenny", "Lola", "Lorenzo", "Louie", "Maddie", "Maive", "Malaya",
            "Naomi", "Olga", "Paula", "Philip", "Pika", "Pipo", "Raeriyala", "Richard", "Sari", "Shanice", "Shiro",
            "Sonny", "Trinnie", "Undreya", "Ysabelle", "Yuuma", "Zayne"};
        private static Texture2D RSVemojis;

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            Helper.Events.GameLoop.DayEnding += OnDayEnd;
            Log.Trace($"Applying Harmony Patch \"{nameof(HarmonyPatch_UntimedSO)}\" prefixing SDV method.");
            harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.GetPortraitForRequester)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_UntimedSO), nameof(SpecialOrdersBoard_GetPortrait_postifx))
            );
            try
            {
                Type QFSpecialBoardClass = Type.GetType("QuestFramework.Framework.Menus.CustomOrderBoard, QuestFramework");
                harmony.Patch(
                    original: AccessTools.Method(QFSpecialBoardClass, "GetPortraitForRequester"),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_UntimedSO), nameof(SpecialOrdersBoard_GetPortrait_postifx))
                );
            }
            catch
            {
                Log.Warn("Couldnt patch Quest Framework. Emojis in the SO board might not show up");
            }
           
        }
        
        private static void SpecialOrdersBoard_GetPortrait_postifx(SpecialOrdersBoard __instance, string requester_name, ref KeyValuePair<Texture2D, Rectangle>?  __result)
        {
            try
            {
                if (RSVemojis == null)
                {
                    RSVemojis = Game1.content.Load<Texture2D>("LooseSprites\\RSVemojis");
                }

                if (__result == null)
                {
                    int index = NPCNames.FindIndex(name => name.Equals(requester_name, StringComparison.OrdinalIgnoreCase));
                    if (index != -1)
                    {
                        __result = new KeyValuePair<Texture2D, Rectangle>(HarmonyPatch_UntimedSO.RSVemojis, new Rectangle(index % 14 * 9, index / 14 * 9, 9, 9));
                        return;
                    }
                }
                return;
            }
            catch(Exception e)
            {
                Log.Error("Error in SpecialOrdersBoard_GetPortrait_postifx");
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                return;
            }
            
        }

        public static void OnDayEnd(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            foreach(SpecialOrder o in Game1.player.team.specialOrders)
            {
                if (o.questKey.Value.StartsWith("RSV.UntimedSpecialOrder"))
                {
                    o.dueDate.Value = Game1.Date.TotalDays + 100;
                }
            }
        }

    }
}
