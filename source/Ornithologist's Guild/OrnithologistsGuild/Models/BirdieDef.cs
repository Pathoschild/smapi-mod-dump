/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using ContentPatcher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using OrnithologistsGuild.Models;
using StardewModdingAPI;
using StardewValley;

namespace OrnithologistsGuild.Content
{
    public class BirdieDef
    {
        public void LoadSoundAssets()
        {
            if (SoundAssetPath != null)
            {
                SoundID = $"{UniqueID}_Sound";

                CueDefinition cueDef = new CueDefinition();

                cueDef.name = SoundID;
                cueDef.instanceLimit = 1;
                cueDef.limitBehavior = CueDefinition.LimitBehavior.FailToPlay;

                string filePathCombined = Path.Combine(ContentPackDef.ContentPack.DirectoryPath, SoundAssetPath);
                SoundEffect audio = SoundEffect.FromFile(filePathCombined);

                cueDef.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Sound"), false);

                Game1.soundBank.AddCue(cueDef);
            }
        }

        public void ParseConditions(IContentPatcherAPI contentPatcher)
        {
            foreach (var condition in this.Conditions)
            {
                condition.ManagedConditions = contentPatcher.ParseConditions(
                       manifest: ModEntry.Instance.ModManifest,
                       rawConditions: condition.When,
                       formatVersion: new SemanticVersion("1.20.0")
                 );

                if (!condition.ManagedConditions.IsValid)
                {
                    throw new Exception(condition.ManagedConditions.ValidationError);
                }
            }
        }

        public string ID;
        public string UniqueID; // Generated
        public string AssetPath;

        public string SoundAssetPath;
        public string SoundID; // Generated

        public int BaseFrame = 0;

        public ContentPackDef ContentPackDef; // Generated

        public int Attributes;

        public bool ShouldUseBath = true;
        public int MaxFlockSize = 1;
        public int Cautiousness = 5;
        public int FlapDuration = 500;
        public float FlySpeed = 5f;

        public float BaseWt = 0.5f;
        public Dictionary<string, float> FoodBaseWts;
        public Dictionary<string, float> FeederBaseWts;
        public List<BirdDefCondition> Conditions;

        public float GetContextualWeight(bool updateContext = true, FeederDef feederDef = null, FoodDef foodDef = null)
        {
            float weight = this.BaseWt;

            if (feederDef != null)
            {
                if (this.CanPerchAt(feederDef))
                {
                    weight += this.FeederBaseWts[feederDef.type];
                } else
                {
                    ModEntry.Instance.Monitor.Log($@"GetContextualWeight 0 {this.ID} (feederDef)");
                    return 0; // Bird does not eat at feeder
                }
            }

            if (foodDef != null)
            {
                if (this.CanEat(foodDef))
                {
                    weight += this.FoodBaseWts[foodDef.type];
                }
                else
                {
                    ModEntry.Instance.Monitor.Log($@"GetContextualWeight 0 {this.ID} (foodDef)");
                    return 0; // Bird does not eat food
                }
            }

            foreach (var condition in this.Conditions)
            {
                if (updateContext) condition.ManagedConditions.UpdateContext();

                if (condition.ManagedConditions.IsMatch)
                {
                    if (condition.NilWt)
                    {
                        ModEntry.Instance.Monitor.Log($@"GetContextualWeight 0 {this.ID} ({string.Join(", ", condition.When.Keys)})");
                        return 0; // Bird not added
                    }

                    weight += condition.AddWt;
                    weight -= condition.SubWt;
                }
            }

            ModEntry.Instance.Monitor.Log($@"GetContextualWeight {MathHelper.Clamp(weight, 0, 1).ToString()} {this.ID}");
            return MathHelper.Clamp(weight, 0, 1);
        }

        public int GetContextualCautiousness()
        {
            var modifier = -Math.Clamp((int)Math.Round(Game1.player.DailyLuck * 10), -1, 1);

            return this.Cautiousness + modifier;
        }

        public bool CanPerchAt(FeederDef feederDef)
        {
            return FeederBaseWts.ContainsKey(feederDef.type);
        }

        public bool CanEat(FoodDef foodDef)
        {
            return FoodBaseWts.ContainsKey(foodDef.type);
        }
    }

    public class BirdDefCondition
    {
        public Dictionary<string, string> When;
        public IManagedConditions ManagedConditions; // Generated

        public bool NilWt = false;
        public float AddWt = 0;
        public float SubWt = 0;
    }
}

