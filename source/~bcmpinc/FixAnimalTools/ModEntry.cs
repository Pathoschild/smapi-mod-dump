using System;
using System.Reflection.Emit;
using StardewValley;
using StardewValley.Tools;

namespace StardewHack.FixAnimalTools
{
    public class ModEntry : Hack<ModEntry>
    {
        // Change the milk pail such that it doesn't do anything while no animal is in range. 
        [BytecodePatch("StardewValley.Tools.MilkPail::beginUsing")]
        void MilkPail_beginUsing() {
            // Find the first animal != null check.
            var hasAnimal = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(MilkPail), "animal"),
                OpCodes.Brfalse
            );
            hasAnimal.Replace(
                // if (this.animal == null) {
                hasAnimal[0],
                hasAnimal[1],
                Instructions.Brtrue(AttachLabel(hasAnimal[3])),
                //    who.forceCanMove();
                Instructions.Ldarg_S(4),
                Instructions.Callvirt(typeof(Farmer), nameof(Farmer.forceCanMove)),
                //    return false;
                Instructions.Ldc_I4_0(),
                Instructions.Ret()
                // }
            );
        }

        // Change the shears such that it doesn't do anything while no animal is in range. 
        [BytecodePatch("StardewValley.Tools.Shears::beginUsing")]
        void Shears_beginUsing() {
            var halt = FindCode(
                OpCodes.Ldarg_S,
                Instructions.Callvirt(typeof(Character), nameof(Character.Halt))
            );
            halt.Replace(
                halt[0],
                // if (this.animal == null) {
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(Shears), "animal"),
                Instructions.Brtrue(AttachLabel(halt[1])),
                //    who.forceCanMove();
                Instructions.Callvirt(typeof(Farmer), nameof(Farmer.forceCanMove)),
                //    return false;
                Instructions.Ldc_I4_0(),
                Instructions.Ret(),
                // }
                halt[1]
            );
        }
    }
}

