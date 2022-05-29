/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.External.GenericModConfigMenu
{
    public class ModConfig
    {
        public bool OutputTextureDataToLog { get; set; }
        public bool UseRandomTexturesWhenPlacingFlooring { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingFruitTree { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingTree { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingHoeDirt { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingGrass { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingFurniture { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingObject { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingFarmAnimal { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingMonster { get; set; } = true;
        public bool UseRandomTexturesWhenPlacingBuilding { get; set; } = true;
        public List<DisabledTextureModel> DisabledTextures { get; set; } = new List<DisabledTextureModel>();

        internal bool IsTextureVariationDisabled(string textureId, int variation)
        {
            if (DisabledTextures.Any(t => t.TextureId.Equals(textureId, StringComparison.OrdinalIgnoreCase) && t.DisabledVariations.Contains(variation)))
            {
                return true;
            }

            return false;
        }

        internal void SetTextureStatus(string textureId, int variation, bool isEnabled)
        {
            if (isEnabled)
            {
                if (!DisabledTextures.Any(t => t.TextureId.Equals(textureId, StringComparison.OrdinalIgnoreCase) && t.DisabledVariations.Contains(variation)))
                {
                    return;
                }

                DisabledTextures
                    .First(t => t.TextureId.Equals(textureId, StringComparison.OrdinalIgnoreCase) && t.DisabledVariations.Contains(variation)).DisabledVariations
                    .Remove(variation);
            }
            else
            {
                var model = DisabledTextures.FirstOrDefault(t => t.TextureId.Equals(textureId, StringComparison.OrdinalIgnoreCase));
                if (model is null)
                {
                    model = new DisabledTextureModel() { TextureId = textureId };
                    DisabledTextures.Add(model);
                }

                if (!model.DisabledVariations.Contains(variation))
                {
                    model.DisabledVariations.Add(variation);
                }
            }

            return;
        }
    }
}
