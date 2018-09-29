using System;
using System.Reflection.Emit;

namespace StardewHack.GrassGrowth
{
    public class ModEntry : Hack<ModEntry>
    {
        // Change the milk pail such that it doesn't do anything while no animal is in range. 
        [BytecodePatch("StardewValley.GameLocation::growWeedGrass")]
        void GameLocation_growWeedGrass() {
            var growWeedGrass = BeginCode();
            // For each ofthe 4 directions
            for (int i=0; i<4; i++) {
                growWeedGrass = growWeedGrass.FindNext(
                    OpCodes.Ldarg_0,
                    null,
                    null,
                    null,
                    null,
                    Instructions.Ldstr("Diggable"),
                    Instructions.Ldstr("Back"),
                    Instructions.Call(typeof(StardewValley.GameLocation), "doesTileHaveProperty", typeof(int), typeof(int), typeof(string), typeof(string)),
                    OpCodes.Brfalse
                );
                growWeedGrass.Remove();
            }
        }
    }
}

