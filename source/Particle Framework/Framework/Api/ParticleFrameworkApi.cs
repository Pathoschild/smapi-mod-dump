/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using ParticleFramework.Framework.Data;
using ParticleFramework.Framework.Managers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParticleFramework.Framework.Api
{
    public interface IParticleFrameworkApi
    {
        public void AddCustomDictPath(string customDictPath);
        public void LoadEffect(ParticleEffectData effectData);
        public void UnloadEffect(string key);
        public List<ParticleData> GetParticleData(string effectKey);
        public List<string> GetEffectNames();
    }
    public class ParticleFrameworkApi
    {
        public void AddCustomDictPath(string customDictPath)
        {
            ParticleEffectManager.dictPaths.Add(customDictPath);
        }
        public void LoadEffect(ParticleEffectData effectData)
        {
            try
            {
                effectData.spriteSheet = ModEntry.modHelper.ModContent.Load<Texture2D>(effectData.spriteSheetPath);
                ParticleEffectManager.effectDict[effectData.key] = effectData;
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error loading particle effect with key '{effectData.key}': {e}", LogLevel.Error);
                ParticleEffectManager.UnloadEffect(effectData.key);
            }
        }

        public void UnloadEffect(string key)
        {
            try
            {
                if (ParticleEffectManager.effectDict.ContainsKey(key))
                {
                    ParticleEffectManager.effectDict.Remove(key);
                    ModEntry.monitor.Log($"Successfully unloaded particle effect '{key}'.", LogLevel.Warn);
                }
                else
                {
                    ModEntry.monitor.Log($"Error unloading particle effect '{key}': Not found in the dictionary.", LogLevel.Warn);
                }
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error unloading particle effect '{key}': {e}", LogLevel.Error);
            }
        }


        public List<ParticleData> GetParticleData(string effectKey)
        {
            if (ParticleEffectManager.effectDict.TryGetValue(effectKey, out ParticleEffectData effectData))
            {
                // Get all particle data for this effect key
                var allParticleData = ParticleEffectManager.GetAllParticleData(effectKey);
                return allParticleData;
            }
            else
            {
                ModEntry.monitor.Log($"Error retrieving particle data for effect '{effectKey}': Effect key not found.", LogLevel.Warn);
                return null;
            }
        }



        public List<string> GetEffectNames()
        {
            return ParticleEffectManager.effectDict.Keys.ToList();
        }
    }
}
