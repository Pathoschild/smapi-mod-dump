/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace StardewRoguelike.Patches
{
    internal class PermanentFoodBuffPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(BuffsDisplay), "tryToAddFoodBuff");

        public static bool Prefix(ref Buff b)
        {
            if (b is not Curse && Curse.HasCurse(CurseType.BuffsMorePotent))
            {
                b.millisecondsDuration = Curse.PotentBuffDuration;
                for (int i = 0; i < b.buffAttributes.Length; i++)
                {
                    if (b.buffAttributes[i] > 0)
                        b.buffAttributes[i] += 1;
                }
            }
            else
                b.millisecondsDuration = int.MaxValue;

            return true;
        }

    }

    internal class PermanentDrinkBuffPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(BuffsDisplay), "tryToAddDrinkBuff");

        public static bool Prefix(BuffsDisplay __instance, ref bool __result, ref Buff b)
        {
            int buffDuration;
            if (Curse.HasCurse(CurseType.BuffsMorePotent))
            {
                buffDuration = Curse.PotentBuffDuration;
                for (int i = 0; i < b.buffAttributes.Length; i++)
                {
                    if (b.buffAttributes[i] > 0)
                        b.buffAttributes[i] += 1;
                }
            }
            else
                buffDuration = int.MaxValue;

            b.millisecondsDuration = buffDuration;
            Buff newb;
            if (b.source.Contains("Beer") || b.source.Contains("Wine") || b.source.Contains("Mead") || b.source.Contains("Pale Ale"))
            {
                newb = new(17);
                newb.millisecondsDuration = buffDuration;
                __instance.addOtherBuff(newb);
            }
            else if (b.source.Equals("Life Elixir"))
            {
                if (Curse.HasCurse(CurseType.HealOverTime))
                    Curse.HOTHealToTick += (int)(Game1.player.maxHealth * 1.5);
                else
                    Game1.player.health = Game1.player.maxHealth;
            }
            if (b.total > 0 && __instance.quenchedLeft <= 0)
            {
                if (__instance.drink is not null)
                {
                    __instance.drink.removeBuff();
                }
                __instance.drink = b;
                __instance.drink.addBuff();
                __instance.syncIcons();
                __result = true;
            }
            else
            {
                __result = false;
            }
            return false;
        }

    }
}
