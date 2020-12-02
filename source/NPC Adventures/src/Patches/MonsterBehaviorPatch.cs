/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using NpcAdventure.Compatibility;
using NpcAdventure.StateMachine.State;
using PurrplingCore.Patching;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;

namespace NpcAdventure.Patches
{
    /// <summary>
    /// Specialized loved monster behavior fixes. 
    /// Ordinary loved behaviors for monster you can found in <see cref="NpcAdventure.AI.Controller.LovePeaceController"/>
    /// </summary>
    internal class MonsterBehaviorPatch : Patch<MonsterBehaviorPatch>
    {
        private CompanionManager Manager { get; set; }

        public override string Name => nameof(MonsterBehaviorPatch);

        public MonsterBehaviorPatch(CompanionManager manager)
        {
            this.Manager = manager;
            Instance = this;
        }

		private static bool IsLovedMonster(Monster monster)
        {
			if (Instance.Manager.IsRecruitedAnyone() && Instance.Manager.GetRecruitedCompanion().GetCurrentStateBehavior() is RecruitedState rstate)
				return rstate.GetAI().IsLovedMonster(monster);

			return false;
		}

        private static bool Duggy_behaviorAtGameTick_Prefix(ref Duggy __instance, GameTime time)
        {
            try
            {
				if (IsLovedMonster(__instance))
				{
					__instance.Sprite.loop = false;
					__instance.IsInvisible = true;
					__instance.Sprite.CurrentFrame = 10;

					return false;
				}

				return true;
            }
            catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(Duggy_behaviorAtGameTick_Prefix));
                return true;
            }
        }

		private static void After_DustSpirit_behaviorAtGameTick(DustSpirit __instance, ref bool ___runningAwayFromFarmer, ref bool ___chargingFarmer)
		{
			if (IsLovedMonster(__instance))
			{
				___runningAwayFromFarmer = false;
				___chargingFarmer = false;
				__instance.controller = null;
			}
		}

		private static bool Before_Serpent_updateAnimation(Serpent __instance, GameTime time)
		{
			if (IsLovedMonster(__instance))
			{
				var ftn = typeof(Monster).GetMethod("updateAnimation", BindingFlags.NonPublic | BindingFlags.Instance).MethodHandle.GetFunctionPointer();
				var action = (Action<GameTime>)Activator.CreateInstance(typeof(Action<GameTime>), __instance, ftn);
				action(time);

				__instance.Sprite.Animate(time, 0, 9, 40f);

				typeof(Monster).GetMethod("resetAnimationSpeed", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((Monster)__instance, new object[] { });
				return false;
			}
			return true;
		}

		private static bool Before_Ghost_updateMovement(Monster __instance, GameTime time)
		{
			if (__instance is Ghost && IsLovedMonster(__instance))
			{
				__instance.defaultMovementBehavior(time);

				return false;
			}

			return true;
		}

		private static void Before_ShadowShaman_behaviorAtGameTick(SquidKid __instance, ref NetBool ___casting)
		{
			if (IsLovedMonster(__instance))
			{
				___casting.Value = false;
			}
		}

		private static void Before_SquidKid_behaviorAtGameTick(ref GameTime time, SquidKid __instance, ref float ___lastFireball)
		{
			if (IsLovedMonster(__instance))
			{
				___lastFireball = Math.Max(1f, ___lastFireball);
				time = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
			}
		}

		private static void After_DinoMonster_behaviorAtGameTick(DinoMonster __instance, ref int ___nextFireTime)
		{
			if (IsLovedMonster(__instance))
			{
				___nextFireTime = 0;
			}
		}

		private static bool Before_Skeleton_behaviorAtGameTick(Skeleton __instance, GameTime time)
		{
			if (IsLovedMonster(__instance))
			{
				var ftn = typeof(Monster).GetMethod("behaviorAtGameTick", BindingFlags.Public | BindingFlags.Instance).MethodHandle.GetFunctionPointer();
				var action = (Action<GameTime>)Activator.CreateInstance(typeof(Action<GameTime>), __instance, ftn);
				action(time);

				return false;
			}

			return true;
		}

		private static void After_Skeleton_behaviorAtGameTick(Skeleton __instance, ref NetBool ___throwing)
		{
			if (IsLovedMonster(__instance))
			{
				__instance.Sprite.StopAnimation();
				___throwing.Value = false;
			}
		}

		private static void After_drawCharacters(GameLocation __instance, SpriteBatch b)
		{
			if (__instance.shouldHideCharacters() || Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
			{
				return;
			}
			if (!Game1.eventUp)
			{
				for (int i = 0; i < __instance.characters.Count; i++)
				{
					if (__instance.characters[i] != null)
					{
						NPC npc = __instance.characters[i];
						if (npc is Monster && npc.IsEmoting)
						{
							Vector2 emotePosition = npc.getLocalPosition(Game1.viewport);
							emotePosition.Y -= (32 + npc.Sprite.SpriteHeight * 4);

							b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle(npc.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, npc.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)npc.getStandingY() / 10000f);
						}
					}
				}
			}
		}

		protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Duggy), nameof(Duggy.behaviorAtGameTick)),
                prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Duggy_behaviorAtGameTick_Prefix))
            );
			harmony.Patch(
			   original: AccessTools.Method(typeof(GameLocation), "drawCharacters"),
			   prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.After_drawCharacters))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(DinoMonster), nameof(DinoMonster.behaviorAtGameTick)),
			   postfix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.After_DinoMonster_behaviorAtGameTick))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(SquidKid), nameof(SquidKid.behaviorAtGameTick)),
			   prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Before_SquidKid_behaviorAtGameTick))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(ShadowShaman), nameof(ShadowShaman.behaviorAtGameTick)),
			   prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Before_ShadowShaman_behaviorAtGameTick))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(Skeleton), nameof(Skeleton.behaviorAtGameTick)),
			   prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Before_Skeleton_behaviorAtGameTick))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Ghost), nameof(Ghost.updateMovement)),
				prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Before_Ghost_updateMovement))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(Serpent), "updateAnimation"),
			   prefix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.Before_Serpent_updateAnimation))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(DustSpirit), nameof(DustSpirit.behaviorAtGameTick)),
			   postfix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.After_DustSpirit_behaviorAtGameTick))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(Skeleton), nameof(Skeleton.behaviorAtGameTick)),
			   postfix: new HarmonyMethod(typeof(MonsterBehaviorPatch), nameof(MonsterBehaviorPatch.After_Skeleton_behaviorAtGameTick))
			);
		}
    }
}
