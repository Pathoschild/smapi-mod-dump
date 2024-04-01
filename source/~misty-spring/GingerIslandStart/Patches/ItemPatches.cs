/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using GingerIslandStart.Additions;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace GingerIslandStart.Patches;

public class ItemPatches
{
    private static string PirateKey => $"{ModEntry.Id}_PirateRecovery";
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ItemPatches)}\": prefixing SDV method \"Item.actionWhenPurchased\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Item), nameof(Item.actionWhenPurchased)),
            prefix: new HarmonyMethod(typeof(ItemPatches), nameof(Pre_actionWhenPurchased))
        );
    }
    
    internal static bool Pre_actionWhenPurchased(Item __instance, string shopId)
    {
        #if DEBUG
        Log(shopId);
        #endif

        if (shopId != PirateKey)
            return true;
        
        if (!__instance.isLostItem)
            return true;
        
        Game1.player.itemsLostLastDeath.Clear();
        __instance.isLostItem = false;
        Game1.player.recoveredItem = __instance;
        ModEntry.Help.Data.WriteSaveData( $"{ModEntry.RecoveryKey}", new ItemSaveData(__instance));
        Game1.playSound("newArtifact");
        Game1.exitActiveMenu();
        var hasMultiple = __instance.Stack > 1;
        
        var pirate = Game1.getCharacterFromName("Marlon");
        pirate.Portrait = Game1.content.Load<Texture2D>($"Mods/{ModEntry.Id}/Shop_Pirate");
        pirate.displayName = Game1.content.LoadString("Strings/UI:LevelUp_ProfessionName_Pirate");
        
        var text = hasMultiple ? ModEntry.Help.Translation.Get("ItemRecovery_Engaged_Stack") : ModEntry.Help.Translation.Get("ItemRecovery_Engaged");
        var dialogueText = string.Format(text, Lexicon.makePlural(__instance.DisplayName, !hasMultiple));
        var dialogue = new Dialogue(pirate, null, dialogueText);
        
        Game1.DrawDialogue(dialogue);
        return false;
    }
}