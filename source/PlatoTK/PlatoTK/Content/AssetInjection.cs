/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using PlatoTK;

namespace PlatoTK.Content
{
    internal abstract class AssetInjection
    {
        public readonly string AssetName;

        public readonly InjectionMethod Method;

        private readonly string Conditions;

        private readonly IPlatoHelper Helper;

        private bool MatchesConditions;

        public bool HasConditions => !string.IsNullOrEmpty(Conditions);

        public bool ConditionsMet
        {
            get
            {
                if (!HasConditions)
                    MatchesConditions = true;
                else
                    MatchesConditions = Helper.CheckConditions(Conditions, this);

                return MatchesConditions;
            }
        }

        private void ConditionsChanged(string conditions, bool newValue)
        {
            if (MatchesConditions != newValue)
            {
                MatchesConditions = newValue;
                Helper.ModHelper.GameContent.InvalidateCache(AssetName);
            }
        }

        public AssetInjection(
            IPlatoHelper helper,
            string assetName,
            InjectionMethod method,
            string conditions = "")
        {
            Method = method;
            Helper = helper;
            AssetName = assetName;
            Conditions = conditions;
            MatchesConditions = true;

            if (HasConditions)
                MatchesConditions = Helper.CheckConditions(Conditions,this);
        }
    }

    internal abstract class AssetInjection<TAsset> : AssetInjection
    {
        public readonly TAsset Value;

        public Type GetAssetType()
        {
            return typeof(TAsset);
        }

        public AssetInjection(
            IPlatoHelper helper,
            string assetName,
            TAsset value,
            InjectionMethod method,
            string conditions = "")
            : base(helper,assetName,method,conditions)
        {
            Value = value;
        }
    }
}
