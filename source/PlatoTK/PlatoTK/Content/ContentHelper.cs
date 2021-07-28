/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using PlatoTK.Reflection;

namespace PlatoTK.Content
{
    internal class ContentHelper : InnerHelper, IContentHelper
    {
        public IInjectionHelper Injections { get; }

        public ITextureHelper Textures { get; }

        public IMapHelper Maps { get; }

        public ContentHelper(IPlatoHelper helper)
            : base(helper)
        {
            Textures = new TextureHelper(helper);
            Injections = new InjectionHelper(helper);
            Maps = new MapHelper(helper);
        }

        public ISaveIndex GetSaveIndex(string id, Func<IDictionary<int, string>> loadData, Func<ISaveIndexHandle, bool> validateValue, Action<ISaveIndexHandle> injectValue, int minIndex = 13000)
        {
            return new SaveIndex(id, loadData, validateValue, injectValue, Plato, minIndex);
        }

        public ISaveIndex GetSaveIndex(string id, string dataSource, Func<ISaveIndexHandle, bool> validateValue, Action<ISaveIndexHandle> injectValue, int minIndex = 13000)
        {
            return new SaveIndex(id, dataSource, validateValue, injectValue, Plato, minIndex);
        }

        public ISaveIndex GetSaveIndex(string id,IPlatoHelper helper,int minIndex = 13000)
        {
            return new SaveIndex(id, id, helper, minIndex);
        }

       /* public T Load<T>(string key, ContentSource source = ContentSource.ModFolder)
        {
            string[] extensions = new string[] { ".xml",".fnt" };
            Type[] types = new Type[] { typeof(SpriteFont) };
            object manager = AccessTools.Field(Plato.ModHelper.Content.GetType(), source == ContentSource.GameContent ? "GameContentManager" : "ModContentManager")?.GetValue(Plato.ModHelper.Content);
            string assetName = Plato.ModHelper.Content.NormalizeAssetName(key);
            if (!types.Contains(typeof(T)) && extensions.Contains()

            return content.Load<T>(key, source);
        }*/
    }
}
