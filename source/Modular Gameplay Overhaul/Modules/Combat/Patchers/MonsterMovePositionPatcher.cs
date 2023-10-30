/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal class MonsterMovePositionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterMovePositionPatcher"/> class.</summary>
    internal MonsterMovePositionPatcher()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.MovePosition));
    }

    #region harmony patches

    /// <summary>Add knockback damage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterMovePositionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: CollisionDetectedSubroutine(this);
        // After: found_collision = true;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MonsterMovePositionPatcher).RequireMethod(nameof(CollisionDetectedSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying debug transpiler.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CollisionDetectedSubroutine(Monster monster)
    {
        if (monster.IsGlider() || monster.Health <= 0 || !monster.Get_KnockedBack())
        {
            return;
        }

        var velocity = new Vector2(monster.xVelocity, monster.yVelocity);
        var speed = velocity.Length();
        var damage = (int)Math.Pow(speed / 10, 2);
        if (damage <= 0)
        {
            return;
        }

        monster.Health = Math.Max(monster.Health - damage, 0);
        var knockbacker = monster.Get_KnockBacker()!;
        if (monster.Health <= 0)
        {
            monster.Die(knockbacker);
        }
        else
        {
            monster.shedChunks(Game1.random.Next(1, 3));
        }

        var monsterBox = monster.GetBoundingBox();
        monster.currentLocation.debris.Add(new Debris(
            damage,
            new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
            Color.White,
            1f,
            monster));
        monster.Set_KnockedBack(null);
    }

    #endregion injected subroutines
}
