using HDSprites.ContentPack;
using HDSprites.Token.Global;
using System.Collections.Generic;
using System.Linq;

namespace HDSprites.Token
{
    public class DynamicTokenManager
    {
        public Dictionary<string, DynamicToken> DynamicTokens { get; set; }
        private Dictionary<string, List<ContentPackAsset>> RegisteredAssets { get; set; }

        public DynamicTokenManager()
        {
            this.DynamicTokens = new Dictionary<string, DynamicToken>();
            this.RegisteredAssets = new Dictionary<string, List<ContentPackAsset>>();

            // Global Tokens
            
            // Date & Weather
            this.AddToken(new DayGlobalToken());
            this.AddToken(new DayEventGlobalToken());
            this.AddToken(new DayOfWeekGlobalToken());
            this.AddToken(new DaysPlayedGlobalToken());
            this.AddToken(new SeasonGlobalToken());
            this.AddToken(new WeatherGlobalToken());
            this.AddToken(new YearGlobalToken());

            // Player
            this.AddToken(new HasFlagGlobalToken());
            this.AddToken(new HasProfessionGlobalToken());
            this.AddToken(new HasReadLetterGlobalToken());
            this.AddToken(new HasSeenEventGlobalToken());
            this.AddToken(new HasWalletItemGlobalToken());
            this.AddToken(new IsMainPlayerGlobalToken());
            this.AddToken(new IsOutdoorsGlobalToken());
            this.AddToken(new LocationNameGlobalToken());
            this.AddToken(new PlayerGenderGlobalToken());
            this.AddToken(new PlayerNameGlobalToken());
            this.AddToken(new PreferredPetGlobalToken());
            this.AddToken(new SkillLevelGlobalToken());

            // Relationships
            this.AddToken(new HeartsGlobalToken());
            this.AddToken(new RelationshipGlobalToken());
            this.AddToken(new SpouseGlobalToken());

            // World
            this.AddToken(new FarmCaveGlobalToken());
            this.AddToken(new FarmhouseUpgradeGlobalToken());
            this.AddToken(new FarmNameGlobalToken());
            this.AddToken(new FarmTypeGlobalToken());
            this.AddToken(new IsCommunityCenterCompleteGlobalToken());
            this.AddToken(new LanguageGlobalToken());
        }

        public void AddToken(DynamicToken token)
        {
            this.DynamicTokens.Add(token.Name, token);
            this.RegisteredAssets.Add(token.Name, new List<ContentPackAsset>());
        }

        public void RegisterAsset(string token, ContentPackAsset asset)
        {
            foreach (var entry in this.RegisteredAssets)
            {
                if (entry.Key.ToLower().Equals(token.ToLower()))
                {
                    entry.Value.Add(asset);
                    break;
                }
            }
        }

        public void CheckTokens()
        {
            Dictionary<string, List<string>> oldTokens = new Dictionary<string, List<string>>();
            foreach (var entry in this.DynamicTokens)
            {
                oldTokens.Add(entry.Key, entry.Value.GetSimpleValues());
            }

            foreach (var entry in this.DynamicTokens)
            {
                if (entry.Value is GlobalToken)
                {
                    ((GlobalToken)entry.Value).Update();
                }
            }

            List<ContentPackAsset> updateAsset = new List<ContentPackAsset>();
            foreach (var entry in this.DynamicTokens)
            {
                List<string> oldValues = oldTokens[entry.Key];
                List<string> values = entry.Value.GetSimpleValues();
                if (oldValues.Count() != values.Count() || oldValues.Intersect(values).Count() != oldValues.Count())
                {
                    foreach (var asset in this.RegisteredAssets[entry.Key])
                    {
                        if (!updateAsset.Contains(asset))
                        {
                            updateAsset.Add(asset);
                        }
                    }
                }
            }

            foreach (var asset in updateAsset)
            {
                asset.Update();
            }
        }
    }
}
