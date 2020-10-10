/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/iSkLz/ExperienceMultiplier
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StardewValley.Tools;
using StardewValley.Locations;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ExperienceMultiplier
{
    public class MainConfig
    {
        public int GeneralMultiplier { get; set; } = 1;
        public int CombatMultiplier { get; set; } = 1;
        public int FarmingMultiplier { get; set; } = 1;
        public int FishingMultiplier { get; set; } = 1;
        public int MiningMultiplier { get; set; } = 1;
        public int ForagingMultiplier { get; set; } = 1;
        public int LuckMultiplier { get; set; } = 1;
    }

    public class Main : Mod
    {
        public MainConfig Config;

        /*public IDetour MonsterExperienceHook;
        public IDetour FishExperienceHook;
        public IDetour MineExperienceHook;*/

        ~Main()
        {
            /*MonsterExperienceHook?.Dispose();
            FishExperienceHook?.Dispose();
            MineExperienceHook?.Dispose();*/
            On.StardewValley.Farmer.gainExperience -= onGainExperience;
            //IL.StardewValley.Crop.harvest -= HarvestCILPatcher;
        }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<MainConfig>();

            helper.Events.GameLoop.SaveLoaded += delegate (object sender, SaveLoadedEventArgs args)
            {
                Config = helper.ReadConfig<MainConfig>();
            };

            //Monitor.Log("Applying hooks");
            /*MethodInfo MonsterExperienceGet = typeof(Monster).GetMethod("get_ExperienceGained",
                BindingFlags.Public | BindingFlags.Instance);
            MethodInfo MonsterExperienceEvent = typeof(Main).GetMethod("onMonsterExperience",
                BindingFlags.Public | BindingFlags.Instance);
            MonsterExperienceHook = new Hook(MonsterExperienceGet, MonsterExperienceEvent, this);
            Monitor.Log("Applied monster experience hook");*/

            On.StardewValley.Farmer.gainExperience += onGainExperience;
            Monitor.Log("Applied experience hook");

            /*IL.StardewValley.Crop.harvest += HarvestCILPatcher;
            IL.StardewValley.Locations.MineShaft.checkStoneForItems += MineCILPatcher;

            FishExperienceHook = new ILHook(typeof(FishingRod).GetMethod("doPullFishFromWater",
                BindingFlags.NonPublic | BindingFlags.Instance), FishCILPatcher);*/
        }

        /*public void MineCILPatcher(ILContext il)
        {
            ILCursor Cursor = new ILCursor(il);

            Cursor.GotoNext(MoveType.Before, x => x.MatchCallvirt(typeof(Farmer).GetMethod(
                "gainExperience", BindingFlags.Public | BindingFlags.Instance)));

            Cursor.Emit(OpCodes.Ldc_I4, Config.MineMultiplier);
            Cursor.Emit(OpCodes.Mul);

            Monitor.Log("Applied mine experience IL code (phase 1) at byte offset " +
                Cursor.Next.Offset.ToString());


            sbyte num = 40;
            Cursor.GotoNext(MoveType.After, x => x.MatchLdcI4(40),
                x => x.MatchLdarg(0),
                x => x.Match(OpCodes.Ldc_I4_M1),
                x => x.MatchCall(typeof(MineShaft).GetMethod(
                "getMineArea", BindingFlags.Public | BindingFlags.Instance)),
                x => x.MatchMul());

            Cursor.Emit(OpCodes.Ldc_I4, Config.MineMultiplier);
            Cursor.Emit(OpCodes.Mul);

            Monitor.Log("Applied mine experience IL code (phase 2) at byte offset " +
                Cursor.Next.Offset.ToString());


            Cursor.GotoNext(MoveType.After, x => x.Match(OpCodes.Ldc_I4_3), x => x.Match(OpCodes.Ldc_I4_5));

            Cursor.Emit(OpCodes.Ldc_I4, Config.MineMultiplier);
            Cursor.Emit(OpCodes.Mul);

            Monitor.Log("Applied mine experience IL code (phase 3) at byte offset " +
                Cursor.Next.Offset.ToString());
        }

        public void FishCILPatcher(ILContext il)
        {
            ILCursor Cursor = new ILCursor(il);

            Cursor.GotoNext(MoveType.Before, x => x.MatchCallvirt(typeof(Farmer).GetMethod(
                "gainExperience", BindingFlags.Public | BindingFlags.Instance)));

            Cursor.Emit(OpCodes.Ldc_I4, Config.MineMultiplier);
            Cursor.Emit(OpCodes.Mul);

            Monitor.Log("Applied fish experience IL code at byte offset " +
                Cursor.Next.Offset.ToString());
        }

        public void HarvestCILPatcher(ILContext il)
        {
            ILCursor Cursor = new ILCursor(il);

            Cursor.GotoNext(MoveType.After, x => x.MatchLdcR8(2.7182818284590451d));
            Cursor.Index += 2;

            double num = Config.HarvestMultiplier;
            Cursor.Emit(OpCodes.Ldc_R8, num);
            Cursor.Emit(OpCodes.Mul);

            Monitor.Log("Applied crop harvest experience IL code at byte offset " +
                Cursor.Next.Offset.ToString());
        }*/

        public void onGainExperience(On.StardewValley.Farmer.orig_gainExperience orig,
            Farmer self, int which, int amount)
        {
            int xp = amount * Config.GeneralMultiplier;

            switch (which)
            {
                case 0:
                    orig(self, which, xp * Config.FarmingMultiplier);
                    break;
                case 1:
                    orig(self, which, xp * Config.FishingMultiplier);
                    break;
                case 2:
                    orig(self, which, xp * Config.ForagingMultiplier);
                    break;
                case 3:
                    orig(self, which, xp * Config.MiningMultiplier);
                    break;
                case 4:
                    orig(self, which, xp * Config.CombatMultiplier);
                    break;
                case 5:
                    orig(self, which, xp * Config.LuckMultiplier);
                    break;
                default:
                    orig(self, which, xp);
                    break;
            }
        }

        /*public delegate int orig_onMonsterExperience(Monster self);

        public int onMonsterExperience(orig_onMonsterExperience orig, Monster self)
        {
            int origExp = orig(self);
            int resExp = origExp * Config.MonsterMultiplier;
            return resExp;
        }*/
    }
}
