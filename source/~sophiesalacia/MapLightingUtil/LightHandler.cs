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
using Microsoft.Xna.Framework;
using StardewValley;

namespace MapLightingUtil;

internal class LightHandler
{
    public static Dictionary<string, LightData> LightData;

    public static void Init()
    {
        LightData = Globals.GameContent.Load<Dictionary<string, LightData>>(Globals.LightsAssetPath);
    }

    internal static void RefreshAsset()
    {
        RemoveAllLights();
        Init();
        AddAllValidLights();
    }

    internal static void RefreshLights()
    {
        RemoveAllLights();
        AddAllValidLights();
    }

    internal static void AddAllValidLights()
    {
        if (LightData is null)
            Init();

        foreach ((string key, LightData lightData) in LightData)
        {
            if (lightData is null)
                continue;

            if (lightData.AreConditionsMet())
            {
                if (Game1.currentLightSources.Add(lightData.LightSource))
                    Log.Trace($"Added light with id {key} to current light sources.");
            }
        }
    }

    internal static void RemoveAllLights()
    {
        if (LightData is null)
            Init();

        foreach ((string key, LightData lightData) in LightData)
        {
            if (lightData is null)
                continue;

            if (Game1.currentLightSources.Remove(lightData.LightSource))
                Log.Trace($"Removed light with id {key} from current light sources.");
        }
    }
}

internal class LightData
{
    public string Location;
    public int StartTime;
    public int EndTime;
    public int TextureType;
    public Vector2 Position;
    public float Radius;

    internal Color Color;
    internal LightSource LightSource;

    public LightData(string location, int startTime, int endTime, int textureType, Vector2 position, float radius, int R, int G, int B, int A)
    {
        Location = location;
        StartTime = startTime;
        EndTime = endTime;
        TextureType = textureType;
        Position = position * 64f;
        Radius = radius;

        Color = new Color(Math.Clamp(R, 0, 255), Math.Clamp(G, 0, 255), Math.Clamp(B, 0, 255), Math.Clamp(A, 0, 255));
        LightSource = new LightSource(TextureType, Position, Radius, Color, LightSource.LightContext.None, 0L);
    }

    public bool AreConditionsMet()
    {
        return Location == Game1.currentLocation.Name && Game1.timeOfDay >= StartTime && Game1.timeOfDay < EndTime;
    }
}
