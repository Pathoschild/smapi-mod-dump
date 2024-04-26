/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace GingerIslandStart.Patches;

public class MenuPatches
{
    private static readonly string DataName = ModEntry.NameInData;
    private static ClickableTextureComponent _islandBtn = ModEntry.IslandButton;
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(MenuPatches)}\": postfixing SDV method \"CharacterCustomization.ResetComponents\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(CharacterCustomization), "ResetComponents"),
            postfix: new HarmonyMethod(typeof(MenuPatches), nameof(Post_ResetComponents))
        );
        
        Log($"Applying Harmony patch \"{nameof(MenuPatches)}\": postfixing SDV method \"CharacterCustomization.draw\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
            postfix: new HarmonyMethod(typeof(MenuPatches), nameof(Post_draw)));

        Log($"Applying Harmony patch \"{nameof(MenuPatches)}\": postfixing SDV method \"CharacterCustomization.draw\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(CharacterCustomization),
                nameof(CharacterCustomization.receiveLeftClick)),
            postfix: new HarmonyMethod(typeof(MenuPatches), nameof(Post_receiveLeftClick)));

        
        Log($"Applying Harmony patch \"{nameof(MenuPatches)}\": postfixing SDV method \"CharacterCustomization.optionButtonClick(string)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(CharacterCustomization),
                "optionButtonClick"),
            prefix: new HarmonyMethod(typeof(MenuPatches), nameof(Pre_optionButtonClick)));
    }

    internal static void Post_draw(CharacterCustomization __instance, SpriteBatch b)
    {
        if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm && __instance.source != CharacterCustomization.Source.NewFarmhand)
            return;

        //checkbox
        var position = new Vector2(_islandBtn.bounds.X, _islandBtn.bounds.Y);
        b.Draw(_islandBtn.texture,position,_islandBtn.sourceRect,Color.White,0,Vector2.Zero,4f,SpriteEffects.None,(float) (0.8600000143051147 + _islandBtn.bounds.Y / 20000.0));
        
        //text
        var whereFrom = new Vector2(_islandBtn.bounds.X + _islandBtn.bounds.Width + 8, _islandBtn.bounds.Y - 12);
        Utility.drawTextWithShadow(b, _islandBtn.label, Game1.smallFont, whereFrom, Game1.textColor);
    }

    // ReSharper disable once UnusedParameter.Local
    private static void Post_receiveLeftClick(CharacterCustomization __instance, int x, int y, bool playSound = true)
    {
        if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm && __instance.source != CharacterCustomization.Source.NewFarmhand)
            return;
        
        if (!_islandBtn.containsPoint(x, y))
            return;
        
        //change island opt
        Game1.playSound("drumkit6");
        _islandBtn.sourceRect.X = _islandBtn.sourceRect.X == 227 ? 236 : 227;
        var toggle = !ModEntry.EnabledOption;
        ModEntry.EnabledOption = toggle;

        //toggle mod data value
        if (!Game1.player.modData.TryGetValue(DataName, out _))
            Game1.player.modData.Add(DataName, toggle.ToString());
        else
            Game1.player.modData[DataName] = toggle.ToString();

        ToggleSkipIntro(__instance, toggle);
    }

    private static void ToggleSkipIntro(CharacterCustomization __instance, bool toggle)
    {
        //if toggled to true, skip intro must also be true
        if (!toggle)
            return;
        
        ModEntry.Help.Reflection.GetField<bool>(__instance, "skipIntro").SetValue(true);
        __instance.skipIntroButton.sourceRect.X = 236;
    }

    private static void Post_ResetComponents(CharacterCustomization __instance)
    {
        if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm && __instance.source != CharacterCustomization.Source.NewFarmhand)
            return;

        __instance.skipIntroButton.downNeighborID = _islandBtn.myID;
        _islandBtn.bounds = __instance.skipIntroButton.bounds;
        _islandBtn.bounds.Y += 64;
        
        if(ModEntry.Help.ModRegistry.IsLoaded("PeacefulEnd.FashionSense") && __instance.source != CharacterCustomization.Source.NewFarmhand)
            _islandBtn.bounds.Y += 16;
    }

    internal static void Pre_optionButtonClick(CharacterCustomization __instance, string name)
    {
        if (name != "OK")
            return;

        ToggleSkipIntro(__instance, true);
        
        if (__instance.source != CharacterCustomization.Source.NewFarmhand)
            return;
        
        Game1.warpFarmer("IslandSouth", ModEntry.StartingPoint.X, ModEntry.StartingPoint.Y, false);
        Game1.delayedActions.Add(new DelayedAction(1000, DoMpChanges));
    }

    private static void DoMpChanges()
    {
        Additions.IslandChanges.ChangeGiftLocation();
        Events.Location.WarpToIsland();
        Events.Day.TryAddQuest();
        
        Game1.player.addItemToInventory(ItemRegistry.Create("(O)TentKit"));
    }
}