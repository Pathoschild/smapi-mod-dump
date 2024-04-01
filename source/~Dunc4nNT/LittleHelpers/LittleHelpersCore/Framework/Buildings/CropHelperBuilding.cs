/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.LittleHelpersCore.Framework.Commands;
using NeverToxic.StardewMods.LittleHelpersCore.Framework.Validators;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.LittleHelpersCore.Framework.Buildings
{
    internal class CropHelperBuilding(int helperCapacity, ILocationValidator locationValidator, int radius, int? location = null, bool canScareCrows = false, bool canReplantCrops = false) : BaseBuilding(helperCapacity, locationValidator, radius: radius, location: location)
    {
        public override List<BaseCommand> Commands { get; set; } = [new HarvestCropCommand(), new CollectFromTreeCommand()];

        public bool CanScareCrows { get; } = canScareCrows;

        public bool CanReplantCrops { get; } = canReplantCrops;

        public override void ExecuteCommands()
        {
            if (this.Commands is null)
                return;

            foreach (int tile in this.Tiles)
                foreach (BaseCommand command in this.Commands)
                {
                    command.Handle(tile);

                    if (command is HarvestCropCommand && this.CanReplantCrops)
                    {
                        int seed = 0;
                        this.ReplantCrop(tile, seed);
                    }
                }
        }

        private void ReplantCrop(int tile, int seed)
        {
        }
    }
}
