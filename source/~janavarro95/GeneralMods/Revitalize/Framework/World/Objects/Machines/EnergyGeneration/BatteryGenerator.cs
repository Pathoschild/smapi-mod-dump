/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Illuminate;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    /// <summary>
    ///Object type that takes in a fuel type and converts it into battery packs.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration.BatteryGenerator")]
    public class BatteryGenerator : ItemRecipeDropInMachine
    {

        public readonly NetColor lightColor = new NetColor();
        public Color LightColor
        {
            get { return this.lightColor.Value; }
            set { this.lightColor.Value = value;}
        }

        public BatteryGenerator()
        {

        }

        public BatteryGenerator(BasicItemInformation Info, Color LightColor) : this(Info, Vector2.Zero, LightColor)
        {

        }

        public BatteryGenerator(BasicItemInformation Info, Vector2 TilePosition, Color LightColor) : base(Info, TilePosition)
        {
            this.LightColor = LightColor;
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddField(this.lightColor);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            base.minutesElapsed(minutes, environment);

            if (this.finishedProduction())
            {
                this.removeLight(Vector2.Zero);
            }
            this.updateAnimation();
            return true;
        }

        public override CraftingResult onSuccessfulRecipeFound(IList<Item> dropInItems, ProcessingRecipe craftingRecipe, Farmer who = null)
        {
            CraftingResult result= base.onSuccessfulRecipeFound(dropInItems, craftingRecipe, who);
            if (result.successful)
            {
                this.addLight(Vector2.Zero, Illuminate.LightManager.LightIdentifier.SconceLight, this.LightColor, 1f);
            }
            return result;
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.furnace);
        }

        public override Item getOne()
        {
            return new BatteryGenerator(this.basicItemInformation.Copy(),this.LightColor);
        }
    }
}
