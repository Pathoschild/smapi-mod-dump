/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using System.Collections.Generic;


namespace BitwiseJonMods
{
#nullable disable

    //Do not show SMAPI build warnings for this file because it was copied from the actual Stardew Valley decompiled Carpenter Menu code and then modified.
#pragma warning disable AvoidImplicitNetFieldCast, AvoidNetField

    //jon, 3/21/24: CarpenterMenu is now completely different in v6. Inherit from base class instead of rewriting.
    public class InstantBuildMenu : CarpenterMenu
    {
        private ModConfig _config;

        //Builder must be set to Robin or Wizard to avoid divide by zero error.
        public InstantBuildMenu(ModConfig config) : base("Robin")
        {
            _config = config;

            //jon, 3/21/24: Add all blueprints and make them buildable instantly at farm.
            int num = 0;
            this.Blueprints.Clear();
            foreach (KeyValuePair<string, BuildingData> keyValuePair in (IEnumerable<KeyValuePair<string, BuildingData>>)Game1.buildingData)
            {
                if (this.TargetLocation.Name == "Farm")
                {
                    this.Blueprints.Add(GetNewModifiedBlueprint(num++, keyValuePair, (string)null));
                    if (keyValuePair.Value.Skins != null)
                    {
                        foreach (BuildingSkin skin in keyValuePair.Value.Skins)
                        {
                            if (skin.ShowAsSeparateConstructionEntry)
                                this.Blueprints.Add(GetNewModifiedBlueprint(num++, keyValuePair, skin.Id));
                        }
                    }
                }
            }
        }

        //jon, 1/30/24: This function is new to update all blueprints to be completed instantly and to be free or not according to config.
        private BlueprintEntry GetNewModifiedBlueprint(int num, KeyValuePair<string, BuildingData> keyValuePair, string skinId = null)
        {
            //Set the cost of all buildings to zero with no items required
            var bd = keyValuePair.Value;
            bd.MagicalConstruction = true;
            bd.BuildDays = 0;

            if (!_config.BuildUsesResources)
            {
                bd.BuildMaterials = new List<BuildingMaterial>();
                bd.BuildCost = 0;
            }

            var bp = new BlueprintEntry(num, keyValuePair.Key, bd, skinId);
            return bp;
        }

    }
}
