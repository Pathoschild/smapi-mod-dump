/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ParticleFramework.Framework.Managers;
using StardewValley;
using StardewValley.Objects;

namespace ParticleFramework.Framework.Patches
{
    internal class CodePatches : PatchTemplate
    {
        internal CodePatches(Harmony harmony) : base(harmony) { }
        internal void Apply()
        {
            Patch(typeof(Farmer), nameof(Farmer.draw), nameof(FarmerDrawPostfix), [typeof(SpriteBatch)]);
            Patch(typeof(Object), nameof(Object.draw), nameof(ObjectDrawPostfix), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
            Patch(typeof(NPC), nameof(NPC.draw), nameof(NPCDrawPostfix), [typeof(SpriteBatch), typeof(float)]);
            Patch(typeof(Furniture), nameof(Furniture.draw), nameof(FurnitureDrawPostfix), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
            Patch(typeof(BedFurniture), nameof(BedFurniture.draw), nameof(BedFurnitureDrawPostfix), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
            Patch(typeof(FishTankFurniture), nameof(FishTankFurniture.draw), nameof(FishTankFurnitureDrawPostfix), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
        }

        public static void FarmerDrawPostfix(Farmer __instance, SpriteBatch b)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                switch (kvp.Value.type.ToLower())
                {
                    case "hat":
                        if (__instance.hat.Value != null && __instance.hat.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                    case "shirt":
                        if (__instance.shirtItem.Value != null && __instance.shirtItem.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                    case "pants":
                        if (__instance.pantsItem.Value != null && __instance.pantsItem.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                    case "boots":
                        if (__instance.boots.Value != null && __instance.boots.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                    case "tool":
                        if (__instance.CurrentItem is Tool && __instance.CurrentItem.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                    case "ring":
                        if (__instance.leftRing.Value != null && __instance.leftRing.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        else if (__instance.rightRing.Value != null && __instance.rightRing.Value.QualifiedItemId == kvp.Value.name)
                            ParticleEffectManager.ShowFarmerParticleEffect(b, __instance, kvp.Value);
                        break;
                }
            }
        }
        public static void ObjectDrawPostfix(SObject __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                if (kvp.Value.type.ToLower() == "object" && kvp.Value.name == __instance.QualifiedItemId)
                {
                    ParticleEffectManager.ShowObjectParticleEffect(spriteBatch, __instance, x, y, kvp.Value);
                }
            }
        }
        public static void NPCDrawPostfix(NPC __instance, SpriteBatch b)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                if (kvp.Value.type.ToLower() == "npc" && kvp.Value.name == __instance.Name)
                {
                    ParticleEffectManager.ShowNPCParticleEffect(b, __instance, kvp.Value);
                }
            }
        }

        public static void FurnitureDrawPostfix(Furniture __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                if (kvp.Value.type.ToLower() == "furniture" && kvp.Value.name == __instance.QualifiedItemId)
                {
                    ParticleEffectManager.ShowFurnitureParticleEffect(spriteBatch, __instance, x, y, kvp.Value);
                }
            }
        }

        public static void BedFurnitureDrawPostfix(BedFurniture __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                if (kvp.Value.type.ToLower() == "furniture" && kvp.Value.name == __instance.QualifiedItemId)
                {
                    ParticleEffectManager.ShowFurnitureParticleEffect(spriteBatch, __instance, x, y, kvp.Value);
                }
            }
        }

        // Fix later
        public static void FishTankFurnitureDrawPostfix(FishTankFurniture __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (!ModEntry.modConfig.EnableMod)
                return;

            foreach (var kvp in ParticleEffectManager.effectDict)
            {
                if (kvp.Value.type.ToLower() == "furniture" && kvp.Value.name == __instance.QualifiedItemId)
                {
                    ParticleEffectManager.ShowFurnitureParticleEffect(spriteBatch, __instance, x, y, kvp.Value);
                }
            }

        }
    }
}
