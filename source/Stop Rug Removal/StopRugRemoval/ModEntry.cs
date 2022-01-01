/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using StardewValley.Objects;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace StopRugRemoval;

public class ModConfig
{
    public bool Enabled { get; set; } = true;
    //public bool CanPlaceRugsOutside { get; set; } = false;
    public bool CanPlaceRugsUnder { get; set; } = true;
}
public class ModEntry : Mod
{
    public static IMonitor ModMonitor;
    private static ModConfig config;
    public override void Entry(IModHelper helper)
    {
        config = Helper.ReadConfig<ModConfig>();
        ModMonitor = Monitor;
        I18n.Init(helper.Translation);

        Harmony harmony = new(ModManifest.UniqueID);
        ModMonitor.Log("Patching Furniture::CanBeRemoved to prevent accidental rug removal", LogLevel.Debug);
        harmony.Patch(
            original: AccessTools.Method(typeof(Furniture), nameof(Furniture.canBeRemoved)),
            postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PostfixCanBeRemoved))
            );
        //harmony.Patch(
        //     original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.CanPlaceThisFurnitureHere)),
        //     postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PostfixCanPlaceFurnitureHere))
        //     );

        helper.Events.GameLoop.GameLaunched += SetUpConfig;
    }

    private void SetUpConfig(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(
            mod: ModManifest,
            reset: () => config = new ModConfig(),
            save: () => Helper.WriteConfig(config)
            );

        configMenu.AddParagraph(
            mod: ModManifest,
            text: I18n.Mod_Description
            );

        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config.Enabled,
            setValue: value => config.Enabled = value,
            name: I18n.Enabled_Title
            );
    }

    private static bool PrefixCanBePlacedHere(Furniture __instance, GameLocation __0, Vector2 tile, bool __result)
    {
        try
        {
            if (!config.Enabled || !config.CanPlaceRugsUnder) { return true; }
            if (__instance.furniture_type.Value.Equals(Furniture.rug)) { return true; }
            if (__instance.placementRestriction != 0) { return true; }//someone requested a custom placement restriction, respect that.
            Rectangle bounds = __instance.boundingBox.Value;
            bool okaytoplace = true;
            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    //check for large terrain+terrain, refuse placement.
                    //check for is placeable everywhere, and if the thing that's blocking placement is an Object or 
                    //another furniture item, I'm still okay to place.
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Ran into errors in PrefixCanBePlacedHere for {__instance.Name} at {__0.NameOrUniqueName} ({tile.X}, {tile.Y})\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
    //private static void PostfixCanPlaceFurnitureHere(GameLocation __instance, Furniture __0, ref bool __result)
    //{
    //    try
    //    {
    //        if (!config.Enabled || !config.CanPlaceRugsOutside) { return; } //mod disabled
    //        if (__instance is MineShaft || __instance is VolcanoDungeon) { return; }//do not want to affect mines
    //        if (__result) { return; }//can already be placed
    //        if (__0.placementRestriction != 0) { return; }//someone requested a custom placement restriction, respect that.
    //        if (__0.furniture_type.Value.Equals(Furniture.rug)) { __result = true; return; }//Let me place rug
    //    }
    //    catch(Exception ex)
    //    {
    //        ModMonitor.Log($"Failed in attempting to place rug outside in PostfixCanPlaceFurnitureHere.\n{ex}", LogLevel.Error);
    //    }
    //}

    private static void PostfixCanBeRemoved(Furniture __instance, ref Farmer __0, ref bool __result)
    {
        try
        {
            if (!config.Enabled) { return; } //mod disabled
            if (!__result) { return; } //can't be removed already
            if (!__instance.furniture_type.Value.Equals(Furniture.rug)) { return; } //only want to deal with rugs
            GameLocation currentLocation = __0.currentLocation; //get location of farmer
            if (currentLocation == null) { return; }

            Rectangle bounds = __instance.boundingBox.Value;
            ModMonitor.Log($"Checking rug: {bounds.X / 64f}, {bounds.Y / 64f}, W/H {bounds.Width / 64f}/{bounds.Height / 64f}");

            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    if (!currentLocation.isTileLocationTotallyClearAndPlaceable(x + bounds.X / 64, y + bounds.Y / 64))
                    {
                        Game1.showRedMessage(I18n.RugRemovalMessage());
                        __result = false;
                        return;
                    };
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Ran into issues with postfix for Furniture::CanBeRemoved for {__instance.Name}\n\n{ex}", LogLevel.Error);
        }
    }
}
