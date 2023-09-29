/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace Umbrellas;

public class AssetManager
{
    internal static Dictionary<string, UmbrellaData> UmbrellaData = new();
    internal static Dictionary<string, bool> Inclusions = new();
    internal static List<string> TexturesList = new();

    internal static void InitializeAssets(object sender, SaveLoadedEventArgs e)
    {
        TexturesList = Globals.ModContent.Load<List<string>>("Assets/textures.json");
        UmbrellaData = Globals.GameContent.Load<Dictionary<string, UmbrellaData>>(Globals.DataPath);
        LoadUmbrellaTextures();
        ExtrapolateAllOffsetData();
        Inclusions = Globals.GameContent.Load<Dictionary<string, bool>>(Globals.InclusionsPath);
    }

    internal static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.DataPath))
        {
            e.LoadFromModFile<Dictionary<string, UmbrellaData>>("Assets/data.json", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(Globals.InclusionsPath))
        {
            e.LoadFromModFile<Dictionary<string, bool>>("Assets/inclusions.json", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.StartsWith("sophie.Umbrellas/UmbrellaTextures"))
        {
            foreach (string textureName in TexturesList)
            {
                if (e.NameWithoutLocale.IsEquivalentTo($"sophie.Umbrellas/UmbrellaTextures/{textureName}"))
                {
                    e.LoadFromModFile<Texture2D>($"Assets/UmbrellaTextures/{textureName}.png", AssetLoadPriority.Medium);
                }
            }

        }
    }

    internal static void UpdateAssets(object sender, AssetReadyEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.DataPath))
        {
            UmbrellaData = Globals.GameContent.Load<Dictionary<string, UmbrellaData>>(Globals.DataPath);

            LoadUmbrellaTextures();
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(Globals.InclusionsPath))
        {
            Inclusions = Globals.GameContent.Load<Dictionary<string, bool>>(Globals.InclusionsPath);
        }
    }

    internal static void ReloadData()
    {
        Globals.GameContent.InvalidateCache(Globals.DataPath);
        Globals.GameContent.InvalidateCache(Globals.InclusionsPath);
        InitializeAssets(null, null);
    }

    private static void LoadUmbrellaTextures()
    {
        foreach(KeyValuePair<string, UmbrellaData> kvp in UmbrellaData)
        {
            if (kvp.Value.UmbrellaTexture is null)
            {
                try {
                    kvp.Value.UmbrellaTexture = Globals.GameContent.Load<Texture2D>(kvp.Value.UmbrellaTexturePath);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load umbrella texture for {kvp.Key}, falling back to default.\nException: {e}");
                    kvp.Value.UmbrellaTexture = Globals.GameContent.Load<Texture2D>("sophie.Umbrellas/UmbrellaTextures/Default");
                }
            }
        }
    }

    private static void ExtrapolateAllOffsetData()
    {
        foreach (UmbrellaData data in UmbrellaData.Values)
        {
            data.ExtrapolateOffsetData();
        }
    }
}
