/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetGalaxySwordPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetGalaxySwordPatcher"/> class.</summary>
    internal GameLocationGetGalaxySwordPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("getGalaxySword");
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static bool GameLocationGetGalaxySwordPrefix()
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            var obtained = player.Read(DataFields.GalaxyArsenalObtained).ParseList<int>();
            int? chosen = null;
            foreach (var item in player.Items.Where(i => (i is MeleeWeapon weapon && !weapon.isScythe()) || i is Slingshot))
            {
                var type = item is MeleeWeapon weapon ? (WeaponType)weapon.type.Value : WeaponType.Slingshot;
                var galaxy = galaxyFromWeaponType(type);
                if (obtained.Contains(galaxy))
                {
                    continue;
                }

                chosen = galaxy;
                break;
            }

            chosen ??= new[]
            {
                Constants.GalaxySwordIndex, Constants.GalaxyHammerIndex, Constants.GalaxyDaggerIndex,
                Constants.GalaxySlingshotIndex,
            }.Except(obtained).First();

            Item chosenAsItem = chosen.Value == Constants.GalaxySlingshotIndex
                ? new Slingshot(chosen.Value)
                : new MeleeWeapon(chosen.Value);

            Game1.flashAlpha = 1f;
            player.holdUpItemThenMessage(chosenAsItem);
            player.reduceActiveItemByOne();
            for (var i = 0; i < obtained.Count; i++)
            {
                player.reduceActiveItemByOne();
            }

            player.Items.First(i => i?.ParentSheetIndex == SObject.iridiumBar).Stack -=
                ArsenalModule.Config.IridiumBarsRequiredForGalaxyArsenal;

            if (!player.addItemToInventoryBool(chosenAsItem))
            {
                Game1.createItemDebris(chosenAsItem, Game1.player.getStandingPosition(), 1);
            }

            player.Append(DataFields.GalaxyArsenalObtained, chosen.Value.ToString());

            //player.mailReceived.Add("galaxySword"); --> don't add mail to prevent galaxy weapons from appearing in stores
            player.jitterStrength = 0f;
            Game1.screenGlowHold = false;
            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .globalChatInfoMessage("GalaxySword", Game1.player.Name);
            return false; // don't run original logic

            int galaxyFromWeaponType(WeaponType type)
            {
                return type switch
                {
                    WeaponType.StabbingSword or WeaponType.DefenseSword => Constants.GalaxySwordIndex,
                    WeaponType.Dagger => Constants.GalaxyDaggerIndex,
                    WeaponType.Club => Constants.GalaxyHammerIndex,
                    WeaponType.Slingshot => Constants.GalaxySlingshotIndex,
                    _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<WeaponType, int>(type),
                };
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
