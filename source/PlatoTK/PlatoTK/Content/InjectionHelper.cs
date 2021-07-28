/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using xTile;

namespace PlatoTK.Content
{
    internal class InjectionHelper : InnerHelper, IInjectionHelper
    {
        private static readonly HashSet<AssetInjector> Injected = new HashSet<AssetInjector>();

        private AssetInjector Injector => Injected.FirstOrDefault(i => i.Mod == Plato.ModHelper.ModRegistry.ModID);

        public InjectionHelper(IPlatoHelper helper)
            : base(helper)
        {
            if (!Injected.Any(i => i.Mod == helper.ModHelper.ModRegistry.ModID))
                Injected.Add(new AssetInjector(helper));
        }

        public void InjectLoad<TAsset>(string assetName, TAsset asset, string conditions = "")
        {
            Injector.AddInjection(new ObjectInjection<TAsset>(Plato, assetName, asset, InjectionMethod.Load, conditions));
        }
        
        public void InjectDataInsert(string assetName, int key, string value, string conditions = "")
        {
            Injector.AddInjection(new DataInjection<int>(Plato,assetName,key, value, InjectionMethod.Replace, conditions));
        }

        public void InjectDataPatch(string assetName, int key, string conditions = "", params string[] values)
        {
            Injector.AddInjection(new DataInjection<int>(Plato, assetName, key, InjectionMethod.Merge, conditions, values));
        }

        public void InjectDataInsert(string assetName, string key, string value, string conditions = "")
        {
            Injector.AddInjection(new DataInjection<string>(Plato, assetName, key, value, InjectionMethod.Replace, conditions));
        }

        public void InjectDataPatch(string assetName, string key, string conditions = "",params string[] values)
        {
            Injector.AddInjection(new DataInjection<string>(Plato, assetName, key, InjectionMethod.Merge, conditions, values));
        }

        public void InjectPatch(string assetName, Texture2D asset, bool overlay = false, Rectangle? sourceArea = null, Rectangle? targetArea = null, string conditions = "")
        {
            Injector.AddInjection(new TextureInjection(Plato, assetName, asset, overlay ? InjectionMethod.Overlay : InjectionMethod.Merge, sourceArea, targetArea, conditions));
        }

        public void InjectPatch(string assetName, Map asset, Rectangle? sourceArea = null, Rectangle? targetArea = null, string conditions = "", bool removeEmpty = false)
        {
            Injector.AddInjection(new MapInjection(Plato, assetName, asset, removeEmpty ? InjectionMethod.Merge : InjectionMethod.Overlay, sourceArea, targetArea, conditions));
        }
    }
}
