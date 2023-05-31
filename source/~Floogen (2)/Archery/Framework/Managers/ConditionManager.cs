/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Display;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Managers
{
    internal class ConditionManager
    {
        private const int MAX_TRACKED_MILLISECONDS = 3600000;

        private HashSet<ItemSpriteModel> _models;

        public ConditionManager(IModHelper helper)
        {
            _models = new HashSet<ItemSpriteModel>();
        }

        public void Track(ItemSpriteModel model)
        {
            _models.Add(model);
        }

        public void Update(ItemSpriteModel model)
        {
            if (_models.Contains(model) is false)
            {
                Track(model);
            }

            model.MillisecondsElapsed += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

            if (model.MillisecondsElapsed > MAX_TRACKED_MILLISECONDS)
            {
                model.MillisecondsElapsed = 0;
            }
        }

        public void Update()
        {
            foreach (var model in _models)
            {
                Update(model);
            }
        }

        public void Reset<T>(List<T> models) where T : ItemSpriteModel
        {
            foreach (var model in models.Where(m => m is not null))
            {
                if (_models.Contains(model) is false)
                {
                    Track(model);
                }

                model.MillisecondsElapsed = 0;
            }
        }
    }
}
