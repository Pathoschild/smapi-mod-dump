/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/FreezeTimeMultiplayer
**
*************************************************/

using StardewModdingAPI;

namespace WeirdSounds;

internal partial class Mod: StardewModdingAPI.Mod
{
    public override void Entry(IModHelper helper)
    {
        EnableMod();
        WeirdSoundsLibrary.Load(this);
        Helper.Events.Input.ButtonPressed += ButtonPressedEvent;
    }

    private bool EnableMod()
    {
        Patcher.PatchAll(this);
        Helper.Events.Player.Warped += WarpedEvent;
        Helper.Events.Display.MenuChanged += MenuChangedEvent;
        Helper.Events.GameLoop.DayStarted += DayStartedEvent;
        Helper.Events.GameLoop.TimeChanged += TimeChangeEvent;
        Helper.Events.GameLoop.UpdateTicked += UpdateTickedEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingEvent;
        return true;
    }

    private bool DisableMod()
    {
        Patcher.UnpatchAll();
        Helper.Events.Player.Warped -= WarpedEvent;
        Helper.Events.Display.MenuChanged -= MenuChangedEvent;
        Helper.Events.GameLoop.DayStarted -= DayStartedEvent;
        Helper.Events.GameLoop.TimeChanged -= TimeChangeEvent;
        Helper.Events.GameLoop.UpdateTicked -= UpdateTickedEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking -= OneSecondUpdateTickingEvent;
        return true;
    }
}

internal struct Mutex
{
    internal static bool DisableMod;

    internal static bool ToolMutex;
        
    internal static bool DeathMutex;
    
    internal static bool WeaponMutex;

    internal static readonly int[] WeaponAnimate = [256, 232, 240, 248, 278, 272, 274, 276, 259, 234, 243, 252, 184, 160, 168, 176];
    
    internal static readonly Dictionary<int, bool> CluckMutex = [];
        
    internal static readonly Dictionary<int, bool> CatFlopDictionary = [];
        
    internal static readonly Dictionary<int, bool> SerpentBarkDictionary = [];

    internal static void DailyClearCache()
    {
        CluckMutex.Clear();
        SerpentBarkDictionary.Clear();
    }
}