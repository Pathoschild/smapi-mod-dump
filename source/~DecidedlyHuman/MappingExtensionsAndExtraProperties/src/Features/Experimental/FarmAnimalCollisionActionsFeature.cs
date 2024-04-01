/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace MappingExtensionsAndExtraProperties.Features.Experimental;

public class FarmAnimalCollisionActionsFeature
{
    // public void Test()
    // {
    //     Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
    //
    //     GameLocation location = Game1.currentLocation;
    //     this.cow = new FarmAnimal("White Cow", multiplayer.getNewID(), -1L);
    //
    //     harmony.Patch(
    //         AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.farmerPushing)),
    //         postfix: new HarmonyMethod(typeof(ModEntry),
    //             nameof(ModEntry.AnimalFarmerCollision)));
    //
    //     location.Animals.Add(this.cow.myID.Value, this.cow);
    // }

    // public static void AnimalFarmerCollision()
    // {
    //     Game1.player.jump(6f);
    //     Game1.player.LerpPosition(Game1.player.Position, Game1.player.Position + new Vector2(64, 64), 0.1f);
    // }

    // helper.Events.GameLoop.UpdateTicked += (sender, args) =>
    // {
    //     if (this.cow == null)
    //         return;
    //
    //     this.cow.controller = new PathFindController(this.cow, this.cow.currentLocation, Game1.player.TilePoint, 0);
    // };
}
