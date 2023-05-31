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
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.Misc
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.Misc.AdvancedCharcoalKiln")]
    public class AdvancedCharcoalKiln : ItemRecipeDropInMachine
    {
        public AdvancedCharcoalKiln()
        {

        }

        public AdvancedCharcoalKiln(BasicItemInformation Info) : this(Info, Vector2.Zero)
        {

        }

        public AdvancedCharcoalKiln(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {
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
            if(result.successful)
            {
                MultiplayerUtilities.GetMultiplayer().broadcastSprites(this.getCurrentLocation(), new TemporaryAnimatedSprite(27, this.TileLocation * 64f + new Vector2(-16f, -128f), Color.White, 4, flipped: false, 50f, 10, 64, (this.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f)
                {
                    alphaFade = 0.005f
                });
                this.addLight(Vector2.Zero, Illuminate.LightManager.LightIdentifier.SconceLight, Color.DarkCyan.Invert(), 1f);
            }
            return result;
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.openBox);
            SoundUtilities.PlaySoundWithDelay(Enums.StardewSound.fireball, 50);
        }

        public override Item getOne()
        {
            return new AdvancedCharcoalKiln(this.basicItemInformation.Copy());
        }
    }
}
