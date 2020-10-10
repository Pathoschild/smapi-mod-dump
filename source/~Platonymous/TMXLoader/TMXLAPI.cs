/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace TMXLoader
{
    public interface ITMXLAPI
    {
        void AddContentPack(IContentPack pack);

        event EventHandler<GameLocation> OnLocationRestoring;
    }

    public class TMXLAPI : ITMXLAPI
    {
        protected static event EventHandler<GameLocation> OnLocationRestoringEvent;
        
        public event EventHandler<GameLocation> OnLocationRestoring
        {
            add
            {
                OnLocationRestoringEvent += value;
            }
            remove
            {
                OnLocationRestoringEvent -= value;
            }
        }

        internal static void RaiseOnLocationRestoringEvent(GameLocation inGame)
        {
            OnLocationRestoringEvent?.Invoke(null, inGame);
        }

        public void AddContentPack(IContentPack pack)
        {
            if (TMXLoaderMod.AddedContentPacks.Contains(pack))
                return;

            TMXLoaderMod.AddedContentPacks.Add(pack);

            if (TMXLoaderMod.contentPacksLoaded)
                TMXLoaderMod._instance.loadPack(pack,"content");
        }
    }
}
