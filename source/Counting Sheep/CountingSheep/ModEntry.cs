/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/CountingSheep
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;

namespace CountingSheep;

public sealed class ModData
{
    public int LastBedtime { get; set; } = 2200;
    public int AlarmClock { get; set; } = 600;
}

internal sealed class ModEntry : Mod
{
    private static ModData _saveData = new();
    private static MethodInfo? _sleepFunc;

    private static int CalculateTimeSlept(int bedTime, int awakeTime)
    {
        var diff = 2400 - bedTime / 100 * 100;
        return awakeTime + diff;
    }

    private static int GetNaturalAwakeTime()
    {
        return Game1.timeOfDay - 1600;
    }

    private static void AdvanceGameTime(int time)
    {
        while (Game1.timeOfDay < time) Game1.performTenMinuteClockUpdate();
    }

    public static void SetAlarm()
    {
        if (!Game1.IsMasterGame)
        {
            _sleepFunc?.Invoke(new GameLocation(), null);
            return;
        }
        if (!Game1.IsMultiplayer && Game1.timeOfDay < 2000)
        {
            Game1.activeClickableMenu = new DialogueBox("It's too early to go to bed!");
            return;
        }
        Game1.activeClickableMenu = new NumberSelectionMenu(
            message: "When would you like to wake up?",
            behaviorOnSelection: (value, price, who) =>
            {
                _saveData.AlarmClock = value * 100;
                Game1.exitActiveMenu();
                _sleepFunc?.Invoke(new GameLocation(), null);
            },
            minValue: 5,
            maxValue: 10,
            defaultNumber: Math.Max(5, GetNaturalAwakeTime() / 100)
        );
    }

    private static IEnumerable<CodeInstruction> AnswerDialogueActionTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        
        for (var i = 0; i < codes.Count; i++)
        {
            if (!(codes[i].opcode == OpCodes.Call && (codes[i].operand as MethodInfo).Name.Contains("startSleep"))) continue;
            _sleepFunc = codes[i].operand as MethodInfo;
            codes.Insert(i++, CodeInstruction.Call(typeof(ModEntry), nameof(SetAlarm)));
            codes.Insert(i++, new CodeInstruction(OpCodes.Ret));
        }

        return codes.AsEnumerable();
    }

    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
            transpiler: new HarmonyMethod(typeof(ModEntry), nameof(AnswerDialogueActionTranspiler))
        );

        helper.Events.GameLoop.DayEnding += (sender, e) =>
        {
            if (!Game1.IsMasterGame) return;
            _saveData.LastBedtime = Game1.timeOfDay;
            helper.Data.WriteSaveData("CountingSheep", _saveData);
        };

        helper.Events.GameLoop.DayStarted += (sender, e) =>
        {
            if (!Game1.IsMasterGame) return;
            _saveData = helper.Data.ReadSaveData<ModData>("CountingSheep") ?? _saveData;
            var hoursSlept = CalculateTimeSlept(_saveData.LastBedtime, _saveData.AlarmClock);
            if (!Game1.IsMultiplayer && _saveData.LastBedtime == 2600)
            {
                Monitor.Log("You're really tired", LogLevel.Info);
                AdvanceGameTime(1100);
                Game1.player.stamina *= 0.5f;
                return;
            }

            if (_saveData.AlarmClock == 500) Game1.timeOfDay = 500;
            else AdvanceGameTime(_saveData.AlarmClock);

            if (Game1.IsMultiplayer) return;
            switch (hoursSlept)
            {
                case < 600:
                    Monitor.Log("You're really tired", LogLevel.Info);
                    Game1.player.stamina *= 0.5f;
                    break;
                case < 800:
                    Monitor.Log("You're tired", LogLevel.Info);
                    Game1.player.stamina *= 0.75f;
                    break;
            }
        };
    }
}