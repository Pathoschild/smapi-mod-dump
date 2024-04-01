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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace GingerIslandStart.Patches;

public class MonsterPatches
{
    private static readonly string[] AllowedMonsters = { "Tiger Slime", "Lava Lurk", "Hot Head", "Magma Sprite", "Magma Duggy", "Magma Sparker", "False Magma Cap", "Dwarvish Sentry"};
    private static ModConfig Config => ModEntry.Config;
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static void Apply(Harmony harmony)
    {
        /*
        Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV constructor \"Monster()\".");
        harmony.Patch(
            original: AccessTools.Constructor(typeof(Monster), null),
            postfix: new HarmonyMethod(typeof(MonsterPatches), nameof(Post_new))
        );
        
        Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV constructor \"Monster(string,Vector2, int)\".");
        harmony.Patch(
            original: AccessTools.Constructor(typeof(Monster), new[]{typeof(string),typeof(Vector2),typeof(int)}),
            postfix: new HarmonyMethod(typeof(MonsterPatches), nameof(Post_newWithParameters))
        );*/
        
        Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV method \"Monster(string,Vector2, int)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Monster), "parseMonsterInfo"),
            postfix: new HarmonyMethod(typeof(MonsterPatches), nameof(Post_parseMonsterInfo))
            );
    }

    internal static void Post_parseMonsterInfo(Monster __instance, string name)
    {
        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;
        
        if(Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
            return;

        if (__instance.currentLocation is null)
        {
            if (AllowedMonsters.Contains(name))
                MakeChanges(__instance);
        }
        else if (__instance.currentLocation.GetLocationContext() == LocationContexts.Island)
        {
            Log($"instance's location name: {__instance.currentLocation.Name}");
            
            if(__instance.currentLocation.Name.Contains("Volcano"))
                MakeChanges(__instance);
        }
    }

    private static void MakeChanges(Monster monster)
    {
        if (monster.DamageToFarmer == 0)
            return;

        if (Config.MonsterDifficulty == "hard")
            return;

        var trueDmg = monster.DamageToFarmer / 2;
        monster.DamageToFarmer = trueDmg;
        
        
        if (Config.MonsterDifficulty != "easy")
            return;
        
        var trueHealth = monster.MaxHealth / 2;
        monster.MaxHealth = trueHealth;
        monster.Health = trueHealth;
    }

    /*
    internal static void Post_newWithParameters(Monster __instance, string name, Vector2 position, int facingDir)
    {
        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;
        
        Post_new(__instance);
    }
    
    /// <summary>
    /// Makes adjustments to volcano monsters depending on config.
    /// </summary>
    /// <param name="__instance"></param>
    internal static void Post_new(Monster __instance)
    {
        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;

        //if not an island monster
        if (__instance.currentLocation.GetLocationContext() != LocationContexts.Island)
            return;
        
        if (__instance.DamageToFarmer == 0)
            return;

        if (Config.MonsterDifficulty == "hard")
            return;

        var trueDmg = __instance.DamageToFarmer / 2; //GetChanged(__instance);
        //if (__instance.DamageToFarmer == trueDmg)
        //    return;
        __instance.DamageToFarmer = trueDmg;
        
        
        if (Config.MonsterDifficulty != "easy")
            return;
        
        var trueHealth = __instance.MaxHealth / 2;
        __instance.MaxHealth = trueHealth;
        __instance.Health = trueHealth;
    }

    private static int GetChanged(Monster who)
    {
        var split = DataLoader.Monsters(Game1.content)[who.Name].Split('/');
        var damageToFarmer = Convert.ToInt32(split[1]);

        return Config.MonsterDifficulty switch
        {
            "easy" => damageToFarmer / 2,
            "hard" => damageToFarmer * 2,
            _ => damageToFarmer
        };
    }*/
}