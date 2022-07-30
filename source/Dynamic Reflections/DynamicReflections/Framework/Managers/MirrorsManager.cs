/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Models;
using DynamicReflections.Framework.Models.Settings;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Managers
{
    internal class MirrorsManager
    {
        private Dictionary<string, MirrorSettings> _furnitureIdToMirrors;
        private Dictionary<string, Texture2D> _furnitureIdToMask;

        public MirrorsManager()
        {
            Reset();
        }

        public void Add(List<ContentPackModel> models)
        {
            foreach (var model in models)
            {
                if (model is null || String.IsNullOrEmpty(model.FurnitureId) || model.Mask is null)
                {
                    continue;
                }

                _furnitureIdToMirrors[model.FurnitureId.ToLower()] = model.Mirror;
                _furnitureIdToMask[model.FurnitureId.ToLower()] = model.Mask;
            }
        }

        public MirrorSettings? GetSettings(string furnitureId)
        {
            if (_furnitureIdToMirrors.ContainsKey(furnitureId.ToLower()) is false)
            {
                return null;
            }

            return _furnitureIdToMirrors[furnitureId.ToLower()];
        }

        public Texture2D? GetMask(string furnitureId)
        {
            if (_furnitureIdToMask.ContainsKey(furnitureId.ToLower()) is false)
            {
                return null;
            }

            return _furnitureIdToMask[furnitureId.ToLower()];
        }

        public void Reset()
        {
            _furnitureIdToMirrors = new Dictionary<string, MirrorSettings>();
            _furnitureIdToMask = new Dictionary<string, Texture2D>();
        }
    }
}
