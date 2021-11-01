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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.Managers
{
    internal class TextureManager
    {
        private IMonitor _monitor;
        private List<AlternativeTextureModel> _alternativeTextures;
        private HashSet<string> _textureIdsInsensitive;

        public TextureManager(IMonitor monitor)
        {
            _monitor = monitor;
            _alternativeTextures = new List<AlternativeTextureModel>();
            _textureIdsInsensitive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddAlternativeTexture(AlternativeTextureModel model)
        {
            if (_alternativeTextures.Any(t => t.GetId() == model.GetId()))
            {
                var replacementIndex = _alternativeTextures.IndexOf(_alternativeTextures.First(t => t.GetId() == model.GetId()));
                _alternativeTextures[replacementIndex] = model;
            }
            else
            {
                _alternativeTextures.Add(model);
                _textureIdsInsensitive.Add(model.GetId());
            }
        }

        public List<AlternativeTextureModel> GetAllTextures()
        {
            return _alternativeTextures;
        }

        public List<string> GetValidTextureNames()
        {
            return _alternativeTextures.Select(t => t.GetId()).ToList();
        }

        public List<string> GetValidTextureNamesWithSeason()
        {
            return _alternativeTextures.Select(t => t.GetTokenId()).ToList();
        }

        public bool DoesObjectHaveAlternativeTexture(int objectId)
        {
            return _alternativeTextures.Any(t => t.ItemId == objectId);
        }

        public bool DoesObjectHaveAlternativeTexture(string objectName)
        {
            return _alternativeTextures.Any(t => String.Equals(t.GetNameWithSeason(), objectName, StringComparison.OrdinalIgnoreCase));
        }

        public bool DoesObjectHaveAlternativeTextureById(string objectId)
        {
            return _textureIdsInsensitive.Contains(objectId);
        }

        public AlternativeTextureModel GetRandomTextureModel(int objectId)
        {
            if (!DoesObjectHaveAlternativeTexture(objectId))
            {
                return null;
            }

            var randomTexture = Game1.random.Next(_alternativeTextures.Select(t => t.ItemId == objectId).Count());
            return _alternativeTextures[randomTexture];
        }

        public AlternativeTextureModel GetRandomTextureModel(string objectName)
        {
            if (!DoesObjectHaveAlternativeTexture(objectName))
            {
                return null;
            }

            var validTextures = _alternativeTextures.Where(t => String.Equals(t.GetNameWithSeason(), objectName, StringComparison.OrdinalIgnoreCase)).ToList();
            return validTextures[Game1.random.Next(validTextures.Count())];
        }

        public AlternativeTextureModel GetSpecificTextureModel(string textureId)
        {
            if (!DoesObjectHaveAlternativeTextureById(textureId))
            {
                return null;
            }

            return _alternativeTextures.First(t => String.Equals(t.GetId(), textureId, StringComparison.OrdinalIgnoreCase));
        }

        public List<AlternativeTextureModel> GetAvailableTextureModels(string objectName, string season)
        {
            if (!DoesObjectHaveAlternativeTexture(objectName) && !DoesObjectHaveAlternativeTexture(String.Concat(objectName, "_", season)))
            {
                return new List<AlternativeTextureModel>();
            }

            var seasonalTextures = _alternativeTextures.Where(t => String.Equals(t.GetNameWithSeason(), String.Concat(objectName, "_", season), StringComparison.OrdinalIgnoreCase)).ToList();
            seasonalTextures.AddRange(_alternativeTextures.Where(t => !seasonalTextures.Any(s => s.GetId() == t.GetId()) && String.Equals(t.GetNameWithSeason(), objectName, StringComparison.OrdinalIgnoreCase)));
            return seasonalTextures;
        }

        public void UpdateTexture(string textureId, Texture2D texture)
        {
            if (!DoesObjectHaveAlternativeTextureById(textureId))
            {
                return;
            }

            var replacementIndex = _alternativeTextures.IndexOf(_alternativeTextures.First(t => String.Equals(t.GetId(), textureId, StringComparison.OrdinalIgnoreCase)));
            _alternativeTextures[replacementIndex].Textures[0] = texture;
        }
    }
}
