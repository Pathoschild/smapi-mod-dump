/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models;
using FashionSense.Framework.Models.Hair;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Managers
{
    class TextureManager
    {
        private IMonitor _monitor;
        private List<AppearanceContentPack> _appearanceTextures;

        public TextureManager(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _appearanceTextures = new List<AppearanceContentPack>();
        }

        public void Reset()
        {
            _appearanceTextures.Clear();
        }

        public void AddAppearanceModel(AppearanceContentPack model)
        {
            if (_appearanceTextures.Any(t => t.Id == model.Id && t.PackType == model.PackType))
            {
                var replacementIndex = _appearanceTextures.IndexOf(_appearanceTextures.First(t => t.Id == model.Id && t.PackType == model.PackType));
                _appearanceTextures[replacementIndex] = model;
            }
            else
            {
                _appearanceTextures.Add(model);
            }
        }

        public List<AppearanceContentPack> GetAllAppearanceModels()
        {
            return _appearanceTextures;
        }

        public List<T> GetAllAppearanceModels<T>() where T : AppearanceContentPack
        {
            return _appearanceTextures.Where(t => t is T) as List<T>;
        }

        public T GetSpecificAppearanceModel<T>(string appearanceId) where T : AppearanceContentPack
        {
            return (T)_appearanceTextures.FirstOrDefault(t => String.Equals(t.Id, appearanceId, StringComparison.OrdinalIgnoreCase) && t is T);
        }
    }
}
